using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Infastructure;
using Tac.SemanticModel;
using Tac.SemanticModel.Operations;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Parser;

namespace Tac.Frontend
{
    internal class PlaceholderValue
    {
        public readonly IBox<IFrontendType> Type;

        public PlaceholderValue(IBox<IFrontendType> testType)
        {
            Type = testType ?? throw new ArgumentNullException(nameof(testType));
        }
    }

    // {D27D98BA-96CF-402C-824C-744DACC63FEE}
    // I have a lot of GetValue on this page
    // I am sure they are ok when they are passed in the consturctor
    // but if they running of typeSolution they are not really safe

    internal static class Help
    {
        public static WeakScope GetScope(Tpn.ITypeSolution typeSolution, Tpn.IHaveMembers haveMembers)
        {

            var members = typeSolution.GetMembers(haveMembers);
            var membersList = new List<IBox<WeakMemberDefinition>>();
            foreach (var member in members)
            {
                membersList.Add(typeSolution.GetMember(member));
            }
            return new WeakScope(membersList);
        }

        private class UnWrappingTypeBox : IBox<IFrontendType>
        {
            private readonly IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> box;

            public UnWrappingTypeBox(IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> box)
            {
                this.box = box ?? throw new ArgumentNullException(nameof(box));
            }

            public IFrontendType GetValue()
            {
                var inner = box.GetValue();
                if (inner.Is1(out var inner1))
                {
                    return inner1;
                }
                else if (inner.Is2(out var inner2))
                {
                    return inner2;
                }
                else if (inner.Is3(out var inner3))
                {
                    return inner3;
                }
                else
                {
                    throw new Exception("wish there was a clearner way to do this");
                }
            }
        }


        private class UnWrappingObjectBox : IBox<IFrontendType>
        {
            private readonly IBox<IOrType<WeakObjectDefinition, WeakModuleDefinition>> box;

            public UnWrappingObjectBox(IBox<IOrType<WeakObjectDefinition, WeakModuleDefinition>> box)
            {
                this.box = box ?? throw new ArgumentNullException(nameof(box));
            }

            public IFrontendType GetValue()
            {
                var inner = box.GetValue();
                if (inner.Is1(out var inner1))
                {
                    return inner1;
                }
                else if (inner.Is2(out var inner2))
                {
                    return inner2;
                }
                else
                {
                    throw new Exception("blarg");
                }
            }
        }

        public static IBox<IFrontendType> GetType(Tpn.ITypeSolution typeSolution, Tpn.ILookUpType lookUpType)
        {
            return typeSolution.GetType(lookUpType).SwitchReturns(
                v1 => new Box<IFrontendType>(typeSolution.GetMethodType(v1).GetValue()),
                v2 => new UnWrappingTypeBox(typeSolution.GetExplicitType(v2)),
                v3 => new UnWrappingObjectBox(typeSolution.GetObject(v3)),
                v4 => typeSolution.GetOrType(v4),
                v5 => typeSolution.GetInferredType(v5, new InferredTypeConverter()));
        }
    }

