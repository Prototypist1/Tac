using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Frontend
{
    public static class TokenParser
    {
        
        public static IProject<TBacking> Parse<TBacking>(string text,IReadOnlyList<IAssembly<TBacking>> dependencies, string name)
            where TBacking:IBacking
        {

            var tokenizer = new Parser.Tokenizer(StaticSymbolsRegistry.SymbolsRegistry.Symbols.ToArray());
            var tokens = tokenizer.Tokenize(text);

            var elementMatchingContest = new ElementMatchingContext();

            var scopePopulators = elementMatchingContest.ParseFile(tokens);

            var dependencyConverter = new DependencyConverter();

            var dependendcyScope = new Semantic_Model.ResolvableScope();

            foreach (var dependency in dependencies)
            {
                var convertedDependency = dependencyConverter.ConvertToType<TBacking>(dependency);
                if (!dependendcyScope.TryAddMember(DefintionLifetime.Instance, dependency.Key, new Box<IIsPossibly<WeakMemberDefinition>>(Possibly.Is(
                    new WeakMemberDefinition(true, dependency.Key, Possibly.Is(
                        new WeakTypeReference(
                            Possibly.Is(
                                new Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>>(
                                    Possibly.Is<IConvertableFrontendType<IVerifiableType>>(
                                        convertedDependency)))))))))) {
                    throw new Exception("could not add dependency!");
                }
            }

            var scope = new Semantic_Model.ResolvableScope(dependendcyScope);

            var populateScopeContex = new PopulateScopeContext(scope);
            var referanceResolvers = scopePopulators.Select(populateScope => populateScope.Run(populateScopeContex)).ToArray();

            var resolveReferanceContext = new ResolveReferanceContext();

            var context = TransformerExtensions.NewConversionContext();

            var module = new WeakModuleDefinition(
                scope, 
                referanceResolvers.Select(reranceResolver => reranceResolver.Run(resolveReferanceContext)).ToArray(), 
                new NameKey(name));

            return new Project<TBacking>(module.Convert<IModuleDefinition>(context), dependencies);
        }
    }
}
