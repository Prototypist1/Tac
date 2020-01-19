using Prototypist.Toolbox;
using Prototypist.Toolbox.Dictionary;
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

        private readonly Dictionary<IVerifiableType, Tpn.TypeProblem2.Type> typeCache = new Dictionary<IVerifiableType, Tpn.TypeProblem2.Type>();

        public IProject<TBacking> Parse<TBacking>(string text, IReadOnlyList<IAssembly<TBacking>> dependencies, string name)
            where TBacking : IBacking
        {

            var tokenizer = new Tokenizer(StaticSymbolsRegistry.SymbolsRegistry.Symbols.ToArray());
            var tokens = tokenizer.Tokenize(text);

            var elementMatchingContest = new ElementMatchingContext();

            var scopePopulators = elementMatchingContest.ParseFile(tokens);

            var dependencyConverter = new DependencyConverter();

            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IResolve<IFrontendCodeElement>[]>(Array.Empty<IResolve<IFrontendCodeElement>>()), new NameKey("test module")));

            foreach (var dependency in dependencies)
            {

                var type = problem.CreateType(problem.Dependency, new WeakTypeDefinitionConverter());
                foreach (var memberPair in dependency.Scope.Members)
                {
                    var innerType = ConvertType(problem,type, memberPair.Value.Value.Type); innerType.Switch(x => {
                        problem.CreateMember(type, memberPair.Key, x, new WeakMemberDefinitionConverter(true, memberPair.Key));
                    }, y =>
                    {
                        problem.CreateMember(type, memberPair.Key, y, new WeakMemberDefinitionConverter(true, memberPair.Key));
                    });
                }
                problem.CreateMember(problem.Dependency, dependency.Key, new OrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType>(type), new WeakMemberDefinitionConverter(true, dependency.Key));

            }

            var populateScopeContex = new SetUpContext(problem);
            var referanceResolvers = scopePopulators.Select(populateScope => populateScope.Run(problem.ModuleRoot, populateScopeContex).Resolve).ToArray();

            var solution = problem.Solve();

            referanceResolvers.Select(reranceResolver => reranceResolver.Run(solution)).ToArray();

            var moduleDefinition = solution.GetObject(problem.ModuleRoot).GetValue().Is2OrThrow();

            var context = TransformerExtensions.NewConversionContext();

            return new Project<TBacking>(moduleDefinition.Convert(context), dependencies);
        }

        private OrType<IKey, OrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType>> ConvertType(
            Tpn.ISetUpTypeProblem problem,
            Tpn.IScope scope,
            IVerifiableType type)
        {

            if (type is NumberType numberType)
            {
                return new OrType<IKey, OrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType>>(new NameKey("number"));
            }
            else if (type is EmptyType emptyType)
            {
                return new OrType<IKey, OrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType>>(new NameKey("empty"));
            }
            else if (type is BooleanType booleanTyp)
            {
                return new OrType<IKey, OrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType>>(new NameKey("bool"));
            }
            else if (type is BlockType blockType)
            {
                throw new Exception("can't be asked");
            }
            else if (type is StringType stringType)
            {
                return new OrType<IKey, OrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType>>(new NameKey("string"));
            }
            else if (type is AnyType anyType)
            {
                throw new Exception("can't be asked");
            }

            if (type is InterfaceType interfaceType)
            {
                if (!typeCache.TryGetValue(type, out var res))
                {
                    return new OrType<IKey, OrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType>>(new OrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType>(res!));
                }

                // we don't know the type key...
                var tpnType = problem.CreateType(scope, new WeakTypeDefinitionConverter());
                typeCache[type] = tpnType;
                var orType = new OrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType>(tpnType);
                foreach (var memberPair in interfaceType.Members)
                {
                    var innerType = ConvertType(problem,tpnType,memberPair.Type);
                    innerType.Switch(x => { 
                    
                        problem.CreateMember(tpnType, memberPair.Key, x, new WeakMemberDefinitionConverter(true, memberPair.Key));
                    }, y =>
                    {
                        problem.CreateMember(tpnType, memberPair.Key, y, new WeakMemberDefinitionConverter(true, memberPair.Key));
                    });
                }
                return new OrType<IKey, OrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType>>(orType);
            }
            else if (type is TypeOr typeOr)
            {
                throw new Exception("i don't want to think about it");
            }
            else if (type is TypeAnd typeAnd)
            {
                throw new Exception("i don't want to think about it");
            }
            else if (type is MethodType methodType)
            {
                var input = ConvertType(problem, scope, methodType.InputType);
                var output = ConvertType(problem, scope, methodType.OutputType);
                if (input.Is2(out var i2) && output.Is2(out var o2))
                {
                    problem.GetMethod(i2, o2);
                }
                else if (input.Is1(out var i1) && output.Is1(out var o1))
                {
                    return new OrType<IKey, OrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType>>(new GenericNameKey(new NameKey("method"), new IKey[] { 
                        i1,o1
                    }));
                }
                else {
                    throw new Exception("they should not be mixed");
                }
            }
            else if (type is ImplementationType implementationType)
            {
                throw new Exception("can't be asked");
            }
            throw new Exception("you have entered the area that does not exist");
        }

        //private IKey GetTypeKey(Tpn.TypeProblem2 problem, Tpn.IScope context, IFrontendType frontendType)
        //{

        //    if (frontendType is SyntaxModel.Elements.AtomicTypes.StringType)
        //    {
        //        return new NameKey("string");
        //    }
        //    else if (frontendType is SyntaxModel.Elements.AtomicTypes.EmptyType) 
        //    {
        //        return new NameKey("empty");
        //    }
        //    else if (frontendType is SyntaxModel.Elements.AtomicTypes.NumberType) 
        //    {
        //        return new NameKey("number");
        //    }
        //    else if (frontendType is SyntaxModel.Elements.AtomicTypes.AnyType)
        //    {
        //        return new NameKey("any");
        //    }
        //    else if (frontendType is SyntaxModel.Elements.AtomicTypes.BooleanType)
        //    {
        //        return new NameKey("bool");
        //    }
        //    else if (frontendType is SyntaxModel.Elements.AtomicTypes.MethodType methodType)
        //    {
        //        return new GenericNameKey(new NameKey("method"), new IKey[] {
        //            GetTypeKey(problem,context,methodType.InputType),
        //            GetTypeKey(problem,context,methodType.OutputType)
        //        });
        //    }
        //    else if (frontendType is SyntaxModel.Elements.AtomicTypes.ImplementationType implementation)
        //    {

        //        return new GenericNameKey(new NameKey("implementation"), new IKey[] {
        //            GetTypeKey(problem,context,implementation.ContextType),
        //            GetTypeKey(problem,context,implementation.InputType),
        //            GetTypeKey(problem,context,implementation.OutputType)
        //        });
        //    }
        //    else if (frontendType is Tac.Frontend._3_Syntax_Model.Operations.WeakTypeOrOperation typeOr)
        //    {
        //        var key = new ImplicitKey(new Guid());
        //        var left = problem.CreateTypeReference(context, GetTypeKey(problem, context, typeOr.Left.GetValue()), throw);
        //        var right = problem.CreateTypeReference(context, GetTypeKey(problem, context, typeOr.Right.GetValue()), throw);
        //        problem.CreateOrType(context, key, left, right, throw);
        //        return key;
        //    }
        //    else if (frontendType is Tac.SemanticModel.WeakMethodDefinition)
        //    {
        //        // this is an interesting case.
        //        // frontendType should never be this
        //        throw new NotImplementedException();
        //    }
        //    else if (frontendType is WeakImplementationDefinition)
        //    {
        //        // frontendType should never be this
        //        throw new NotImplementedException();
        //    }
        //    else if (frontendType is WeakObjectDefinition weakObjectDefinition)
        //    {
        //        var key = new ImplicitKey(new Guid());
        //        var nextScope = problem.CreateObjectOrModule(context, key, throw);
        //        AddMembers(problem,nextScope, weakObjectDefinition.Scope.GetValue());
        //        return key;
        //    }
        //    else if (frontendType is WeakTypeDefinition weakTypeDefinition)
        //    {
        //        var key = new ImplicitKey(new Guid());
        //        var nextScope = problem.CreateType(context, key, throw);
        //        AddMembers(problem,nextScope, weakTypeDefinition.Scope.GetValue());
        //        return key;
        //    }
        //    else if (frontendType is WeakModuleDefinition weakModuleDefinition)
        //    {
        //        var key = new ImplicitKey(new Guid());
        //        var nextScope = problem.CreateObjectOrModule(context, key, throw);
        //        AddMembers(problem,nextScope, weakModuleDefinition.Scope.GetValue());
        //        return key;
        //    }

        //    throw new Exception($"that is unexpected! {frontendType.GetType().Name}");
        //}

        //private void AddMembers(Tpn.TypeProblem2 problem, Tpn.IScope nextScope, WeakScope weakScope)
        //{
        //    foreach (var member in weakScope.membersList.Select(x => x.GetValue()))
        //    {
        //        var innerTypeKey = GetTypeKey(problem, nextScope, member.Type.GetValue());
        //        problem.CreateMember(nextScope, member.Key, innerTypeKey, throw);
        //    }
        //}

        //private void AddMembers(Tpn.TypeProblem2 problem, Tpn.IScope nextScope, IReadOnlyList<IMemberDefinition> members)
        //{
        //    foreach (var member in members)
        //    {
        //        var innerTypeKey = GetTypeKey(problem, nextScope, member.Type);
        //        problem.CreateMember(nextScope, member.Key, innerTypeKey, throw);
        //    }
        //}
    }
}
