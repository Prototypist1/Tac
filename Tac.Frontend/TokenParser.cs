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
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Frontend
{
    public static class TokenParser
    {
        
        public static IProject<TBacking> Parse<TBacking>(string text,IReadOnlyList<IAssembly<TBacking>> dependencies)
            where TBacking:IBacking
        {

            var tokenizer = new Parser.Tokenizer(Symbols.GetSymbols());
            var tokens = tokenizer.Tokenize(text);

            var elementMatchingContest = new ElementMatchingContext();

            var scopePopulators = elementMatchingContest.ParseFile(tokens);

            var dependencyConverter = new DependencyConverter();

            var stack = new NewScope();

            foreach (var dependency in dependencies)
            {
                var convertedDependency = dependencyConverter.ConvertToType<TBacking>(dependency);
                if (!stack.TryAddMember(DefintionLifetime.Instance, dependency.Key, new Box<IIsPossibly<WeakMemberDefinition>>(Possibly.Is(
                    new WeakMemberDefinition(true, dependency.Key, Possibly.Is(
                        new WeakTypeReference(
                            Possibly.Is(
                                new Box<IIsPossibly<IFrontendType<IVerifiableType>>>(
                                    Possibly.Is<IFrontendType<IVerifiableType>>(
                                        convertedDependency)))))))))) {
                    throw new Exception("could not add dependency!");
                }
            }

            var populateScopeContex = new PopulateScopeContext(stack);
            var referanceResolvers = scopePopulators.Select(populateScope => populateScope.Run(populateScopeContex)).ToArray();

            var resolveReferanceContext = new ResolveReferanceContext();
            var result = referanceResolvers.Select(reranceResolver => reranceResolver.Run(resolveReferanceContext)).ToArray().Single().GetOrThrow().Cast<WeakModuleDefinition>();

            var context = new ConversionContext();

            var module = result.Convert<IModuleDefinition>(context);

            return new Project<TBacking>(module, dependencies);
        }
    }
}
