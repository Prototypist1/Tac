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

            var context = TypeManager.InstanceScope(
                (new NameKey("x"), TypeManager.Member<IInterpetedAnyType>(TypeManager.Double( 5))),
                (new NameKey("y"), TypeManager.Member<IInterpetedAnyType>(TypeManager.Double(7))));

            {
                Assert.False(implementation.Invoke(TypeManager.Member<IInterpetedScope>(context)).IsReturn(out var _, out var method));

                method.Value.Invoke(TypeManager.Member<IInterpedEmpty>(TypeManager.Empty()));
            }

            Assert.Equal(7,context.GetMember<IInterpetedAnyType>(new NameKey("x")).Value.Cast<IBoxedDouble>().Value);
            Assert.Equal(5, context.GetMember<IInterpetedAnyType>(new NameKey("y")).Value.Cast<IBoxedDouble>().Value);

            {
                Assert.False(implementation.Invoke(TypeManager.Member<IInterpetedScope>(context)).IsReturn(out var _, out var method));

                method.Value.Invoke(TypeManager.Member<IInterpedEmpty>(TypeManager.Empty()));
            }

            Assert.Equal(5, context.GetMember<IInterpetedAnyType>(new NameKey("x")).Value.Cast<IBoxedDouble>().Value);
            Assert.Equal(7, context.GetMember<IInterpetedAnyType>(new NameKey("y")).Value.Cast<IBoxedDouble>().Value);

        }
    }
}
