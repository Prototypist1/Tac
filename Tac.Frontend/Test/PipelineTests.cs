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

namespace Tac.Tests
{
    public class PipelineTests
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores
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

        [Fact]
        public void Token_CodeElements_Token_Or()
        {
            Toke_CodeElements(new WrappedOr());
        }

        private static void Toke_CodeElements(IWrappedTestCase sample) { 

            var elementMatchingContest = new ElementMatchingContext();

            var scopePopulators = elementMatchingContest.ParseFile(sample.Token.SafeCastTo<IToken,FileToken>());


            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IResolve<IFrontendCodeElement>[]>(Array.Empty<IResolve<IFrontendCodeElement>>()), new NameKey("test module")));

            var populateScopeContex = new SetUpContext(problem);
            var referanceResolvers = scopePopulators.Select(populateScope => populateScope.Run(problem.ModuleRoot, populateScopeContex).Resolve).ToArray();


            var solution = problem.Solve();

            var result = referanceResolvers.Select(reranceResolver => reranceResolver.Run(solution)).ToArray().Single().GetValue().CastTo<WeakModuleDefinition>();

            var target = sample.Module;

            var context = TransformerExtensions.NewConversionContext();

            var converted = result.Convert(context);
            
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
            var text = sample.Text;
            
            var tokenizer = new Parser.Tokenizer(StaticSymbolsRegistry.SymbolsRegistry.Symbols.ToArray());
            var res = tokenizer.Tokenize(text);

            var target = sample.Token;

#pragma warning disable IDE0059 // Value assigned to symbol is never used
            var targetJson = JsonConvert.SerializeObject(target);
            var resJson = JsonConvert.SerializeObject(res);
#pragma warning restore IDE0059 // Value assigned to symbol is never used

            Assert.Equal(target.ToString(), res.ToString());
        }


#pragma warning restore CA1707 // Identifiers should not contain underscores
    }
}
