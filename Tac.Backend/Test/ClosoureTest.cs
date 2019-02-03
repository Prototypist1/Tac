using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Tests.Samples;
using Xunit;

namespace Tac.Backend.Test
{
    public class ClosoureTest
    {
        [Fact]
        public void Test()
        {
            var testCase = new Closoure();
            var conversionContext = new Definitions();
            var module = testCase.Module.Convert(conversionContext);

            var res = module.Interpet(InterpetedContext.Root());

            var scope = res.Get<IInterpetedScope>();
            var method = scope.GetMember(new NameKey("create-accululator")).Value.Cast<InterpetedMethod>();
            
            var innerMethod = method.Invoke(new RuntimeNumber(1)).Get<InterpetedMethod>();
            
            Assert.Equal(3, innerMethod.Invoke(new RuntimeNumber(2)).Get<RuntimeNumber>().d);
            Assert.Equal(6, innerMethod.Invoke(new RuntimeNumber(3)).Get<RuntimeNumber>().d);
            Assert.Equal(10, innerMethod.Invoke(new RuntimeNumber(4)).Get<RuntimeNumber>().d);
        }
    }
}
