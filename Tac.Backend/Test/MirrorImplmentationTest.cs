using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model;
using Tac.Model.Instantiated;
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

            var scope = value.Value.CastTo<IInterpetedScope>();
            var implementation = scope.GetMember<IInterpetedAnyType>(new NameKey("mirror")).Value.CastTo<IInterpetedImplementation<IInterpetedScope, IInterpedEmpty, IInterpedEmpty>>();

            var context = TypeManager.InstanceScope(
                (new NameKey("x"), TypeManager.AnyMember(TypeManager.Double( 5))),
                (new NameKey("y"), TypeManager.AnyMember(TypeManager.Double(7))));

            {
                Assert.False(implementation.Invoke(TypeManager.Member<IInterpetedScope>(context.Convert(TransformerExtensions.NewConversionContext()), context)).IsReturn(out var _, out var method));

                method.Value.Invoke(TypeManager.EmptyMember(TypeManager.Empty()));
            }

            Assert.Equal(7,context.GetMember<IInterpetedAnyType>(new NameKey("x")).Value.CastTo<IBoxedDouble>().Value);
            Assert.Equal(5, context.GetMember<IInterpetedAnyType>(new NameKey("y")).Value.CastTo<IBoxedDouble>().Value);

            {
                Assert.False(implementation.Invoke(TypeManager.Member<IInterpetedScope>(context.Convert(TransformerExtensions.NewConversionContext()), context)).IsReturn(out var _, out var method));

                method.Value.Invoke(TypeManager.EmptyMember(TypeManager.Empty()));
            }

            Assert.Equal(5, context.GetMember<IInterpetedAnyType>(new NameKey("x")).Value.CastTo<IBoxedDouble>().Value);
            Assert.Equal(7, context.GetMember<IInterpetedAnyType>(new NameKey("y")).Value.CastTo<IBoxedDouble>().Value);

        }
    }
}
