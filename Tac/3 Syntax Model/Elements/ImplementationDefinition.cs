using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    internal class WeakImplementationDefinition: IImplementationType, ICodeElement, IImplementationDefinition
    {

        public WeakImplementationDefinition(
            IBox<WeakMemberDefinition> contextDefinition, 
            IBox<WeakMemberDefinition> parameterDefinition, 
            IBox<IVarifiableType> outputType, 
            IEnumerable<ICodeElement> metohdBody,
            IFinalizedScope scope, 
            IEnumerable<ICodeElement> staticInitializers)
        {
            ContextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            MethodBody = metohdBody ?? throw new ArgumentNullException(nameof(metohdBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialzers = staticInitializers ?? throw new ArgumentNullException(nameof(staticInitializers));
        }

        // dang! these could also be inline definitions 
        public IBox<IVarifiableType> ContextTypeBox
        {
            get
            {
                return ContextDefinition.GetValue().Type;
            }
        }
        public IBox<IVarifiableType> InputTypeBox
        {
            get
            {
                return ParameterDefinition.GetValue().Type;
            }
        }
        public IBox<IVarifiableType> OutputType { get; }
        public IBox<WeakMemberDefinition> ContextDefinition { get; }
        public IBox<WeakMemberDefinition> ParameterDefinition { get; }
        public IFinalizedScope Scope { get; }
        public IEnumerable<ICodeElement> MethodBody { get; }
        public IEnumerable<ICodeElement> StaticInitialzers { get; }

        #region IImplementationDefinition

        IVarifiableType IImplementationDefinition.OutputType => OutputType.GetValue();
        IMemberDefinition IImplementationDefinition.ContextDefinition => ContextDefinition.GetValue();
        IMemberDefinition IImplementationDefinition.ParameterDefinition => ParameterDefinition.GetValue();


        #endregion


        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ImplementationDefinition(this);
        }
        
        public IVarifiableType Returns()
        {
            return this;
        }
    }

    internal class ImplementationDefinitionMaker : IMaker<WeakImplementationDefinition>
    {
        public ImplementationDefinitionMaker()
        {
        }


        public IResult<IPopulateScope<WeakImplementationDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("implementation"), out var _)
                // WHY doe this return AtomicToken?? it should return IKey
                .Has(ElementMatcher.Generic3, out AtomicToken contextType, out AtomicToken inputType, out AtomicToken outputType)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken contextName)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken parameterName)
                .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {


                var elements = matchingContext.ParseBlock(body);

                var contextNameString = contextName?.Item ?? "context";
                var contextDefinition = new MemberDefinitionPopulateScope(
                        contextNameString,
                        false,
                        new NameKey(contextType.Item)
                        );


                var parameterNameString = parameterName?.Item ?? "input";
                var parameterDefinition = new MemberDefinitionPopulateScope(
                        parameterNameString,
                        false,
                        new NameKey(inputType.Item)
                        );

                var outputTypeName= new NameKey(outputType.Item);

                return ResultExtension.Good(
                    new PopulateScopeImplementationDefinition(
                        contextDefinition, 
                        parameterDefinition, 
                        elements, 
                        outputTypeName));
            }


            return ResultExtension.Bad<IPopulateScope<WeakImplementationDefinition>>();
        }
    }

    internal class PopulateScopeImplementationDefinition : IPopulateScope<WeakImplementationDefinition>
    {
        private readonly IPopulateScope<WeakMemberReferance> contextDefinition;
        private readonly IPopulateScope<WeakMemberReferance> parameterDefinition;
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly NameKey outputTypeName;
        private readonly Box<IVarifiableType> box = new Box<IVarifiableType>();

        public PopulateScopeImplementationDefinition(
            IPopulateScope<WeakMemberReferance> contextDefinition,
            IPopulateScope<WeakMemberReferance> parameterDefinition,
            IPopulateScope<ICodeElement>[] elements,
            NameKey outputTypeName)
        {
            this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.outputTypeName = outputTypeName ?? throw new ArgumentNullException(nameof(outputTypeName));
        }

        public IPopulateBoxes<WeakImplementationDefinition> Run(IPopulateScopeContext context)
        {

            var nextContext = context.Child();
            return new ImplementationDefinitionResolveReferance(
                contextDefinition.Run(nextContext), 
                parameterDefinition.Run(nextContext),
                nextContext.GetResolvableScope(), 
                elements.Select(x => x.Run(nextContext)).ToArray(),
                outputTypeName,
                box);
        }
        
        public IBox<IVarifiableType> GetReturnType()
        {
            return box;
        }

    }

    internal class ImplementationDefinitionResolveReferance : IPopulateBoxes<WeakImplementationDefinition>
    {
        private readonly IPopulateBoxes<WeakMemberReferance> contextDefinition;
        private readonly IPopulateBoxes<WeakMemberReferance> parameterDefinition;
        private readonly IResolvableScope methodScope;
        private readonly IPopulateBoxes<ICodeElement>[] elements;
        private readonly NameKey outputTypeName;
        private readonly Box<IVarifiableType> box;

        public ImplementationDefinitionResolveReferance(
            IPopulateBoxes<WeakMemberReferance> contextDefinition,
            IPopulateBoxes<WeakMemberReferance> parameterDefinition,
            IResolvableScope methodScope,
            IPopulateBoxes<ICodeElement>[] elements,
            NameKey outputTypeName,
            Box<IVarifiableType> box)
        {
            this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.outputTypeName = outputTypeName ?? throw new ArgumentNullException(nameof(outputTypeName));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }
        
        public WeakImplementationDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(new WeakImplementationDefinition(
                contextDefinition.Run(context).MemberDefinition,
                parameterDefinition.Run(context).MemberDefinition,
                methodScope.GetTypeOrThrow(outputTypeName), 
                elements.Select(x => x.Run(context)).ToArray(), 
                methodScope.GetFinalized(), 
                new ICodeElement[0]));
        }
    }


}
