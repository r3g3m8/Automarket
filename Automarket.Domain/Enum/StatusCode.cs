﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automarket.Domain.Enum
{
    public enum StatusCode
    {
        UserNotFound = 0,
        CarNotFound = 10,
        Ok = 200,
        InternalServerError = 500
    }
}
