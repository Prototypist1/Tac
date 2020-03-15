using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.TestCases
{
    public interface ITestCase
    {
        string Text { get; }
        IModuleDefinition ModuleDefinition { get; }
    }
}
