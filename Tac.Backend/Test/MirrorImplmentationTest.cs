using Prototypist.LeftToRight;
using System.Collections.Generic;
using System.Linq;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Model;
using Tac.Semantic_Model.Names;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.TestCases.Samples;
using Tac.Tests.Samples;
using Xunit;

namespace Tac.Backend.Test
{
    public class MirrorImplmentationTest {

        [Fact]
        public void Test()
        {
            var testCase = new MirrorPointImplementation();
            var conversionContext = new Definitions();
            var lines = testCase.CodeElements.Select(x => x.Convert(conversionContext)).ToArray();

            var currentScope =
                InterpetedContext.Root();

            foreach (var scopeLayer in finalizedScopes())
            {
                currentScope = currentScope.Child(InterpetedInstanceScope.Make(scopeLayer));
            }

            var implementation = Assert.Single(lines).Interpet(currentScope).Get<InterpetedImplementation>();

            var context = InterpetedInstanceScope.Make(
                (new NameKey("x"), new InterpetedMember(new RuntimeNumber(5))),
                (new NameKey("y"), new InterpetedMember(new RuntimeNumber(7))));

            implementation.Invoke(context).Get<InterpetedMethod>().Invoke(new RunTimeEmpty());

            Assert.Equal(7,context.GetMember(new NameKey("x")).Value.Cast<RuntimeNumber>().d);
            Assert.Equal(5, context.GetMember(new NameKey("y")).Value.Cast<RuntimeNumber>().d);

            IEnumerable<IFinalizedScope> finalizedScopes()
            {
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
