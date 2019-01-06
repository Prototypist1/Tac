using Prototypist.LeftToRight;
using System.Collections.Generic;
using System.Linq;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Tests.Samples;
using Xunit;

namespace Tac.Backend.Test
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
            var lines = testCase.CodeElements.Select(x => x.Convert(conversionContext)).ToArray();

            var currentScope = 
                InterpetedContext.Root();

            foreach (var scopeLayer in finalizedScopes())
            {
                currentScope = currentScope.Child(InterpetedInstanceScope.Make(scopeLayer));
            }
            
            var method = Assert.Single(lines).Interpet(currentScope).Get<InterpetedMethod>();

            currentScope.GetMember(new NameKey("fac")).Value = method;
            
            return method.Invoke(new RuntimeNumber(d)).Get<RuntimeNumber>().d;

            IEnumerable<IFinalizedScope> finalizedScopes() {
                var items = new List<IFinalizedScope>();
                var at = testCase.Scope;
                do
                {
                    items.Add(at);
                } while (testCase.Scope.TryGetParent(out at));
                items.Reverse();
                return items;
            }
        }

    }
}
