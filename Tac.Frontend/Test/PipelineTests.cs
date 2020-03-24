using Newtonsoft.Json;
using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Test.Samples;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SemanticModel.CodeStuff;
using Tac.Tests.Help;
using Tac.Tests.Samples;
using Xunit;
using Tac.TestCases;

namespace Tac.Tests
{
    public class PipelineTests
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores
        [Fact]
        public void Token_CodeElements_Factorial()
        {
            Token_CodeElements(new WrappedFactorial());
        }

        [Fact]
        public void Token_CodeElements_Arithmetic()
        {
            Token_CodeElements(new WrappedArithmetic());
        }

        [Fact]
        public void Token_CodeElements_PointObject()
        {
            Token_CodeElements(new WrappedPointObject());
        }

        [Fact]
        public void Token_CodeElements_MirrorPointImplementation()
        {
            Token_CodeElements(new WrappedMirrorPointImplementation());
        }

        [Fact]
        public void Token_CodeElements_Closoure()
        {
            Token_CodeElements(new WrappedClosoure());
        }

        [Fact]
        public void Token_CodeElements_PairType()
        {
            Token_CodeElements(new WrappedPairType());
        }

        [Fact]
        public void Token_CodeElements_Token_Or()
        {
            Token_CodeElements(new WrappedOr());
        }

        private static void Token_CodeElements(IWrappedTestCase sample)
        {

            var target = sample.ModuleDefinition;

            var converted = Convert(sample.Token.SafeCastTo<IToken, FileToken>());

            converted.ValueEqualOrThrow(target);
        }

        private static IModuleDefinition Convert(FileToken fileToken)
        {
            var elementMatchingContest = new ElementMatchingContext();

            var scopePopulators = elementMatchingContest.ParseFile(fileToken);


            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IFrontendCodeElement>, IError>>>(new List<IOrType<IResolve<IFrontendCodeElement>, IError>>()), new NameKey("test module")));

            var populateScopeContex = new SetUpContext(problem);
            var referanceResolvers = scopePopulators.Select(or => or.TransformInner(populateScope => populateScope.Run(problem.ModuleRoot, populateScopeContex).Resolve)).ToArray();


            var solution = problem.Solve();

            var result = referanceResolvers
                .Select(or=>or.TransformInner( reranceResolver => reranceResolver.Run(solution)))
                .ToArray()
                .Single()
                .Is1OrThrow()
                .GetValue()
                .CastTo<WeakModuleDefinition>();


            var context = TransformerExtensions.NewConversionContext();

            var converted = result.Convert(context);
            return converted;
        }

        [Fact]
        public void Text_Token_Factorial()
        {
            Text_Token(new WrappedFactorial());
        }

        [Fact]
        public void Text_Token_Or()
        {
            Text_Token(new WrappedOr());
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

        [Fact]
        public void TokenizeMissingElement() {
            var res =Tokenize("5 + + 10;");
            var converted = Convert(res);
        }

        private static void Text_Token(IWrappedTestCase sample)
        {
            var res = Tokenize(sample.Text);

            var target = sample.Token;

#pragma warning disable IDE0059 // Value assigned to symbol is never used
            var targetJson = JsonConvert.SerializeObject(target);
            var resJson = JsonConvert.SerializeObject(res);
#pragma warning restore IDE0059 // Value assigned to symbol is never used

            Assert.Equal(target.ToString(), res.ToString());
        }

        private static FileToken Tokenize(string text)
        {

            var tokenizer = new Parser.Tokenizer(StaticSymbolsRegistry.SymbolsRegistry.Symbols.ToArray());
            var res = tokenizer.Tokenize(text);
            return res;
        }

        private static void TokenizeAndConvert(ITestCase sample)
        {
            var fileToken = Tokenize(sample.Text);
            var converted = Convert(fileToken);

            converted.ValueEqualOrThrow(sample.ModuleDefinition);
        }


#pragma warning restore CA1707 // Identifiers should not contain underscores
    }
}
