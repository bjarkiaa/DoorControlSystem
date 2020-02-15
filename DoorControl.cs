using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DoorControlSystem
{
    public class DoorControl
    {
        //States
        enum DoorControlStates
        {
            DoorClosed,
            DoorOpening,
            DoorClosing,
            DoorAlarm
        }

        //Objects
        private IAlarm _alarm;
        private IUserValidation _userValidation;
        private IEntryNotification _entryNotification;
        private IDoor _door;

        //Enums
        private DoorControlStates _doorControlStates;

        //Constructor
        public DoorControl(IAlarm alarm, IUserValidation userValidation, IEntryNotification entryNotification, IDoor door)
        {
            //Save reference to all dependencies
            _alarm = alarm;
            _userValidation = userValidation;
            _entryNotification = entryNotification;
            _door = door;

            //Subscribe to all needed events
            _door.DoorOpen += DoorOpened;                               //Event invoked when door is opened
            _door.DoorClosed += DoorClosed;                             //Event invoked when door is closed

            //Set initial state
            _doorControlStates = DoorControlStates.DoorClosed;
        }

        public void RequestEntry(String id)
        {
            //Door is closed and ready for new request
            switch (_doorControlStates)
            {
                case DoorControlStates.DoorClosed:
                    //Validate user
                    if (!_userValidation.ValidateEntryRequest(id))
                    {
                        _entryNotification.NotifyEntryDenied();
                        return;
                    }
                        

                    //Change state
                    _doorControlStates = DoorControlStates.DoorOpening;
                    
                    //Open door
                    _door.Open();

                    //Notify user
                    _entryNotification.NotifyEntryGranted();

                    //End case
                    break;

                //Alarm is currently active, userValidation disables alarm
                case DoorControlStates.DoorAlarm:
                    //Validate user
                    if (!_userValidation.ValidateEntryRequest(id))
                        return;

                    //Change state
                    _doorControlStates = DoorControlStates.DoorClosed;

                    //Lower alarm
                    _alarm.Lower();

                    //End case
                    break;
            }
        }

        //Event handlers
        protected void DoorOpened(object sender, EventArgs e)
        {
            switch (_doorControlStates)
            {
                //Door should be opening here
                case DoorControlStates.DoorOpening:
                    //Change state
                    _doorControlStates = DoorControlStates.DoorClosing;
                    
                    //Close door
                    _door.Close();

                    //End case
                    break;

                //In case of breach where door is expected closed
                case DoorControlStates.DoorClosed:
                    //Change state
                    _doorControlStates = DoorControlStates.DoorAlarm;

                    //Raise alarm
                    _alarm.Raise();

                    //End case
                    break;
            }
        }

        protected void DoorClosed(object sender, EventArgs e)
        {
            switch (_doorControlStates)
            {
                case DoorControlStates.DoorClosing:
                    //Change state
                    _doorControlStates = DoorControlStates.DoorClosed;

                    //End case
                    break;
            }
        }


    }
}
