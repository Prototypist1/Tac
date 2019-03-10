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
            var implementation = scope.GetMember<IInterpetedAnyType>(new NameKey("mirror")).Value.Cast<IInterpetedImplementation<IInterpetedScope, IInterpedEmpty, IInterpedEmpty>>();

            var context = InterpetedInstanceScope.Make(
                (new NameKey("x"), new InterpetedMember<BoxedDouble>(new BoxedDouble( 5))),
                (new NameKey("y"), new InterpetedMember<BoxedDouble>(new BoxedDouble(7))));

            {
                Assert.False(implementation.Invoke(new InterpetedMember<IInterpetedScope>(context)).IsReturn(out var _, out var method));

                method.Value.Invoke(new InterpetedMember<IInterpedEmpty>(new RunTimeEmpty()));
            }

            Assert.Equal(7,context.GetMember<BoxedDouble>(new NameKey("x")).Value.Value);
            Assert.Equal(5, context.GetMember<BoxedDouble>(new NameKey("y")).Value.Value);

            {
                Assert.False(implementation.Invoke(new InterpetedMember<IInterpetedScope>(context)).IsReturn(out var _, out var method));

                method.Value.Invoke(new InterpetedMember<IInterpedEmpty>(new RunTimeEmpty()));
            }

            Assert.Equal(5, context.GetMember<BoxedDouble>(new NameKey("x")).Value.Value);
            Assert.Equal(7, context.GetMember<BoxedDouble>(new NameKey("y")).Value.Value);

        }
    }
}
