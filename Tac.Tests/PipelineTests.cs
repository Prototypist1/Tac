using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;
using Tac.Tests.Samples;
using Tac.Tests.Tokenizer;
using Xunit;

namespace Tac.Tests.Parser
{
    public class PipelineTests
    {
        [Theory]
        [InlineData(nameof(Factorial))]
        [InlineData(nameof(Arithmetic))]
        public void Token_CodeElements(string className)
        {
            var sample = (ISample)Activator.CreateInstance(Type.GetType(className));
            
            var res = TokenParser.ParseFile(sample.Token as FileToken);

            var target = sample.CodeElements.ToArray();

            Assert.Equal(target.Length, target.Length);
            for (int i = 0; i < target.Length; i++)
            {
                Assert.Equal(res[i], target[i]);
            }
        }
        
        [Theory]
        [InlineData(nameof(Factorial))]
        [InlineData(nameof(Arithmetic))]
        public void Text_Token(string className)
        {
            var sample = (ISample)Activator.CreateInstance(Type.GetType(className));

            var text = sample.Text;

            var tokenizer = new Tac.Parser.Tokenizer();
            var res = tokenizer.Tokenize(text);

            var target = sample.Token;

            Assert.Equal(target, res);
        }

    }
}
