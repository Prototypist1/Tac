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
        public void PassThrough()
        {
            var res = Run.CompileAndRun<double, double>("test", "entry-point [number; number;] input { input return; }", 1.0);
            Assert.Equal(1.0, res);
        }

        [Fact]
        public void TwoMemberReferences() {
            var res = Run.CompileAndRun<double, double>("test", "entry-point [number; number;] input { input + input return; }", 1.0);
            Assert.Equal(2.0, res);
        }

        [Fact]
        public void Fact()
        {
            var res = Run.CompileAndRun<double, double>("test",
@"

method [ number ; number ; ] input {
    input <? 2 then {
        1 return ;
    } else {
        ( input - 1 > fac ) * input return ;  
    }
} =: fac ;

entry-point [number; number;] input { input > fac return; }", 3.0);
            Assert.Equal(6.0, res);
        }
    }
}
