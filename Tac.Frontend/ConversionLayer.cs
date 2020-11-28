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
using Tac.Frontend._3_Syntax_Model.Elements;

namespace Tac.Frontend
{
    internal class PlaceholderValue
    {
        public readonly IBox<IOrType<IFrontendType, IError>> Type;

        public PlaceholderValue(IBox<IOrType<IFrontendType, IError>> testType)
        {
            Type = testType ?? throw new ArgumentNullException(nameof(testType));
        }
    }

    internal class WeakTypeDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType>>
    {

        public WeakTypeDefinitionConverter()
        {
        }

        public IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Type from)
        {
            var placeHolders = typeSolution.HasPlacholders(from);

            return placeHolders.IfElseReturn(x =>
            {
                return OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType>(
                    new WeakGenericTypeDefinition(
                        from.Key,
                        typeSolution.GetWeakScope(typeSolution.GetFlowNode(from)).TransformInner(x => new Box<WeakScope>(x)), // wrapping in a box here is weird 
                        x.Select(x=> Possibly.Is<IGenericTypeParameterPlacholder>(new GenericTypeParameterPlacholder(x))).ToArray()));//, key
            },
            () =>
            {
                return OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType>(
                    new WeakTypeDefinition(
                        typeSolution.GetWeakScope(typeSolution.GetFlowNode(from)).TransformInner(y=>new Box<WeakScope>(y))));//, key ?
            });
        }
    }


    internal class PrimitiveTypeConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType>>
    {
        public PrimitiveTypeConverter(Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType primitiveType)
        {
            PrimitiveType = primitiveType;
        }

        public Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType PrimitiveType { get; }

        public IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Type from)
        {
            return OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType>(PrimitiveType);
        }
    }

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
                    typeSolution.GetType(OrType.Make<Tpn.IVirtualFlowNode,IError>(typeSolution.GetFlowNode(from.Input.GetOrThrow()))),
                    typeSolution.GetType(OrType.Make<Tpn.IVirtualFlowNode, IError>(typeSolution.GetFlowNode(from.Returns.GetOrThrow()))));
        }
    }

    internal class WeakMethodDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition,WeakEntryPointDefinition>>
    {
        private readonly IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> body;

        public WeakMethodDefinitionConverter(IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> body)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Method from)
        {

            var inputKey = from.PrivateMembers.Single(x => x.Value == from.Input.GetOrThrow());

            return OrType.Make<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>( new WeakMethodDefinition(
                typeSolution.GetType(OrType.Make <Tpn.IVirtualFlowNode, IError > (typeSolution.GetFlowNode( from.Returns.GetOrThrow()))),
                new Box<WeakMemberDefinition>(typeSolution.GetMethodMember(from, inputKey.Key)), 
                body,
                OrType.Make<IBox<WeakScope>, IError>( new Box<WeakScope>(typeSolution.GetWeakScope(from))), 
                Array.Empty<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>>()));
        }
    }

    internal class WeakImplementationDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>
    {

        private readonly IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> body;
        private readonly IBox<Tpn.TypeProblem2.Method> inner;

        public WeakImplementationDefinitionConverter(IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> body, IBox<Tpn.TypeProblem2.Method> inner)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public IOrType<WeakMethodDefinition, WeakImplementationDefinition,WeakEntryPointDefinition> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Method from)
        {
            //

            var inputKey = from.PrivateMembers.Single(x => x.Value == from.Input.GetOrThrow());
            var innerInputKey = inner.GetValue().PrivateMembers.Single(x => x.Value == from.Input.GetOrThrow());

            return OrType.Make<WeakMethodDefinition, WeakImplementationDefinition,WeakEntryPointDefinition>(new WeakImplementationDefinition(
                new Box<WeakMemberDefinition>(typeSolution.GetMethodMember(from, inputKey.Key)), 
                new Box<WeakMemberDefinition>(typeSolution.GetMethodMember(inner.GetValue(), innerInputKey.Key)),
                typeSolution.GetType(OrType.Make<Tpn.IVirtualFlowNode, IError>(typeSolution.GetFlowNode(inner.GetValue().Returns.GetOrThrow()))),
                body,
                new Box<WeakScope>(typeSolution.GetWeakScope(from)),
                Array.Empty<IFrontendCodeElement>()));
        }

    }

    internal class WeakMemberDefinitionConverter : Tpn.IConvertTo<IOrType<Tpn.IVirtualFlowNode, IError>, WeakMemberDefinition>
    {
        private readonly Access access;
        private readonly IKey nameKey;

        public WeakMemberDefinitionConverter(Access access, IKey nameKey)
        {
            this.access = access;
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
        }

        public WeakMemberDefinition Convert(Tpn.TypeSolution typeSolution, IOrType< Tpn.IVirtualFlowNode, IError> from)
        {
            return new WeakMemberDefinition(access, nameKey, typeSolution.GetType(from));
        }
    }

    internal class WeakTypeReferenceConverter : Tpn.IConvertTo<Tpn.TypeProblem2.TypeReference, IFrontendType>
    {
        public IFrontendType Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.TypeReference from)
        {
            // I don't think this is safe see:
            // {D27D98BA-96CF-402C-824C-744DACC63FEE}
            return new WeakTypeReference(typeSolution.GetType(from));
        }
    }

    internal class WeakTypeOrOperationConverter : Tpn.IConvertTo<Tpn.TypeProblem2.OrType, WeakTypeOrOperation>
    {
        public WeakTypeOrOperation Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.OrType from)
        {
            return new WeakTypeOrOperation(
                from.Left.IfElseReturn(value => typeSolution.GetType(value), () => new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(Error.Other("that should have had a value")))),
                from.Right.IfElseReturn(value => typeSolution.GetType(value), () => new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(Error.Other("that should have had a value"))))
                );
        }
    }

    internal class PlaceholderValueConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Value, PlaceholderValue>
    {
        public PlaceholderValue Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Value from)
        {
            return new PlaceholderValue(typeSolution.GetType(from));
        }
    }

    internal class WeakBlockDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Scope, IOrType<WeakBlockDefinition, WeakScope,WeakEntryPointDefinition>>
    {

        private readonly IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> body;

        public WeakBlockDefinitionConverter(IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> body)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Scope from)
        {
            return OrType.Make<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>(
                new WeakBlockDefinition(
                    body,
                    OrType.Make<IBox<WeakScope>, IError>(new Box<WeakScope>(typeSolution.GetWeakScope(from))),
                    Array.Empty<IIsPossibly<IFrontendCodeElement>>()));
        }
    }

    internal class WeakEntryPointConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>
    {
        private readonly IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> body;

        public WeakEntryPointConverter(IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> body)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Method from)
        {

            var inputKey = from.PrivateMembers.Single(x => x.Value == from.Input.GetOrThrow());

            return OrType.Make<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>(new WeakEntryPointDefinition(
                typeSolution.GetType(OrType.Make<Tpn.IVirtualFlowNode, IError>(typeSolution.GetFlowNode(from.Returns.GetOrThrow()))),
                new Box<WeakMemberDefinition>(typeSolution.GetMethodMember(from, inputKey.Key)), // the Is1OrThrow here is bad
                body,
                OrType.Make<IBox<WeakScope>, IError>( new Box<WeakScope>(typeSolution.GetWeakScope(from))),
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
            return OrType.Make<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>(typeSolution.GetWeakScope(from));
        }
    }

    internal class WeakObjectConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Object, IOrType<WeakObjectDefinition, WeakRootScope>>
    {
        private readonly IBox<IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>>> box;

        public WeakObjectConverter(IBox<IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>>> box)
        {
            this.box = box;
        }

        public IOrType<WeakObjectDefinition, WeakRootScope> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Object from)
        {
            return OrType.Make<WeakObjectDefinition,  WeakRootScope>(new WeakObjectDefinition(
                typeSolution.GetWeakScope(typeSolution.GetFlowNode(from)).TransformInner(x => new Box<WeakScope>(x)),
                box));
        }
    }

    internal class WeakRootConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Object, IOrType<WeakObjectDefinition, WeakRootScope>>
    {
        private readonly Box<IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>>> assigns;
        private readonly Box<IOrType<IBox<WeakEntryPointDefinition>, IError>> entryPoint;

        public WeakRootConverter(
            Box<IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>>> assigns,
            Box<IOrType<IBox<WeakEntryPointDefinition>, IError>> EntryPoint)
        {
            this.assigns = assigns ?? throw new ArgumentNullException(nameof(assigns));
            entryPoint = EntryPoint ?? throw new ArgumentNullException(nameof(EntryPoint));
        }

        public IOrType<WeakObjectDefinition, WeakRootScope> Convert(Tpn.TypeSolution typeSolution, Tpn.TypeProblem2.Object from)
        {
            return OrType.Make<WeakObjectDefinition, WeakRootScope>(new WeakRootScope(
                typeSolution.GetWeakScope(typeSolution.GetFlowNode(from)).TransformInner(x => new Box<WeakScope>(x)),
                assigns,
                entryPoint));
        }
    }
}
