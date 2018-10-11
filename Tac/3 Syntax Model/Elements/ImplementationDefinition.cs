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
        public ImplementationDefinition(MemberDefinition contextDefinition,  MemberDefinition parameterDefinition, IBox<IReturnable> outputType, IEnumerable<ICodeElement> metohdBody, IResolvableScope scope, IEnumerable<ICodeElement> staticInitializers)
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
                return ContextDefinition.Type;
            }
        }
        public IBox<IReturnable> InputType
        {
            get
            {
                return ParameterDefinition.Type;
            }
        }
        public IBox<IReturnable> OutputType { get; }
        public MemberDefinition ContextDefinition { get; }
        public MemberDefinition ParameterDefinition { get; }
        public IResolvableScope Scope { get; }
        public IEnumerable<ICodeElement> MethodBody { get; }
        public IEnumerable<ICodeElement> StaticInitialzers { get; }

        // TODO do i need this??
        public IKey Key
        {
            get
            {
                return new GenericNameKey(RootKeys.MethodType, ContextDefinition.Type.GetValue().Key, new GenericNameKey(RootKeys.MethodType,  ParameterDefinition.Type.GetValue().Key, OutputType.GetValue().Key));
            }
        }

        public IReturnable ReturnType(IElementBuilders elementBuilders)
        {
            return this;
        }
    }

    public class ImplementationDefinitionMaker : IMaker<ImplementationDefinition>
    {
        public ImplementationDefinitionMaker(Func<MemberDefinition  , MemberDefinition , IBox<IReturnable>, IEnumerable<ICodeElement> , IResolvableScope, IEnumerable<ICodeElement> , ImplementationDefinition> make,
            IElementBuilders elementBuilders)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        private Func<MemberDefinition,  MemberDefinition, IBox<IReturnable>, IEnumerable<ICodeElement>, IResolvableScope, IEnumerable<ICodeElement>, ImplementationDefinition> Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<ImplementationDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("implementation"), out var _)
                .Has(ElementMatcher.Generic3, out AtomicToken contextType, out AtomicToken inputType, out AtomicToken outputType)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken contextName)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken parameterName)
                .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var methodScope = Tac.Semantic_Model.Scope.LocalStaticScope();

                var newMatchingContext = matchingContext.Child(methodScope);
                var elements = newMatchingContext.ParseBlock(body);

                var contextKey = new NameKey(parameterName?.Item ?? "context");
                var contextDefinition = ElementBuilders.MemberDefinition.Make(
                        false,
                        contextKey,
                        new NameKey(contextType.Item)
                        );


                var parameterKey = new NameKey(parameterName?.Item ?? "input");
                var parameterDefinition = ElementBuilders.MemberDefinition.Make(
                        false,
                        parameterKey,
                        new NameKey(inputType.Item)
                        );

                var outputTypeName= new NameKey(outputType.Item);

                return ResultExtension.Good(new PopulateScopeImplementationDefinition(contextDefinition, parameterDefinition, methodScope, elements, outputTypeName,Make, parameterKey, contextKey));
            }


            return ResultExtension.Bad<IPopulateScope<ImplementationDefinition>>();
        }
    }

    public class PopulateScopeImplementationDefinition : IPopulateScope<ImplementationDefinition>
    {
        private readonly IPopulateScope<MemberDefinition> contextDefinition;
        private readonly IPopulateScope<MemberDefinition> parameterDefinition;
        private readonly ILocalStaticScope methodScope;
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly NameKey outputTypeName;
        private readonly NameKey parameterKey;
        private readonly NameKey contextKey;
        private readonly Func<MemberDefinition, MemberDefinition, IBox<IReturnable>, IEnumerable<ICodeElement>, IResolvableScope, IEnumerable<ICodeElement>, ImplementationDefinition> make;
        private readonly Box<IReturnable> box = new Box<IReturnable>();

        public PopulateScopeImplementationDefinition(IPopulateScope<MemberDefinition> contextDefinition, IPopulateScope<MemberDefinition> parameterDefinition, ILocalStaticScope methodScope, IPopulateScope<ICodeElement>[] elements, NameKey outputTypeName, Func<MemberDefinition, MemberDefinition, IBox<IReturnable>, IEnumerable<ICodeElement>, IResolvableScope, IEnumerable<ICodeElement>, ImplementationDefinition> make, NameKey parameterKey, NameKey contextKey)
        {
            this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.outputTypeName = outputTypeName ?? throw new ArgumentNullException(nameof(outputTypeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.parameterKey = parameterKey ?? throw new ArgumentNullException(nameof(parameterKey));
            this.contextKey = contextKey ?? throw new ArgumentNullException(nameof(contextKey));
        }

        public IResolveReference<ImplementationDefinition> Run(IPopulateScopeContext context)
        {
            var resolve = methodScope.ToResolvable();
            var newContext = context.Child(this, methodScope);
            return new ImplementationDefinitionResolveReferance(
                contextDefinition.Run(newContext), 
                parameterDefinition.Run(newContext), 
                resolve, 
                elements.Select(x => x.Run(newContext)).ToArray(),
                outputTypeName,
                make,
                parameterKey,
                contextKey,
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
        private readonly NameKey parameterKey;
        private readonly NameKey contextKey;
        private readonly Func<MemberDefinition, MemberDefinition, IBox<IReturnable>, IEnumerable<ICodeElement>, IResolvableScope, IEnumerable<ICodeElement>, ImplementationDefinition> make;
        private readonly Box<IReturnable> box;

        public ImplementationDefinitionResolveReferance(
            IResolveReference<MemberDefinition> contextDefinition,
            IResolveReference<MemberDefinition> parameterDefinition,
            IResolvableScope methodScope,
            IResolveReference<ICodeElement>[] elements,
            NameKey outputTypeName,
            Func<MemberDefinition, MemberDefinition, IBox<IReturnable>, IEnumerable<ICodeElement>, IResolvableScope, IEnumerable<ICodeElement>, ImplementationDefinition> make,
            NameKey parameterKey,
            NameKey contextKey, 
            Box<IReturnable> box)
        {
            this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.outputTypeName = outputTypeName ?? throw new ArgumentNullException(nameof(outputTypeName));
            this.parameterKey = parameterKey ?? throw new ArgumentNullException(nameof(parameterKey));
            this.contextKey = contextKey ?? throw new ArgumentNullException(nameof(contextKey));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }
        
        public ImplementationDefinition Run(IResolveReferanceContext context)
        {
            var newContext = context.Child(this, methodScope);
            return box.Fill(make(contextDefinition.Run(newContext), parameterDefinition.Run(newContext), context.GetTypeDefintion(outputTypeName), elements.Select(x => x.Run(newContext)).ToArray(), methodScope, new ICodeElement[0]));
        }
    }
}
