﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoorControlSystem
{
    public interface IUserValidation
    {
        bool ValidateEntryRequest(String id);
    }
}