    internal class InferredTypeConverter : Tpn.IConvertTo<Tpn.TypeProblem2.InferredType, IFrontendType>
    {
        public IFrontendType Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.InferredType from)
        {


            var scope = Help.GetScope(typeSolution, from);
            typeSolution.TryGetInputMember(OrType.Make<Tpn.TypeProblem2.Method, Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.InferredType>(from), out var input);
            typeSolution.TryGetResultMember(OrType.Make<Tpn.TypeProblem2.Method, Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.InferredType>(from), out var output);

            if ((input != default || output != default) && scope.membersList.Count > 1) {
                // this might be wrong
                // methods might end up with more than one member
                // input counts as a member but it is really something different
                // todo
                throw new Exception("so... this is a type and a method?!");
            }

            if (input != default && output != default)
            {
                // I don't think this is safe see:
                //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                return
                    new MethodType(
                        Help.GetType(typeSolution, input).GetValue().CastTo<IConvertableFrontendType<IVerifiableType>>(),
                        Help.GetType(typeSolution, output).GetValue().CastTo<IConvertableFrontendType<IVerifiableType>>());
            }


            if (input != default)
            {
                // I don't think this is safe see:
                //  {D27D98BA-96CF-402C-824C-744DACC63FEE}e
                return
                    new MethodType(
                        Help.GetType(typeSolution, input).GetValue().CastTo<IConvertableFrontendType<IVerifiableType>>(),
                        new EmptyType());
            }

            if (output != default)
            {
                // I don't think this is safe see:
                //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                return
                    new MethodType(
                        new EmptyType(),
                        Help.GetType(typeSolution, output).GetValue().CastTo<IConvertableFrontendType<IVerifiableType>>());
            }

            return new WeakTypeDefinition(new Box<WeakScope>(scope));
        }
    }

    internal class WeakTypeDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>
    {

        public WeakTypeDefinitionConverter()
        {
        }

        public IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Type from)
        {
            return OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>(new WeakTypeDefinition(new Box<WeakScope>(Help.GetScope(typeSolution, from))));//, key
        }
    }


    internal class PrimitiveTypeConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>
    {
        public PrimitiveTypeConverter(IPrimitiveType primitiveType)
        {
            PrimitiveType = primitiveType;
        }

        public IPrimitiveType PrimitiveType { get; }

        public IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Type from)
        {
            return OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>(PrimitiveType);
        }
    }


    internal class WeakGenericTypeDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>
    {

        private readonly NameKey key;
        private readonly IGenericTypeParameterPlacholder[] TypeParameterDefinitions;

        public WeakGenericTypeDefinitionConverter(NameKey key, IGenericTypeParameterPlacholder[] typeParameterDefinitions)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            TypeParameterDefinitions = typeParameterDefinitions ?? throw new ArgumentNullException(nameof(typeParameterDefinitions));
        }

        public IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Type from)
        {
            return OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>(
                new WeakGenericTypeDefinition(
                    Possibly.Is(key),
                    new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                    TypeParameterDefinitions.Select(x => Possibly.Is(x)).ToArray()));//, key
        }
    }


    internal class MethodTypeConverter : Tpn.IConvertTo<Tpn.TypeProblem2.MethodType, MethodType>
    {
        public MethodTypeConverter()
        {
        }

        public MethodType Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.MethodType from)
        {
            // TODO I added the CastTo b/c I am sick of it not compiling
            // 

            // I don't think this is safe see:
            //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
            return
                new MethodType(
                    Help.GetType(typeSolution, typeSolution.GetInputMember(OrType.Make<Tpn.TypeProblem2.Method, Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.InferredType>(from))).GetValue().CastTo<IConvertableFrontendType<IVerifiableType>>(),
                    Help.GetType(typeSolution, typeSolution.GetResultMember(OrType.Make<Tpn.TypeProblem2.Method, Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.InferredType>(from))).GetValue().CastTo< IConvertableFrontendType<IVerifiableType>>());
        }
    }

    internal class WeakMethodDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>>
    {
        private readonly IBox<IReadOnlyList<IOrType<IResolve<IFrontendCodeElement>, IError>>> body;
        private readonly bool isEntryPoint;

        public WeakMethodDefinitionConverter(IBox<IReadOnlyList<IOrType< IResolve<IFrontendCodeElement>,IError>>> body, bool isEntryPoint)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
            this.isEntryPoint = isEntryPoint;
        }

        public IOrType<WeakMethodDefinition, WeakImplementationDefinition> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Method from)
        {
            return OrType.Make<WeakMethodDefinition, WeakImplementationDefinition>( new WeakMethodDefinition(
                Help.GetType(typeSolution, typeSolution.GetResultMember(OrType.Make<Tpn.TypeProblem2.Method, Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.InferredType>( from))),
                typeSolution.GetMember(typeSolution.GetInputMember(OrType.Make<Tpn.TypeProblem2.Method, Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.InferredType>( from))),
                body.GetValue().Select(x => x.TransformInner(y=>y.Run(typeSolution))).ToArray(),
                new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                Array.Empty<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>>()));
        }

        
    }

    internal class WeakImplementationDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>>
    {

        private readonly IBox<IResolve<IFrontendCodeElement>[]> body;
        private readonly 
            IBox<
                Tpn.TypeProblem2.Method
            > inner;

        public WeakImplementationDefinitionConverter(IBox<IResolve<IFrontendCodeElement>[]> body, IBox<Tpn.TypeProblem2.Method> inner)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public IOrType<WeakMethodDefinition, WeakImplementationDefinition> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Method from)
        {
            return OrType.Make<WeakMethodDefinition, WeakImplementationDefinition>(new WeakImplementationDefinition(
                typeSolution.GetMember(typeSolution.GetInputMember(OrType.Make<Tpn.TypeProblem2.Method, Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.InferredType>( from))),
                typeSolution.GetMember(typeSolution.GetInputMember(OrType.Make<Tpn.TypeProblem2.Method, Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.InferredType>( inner.GetValue()))),
                Help.GetType(typeSolution, typeSolution.GetResultMember(OrType.Make<Tpn.TypeProblem2.Method, Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.InferredType>( inner.GetValue()))),
                body.GetValue().Select(x => x.Run(typeSolution)).ToArray(),
                new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                Array.Empty<IFrontendCodeElement>()));
        }

    }

    internal class WeakMemberDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Member, WeakMemberDefinition>
    {
        private readonly bool isReadonly;
        private readonly IKey nameKey;

        public WeakMemberDefinitionConverter(bool isReadonly, IKey nameKey)
        {
            this.isReadonly = isReadonly;
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
        }

        public WeakMemberDefinition Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Member from)
        {
            return new WeakMemberDefinition(isReadonly, nameKey, Help.GetType(typeSolution, from));
        }
    }

    internal class WeakTypeReferenceConverter : Tpn.IConvertTo<Tpn.TypeProblem2.TypeReference, IFrontendType>
    {
        public IFrontendType Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.TypeReference from)
        {
            // I don't think this is safe see:
            // {D27D98BA-96CF-402C-824C-744DACC63FEE}
            return Help.GetType(typeSolution, from).GetValue();
        }
    }

    internal class WeakTypeOrOperationConverter : Tpn.IConvertTo<Tpn.TypeProblem2.OrType, WeakTypeOrOperation>
    {
        public WeakTypeOrOperation Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.OrType from)
        {
            var (left, right) = typeSolution.GetOrTypeElements(from);
            return new WeakTypeOrOperation(Help.GetType(typeSolution, left), Help.GetType(typeSolution, right));
        }
    }

    internal class PlaceholderValueConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Value, PlaceholderValue>
    {
        public PlaceholderValue Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Value from)
        {
            return new PlaceholderValue(Help.GetType(typeSolution, from));
        }
    }

    internal class WeakBlockDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Scope, IOrType<WeakBlockDefinition, WeakScope,WeakEntryPointDefinition>>
    {

        private readonly IBox<IOrType<IResolve<IFrontendCodeElement>, IError>[]> body;

        public WeakBlockDefinitionConverter(IBox<IOrType<IResolve<IFrontendCodeElement>,IError>[]> body)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Scope from)
        {
            return OrType.Make<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>(
                new WeakBlockDefinition(
                    body.GetValue().Select(or => or.TransformInner(x=>x.Run(typeSolution))).ToArray(),
                    new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                    Array.Empty<IIsPossibly<IFrontendCodeElement>>()));
        }
    }

    internal class WeakEntryPointConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>
    {

        private readonly IBox<IOrType<IResolve<IFrontendCodeElement>, IError>[]> body;

        public WeakEntryPointConverter(IBox<IOrType<IResolve<IFrontendCodeElement>, IError>[]> body)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Scope from)
        {
            return OrType.Make<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>(
                new WeakEntryPointDefinition(
                    body.GetValue().Select(x => x.TransformInner(y=>y.Run(typeSolution))).ToArray(),
                    new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                    Array.Empty<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>>()));
        }
    }

    internal class WeakScopeConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>
    {
        public WeakScopeConverter()
        {
        }

        public IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Scope from)
        {
            return OrType.Make<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>(Help.GetScope(typeSolution, from));
        }
    }

    internal class WeakObjectConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>>
    {
        private readonly Box<IReadOnlyList<IOrType<IResolve<IFrontendCodeElement>, IError>>> box;

        public WeakObjectConverter(Box<IReadOnlyList<IOrType<IResolve<IFrontendCodeElement>, IError>>> box)
        {
            this.box = box;
        }

        public IOrType<WeakObjectDefinition, WeakModuleDefinition> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Object from)
        {
            return OrType.Make<WeakObjectDefinition, WeakModuleDefinition>(new WeakObjectDefinition(
                new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                box.GetValue().Select(x => x.SwitchReturns<IOrType<IBox<WeakAssignOperation>, IError>>(
                    y=> { 
                        var res = y.Run(typeSolution).GetValue();
                        if (res is WeakAssignOperation weakAssign)
                        {
                            return OrType.Make<IBox<WeakAssignOperation>, IError>(new Box<WeakAssignOperation>(weakAssign));
                        }
                        else {
                            return OrType.Make<IBox<WeakAssignOperation>, IError>(new Error("lines in an object must me assignments"));
                        }
                    },
                    y=> OrType.Make<IBox<WeakAssignOperation>, IError>(y))).ToArray()));
        }
    }

    internal class WeakModuleConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>>
    {
        private readonly Box<IReadOnlyList<IOrType<IResolve<IFrontendCodeElement>,IError>>> box;
        private readonly IKey key;

        public WeakModuleConverter(Box<IReadOnlyList<IOrType<IResolve<IFrontendCodeElement>,IError>>> box, IKey key)
        {
            this.box = box;
            this.key = key;
        }

        public IOrType<WeakObjectDefinition, WeakModuleDefinition> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Object from)
        {
            WeakEntryPointDefinition weakEntryPoint;
            if (typeSolution.GetEntryPoint(from) is IIsDefinately<Tpn.TypeProblem2.Scope> scope)
            {
                weakEntryPoint = typeSolution.GetScope(scope.Value).GetValue().Is3OrThrow();
            }
            else {
                weakEntryPoint = new WeakEntryPointDefinition(
                    Array.Empty<IOrType<IBox<IFrontendCodeElement>,IError>>(),
                    new Box<WeakScope>(new WeakScope(new List<IBox<WeakMemberDefinition>>())),
                    Array.Empty<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>>());
            }


            return OrType.Make<WeakObjectDefinition, WeakModuleDefinition>(new WeakModuleDefinition(
                new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                box.GetValue().Select(x => x.TransformInner(y=> y.Run(typeSolution))).ToArray(), 
                key,
                new Box<WeakEntryPointDefinition>(weakEntryPoint)));
        }
    }

}
