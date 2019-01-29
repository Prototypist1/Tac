﻿using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Tests.Samples;
using Xunit;

namespace Tac.Backend.Test
{
    public class PairTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Test(double input)
        {
            Pairify(input);
        }

        private void Pairify(double d)
        {
            var testCase = new PairType();
            var conversionContext = new Definitions(new ExternalMethodSource(new Dictionary<Guid, InterpetedExternalMethodDefinition>()));

            var res = testCase.Module.Convert(conversionContext).Interpet(InterpetedContext.Root());

            var method = res.Get<IInterpetedScope>().GetMember(new NameKey("pairify")).Value.Cast<InterpetedMethod>();

            var scope = method.Invoke(new RuntimeNumber(d)).Get<InterpetedInstanceScope>();

            Assert.Equal(d,scope.GetMember(new NameKey("x")).Value.Cast<RuntimeNumber>().d);
            Assert.Equal(d, scope.GetMember(new NameKey("y")).Value.Cast<RuntimeNumber>().d);
        }
    }
}
