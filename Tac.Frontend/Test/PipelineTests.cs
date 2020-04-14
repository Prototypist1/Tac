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
using Tac.Model.Operations;

namespace Tac.Tests
{

    public class BadCode {

        [Fact]
        public void TokenizeMissingElement()
        {
            var res = TestSupport.Tokenize("module tokenize-missing-element { 5 + + 10 =: x ; }");
            var converted = TestSupport.Convert(res);
            var line = Assert.Single(converted.StaticInitialization);
            var validLine = line.Is1OrThrow();
            var assignOperation = validLine.SafeCastTo<ICodeElement, IAssignOperation>();
            var addition = assignOperation.Left.Is1OrThrow();
            var addOperation = addition.SafeCastTo<ICodeElement, IAddOperation>();
            addOperation.Left.Is2OrThrow();
        }

        [Fact]
        public void MissingCurleyBracket()
        {
            var res = TestSupport.Tokenize("module test { 5 + 10 =: x ; ");
            var converted = TestSupport.Convert(res);

            throw new NotImplementedException();
        }

        [Fact]
        public void MissingSquareBracket()
        {
            var res = TestSupport.Tokenize(@" module tet { method [ number ; number ;  input { input return ;} =: pass-through ; }");
            var converted = TestSupport.Convert(res);

            throw new NotImplementedException();
        }

        [Fact]
        public void YouCantInvokeANumber()
        {
            var res = TestSupport.Tokenize("module test { 5 =: x ; 5 > x ; }");
            var converted = TestSupport.Convert(res);

            throw new NotImplementedException();
        }

        // I think this actaully will not error out here
        // a is used before it is assigned 
        // but that is flow analysis 
        [Fact]
        public void UndefinedVariable()
        {
            var res = TestSupport.Tokenize("module test { a + 2 =: x ; }");
            var converted = TestSupport.Convert(res);

            throw new NotImplementedException();
        }

        [Fact]
        public void UndefinedType()
        {
            var res = TestSupport.Tokenize(@" module test { method [ chicken ; number ; ] input { 1 return ;} =: chicken-to-one ; }");
            var converted = TestSupport.Convert(res);

            throw new NotImplementedException();
        }
    }

    internal static class TestSupport {

        internal static FileToken Tokenize(string text)
        {

            var tokenizer = new Parser.Tokenizer(StaticSymbolsRegistry.SymbolsRegistry.Symbols.ToArray());
            var res = tokenizer.Tokenize(text);
            return res;
        }

        internal static IModuleDefinition Convert(FileToken fileToken)
        {
            var elementMatchingContest = new ElementMatchingContext();

            var scopePopulators = elementMatchingContest.ParseFile(fileToken);


            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var populateScopeContex = new SetUpContext(problem);
            var referanceResolvers = scopePopulators.Select(or => or.TransformInner(populateScope => populateScope.Run(problem.ModuleRoot, populateScopeContex).Resolve)).ToArray();


            var solution = problem.Solve();

            var result = referanceResolvers
                .Select(or => or.TransformInner(reranceResolver => reranceResolver.Run(solution)))
                .ToArray()
                .Single()
                .Is1OrThrow()
                .GetValue()
                .SafeCastTo<IFrontendCodeElement, WeakModuleDefinition>();


            var context = TransformerExtensions.NewConversionContext();

            var converted = result.Convert(context);
            return converted;
        }

    }


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

            var converted = TestSupport.Convert(sample.Token.SafeCastTo<IToken, FileToken>());

            converted.ValueEqualOrThrow(target);
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


        private static void Text_Token(IWrappedTestCase sample)
        {
            var res = TestSupport.Tokenize(sample.Text);

            var target = sample.Token;

#pragma warning disable IDE0059 // Value assigned to symbol is never used
            var targetJson = JsonConvert.SerializeObject(target);
            var resJson = JsonConvert.SerializeObject(res);
#pragma warning restore IDE0059 // Value assigned to symbol is never used

            Assert.Equal(target.ToString(), res.ToString());
        }

        private static void TokenizeAndConvert(ITestCase sample)
        {
            var fileToken = TestSupport.Tokenize(sample.Text);
            var converted = TestSupport.Convert(fileToken);

            converted.ValueEqualOrThrow(sample.ModuleDefinition);
        }


#pragma warning restore CA1707 // Identifiers should not contain underscores
    }
}
