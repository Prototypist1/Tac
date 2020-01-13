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
    public class TokenParser
    {
        
        public IProject<TBacking> Parse<TBacking>(string text,IReadOnlyList<IAssembly<TBacking>> dependencies, string name)
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

                var convertedDependency = dependencyConverter.ConvertToType(dependency);

                var typeKey = new ImplicitKey(Guid.NewGuid());
                var type = problem.CreateType(problem.Dependency, typeKey, throw);
                problem.CreateMember(problem.Dependency, dependency.Key, typeKey, throw);

                AddMembers(problem, type, dependency.Scope.Members);


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

        private IKey GetTypeKey(Tpn.TypeProblem2 problem, Tpn.IScope context, IFrontendType frontendType)
        {

            if (frontendType is SyntaxModel.Elements.AtomicTypes.StringType)
            {
                return new NameKey("string");
            }
            else if (frontendType is SyntaxModel.Elements.AtomicTypes.EmptyType) 
            {
                return new NameKey("empty");
            }
            else if (frontendType is SyntaxModel.Elements.AtomicTypes.NumberType) 
            {
                return new NameKey("number");
            }
            else if (frontendType is SyntaxModel.Elements.AtomicTypes.AnyType)
            {
                return new NameKey("any");
            }
            else if (frontendType is SyntaxModel.Elements.AtomicTypes.BooleanType)
            {
                return new NameKey("bool");
            }
            else if (frontendType is SyntaxModel.Elements.AtomicTypes.MethodType methodType)
            {
                return new GenericNameKey(new NameKey("method"), new IKey[] {
                    GetTypeKey(problem,context,methodType.InputType),
                    GetTypeKey(problem,context,methodType.OutputType)
                });
            }
            else if (frontendType is SyntaxModel.Elements.AtomicTypes.ImplementationType implementation)
            {

                return new GenericNameKey(new NameKey("implementation"), new IKey[] {
                    GetTypeKey(problem,context,implementation.ContextType),
                    GetTypeKey(problem,context,implementation.InputType),
                    GetTypeKey(problem,context,implementation.OutputType)
                });
            }
            else if (frontendType is Tac.Frontend._3_Syntax_Model.Operations.WeakTypeOrOperation typeOr)
            {
                var key = new ImplicitKey(new Guid());
                var left = problem.CreateTypeReference(context, GetTypeKey(problem, context, typeOr.Left.GetValue()), throw);
                var right = problem.CreateTypeReference(context, GetTypeKey(problem, context, typeOr.Right.GetValue()), throw);
                problem.CreateOrType(context, key, left, right, throw);
                return key;
            }
            else if (frontendType is Tac.SemanticModel.WeakMethodDefinition)
            {
                // this is an interesting case.
                // frontendType should never be this
                throw new NotImplementedException();
            }
            else if (frontendType is WeakImplementationDefinition)
            {
                // frontendType should never be this
                throw new NotImplementedException();
            }
            else if (frontendType is WeakObjectDefinition weakObjectDefinition)
            {
                var key = new ImplicitKey(new Guid());
                var nextScope = problem.CreateObjectOrModule(context, key, throw);
                AddMembers(problem,nextScope, weakObjectDefinition.Scope.GetValue());
                return key;
            }
            else if (frontendType is WeakTypeDefinition weakTypeDefinition)
            {
                var key = new ImplicitKey(new Guid());
                var nextScope = problem.CreateType(context, key, throw);
                AddMembers(problem,nextScope, weakTypeDefinition.Scope.GetValue());
                return key;
            }
            else if (frontendType is WeakModuleDefinition weakModuleDefinition)
            {
                var key = new ImplicitKey(new Guid());
                var nextScope = problem.CreateObjectOrModule(context, key, throw);
                AddMembers(problem,nextScope, weakModuleDefinition.Scope.GetValue());
                return key;
            }

            throw new Exception($"that is unexpected! {frontendType.GetType().Name}");
        }

        private void AddMembers(Tpn.TypeProblem2 problem, Tpn.IScope nextScope, WeakScope weakScope)
        {
            foreach (var member in weakScope.membersList.Select(x => x.GetValue()))
            {
                var innerTypeKey = GetTypeKey(problem, nextScope, member.Type.GetValue());
                problem.CreateMember(nextScope, member.Key, innerTypeKey, throw);
            }
        }

        private void AddMembers(Tpn.TypeProblem2 problem, Tpn.IScope nextScope, IReadOnlyList<IMemberDefinition> members)
        {
            foreach (var member in members)
            {
                var innerTypeKey = GetTypeKey(problem, nextScope, member.Type);
                problem.CreateMember(nextScope, member.Key, innerTypeKey, throw);
            }
        }
    }
}
