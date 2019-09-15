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

            var dependendcyScope = new PopulatableScope();

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

            var scope = new PopulatableScope(dependendcyScope);

            var populateScopeContex = new PopulateScopeContext();
            var referanceResolvers = scopePopulators.Select(populateScope => populateScope.Run(scope,populateScopeContex)).ToArray();

            var resolvableDependencyScope = dependendcyScope.GetResolvelizableScope().FinalizeScope();

            var resolvalbe = scope.GetResolvelizableScope().FinalizeScope(resolvableDependencyScope);
            var finalizeScopeContext = new FinalizeScopeContext();
            var populateBoxes = referanceResolvers.Select(reranceResolver => reranceResolver.Run(resolvalbe, finalizeScopeContext)).ToArray();

             var resolveReferenceContext = new ResolveReferanceContext();

            var module = new WeakModuleDefinition(
                resolvalbe,
                populateBoxes.Select(reranceResolver => reranceResolver.Run(resolvalbe, resolveReferenceContext)).ToArray(), 
                new NameKey(name));

            var resolveReferanceContext = new ResolveReferanceContext();


            var context = TransformerExtensions.NewConversionContext();


            return new Project<TBacking>(module.Convert<IModuleDefinition>(context), dependencies);
        }
    }
}
