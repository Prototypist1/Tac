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

        public IOrType< IProject<TAssembly, TBacking>, IReadOnlyList<IError>> Parse<TAssembly,TBacking>(string text, IReadOnlyList<TAssembly> dependencies, string name)
            where TAssembly: IAssembly<TBacking>
        {

            var tokenizer = new Tokenizer(StaticSymbolsRegistry.SymbolsRegistry.Symbols.Except(new[] { SymbolsRegistry.StaticSubtractSymbol }).ToArray());
            var tokens = tokenizer.Tokenize(text);

            var elementMatchingContest = new ElementMatchingContext();

            var scopePopulator = elementMatchingContest.ParseFile(tokens);

            var dependencyConverter = new DependencyConverter();

            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), scopePopulator, prob => {

                foreach (var dependency in dependencies)
                {
                    var type = prob.builder.CreateTypeExternalType(prob.Dependency, new WeakTypeDefinitionConverter(), dependency.Scope);

                    foreach (var memberPair in dependency.Scope.Members)
                    {
                        var innerType = ConvertType(prob.builder, type, OrType.Make<IVerifiableType, IError>(memberPair.Type));
                        innerType.Switch(x =>
                        {
                            prob.builder.CreatePublicMember(type, type, memberPair.Key, OrType.Make<IKey, IError>(x));
                        }, y =>
                        {
                            prob.builder.CreatePublicMember(type, memberPair.Key, y);
                        });
                    }
                    prob.builder.CreatePrivateMember(prob.Dependency, dependency.Key, OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError >(type));

                }
            });


            var populateScopeContex = new SetUpContext(problem.builder);
            var referanceResolver = scopePopulator.Run(problem.ModuleRoot, populateScopeContex).Resolve;

            var solution = problem.Solve();

            var rootScope = referanceResolver.Run(solution,new List<Tpn.ITypeProblemNode>()).GetValue();

            var errors = rootScope.Validate().ToArray();

            if (errors.Any()) {
                return OrType.Make<IProject<TAssembly, TBacking>, IReadOnlyList<IError>>(errors);
            }

            var dependencyScope = problem.Dependency.Converter.Convert(solution, problem.Dependency).Is2OrThrow();


            var context = TransformerExtensions.NewConversionContext();

            return OrType.Make<IProject<TAssembly, TBacking>, IReadOnlyList<IError>>(new Project<TAssembly, TBacking>(rootScope.Convert(context), dependencies, dependencyScope.Convert(context)));
        }

        private OrType<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>> ConvertType(
            Tpn.TypeProblem2.Builder problem,
            Tpn.IStaticScope scope,
            IOrType<IVerifiableType, IError> typeOrError)
        {

            return typeOrError.SwitchReturns(type =>
            {

                if (type is NumberType numberType)
                {
                    return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType,Tpn.TypeProblem2.GenericTypeParameter, IError>>(new NameKey("number"));
                }
                else if (type is EmptyType emptyType)
                {
                    return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType,Tpn.TypeProblem2.GenericTypeParameter, IError>>(new NameKey("empty"));
                }
                else if (type is BooleanType booleanTyp)
                {
                    return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType,Tpn.TypeProblem2.GenericTypeParameter, IError>>(new NameKey("bool"));
                }
                else if (type is BlockType blockType)
                {
                    throw new Exception("can't be asked");
                }
                else if (type is StringType stringType)
                {
                    return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType,Tpn.TypeProblem2.GenericTypeParameter, IError>>(new NameKey("string"));
                }
                else if (type is AnyType anyType)
                {
                    throw new Exception("can't be asked");
                }

                if (type is InterfaceType interfaceType)
                {
                    if (!typeCache.TryGetValue(type, out var res))
                    {
                        return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>>(OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(res!));
                    }

                    // we don't know the type key...
                    var tpnType = problem.CreateTypeExternalType(scope, new WeakTypeDefinitionConverter(), interfaceType);
                    typeCache[type] = tpnType;
                    var orType = OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(tpnType);
                    foreach (var memberPair in interfaceType.Members)
                    {
                        var innerType = ConvertType(problem, tpnType, OrType.Make<IVerifiableType, IError>(memberPair.Type));
                        innerType.Switch(x =>
                        {
                            problem.CreatePublicMember(tpnType, tpnType, memberPair.Key, OrType.Make<IKey, IError>(x));
                        }, y =>
                        {
                            problem.CreatePublicMember(tpnType, memberPair.Key, y);
                        });
                    }
                    return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>>(orType);
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
                        return OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>>(new GenericNameKey(new NameKey("method"), new IOrType<IKey, IError>[] {
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
            , x => OrType.Make<IKey, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>>(
                 OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(x)));

        }
    }
}
