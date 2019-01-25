using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Frontend
{
    public static class TokenParser
    {
        
        public static IModuleDefinition Parse(string text) {

            var tokenizer = new Parser.Tokenizer(Symbols.GetSymbols());
            var tokens = tokenizer.Tokenize(text);

            var elementMatchingContest = new ElementMatchingContext();

            var scopePopulators = elementMatchingContest.ParseFile(tokens);
            
            var stack = new NewScope();

            var populateScopeContex = new PopulateScopeContext(stack);
            var referanceResolvers = scopePopulators.Select(populateScope => populateScope.Run(populateScopeContex)).ToArray();

            var resolveReferanceContext = new ResolveReferanceContext();
            var result = referanceResolvers.Select(reranceResolver => reranceResolver.Run(resolveReferanceContext)).ToArray().Single().GetOrThrow().Cast<WeakModuleDefinition>();

            var context = new ConversionContext();

            return result.Convert<IModuleDefinition>(context);

        }
    }
}
