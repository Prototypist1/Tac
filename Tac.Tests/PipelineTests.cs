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
            var elementMatchingContest = new ElementMatchingContext(
                new InterpeterElementBuilder(), new InterpeterOperationBuilder());

            var scopePopulators = elementMatchingContest.ParseFile(sample.Token as FileToken);

            foreach (var populateScope in scopePopulators)
            {
                var (scope,stack) = ScopeStack.Root();
                populateScope.Run(new PopulateScopeContext(stack,scope,new InterpeterElementBuilder()));
            }

            string s = 5;
            // TODO you are here!
            // we still need to resolve referances!

            var target = sample.CodeElements.ToArray();

            Assert.Equal(target.Length, target.Length);
            for (var i = 0; i < target.Length; i++)
            {
                scopePopulators[i].ValueEqualOrThrow(target[i]);
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
