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
using Tac.SemanticModel;
using Tac.SemanticModel.CodeStuff;

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

            //throw new NotImplementedException("I need to rethink this a bit");

            foreach (var dependency in dependencies)
            {

                // new new WIP


                //var convertedDependency = dependencyConverter.ConvertToType(dependency);

                //var typeKey = new ImplicitKey(Guid.NewGuid());
                //var type = problem.CreateType(problem.Dependency, typeKey, throw);
                //problem.CreateMember(problem.Dependency, dependency.Key, typeKey, throw);

                //foreach (var member in convertedDependency.Scope.GetValue().membersList.Select(x => x.GetValue()))
                //{
                //    var innerTypeKey = GetTypeKey(type, member.Type.GetValue());
                //    problem.CreateMember(type, member.Key, innerTypeKey, throw);
                //}



                // new code, work in progress
                //problem.CreateMember(problem.Dependency, dependency.Key, new WeakMemberDefinitionConverter(true,dependency.Key));
                //var key = new ImplicitKey(Guid.NewGuid());
                //var dependencyModule = problem.CreateObject(problem.Dependency, key, new WeakModuleConverter(new Box<IResolve<IFrontendCodeElement>[]>(Array.Empty<IResolve<IFrontendCodeElement>>()), key));

                //foreach (var member in dependency.Scope.Members)
                //{
                //    problem.CreateType(dependencyModule,  )
                //    problem.CreateMember(dependencyModule, member.Key, new WeakMemberDefinitionConverter(true, member.Key));


                //}


                // the whole dependency is a type
                // 


                // old code
                //var dependencyScope = problem.CreateScope(problem.Base, new WeakScopeConverter());
                //var convertedDependency = dependencyConverter.ConvertToType(dependency);
                //if (!dependendcyScope.TryAddMember(DefintionLifetime.Instance, dependency.Key, new Box<IIsPossibly<WeakMemberDefinition>>(Possibly.Is(
                //    new WeakMemberDefinition(
                //        true,
                //        dependency.Key,
                //        new Box<IFrontendType>(convertedDependency))))))
                //{
                //    throw new Exception("could not add dependency!");
                //}
            }


            var populateScopeContex = new SetUpContext(problem);
            var referanceResolvers = scopePopulators.Select(populateScope => populateScope.Run(problem.ModuleRoot, populateScopeContex).Resolve).ToArray();

            var solution = problem.Solve(new WeakTypeDefinitionConverter());

            var module = referanceResolvers.Select(reranceResolver => reranceResolver.Run(solution)).ToArray().Single().GetValue().CastTo<WeakModuleDefinition>(); ;

            var context = TransformerExtensions.NewConversionContext();

            return new Project<TBacking>(module.Convert(context), dependencies);
        }

        private static IKey GetTypeKey(Tpn.IScope context, IFrontendType frontendType)
        {
            throw new NotImplementedException();

            if (frontendType is SyntaxModel.Elements.AtomicTypes.StringType)
            {

            }
            else if (frontendType is SyntaxModel.Elements.AtomicTypes.EmptyType) { 
            
            }
            else if (frontendType is SyntaxModel.Elements.AtomicTypes.NumberType) {

            }
            else if (frontendType is SyntaxModel.Elements.AtomicTypes.AnyType)
            {

            }
            else if (frontendType is SyntaxModel.Elements.AtomicTypes.BooleanType)
            {

            }
            else if (frontendType is SyntaxModel.Elements.AtomicTypes.MethodType)
            {

            }
            else if (frontendType is SyntaxModel.Elements.AtomicTypes.ImplementationType)
            {

            }
            else if (frontendType is SyntaxModel.Elements.AtomicTypes.ImplementationType)
            {

            }
            else if (frontendType is Tac.Frontend._3_Syntax_Model.Operations.WeakTypeOrOperation)
            {

            }
            else if (frontendType is Tac.SemanticModel.WeakMethodDefinition)
            {

            }
            else if (frontendType is WeakImplementationDefinition)
            {

            }
            else if (frontendType is WeakObjectDefinition)
            {

            }
            else if (frontendType is WeakTypeDefinition)
            {

            }
            else if (frontendType is WeakModuleDefinition)
            {

            }
        }
    }
}
