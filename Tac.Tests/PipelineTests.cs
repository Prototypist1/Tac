using Newtonsoft.Json;
using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Tac.Tests
{
    public class PipelineTests
    {
        [Theory]
        [InlineData(nameof(Factorial))]
        [InlineData(nameof(Arithmetic))]
        [InlineData(nameof(PointObject))]
        public void Token_CodeElements(string className)
        {
            //💩💩💩
            var type = Type.GetType($"Tac.Tests.Samples.{className}, Tac.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            var sample = Activator.CreateInstance(type).Cast<ISample>();

            var tree = new ScopeTree();

            var elementMatchingContest = ElementMatchingContext.Root(tree,
                new InterpeterElementBuilder(),new InterpeterOperationBuilder());

            var res = elementMatchingContest.ParseFile(sample.Token as FileToken);

            var target = sample.CodeElements.ToArray();
            
            Assert.Equal(target.Length, target.Length);
            for (var i = 0; i < target.Length; i++)
            {
                res[i].ValueEqualOrThrow(target[i]);
            }
        }
        
        [Theory]
        [InlineData(nameof(Factorial))]
        [InlineData(nameof(Arithmetic))]
        [InlineData(nameof(PointObject))]
        public void Text_Token(string className)
        {
            //💩💩💩
            var type = Type.GetType($"Tac.Tests.Samples.{className}, Tac.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            var sample = Activator.CreateInstance(type).Cast<ISample>();

            var text = sample.Text;

            var operationBuilder =  new InterpeterOperationBuilder();
            
            var tokenizer = new Tac.Parser.Tokenizer(operationBuilder.Operations.Select(x=>x.Expressed).ToArray());
            var res = tokenizer.Tokenize(text);

            var target = sample.Token;

            var targetJson = JsonConvert.SerializeObject(target);
            var resJson = JsonConvert.SerializeObject(res);

            target.ValueEqualOrThrow(res);
        }

    }
}
