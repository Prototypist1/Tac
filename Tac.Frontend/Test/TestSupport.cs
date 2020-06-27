using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SemanticModel.CodeStuff;

namespace Tac.Tests
{
    internal static class TestSupport {

        internal static FileToken Tokenize(string text)
        {

            var tokenizer = new Parser.Tokenizer(StaticSymbolsRegistry.SymbolsRegistry.Symbols.ToArray());
            var res = tokenizer.Tokenize(text);
            return res;
        }

        internal static IModuleDefinition Convert(FileToken fileToken)
        {
            var result = ConvertToWeak(fileToken);

            var context = TransformerExtensions.NewConversionContext();

            var converted = result.Convert(context);
            return converted;
        }

        internal static WeakModuleDefinition ConvertToWeak(FileToken fileToken)
        {
            var elementMatchingContest = new ElementMatchingContext();

            var scopePopulators = elementMatchingContest.ParseFile(fileToken);


            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var populateScopeContex = new SetUpContext(problem.builder);
            var referanceResolvers = scopePopulators.Select(or => or.TransformInner(populateScope => populateScope.Run(problem.ModuleRoot, populateScopeContex).Resolve)).ToArray();


            var solution = problem.Solve();

            var result = referanceResolvers
                .Select(or => or.TransformInner(reranceResolver => reranceResolver.Run(solution)))
                .ToArray()
                .Single()
                .Is1OrThrow()
                .GetValue()
                .SafeCastTo<IFrontendCodeElement, WeakModuleDefinition>();
            return result;
        }
    }
}
