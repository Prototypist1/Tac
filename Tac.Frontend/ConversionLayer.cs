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
        public readonly IOrType<IBox<IFrontendType>,IError> Type;

        public PlaceholderValue(IOrType<IBox<IFrontendType>, IError> testType)
        {
            Type = testType ?? throw new ArgumentNullException(nameof(testType));
        }
    }


    internal class UnWrappingTypeBox : IBox<IFrontendType>
    {
        private readonly IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> box;

        public UnWrappingTypeBox(IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> box)
        {
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IFrontendType GetValue() => box.GetValue().SwitchReturns<IFrontendType>(x => x.FrontendType(), x => x.FrontendType(), x => x);
    }

    internal class UnWrappingOrBox : IBox<IFrontendType>
    {
        private readonly IBox<WeakTypeOrOperation> box;

        public UnWrappingOrBox(IBox<WeakTypeOrOperation> box)
        {
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IFrontendType GetValue() => box.GetValue().FrontendType();
    }


    internal class UnWrappingObjectBox : IBox<IFrontendType>
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
                return inner1.AssuredReturns();
            }
            else if (inner.Is2(out var inner2))
            {
                return inner2.AssuredReturns();
            }
            else
            {
                throw new Exception("blarg");
            }
        }
    }

    // {D27D98BA-96CF-402C-824C-744DACC63FEE}
    // I have a lot of GetValue on this page
    // I am sure they are ok when they are passed in the consturctor
    // but if they running of typeSolution they are not really safe

    internal static class Help
    {
        public static WeakScope GetScope(Tpn.TypeSolution typeSolution, Tpn.IHavePrivateMembers haveMembers)
        {
            // ah, there needs to be 2 typeSolution.GetMember
            // one for TypeProblem2.Member
            // the other for members that come out of the flow nodes
            // 

            return new WeakScope(typeSolution.GetPrivateMembers(haveMembers).Select(x => typeSolution.GetMember(x)).ToList());
        }

        public static WeakScope GetScope(Tpn.TypeSolution typeSolution,  Tpn.IVirtualFlowNode haveMembers)
        {
            // ah, there needs to be 2 typeSolution.GetMember
            // one for TypeProblem2.Member
            // the other for members that come out of the flow nodes
            // 

            return new WeakScope(typeSolution.GetPublicMembers(haveMembers).Select(x => typeSolution.GetMember(x, new WeakMemberDefinitionConverter(false, x.Key))).ToList());
        }

        public static IOrType< IBox<IFrontendType>,IError> GetType(Tpn.TypeSolution typeSolution, Tpn.ILookUpType lookUpType)
        {
            return typeSolution.GetFlowNode(lookUpType).SwitchReturns(
                v1 => OrType.Make<IBox<IFrontendType>, IError>(new Box<IFrontendType>(typeSolution.GetMethodType(v1.Source.GetOrThrow()).GetValue())),
                v2 => OrType.Make<IBox<IFrontendType>, IError>(new UnWrappingTypeBox(typeSolution.GetExplicitType(v2.Source.GetOrThrow()))),
                v3 => OrType.Make<IBox<IFrontendType>, IError>(new UnWrappingObjectBox(typeSolution.GetObject(v3.Source.GetOrThrow()))),
                v4 => OrType.Make<IBox<IFrontendType>, IError>(new UnWrappingOrBox(typeSolution.GetOrType(v4.Source.GetOrThrow()))),
                v5 => OrType.Make<IBox<IFrontendType>, IError>(typeSolution.GetInferredType(new Tpn.VirtualNode( v5.ToRep(), v5.SourcePath()))),
                v6 => OrType.Make<IBox<IFrontendType>, IError>(v6)
                );
        }
    }

    internal class InferredTypeConverter : Tpn.IConvertTo<Tpn.CombinedTypesAnd, IFrontendType>
    {
        public IFrontendType Convert(Tpn.TypeSolution typeSolution, Tpn.CombinedTypesAnd flowNode)
        {
            if (flowNode.And.Count == 0)
            {
                return new AnyType();
            }

            if (flowNode.Primitive().Is(out var prim)) {
                var single = flowNode.And.Single().Is2OrThrow() ;
                return typeSolution.GetExplicitType(single.Source.GetOrThrow()).GetValue().Is3OrThrow();
            }

            //var flowNode = typeSolution.GetFlowNode(from);

            var scope = Help.GetScope(typeSolution, flowNode);

            typeSolution.TryGetInputMember(flowNode, out var input);
            typeSolution.TryGetResultMember(flowNode, out var output);

            if ((input != default || output != default) && scope.membersList.Count > 1)
            {
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
                        typeSolution.GetType(OrType.Make<Tpn.IVirtualFlowNode, IError>(input)).TransformInner(x => x.GetValue().CastTo<IConvertableFrontendType<IVerifiableType>>()),
                        typeSolution.GetType(OrType.Make<Tpn.IVirtualFlowNode, IError>(output)).TransformInner(x => x.GetValue().CastTo<IConvertableFrontendType<IVerifiableType>>()));
            }


            if (input != default)
            {
                // I don't think this is safe see:
                //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                return
                    new MethodType(
                        typeSolution.GetType(OrType.Make<Tpn.IVirtualFlowNode, IError>(input)).TransformInner(x => x.GetValue().SafeCastTo<IFrontendType, IConvertableFrontendType<IVerifiableType>>()),
                        OrType.Make<IConvertableFrontendType<IVerifiableType>, IError>(new EmptyType()));
            }

            if (output != default)
            {
                // I don't think this is safe see:
                //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                return
                    new MethodType(
                        OrType.Make<IConvertableFrontendType<IVerifiableType>, IError>(new EmptyType()),
                        typeSolution.GetType(OrType.Make<Tpn.IVirtualFlowNode, IError>(output)).TransformInner(x => x.GetValue().SafeCastTo<IFrontendType, IConvertableFrontendType<IVerifiableType>>()));
            }

            // if it has members it must be a scope
            if (scope.membersList.Any())
            {
                return new WeakTypeDefinition(new Box<WeakScope>(scope)).FrontendType();
            }

            return new AnyType();
        }
    }

    internal class InferredTypeConverter2 : Tpn.IConvertTo<Tpn.VirtualNode, IFrontendType>
    {
        public IFrontendType Convert(Tpn.TypeSolution typeSolution, Tpn.VirtualNode flowNode)
        {
            if (flowNode.Or.Count == 0) {
                return new AnyType();
            }

            if (flowNode.Or.Count == 1)
            {
                return typeSolution.GetInferredType(flowNode.Or.First()).GetValue();
            }

            // make a big Or!
            var array = flowNode.Or.ToArray();
            var first = array[0];
            var second = array[1];
            var res = new FrontEndOrType(OrType.Make<IFrontendType, IError>(typeSolution.GetInferredType(first).GetValue()), OrType.Make<IFrontendType, IError>(typeSolution.GetInferredType(second).GetValue()));
            foreach (var entry in array.Skip(2))
            {
                res = new FrontEndOrType(OrType.Make<IFrontendType, IError>(res), OrType.Make<IFrontendType, IError>(typeSolution.GetInferredType(entry).GetValue()));
            }

            return res;

        }
    }

    internal class WeakTypeDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>
    {

        public WeakTypeDefinitionConverter()
        {
        }

        public IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Type from)
        {
            var placeHolders = typeSolution.HasPlacholders(from);

            return placeHolders.IfElseReturn(x =>
            {
                return OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>(
                    new WeakGenericTypeDefinition(
                        from.Key,
                        new Box<WeakScope>(Help.GetScope(typeSolution, typeSolution.GetFlowNode(from))),
                        x.Select(x=> Possibly.Is<IGenericTypeParameterPlacholder>(new GenericTypeParameterPlacholder(x))).ToArray()));//, key
            },
            () =>
            {
                return OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>(new WeakTypeDefinition(new Box<WeakScope>(Help.GetScope(typeSolution, typeSolution.GetFlowNode(from)))));//, key ?
            });

        }
    }


    internal class PrimitiveTypeConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>
    {
        public PrimitiveTypeConverter(IPrimitiveType primitiveType)
        {
            PrimitiveType = primitiveType;
        }

        public IPrimitiveType PrimitiveType { get; }

        public IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Type from)
        {
            return OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>(PrimitiveType);
        }
    }


    //internal class WeakGenericTypeDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>
    //{

    //    private readonly NameKey key;
    //    private readonly IGenericTypeParameterPlacholder[] TypeParameterDefinitions;

    //    public WeakGenericTypeDefinitionConverter(NameKey key, IGenericTypeParameterPlacholder[] typeParameterDefinitions)
    //    {
    //        this.key = key ?? throw new ArgumentNullException(nameof(key));
    //        TypeParameterDefinitions = typeParameterDefinitions ?? throw new ArgumentNullException(nameof(typeParameterDefinitions));
    //    }

    //    public IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Type from)
    //    {
    //        return OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>(
    //            new WeakGenericTypeDefinition(
    //                Possibly.Is(key),
    //                new Box<WeakScope>(Help.GetScope(typeSolution, from)),
    //                TypeParameterDefinitions.Select(x => Possibly.Is(x)).ToArray()));//, key
    //    }
    //}


    internal class MethodTypeConverter : Tpn.IConvertTo<Tpn.TypeProblem2.MethodType, MethodType>
    {
        public MethodTypeConverter()
        {
        }

        public MethodType Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.MethodType from)
        {
            // TODO I added the CastTo b/c I am sick of it not compiling
            // 

            // I don't think this is safe see:
            //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
            return
                new MethodType(
                    typeSolution.GetType(typeSolution.GetFlowNode2(from.Input.GetOrThrow()))
                    .TransformInner(x=>x.GetValue().CastTo<IConvertableFrontendType<IVerifiableType>>()),
                    typeSolution.GetType(typeSolution.GetFlowNode2(from.Returns.GetOrThrow()))
                    .TransformInner(x => x.GetValue().CastTo< IConvertableFrontendType<IVerifiableType>>()));
        }
    }

    internal class WeakMethodDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>>
    {
        private readonly IBox<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>> body;
        private readonly bool isEntryPoint;

        public WeakMethodDefinitionConverter(IBox<IReadOnlyList<IOrType< IResolve<IBox<IFrontendCodeElement>>,IError>>> body, bool isEntryPoint)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
            this.isEntryPoint = isEntryPoint;
        }

        public IOrType<WeakMethodDefinition, WeakImplementationDefinition> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Method from)
        {
            return OrType.Make<WeakMethodDefinition, WeakImplementationDefinition>( new WeakMethodDefinition(
                typeSolution.GetType(typeSolution.GetFlowNode2(from.Returns.GetOrThrow())),
                 typeSolution.GetMember(from.Input.GetOrThrow()),
                body.GetValue().Select(x => x.TransformInner(y=>y.Run(typeSolution))).ToArray(),
                new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                Array.Empty<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>>()));
        }

        
    }

    internal class WeakImplementationDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>>
    {

        private readonly IBox<IResolve<IBox<IFrontendCodeElement>>[]> body;
        private readonly 
            IBox<
                Tpn.TypeProblem2.Method
            > inner;

        public WeakImplementationDefinitionConverter(IBox<IResolve<IBox<IFrontendCodeElement>>[]> body, IBox<Tpn.TypeProblem2.Method> inner)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public IOrType<WeakMethodDefinition, WeakImplementationDefinition> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Method from)
        {
            //
            

            return OrType.Make<WeakMethodDefinition, WeakImplementationDefinition>(new WeakImplementationDefinition(
                typeSolution.GetMember(from.Input.GetOrThrow()), // that is never going to work!
                typeSolution.GetMember(inner.GetValue().Input.GetOrThrow()),
                typeSolution.GetType(typeSolution.GetFlowNode2( inner.GetValue().Returns.GetOrThrow())),
                body.GetValue().Select(x => x.Run(typeSolution)).ToArray(),
                new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                Array.Empty<IFrontendCodeElement>()));
        }

    }

    //internal class WeakMemberDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Member, WeakMemberDefinition>
    //{
    //    private readonly bool isReadonly;
    //    private readonly IKey nameKey;

    //    public WeakMemberDefinitionConverter(bool isReadonly, IKey nameKey)
    //    {
    //        this.isReadonly = isReadonly;
    //        this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
    //    }

    //    public WeakMemberDefinition Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Member from)
    //    {
    //        return new WeakMemberDefinition(isReadonly, nameKey, Help.GetType(typeSolution, from));
    //    }
    //}

    internal class WeakMemberDefinitionConverter : Tpn.IConvertTo<IOrType<Tpn.IVirtualFlowNode, IError>, WeakMemberDefinition>
    {
        private readonly bool isReadonly;
        private readonly IKey nameKey;

        public WeakMemberDefinitionConverter(bool isReadonly, IKey nameKey)
        {
            this.isReadonly = isReadonly;
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
        }

        public WeakMemberDefinition Convert(Tpn.TypeSolution typeSolution, IOrType< Tpn.IVirtualFlowNode, IError> from)
        {
            return new WeakMemberDefinition(isReadonly, nameKey, typeSolution.GetType(from));
        }
    }

    internal class WeakTypeReferenceConverter : Tpn.IConvertTo<Tpn.TypeProblem2.TypeReference, IOrType< IFrontendType,IError>>
    {
        public IOrType< IFrontendType,IError> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.TypeReference from)
        {
            // I don't think this is safe see:
            // {D27D98BA-96CF-402C-824C-744DACC63FEE}
            return Help.GetType(typeSolution, from).TransformInner(x=>x.GetValue());
        }
    }

    internal class WeakTypeOrOperationConverter : Tpn.IConvertTo<Tpn.TypeProblem2.OrType, WeakTypeOrOperation>
    {
        public WeakTypeOrOperation Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.OrType from)
        {
            var (left, right) = typeSolution.GetOrTypeElements(from);
            return new WeakTypeOrOperation(Help.GetType(typeSolution, left), Help.GetType(typeSolution, right));
        }
    }

    internal class PlaceholderValueConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Value, PlaceholderValue>
    {
        public PlaceholderValue Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Value from)
        {
            return new PlaceholderValue(Help.GetType(typeSolution, from));
        }
    }

    internal class WeakBlockDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Scope, IOrType<WeakBlockDefinition, WeakScope,WeakEntryPointDefinition>>
    {

        private readonly IBox<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[]> body;

        public WeakBlockDefinitionConverter(IBox<IOrType<IResolve<IBox<IFrontendCodeElement>>,IError>[]> body)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Scope from)
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

        private readonly IBox<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[]> body;

        public WeakEntryPointConverter(IBox<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[]> body)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Scope from)
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

        public IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Scope from)
        {
            return OrType.Make<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>(Help.GetScope(typeSolution, from));
        }
    }

    internal class WeakObjectConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>>
    {
        private readonly Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>> box;

        public WeakObjectConverter(Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>> box)
        {
            this.box = box;
        }

        public IOrType<WeakObjectDefinition, WeakModuleDefinition> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Object from)
        {
            return OrType.Make<WeakObjectDefinition, WeakModuleDefinition>(new WeakObjectDefinition(
                new Box<WeakScope>(Help.GetScope(typeSolution, typeSolution.GetFlowNode(from))),
                box.GetValue().Select(x => x.SwitchReturns<IOrType<IBox<WeakAssignOperation>, IError>>(
                    y=> { 
                        var res = y.Run(typeSolution).GetValue();
                        if (res is WeakAssignOperation weakAssign)
                        {
                            return OrType.Make<IBox<WeakAssignOperation>, IError>(new Box<WeakAssignOperation>(weakAssign));
                        }
                        else {
                            return OrType.Make<IBox<WeakAssignOperation>, IError>(Error.Other("lines in an object must me assignments"));
                        }
                    },
                    y=> OrType.Make<IBox<WeakAssignOperation>, IError>(y))).ToArray()));
        }
    }

    internal class WeakModuleConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>>
    {
        private readonly Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>,IError>>> box;
        private readonly IKey key;

        public WeakModuleConverter(Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>,IError>>> box, IKey key)
        {
            this.box = box;
            this.key = key;
        }

        public IOrType<WeakObjectDefinition, WeakModuleDefinition> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Object from)
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
                new Box<WeakScope>(Help.GetScope(typeSolution, typeSolution.GetFlowNode(from))),
                box.GetValue().Select(x => x.TransformInner(y=> y.Run(typeSolution))).ToArray(), 
                key,
                new Box<WeakEntryPointDefinition>(weakEntryPoint)));
        }
    }

}
