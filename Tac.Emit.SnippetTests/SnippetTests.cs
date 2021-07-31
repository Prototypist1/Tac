using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Emit;
using Tac.Backend.Emit.Walkers;
using Tac.Emit.Runner;
using Tac.Model;
using Xunit;

namespace Tac.Emit.SnippetTests
{
    
    public class SnippetTests
    {
        // TODO 
        // I should be able to add the "return empty" for the user
        // but that really requires flow analysis
        // and I don't have that yet
        //[Fact]
        //public void SuperSimple()
        //{
        //    Run.CompileAndRun<Empty, Empty>("test", "entry-point [empty; empty;] input { }", new Empty(), Array.Empty<IAssembly<object>>());
        //}

        [Fact]
        public void PassThrough()
        {
            var res = Run.CompileAndRun<double, double>("test", "entry-point [number; number;] input { input return; }", 1.0, Array.Empty<Tac.Backend.Emit.Assembly>());
            Assert.Equal(1.0, res);
        }

        [Fact]
        public void TwoMemberReferences() {
            var res = Run.CompileAndRun<double, double>("test", "entry-point [number; number;] input { input + input return; }", 1.0, Array.Empty<Tac.Backend.Emit.Assembly>());
            Assert.Equal(2.0, res);
        }

        // {48146F3A-6D75-4F24-B857-BED24CE846EA}
        // this is probably 
//        [Fact]
//        public void DoesItResolveTheRightX()
//        {
//            var res = Run.CompileAndRun<Empty, double>(
//                "test", @"
//entry-point [empty; number;] input { 
//    1 =: x;
//    object { x =: x; 2 =: y }.x return; 
//}", 
//                null, 
//                Array.Empty<Tac.Backend.Emit.Assembly>());
//            Assert.Equal(1.0, res);
//        }

        [Fact]
        public void Fac()
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

entry-point [number; number;] input { input > fac return; }", 3.0, Array.Empty<Tac.Backend.Emit.Assembly>());
            Assert.Equal(6.0, res);
        }
    }
}
