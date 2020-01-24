using Prototypist.Toolbox;
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
using Prototypist.Toolbox.Object;

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

            Assert.False(res.IsReturn(out var _, out var value));

            var scope = value!.Value.CastTo<IInterpetedScope>();
            var method = scope.GetMember<IInterpetedAnyType>(new NameKey("create-accululator")).Value.CastTo<IInterpetedMethod<IBoxedDouble, IInterpetedMethod<IBoxedDouble, IBoxedDouble>>>();
            
            Assert.False(method.Invoke(TypeManager.NumberMember(TypeManager.Double(1))).IsReturn(out var _, out var innerMethod));

            Assert.False(innerMethod!.Value.Invoke(TypeManager.NumberMember(TypeManager.Double(2))).IsReturn(out var _, out var res1));
            Assert.Equal(3, res1!.Value.Value);

            Assert.False(innerMethod.Value.Invoke(TypeManager.NumberMember(TypeManager.Double(3))).IsReturn(out var _, out var res2));
            Assert.Equal(6, res2!.Value.Value);

            Assert.False(innerMethod.Value.Invoke(TypeManager.NumberMember(TypeManager.Double(4))).IsReturn(out var _, out var res3));
            Assert.Equal(10, res3!.Value.Value);
        }
    }
}
