﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoorControlSystem
{
    public interface IDoor
    {
        void Open();
        void Close();

        event EventHandler DoorOpen;
        event EventHandler DoorClosed;
    }
}
