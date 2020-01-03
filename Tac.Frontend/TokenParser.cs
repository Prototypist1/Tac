using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend.New.CrzayNamespace;
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

            var tokenizer = new Tokenizer(StaticSymbolsRegistry.SymbolsRegistry.Symbols.ToArray());
            var tokens = tokenizer.Tokenize(text);

            var elementMatchingContest = new ElementMatchingContext();

            var scopePopulators = elementMatchingContest.ParseFile(tokens);

            var dependencyConverter = new DependencyConverter();

            var problem = new Tpn.TypeProblem2(new WeakScopeConverter());

            throw new NotImplementedException("I need to rethink this a bit");
            
            //foreach (var dependency in dependencies)
            //{
            //    //var dependencyScope = problem.CreateScope(problem.Base, new WeakScopeConverter());
            //    problem.CreateMember(problem.Base, dependency.Key );
            //    var convertedDependency = dependencyConverter.ConvertToType<TBacking>(dependency);
            //    if (!dependendcyScope.TryAddMember(DefintionLifetime.Instance, dependency.Key, new Box<IIsPossibly<WeakMemberDefinition>>(Possibly.Is(
            //        new WeakMemberDefinition(
            //            true, 
            //            dependency.Key, 
            //            new Box<IFrontendType>(convertedDependency)))))) {
            //        throw new Exception("could not add dependency!");
            //    }
            //}


            var populateScopeContex = new SetUpContext(problem);
            var referanceResolvers = scopePopulators.Select(populateScope => populateScope.Run(problem.Root, populateScopeContex).Resolve).ToArray();

            var solution = problem.Solve(new WeakTypeDefinitionConverter());

            var module = referanceResolvers.Select(reranceResolver => reranceResolver.Run(solution)).ToArray().Single().GetValue().CastTo<WeakModuleDefinition>(); ;

            var context = TransformerExtensions.NewConversionContext();

            return new Project<TBacking>(module.Convert(context), dependencies);
        }
    }
}
