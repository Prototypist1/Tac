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
    // really really not sure how these work atm
    // for now they just hold everything you need to ake a method

    // this is really just a method....
    // should this even exist?
     
    public class ImplementationDefinition: IReturnable, ICodeElement
    {
        // this is an interesting pattern
        // we define the delegate here to communicate that it should be ti8red to the constructor
        public delegate ImplementationDefinition Make(IBox<MemberDefinition> contextDefinition, IBox<MemberDefinition> parameterDefinition, IBox<IReturnable> outputType, IEnumerable<ICodeElement> metohdBody, IFinalizedScope scope, IEnumerable<ICodeElement> staticInitializers);

        public ImplementationDefinition(
            IBox<MemberDefinition> contextDefinition, 
            IBox<MemberDefinition> parameterDefinition, 
            IBox<IReturnable> outputType, 
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
        public IBox<IReturnable> ContextType
        {
            get
            {
                return ContextDefinition.GetValue().Type;
            }
        }
        public IBox<IReturnable> InputType
        {
            get
            {
                return ParameterDefinition.GetValue().Type;
            }
        }
        public IBox<IReturnable> OutputType { get; }
        public IBox<MemberDefinition> ContextDefinition { get; }
        public IBox<MemberDefinition> ParameterDefinition { get; }
        public IFinalizedScope Scope { get; }
        public IEnumerable<ICodeElement> MethodBody { get; }
        public IEnumerable<ICodeElement> StaticInitialzers { get; }
        
        public IReturnable Returns(IElementBuilders elementBuilders)
        {
            return this;
        }
    }

    public class ImplementationDefinitionMaker : IMaker<ImplementationDefinition>
    {
        public ImplementationDefinitionMaker(ImplementationDefinition.Make make,
            IElementBuilders elementBuilders)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        private ImplementationDefinition.Make Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<ImplementationDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
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
                        new NameKey(contextType.Item),
                        matchingContext.Builders.MemberDefinition
                        );


                var parameterNameString = parameterName?.Item ?? "input";
                var parameterDefinition = new MemberDefinitionPopulateScope(
                        parameterNameString,
                        false,
                        new NameKey(inputType.Item),
                        matchingContext.Builders.MemberDefinition
                        );

                var outputTypeName= new NameKey(outputType.Item);

                return ResultExtension.Good(
                    new PopulateScopeImplementationDefinition(
                        contextDefinition, 
                        parameterDefinition, 
                        elements, 
                        outputTypeName,
                        Make));
            }


            return ResultExtension.Bad<IPopulateScope<ImplementationDefinition>>();
        }
    }

    public class PopulateScopeImplementationDefinition : IPopulateScope<ImplementationDefinition>
    {
        private readonly IPopulateScope<MemberDefinition> contextDefinition;
        private readonly IPopulateScope<MemberDefinition> parameterDefinition;
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly NameKey outputTypeName;
        private readonly ImplementationDefinition.Make make;
        private readonly Box<IReturnable> box = new Box<IReturnable>();

        public PopulateScopeImplementationDefinition(
            IPopulateScope<MemberDefinition> contextDefinition,
            IPopulateScope<MemberDefinition> parameterDefinition,
            IPopulateScope<ICodeElement>[] elements,
            NameKey outputTypeName,
            ImplementationDefinition.Make make)
        {
            this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.outputTypeName = outputTypeName ?? throw new ArgumentNullException(nameof(outputTypeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReference<ImplementationDefinition> Run(IPopulateScopeContext context)
        {

            var nextContext = context.Child();
            return new ImplementationDefinitionResolveReferance(
                contextDefinition.Run(nextContext), 
                parameterDefinition.Run(nextContext),
                nextContext.GetResolvableScope(), 
                elements.Select(x => x.Run(nextContext)).ToArray(),
                outputTypeName,
                make,
                box);
        }
        
        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

    }

    public class ImplementationDefinitionResolveReferance : IResolveReference<ImplementationDefinition>
    {
        private readonly IResolveReference<MemberDefinition> contextDefinition;
        private readonly IResolveReference<MemberDefinition> parameterDefinition;
        private readonly IResolvableScope methodScope;
        private readonly IResolveReference<ICodeElement>[] elements;
        private readonly NameKey outputTypeName;
        private readonly ImplementationDefinition.Make make;
        private readonly Box<IReturnable> box;

        public ImplementationDefinitionResolveReferance(
            IResolveReference<MemberDefinition> contextDefinition,
            IResolveReference<MemberDefinition> parameterDefinition,
            IResolvableScope methodScope,
            IResolveReference<ICodeElement>[] elements,
            NameKey outputTypeName,
            ImplementationDefinition.Make make,
            Box<IReturnable> box)
        {
            this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.outputTypeName = outputTypeName ?? throw new ArgumentNullException(nameof(outputTypeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }
        
        public ImplementationDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(make(
                new Box<MemberDefinition>(contextDefinition.Run(context)),
                new Box<MemberDefinition>(parameterDefinition.Run(context)),
                methodScope.GetType(outputTypeName), 
                elements.Select(x => x.Run(context)).ToArray(), 
                methodScope.Finalize(), 
                new ICodeElement[0]));
        }
    }
}
