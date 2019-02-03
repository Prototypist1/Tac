using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.TestCases.Samples;
using Xunit;

namespace Tac.Backend.Test
{
    public class MirrorImplmentationTest {

        [Fact]
        public void Test()
        {
            var testCase = new MirrorPointImplementation();
            var conversionContext = new Definitions();
            var module = testCase.Module.Convert(conversionContext);

            var res = module.Interpet(InterpetedContext.Root());

            var scope = res.Get<IInterpetedScope>();
            var implementation = scope.GetMember(new NameKey("mirror")).Value.Cast<InterpetedImplementation>();

            var context = InterpetedInstanceScope.Make(
                (new NameKey("x"), new InterpetedMember(new RuntimeNumber(5))),
                (new NameKey("y"), new InterpetedMember(new RuntimeNumber(7))));

            implementation.Invoke(context).Get<InterpetedMethod>().Invoke(new RunTimeEmpty());

            Assert.Equal(7,context.GetMember(new NameKey("x")).Value.Cast<RuntimeNumber>().d);
            Assert.Equal(5, context.GetMember(new NameKey("y")).Value.Cast<RuntimeNumber>().d);
            
            implementation.Invoke(context).Get<InterpetedMethod>().Invoke(new RunTimeEmpty());

            Assert.Equal(5, context.GetMember(new NameKey("x")).Value.Cast<RuntimeNumber>().d);
            Assert.Equal(7, context.GetMember(new NameKey("y")).Value.Cast<RuntimeNumber>().d);
        }
    }
}
