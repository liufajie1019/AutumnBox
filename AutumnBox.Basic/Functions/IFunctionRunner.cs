﻿using AutumnBox.Basic.Arg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutumnBox.Basic.Functions
{
    internal interface IThreadFunctionRunner
    {
        void Run(IArgs arg);
    }
}
