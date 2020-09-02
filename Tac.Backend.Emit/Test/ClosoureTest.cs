using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Backend.Emit.SyntaxModel;
using Tac.Backend.Emit.SyntaxModel.Elements;
using Tac.Model;
using Tac.Backend.Emit.SyntaxModel;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;
using Tac.Tests.Samples;
using Xunit;
using Prototypist.Toolbox.Object;

namespace Tac.Backend.Emit.Test
{
    public class ClosoureTest
    {
        [Fact]
        public void Test()
        {
            var testCase = new Closoure();
            var conversionContext = new Definitions();
            var module = testCase.ModuleDefinition.Convert(conversionContext);

            var res = module.Interpet(InterpetedContext.Root());

            Assert.False(res.IsReturn(out var _, out var value));

            var scope = value!.Value.CastTo<IInterpetedScope>();
            var method = scope.GetMember(new NameKey("create-accululator")).Value;
            
            Assert.False(method.Has<IInterpetedMethod>().Invoke(TypeManager.NumberMember(TypeManager.Double(1))).IsReturn(out var _, out var innerMethod));

            Assert.False(innerMethod!.Value.Has<IInterpetedMethod>().Invoke(TypeManager.NumberMember(TypeManager.Double(2))).IsReturn(out var _, out var res1));
            Assert.Equal(3, res1!.Value.Has<IBoxedDouble>().Value);

            Assert.False(innerMethod.Value.Has<IInterpetedMethod>().Invoke(TypeManager.NumberMember(TypeManager.Double(3))).IsReturn(out var _, out var res2));
            Assert.Equal(6, res2!.Value.Has<IBoxedDouble>().Value);

            Assert.False(innerMethod.Value.Has<IInterpetedMethod>().Invoke(TypeManager.NumberMember(TypeManager.Double(4))).IsReturn(out var _, out var res3));
            Assert.Equal(10, res3!.Value.Has<IBoxedDouble>().Value);
        }
    }
}
