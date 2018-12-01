using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    internal class WeakImplementationDefinition: ICodeElement, IImplementationDefinition
    {

        public WeakImplementationDefinition(
            IBox<WeakMemberDefinition> contextDefinition, 
            IBox<WeakMemberDefinition> parameterDefinition, 
            WeakTypeReferance outputType, 
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
        public WeakTypeReferance ContextTypeBox
        {
            get
            {
                return ContextDefinition.GetValue().Type;
            }
        }
        public WeakTypeReferance InputTypeBox
        {
            get
            {
                return ParameterDefinition.GetValue().Type;
            }
        }
        public WeakTypeReferance OutputType { get; }
        // are these really boxes
        public IBox<WeakMemberDefinition> ContextDefinition { get; }
        public IBox<WeakMemberDefinition> ParameterDefinition { get; }
        public IFinalizedScope Scope { get; }
        public IEnumerable<ICodeElement> MethodBody { get; }
        public IEnumerable<ICodeElement> StaticInitialzers { get; }

        #region IImplementationDefinition
        
        public IVarifiableType InputType => ParameterDefinition.GetValue().Type;
        public IVarifiableType ContextType => ContextDefinition.GetValue().Type;
        
        IMemberDefinition IImplementationDefinition.ContextDefinition => ContextDefinition.GetValue();
        IMemberDefinition IImplementationDefinition.ParameterDefinition => ParameterDefinition.GetValue();
        IVarifiableType IImplementationType.OutputType => OutputType;

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

    internal class ImplementationDefinitionMaker : IMaker<IPopulateScope<WeakImplementationDefinition>>
    {
        public ImplementationDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakImplementationDefinition>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            IPopulateScope<WeakTypeReferance> context= null, input = null, output = null;

            var match = tokenMatching
                .Has(new KeyWordMaker("implementation"), out var _)
                .HasSquare(x => x
                    .HasLine(y=>y
                        .HasElement(z=>z
                            .HasOne(
                                w => w.Has(new TypeReferanceMaker(), out var _)
                                    .Has(new DoneMaker()),
                                w => w.Has(new TypeDefinitionMaker(), out var _)
                                    .Has(new DoneMaker()),
                                out context))
                         .Has(new DoneMaker()))
                    .HasLine(y => y
                        .HasElement(z => z
                            .HasOne(
                                w => w.Has(new TypeReferanceMaker(), out var _)
                                    .Has(new DoneMaker()),
                                w => w.Has(new TypeDefinitionMaker(), out var _)
                                    .Has(new DoneMaker()),
                                out input))
                        .Has(new DoneMaker()))
                    .HasLine(y => y
                        .HasElement(z => z
                           .HasOne(
                                w => w.Has(new TypeReferanceMaker(), out var _)
                                    .Has(new DoneMaker()),
                                w => w.Has(new TypeDefinitionMaker(), out var _)
                                    .Has(new DoneMaker()),
                                out output))
                        .Has(new DoneMaker()))
                    .Has(new DoneMaker()))
                .OptionalHas(new NameMaker(), out AtomicToken contextName)
                .OptionalHas(new NameMaker(), out AtomicToken parameterName)
                .Has(new BodyMaker(), out CurleyBracketToken body);
            if (match is IMatchedTokenMatching matched)
            {
                var elements = tokenMatching.Context.ParseBlock(body);
                
                var contextNameString = contextName?.Item ?? "context";
                var contextDefinition = new MemberDefinitionPopulateScope(
                        contextNameString,
                        false,
                        context
                        );
                
                var parameterNameString = parameterName?.Item ?? "input";
                var parameterDefinition = new MemberDefinitionPopulateScope(
                        parameterNameString,
                        false,
                        input
                        );
                
                return TokenMatching<IPopulateScope<WeakImplementationDefinition>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new PopulateScopeImplementationDefinition(
                        contextDefinition, 
                        parameterDefinition, 
                        elements,
                        output));
            }


            return TokenMatching<IPopulateScope<WeakImplementationDefinition>>.MakeNotMatch(match.Context);
        }
    }

    internal class PopulateScopeImplementationDefinition : IPopulateScope<WeakImplementationDefinition>
    {
        private readonly IPopulateScope<WeakMemberReferance> contextDefinition;
        private readonly IPopulateScope<WeakMemberReferance> parameterDefinition;
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly IPopulateScope<WeakTypeReferance> output;
        private readonly Box<IVarifiableType> box = new Box<IVarifiableType>();

        public PopulateScopeImplementationDefinition(
            IPopulateScope<WeakMemberReferance> contextDefinition,
            IPopulateScope<WeakMemberReferance> parameterDefinition,
            IPopulateScope<ICodeElement>[] elements,
            IPopulateScope<WeakTypeReferance> output)
        {
            this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public IPopulateBoxes<WeakImplementationDefinition> Run(IPopulateScopeContext context)
        {

            var nextContext = context.Child();
            return new ImplementationDefinitionResolveReferance(
                contextDefinition.Run(nextContext), 
                parameterDefinition.Run(nextContext),
                nextContext.GetResolvableScope(), 
                elements.Select(x => x.Run(nextContext)).ToArray(),
                output.Run(context),
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
        private readonly IPopulateBoxes<WeakTypeReferance> output;
        private readonly Box<IVarifiableType> box;

        public ImplementationDefinitionResolveReferance(
            IPopulateBoxes<WeakMemberReferance> contextDefinition,
            IPopulateBoxes<WeakMemberReferance> parameterDefinition,
            IResolvableScope methodScope,
            IPopulateBoxes<ICodeElement>[] elements,
            IPopulateBoxes<WeakTypeReferance> output,
            Box<IVarifiableType> box)
        {
            this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }
        
        public WeakImplementationDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(new WeakImplementationDefinition(
                contextDefinition.Run(context).MemberDefinition,
                parameterDefinition.Run(context).MemberDefinition,
                output.Run(context), 
                elements.Select(x => x.Run(context)).ToArray(), 
                methodScope.GetFinalized(), 
                new ICodeElement[0]));
        }
    }


}
