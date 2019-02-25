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

            Assert.False(res.IsReturn(out var _, out var value));

            var scope = value.Value.Cast<IInterpetedScope>();
            var implementation = scope.GetMember<InterpetedImplementation<IInterpetedScope,IInterpedEmpty, IInterpedEmpty>>(new NameKey("mirror"));

            var context = InterpetedInstanceScope.Make(
                (new NameKey("x"), new InterpetedMember<double>(5)),
                (new NameKey("y"), new InterpetedMember<double>(7)));

            {
                Assert.False(implementation.Value.Invoke(new InterpetedMember<IInterpetedScope>(context)).IsReturn(out var _, out var method));

                method.Value.Invoke(new InterpetedMember<IInterpedEmpty>(new RunTimeEmpty()));
            }

            Assert.Equal(7,context.GetMember<double>(new NameKey("x")).Value.Cast<InterpetedMember<double>>().Value);
            Assert.Equal(5, context.GetMember<double>(new NameKey("y")).Value.Cast<InterpetedMember<double>>().Value);

            {
                Assert.False(implementation.Value.Invoke(new InterpetedMember<IInterpetedScope>(context)).IsReturn(out var _, out var method));

                method.Value.Invoke(new InterpetedMember<IInterpedEmpty>(new RunTimeEmpty()));
            }

            Assert.Equal(5, context.GetMember<double>(new NameKey("x")).Value.Cast<InterpetedMember<double>>().Value);
            Assert.Equal(7, context.GetMember<double>(new NameKey("y")).Value.Cast<InterpetedMember<double>>().Value);

        }
    }
}
