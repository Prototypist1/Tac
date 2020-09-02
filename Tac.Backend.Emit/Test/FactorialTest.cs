using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Backend.Emit.SyntaxModel;
using Tac.Backend.Emit.SyntaxModel.Elements;
using Tac.Model;
using Tac.Backend.Emit.SyntaxModel;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;
using Tac.Tests.Samples;
using Xunit;

namespace Tac.Backend.Emit.Test
{

    public class FactorialTest
    {
        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 6)]
        [InlineData(4, 24)]
        public void Test(double input, double output)
        {
            Assert.Equal(output, Fac(input));
        }
        
        private double Fac(double d)
        {
            var testCase = new Factorial();
            var conversionContext = new Definitions();
            var module = testCase.ModuleDefinition.Convert(conversionContext);

            var res = module.Interpet(InterpetedContext.Root());

            Assert.False(res.IsReturn(out var _, out var value));

            var scope = value!.Value.CastTo<IInterpetedScope>();
            // no way this cast works...
            var method = scope.GetMember(new NameKey("fac"));

            Assert.False(method.Value.Has<IInterpetedMethod>().Invoke(TypeManager.NumberMember(TypeManager.Double(d))).IsReturn(out var _, out var methodReturn));

            return methodReturn!.Value.CastTo<IBoxedDouble>().Value;
        }

    }
}
