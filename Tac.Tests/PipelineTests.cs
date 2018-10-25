using Newtonsoft.Json;
using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter;
using Tac.Tests.Help;
using Tac.Tests.Samples;
using Tac.Tests.Tokenizer;
using Xunit;
using static Tac.Semantic_Model.ScopeTree;

namespace Tac.Tests
{
    public class PipelineTests
    {
        [Fact]
        public void Token_CodeElements_Factorial()
        {
            Toke_CodeElements(new Factorial());
        }

        [Fact]
        public void Token_CodeElements_Arithmetic()
        {
            Toke_CodeElements(new Arithmetic());
        }

        [Fact]
        public void Token_CodeElements_PointObject()
        {
            Toke_CodeElements(new PointObject());
        }

        private static void Toke_CodeElements(ISample sample)
        {
            var builders = new InterpeterElementBuilder();
            var elementMatchingContest = new ElementMatchingContext(
                builders, new InterpeterOperationBuilder());

            var scopePopulators = elementMatchingContest.ParseFile(sample.Token as FileToken);

            var (scope, stack) = ScopeStack.Root(builders);
            var populateScopeContex = new PopulateScopeContext(stack, scope, builders);
            var referanceResolvers = scopePopulators.Select(populateScope => populateScope.Run(populateScopeContex)).ToArray();

            var resolveReferanceContext = new ResolveReferanceContext(builders);
            var result = referanceResolvers.Select(reranceResolver => reranceResolver.Run(resolveReferanceContext)).ToArray();
            
            var target = sample.CodeElements.ToArray();

            Assert.Equal(result.Length, target.Length);
            for (var i = 0; i < result.Length; i++)
            {
                result[i].ValueEqualOrThrow(target[i]);
            }
        }

        [Fact]
        public void Text_Token_Factorial()
        {
            Text_Token(new Factorial());
        }

        [Fact]
        public void Text_Token_Arithmetic()
        {
            Text_Token(new Arithmetic());
        }

        [Fact]
        public void Text_Token_PointObject()
        {
            Text_Token(new PointObject());
        }

        private static void Text_Token(ISample sample)
        {
            var text = sample.Text;

            var operationBuilder = new InterpeterOperationBuilder();

            var tokenizer = new Tac.Parser.Tokenizer(operationBuilder.Identifiers);
            var res = tokenizer.Tokenize(text);

            var target = sample.Token;

            var targetJson = JsonConvert.SerializeObject(target);
            var resJson = JsonConvert.SerializeObject(res);

            target.ValueEqualOrThrow(res);
        }
    }
}
