using Tac.Parser;
using Tac.TestCases;

namespace Tac.Tests.Samples
{
    internal interface IWrappedTestCase: ITestCase {
        IToken Token { get; }
    }
}
