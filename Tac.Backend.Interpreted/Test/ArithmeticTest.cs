﻿using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Elements;
using Tac.Model;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;
using Tac.Tests.Samples;
using Xunit;
using Prototypist.Toolbox.Object;

namespace Tac.Backend.Interpreted.Test
{
    public class ArithmeticTest
    {
        [Fact]
        public void Test() {
            var testCase = new Arithmetic();
            var conversionContext = new Definitions();
            var module = testCase.RootScope.Convert(conversionContext);

            var res = module.Interpet(InterpetedContext.Root());

            Assert.False(res.IsReturn(out var _, out var member));

            var scope = member!.Value.CastTo<IInterpetedScope>();
            var x = scope.GetMember(new NameKey("x"));

            Assert.Equal(63.0, x.Value.CastTo<IBoxedDouble>().Value);
        }
    }
}
