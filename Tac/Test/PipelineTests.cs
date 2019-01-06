using Newtonsoft.Json;
using Prototypist.LeftToRight;
using System;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.Test.Samples;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Tests.Help;
using Tac.Tests.Samples;
using Xunit;


namespace Tac.Tests
{
    public class PipelineTests
    {
        [Fact]
        public void Token_CodeElements_Factorial()
        {
            Toke_CodeElements(new WrappedFactorial());
        }

        [Fact]
        public void Token_CodeElements_Arithmetic()
        {
            Toke_CodeElements(new WrappedArithmetic());
        }

        [Fact]
        public void Token_CodeElements_PointObject()
        {
            Toke_CodeElements(new WrappedPointObject());
        }

        [Fact]
        public void Token_CodeElements_MirrorPointImplementation()
        {
            Toke_CodeElements(new WrappedMirrorPointImplementation());
        }

        [Fact]
        public void Token_CodeElements_Closoure()
        {
            Toke_CodeElements(new WrappedClosoure());
        }

        [Fact]
        public void Token_CodeElements_PairType()
        {
            Toke_CodeElements(new WrappedPairType());
        }

        private static void Toke_CodeElements(IWrappedTestCase sample) { 

            var elementMatchingContest = new ElementMatchingContext();

            var scopePopulators = elementMatchingContest.ParseFile(sample.Token as FileToken);

            var stack = new NewScope();
            
            var populateScopeContex = new PopulateScopeContext(stack);
            var referanceResolvers = scopePopulators.Select(populateScope => populateScope.Run(populateScopeContex)).ToArray();

            var resolveReferanceContext = new ResolveReferanceContext();
            var result = referanceResolvers.Select(reranceResolver => reranceResolver.Run(resolveReferanceContext)).ToArray().Single().Cast<WeakModuleDefinition>();
            
            var target = sample.Module;
            
            result.ValueEqualOrThrow(target);
            
        }

        [Fact]
        public void Text_Token_Factorial()
        {
            Text_Token(new WrappedFactorial());
        }

        [Fact]
        public void Text_Token_Arithmetic()
        {
            Text_Token(new WrappedArithmetic());
        }

        [Fact]
        public void Text_Token_PointObject()
        {
            Text_Token(new WrappedPointObject());
        }

        [Fact]
        public void Text_Token_PairType()
        {
            Text_Token(new WrappedPairType());
        }

        [Fact]
        public void Text_Token_MirrorPointImplementation()
        {
            Text_Token(new WrappedMirrorPointImplementation());
        }
        
        [Fact]
        public void Text_Token_Closoure()
        {
            Text_Token(new WrappedClosoure());
        }

        private static void Text_Token(IWrappedTestCase sample)
        {
            var text = sample.Text;
            
            var tokenizer = new Parser.Tokenizer(Symbols.GetSymbols());
            var res = tokenizer.Tokenize(text);

            var target = sample.Token;

            var targetJson = JsonConvert.SerializeObject(target);
            var resJson = JsonConvert.SerializeObject(res);

            target.ValueEqualOrThrow(res);
        }
    }
}
