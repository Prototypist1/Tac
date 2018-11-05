using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;

namespace Tac.TestCases
{
    public interface ITestCase
    {
        string Text { get; }
        ICodeElement[] CodeElements { get; }
        IFinalizedScope Scope { get; }
    }
}
