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
using Tac.Frontend._3_Syntax_Model.Elements;
using Tac.Model.Operations;
using Tac.SemanticModel.Operations;

namespace Tac.Tests
{
    internal static class TestSupport {

        internal static FileToken Tokenize(string text)
        {

            var tokenizer = new Parser.Tokenizer(StaticSymbolsRegistry.SymbolsRegistry.Symbols.Except(new[] { SymbolsRegistry.StaticSubtractSymbol, SymbolsRegistry.TryAssignSymbol }).ToArray());
            var res = tokenizer.Tokenize(text);
            return res;
        }

        internal static IRootScope Convert(FileToken fileToken)
        {
            var result = ConvertToWeak(fileToken);

            var context = TransformerExtensions.NewConversionContext();

            var converted = result.Convert(context);
            return converted;
        }

        internal static WeakRootScope ConvertToWeak(FileToken fileToken)
        {
            var elementMatchingContest = new ElementMatchingContext();

            var scopePopulators = elementMatchingContest.ParseFile(fileToken);

            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), scopePopulators, _ => { });

            var populateScopeContex = new SetUpContext(problem.builder);
            var referanceResolver = scopePopulators.Run(problem.ModuleRoot, populateScopeContex).Resolve;

            var solution = problem.Solve();

            var res = referanceResolver.Run(solution);

            return res.GetValue();
        }
    }
}
