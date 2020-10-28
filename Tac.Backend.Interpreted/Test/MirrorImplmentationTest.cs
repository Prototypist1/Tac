using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Elements;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;
using Tac.TestCases.Samples;
using Xunit;

namespace Tac.Backend.Interpreted.Test
{
    public class MirrorImplmentationTest {

        [Fact]
        public void Test()
        {
            var testCase = new MirrorPointImplementation();
            var conversionContext = new Definitions();
            var module = testCase.RootScope.Convert(conversionContext);

            var res = module.Interpet(InterpetedContext.Root());

            Assert.False(res.IsReturn(out var _, out var value));

            var scope = value!.Value.CastTo<IInterpetedScope>();
            var implementation = scope.GetMember(new NameKey("mirror")).Value.Has<IInterpetedMethod>();

            var context = TypeManager.InstanceScope(
                (new NameKey("x"), TypeManager.AnyMember(TypeManager.Double( 5))),
                (new NameKey("y"), TypeManager.AnyMember(TypeManager.Double(7))));

            {
                Assert.False(implementation.Invoke(TypeManager.Member(context.Convert(TransformerExtensions.NewConversionContext()), context)).IsReturn(out var _, out var method));

                method!.Value.Has<IInterpetedMethod>().Invoke(TypeManager.EmptyMember(TypeManager.Empty()));
            }

            Assert.Equal(7,context.GetMember(new NameKey("x")).Value.Has<IBoxedDouble>().Value);
            Assert.Equal(5, context.GetMember(new NameKey("y")).Value.Has<IBoxedDouble>().Value);

            {
                Assert.False(implementation.Invoke(TypeManager.Member(context.Convert(TransformerExtensions.NewConversionContext()), context)).IsReturn(out var _, out var method));

                method!.Value.Has<IInterpetedMethod>().Invoke(TypeManager.EmptyMember(TypeManager.Empty()));
            }

            Assert.Equal(5, context.GetMember(new NameKey("x")).Value.Has<IBoxedDouble>().Value);
            Assert.Equal(7, context.GetMember(new NameKey("y")).Value.Has<IBoxedDouble>().Value);

        }
    }
}
