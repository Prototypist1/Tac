﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    class NumberType: IRunTime
    {
        public readonly double d;

        public NumberType(double d)
        {
            this.d = d;
        }
    }
}
