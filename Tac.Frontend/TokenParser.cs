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
using Tac.Infastructure;
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

            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox< IFrontendCodeElement>>, IError>>>(Array.Empty<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            foreach (var dependency in dependencies)
            {

                var type = problem.CreateType(problem.Dependency, new WeakTypeDefinitionConverter());
                foreach (var memberPair in dependency.Scope.Members)
                {
                    var innerType = ConvertType(problem, type, OrType.Make<IVerifiableType, IError>( memberPair.Value.Value.Type));
                    innerType.Switch(x =>
                    {
                        problem.CreateMember(type, memberPair.Key, OrType.Make<IKey, IError>(x), new WeakMemberDefinitionConverter(true, memberPair.Key));
                    }, y =>
                    {
                        problem.CreateMember(type, memberPair.Key, y, new WeakMemberDefinitionConverter(true, memberPair.Key));
                    });
                }
                problem.CreateMember(problem.Dependency, dependency.Key, OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(type), new WeakMemberDefinitionConverter(true, dependency.Key));

            }

            var populateScopeContex = new SetUpContext(problem);
            var referanceResolvers = scopePopulators.Select(or => or.TransformInner(populateScope => populateScope.Run(problem.ModuleRoot, populateScopeContex).Resolve)).ToArray();

            var solution = problem.Solve();

            referanceResolvers.Select(or => or.TransformInner(reranceResolver => reranceResolver.Run(solution))).ToArray();

            var moduleDefinition = solution.GetObject(problem.ModuleRoot).GetValue().Is2OrThrow();

            var context = TransformerExtensions.NewConversionContext();

            return new Project<TBacking>(moduleDefinition.Convert(context), dependencies);
        }

        private OrType<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>> ConvertType(
            Tpn.ISetUpTypeProblem problem,
            Tpn.IStaticScope scope,
            IOrType<IVerifiableType, IError> typeOrError)
        {

            return typeOrError.SwitchReturns(type =>
            {

                if (type is NumberType numberType)
                {
                    return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>>(new NameKey("number"));
                }
                else if (type is EmptyType emptyType)
                {
                    return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>>(new NameKey("empty"));
                }
                else if (type is BooleanType booleanTyp)
                {
                    return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>>(new NameKey("bool"));
                }
                else if (type is BlockType blockType)
                {
                    throw new Exception("can't be asked");
                }
                else if (type is StringType stringType)
                {
                    return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>>(new NameKey("string"));
                }
                else if (type is AnyType anyType)
                {
                    throw new Exception("can't be asked");
                }

                if (type is InterfaceType interfaceType)
                {
                    if (!typeCache.TryGetValue(type, out var res))
                    {
                        return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>>(OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(res!));
                    }

                    // we don't know the type key...
                    var tpnType = problem.CreateType(scope, new WeakTypeDefinitionConverter());
                    typeCache[type] = tpnType;
                    var orType = OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(tpnType);
                    foreach (var memberPair in interfaceType.Members)
                    {
                        var innerType = ConvertType(problem, tpnType, OrType.Make<IVerifiableType, IError>(memberPair.Type));
                        innerType.Switch(x =>
                        {
                            problem.CreateMember(tpnType, memberPair.Key, OrType.Make<IKey, IError>(x), new WeakMemberDefinitionConverter(true, memberPair.Key));
                        }, y =>
                        {
                            problem.CreateMember(tpnType, memberPair.Key, y, new WeakMemberDefinitionConverter(true, memberPair.Key));
                        });
                    }
                    return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>>(orType);
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
                    var input = ConvertType(problem, scope, OrType.Make<IVerifiableType, IError>(methodType.InputType));
                    var output = ConvertType(problem, scope, OrType.Make<IVerifiableType, IError>(methodType.OutputType));
                    if (input.Is2(out var i2) && output.Is2(out var o2))
                    {
                        problem.GetMethod(i2, o2);
                    }
                    else if (input.Is1(out var i1) && output.Is1(out var o1))
                    {
                        return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>>(new GenericNameKey(new NameKey("method"), new IOrType<IKey, IError>[] {
                        OrType.Make<IKey, IError>(i1), OrType.Make<IKey, IError>(o1)
                    }));
                    }
                    else
                    {
                        throw new Exception("they should not be mixed");
                    }
                }
                throw new Exception("you have entered the area that does not exist");
            }
            , x => OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>>(
                 OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(x)));

        }
    }
}
