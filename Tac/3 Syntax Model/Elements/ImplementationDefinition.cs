using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public class WeakImplementationDefinition: IWeakReturnable, IWeakCodeElement
    {

        public WeakImplementationDefinition(
            IBox<WeakMemberDefinition> contextDefinition, 
            IBox<WeakMemberDefinition> parameterDefinition, 
            IBox<IWeakReturnable> outputType, 
            IEnumerable<IWeakCodeElement> metohdBody,
            IWeakFinalizedScope scope, 
            IEnumerable<IWeakCodeElement> staticInitializers)
        {
            ContextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            MethodBody = metohdBody ?? throw new ArgumentNullException(nameof(metohdBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialzers = staticInitializers ?? throw new ArgumentNullException(nameof(staticInitializers));
        }

        // dang! these could also be inline definitions 
        public IBox<IWeakReturnable> ContextType
        {
            get
            {
                return ContextDefinition.GetValue().Type;
            }
        }
        public IBox<IWeakReturnable> InputType
        {
            get
            {
                return ParameterDefinition.GetValue().Type;
            }
        }
        public IBox<IWeakReturnable> OutputType { get; }
        public IBox<WeakMemberDefinition> ContextDefinition { get; }
        public IBox<WeakMemberDefinition> ParameterDefinition { get; }
        public IWeakFinalizedScope Scope { get; }
        public IEnumerable<IWeakCodeElement> MethodBody { get; }
        public IEnumerable<IWeakCodeElement> StaticInitialzers { get; }
        
        public IWeakReturnable Returns()
        {
            return this;
        }
    }

    public class ImplementationDefinitionMaker : IMaker<WeakImplementationDefinition>
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

    public class PopulateScopeImplementationDefinition : IPopulateScope<WeakImplementationDefinition>
    {
        private readonly IPopulateScope<WeakMemberReferance> contextDefinition;
        private readonly IPopulateScope<WeakMemberReferance> parameterDefinition;
        private readonly IPopulateScope<IWeakCodeElement>[] elements;
        private readonly NameKey outputTypeName;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public PopulateScopeImplementationDefinition(
            IPopulateScope<WeakMemberReferance> contextDefinition,
            IPopulateScope<WeakMemberReferance> parameterDefinition,
            IPopulateScope<IWeakCodeElement>[] elements,
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
        
        public IBox<IWeakReturnable> GetReturnType()
        {
            return box;
        }

    }

    public class ImplementationDefinitionResolveReferance : IPopulateBoxes<WeakImplementationDefinition>
    {
        private readonly IPopulateBoxes<WeakMemberReferance> contextDefinition;
        private readonly IPopulateBoxes<WeakMemberReferance> parameterDefinition;
        private readonly IResolvableScope methodScope;
        private readonly IPopulateBoxes<IWeakCodeElement>[] elements;
        private readonly NameKey outputTypeName;
        private readonly Box<IWeakReturnable> box;

        public ImplementationDefinitionResolveReferance(
            IPopulateBoxes<WeakMemberReferance> contextDefinition,
            IPopulateBoxes<WeakMemberReferance> parameterDefinition,
            IResolvableScope methodScope,
            IPopulateBoxes<IWeakCodeElement>[] elements,
            NameKey outputTypeName,
            Box<IWeakReturnable> box)
        {
            this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.outputTypeName = outputTypeName ?? throw new ArgumentNullException(nameof(outputTypeName));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }
        
        public IOpenBoxes<WeakImplementationDefinition> Run(IResolveReferanceContext context)
        {
            var item = box.Fill(new WeakImplementationDefinition(
                contextDefinition.Run(context).CodeElement.MemberDefinition,
                parameterDefinition.Run(context).CodeElement.MemberDefinition,
                methodScope.GetTypeOrThrow(outputTypeName), 
                elements.Select(x => x.Run(context).CodeElement).ToArray(), 
                methodScope.GetFinalized(), 
                new IWeakCodeElement[0]));
            return new ImplementationDefinitionPopulateBoxes(item);
        }
    }

    internal class ImplementationDefinitionPopulateBoxes : IOpenBoxes<WeakImplementationDefinition>
    {
        public WeakImplementationDefinition CodeElement { get; }

        public ImplementationDefinitionPopulateBoxes(WeakImplementationDefinition item)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
        }

        public T Run<T>(IOpenBoxesContext<T> context)
        {
            return context.ImplementationDefinition(CodeElement);
        }
    }

    // TODO TODO
    // you are here!
    // ok so instead of this shitty pass make threw thing
    // nothing is <T>
    // public T Run(IOpenBoxesContext context)
    // becomes 
    // public T Run<T>(IOpenBoxesContext<T> context)
    // and that has makes for all the types
    // also, run can just be on the weak code elements
    

}
