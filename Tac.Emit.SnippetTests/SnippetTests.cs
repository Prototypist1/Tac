using System;
using System.Collections.Generic;
using System.Text;
using Tac.Emit.Runner;
using Xunit;

namespace Tac.Emit.SnippetTests
{
    
    public class SnippetTests
    {

        [Fact]
        public void PassThrough() {
            var res = Run.CompileAndRun<double, double>("test", "entry-point [number; number;] input { input return; }", 1.0);
            Assert.Equal(1.0, res);

        }
    }
}
