using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;

namespace Tac.TestCases
{
    public interface ITestCase
    {
        string Test { get; }
        ICodeElement[] CodeElements { get; }
    }
}
