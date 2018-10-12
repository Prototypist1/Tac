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
    public class MethodDefinition : AbstractBlockDefinition, IReturnable
    {
        public MethodDefinition(IBox<IReturnable> outputType, MemberDefinition parameterDefinition, ICodeElement[] body, IResolvableScope scope, IEnumerable<ICodeElement> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }

        public IBox<IReturnable> InputType
        {
            get
            {
                return ParameterDefinition.Type;
            }
        }
        public IBox<IReturnable> OutputType { get; }
        public MemberDefinition ParameterDefinition { get; }

        // TODO do I need this?
        public IKey Key
        {
            get
            {
                return new GenericNameKey(RootKeys.MethodType, InputType.GetValue().Key, OutputType.GetValue().Key);
            }
        }
    }


    public class MethodDefinitionMaker : IMaker<MethodDefinition>
    {
        public MethodDefinitionMaker(Func<MemberDefinition, IBox<IReturnable>, IEnumerable<ICodeElement>, IResolvableScope, IEnumerable<ICodeElement>, MethodDefinition> make,
            IElementBuilders elementBuilders)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        private Func<MemberDefinition, IBox<IReturnable>, IEnumerable<ICodeElement>, IResolvableScope, IEnumerable<ICodeElement>, MethodDefinition> Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<MethodDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("implementation"), out var _)
                .Has(ElementMatcher.Generic2, out AtomicToken inputType, out AtomicToken outputType)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken parameterName)
                .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var methodScope = Scope.LocalStaticScope();

                var newMatchingContext = matchingContext.Child(methodScope);
                var elements = newMatchingContext.ParseBlock(body);
                
                var parameterDefinition = ElementBuilders.MemberDefinition.Make(
                        false,
                       new NameKey(parameterName?.Item ?? "input"),
                        new NameKey(inputType.Item)
                        );

                var outputTypeName = new NameKey(outputType.Item);
                
                return ResultExtension.Good(new MethodDefinitionPopulateScope(parameterDefinition, methodScope, elements, outputTypeName, Make));
            }

            return ResultExtension.Bad<IPopulateScope<MethodDefinition>>();
        }

    }


    public class MethodDefinitionPopulateScope : IPopulateScope<MethodDefinition>
    {
        private readonly IPopulateScope<MemberDefinition> parameterDefinition;
        private readonly ILocalStaticScope methodScope;
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly NameKey outputTypeName;
        private readonly Func<MemberDefinition, IBox<IReturnable>, IEnumerable<ICodeElement>, IResolvableScope, IEnumerable<ICodeElement>, MethodDefinition> make;
        private readonly Box<IReturnable> box = new Box<IReturnable>();

        public MethodDefinitionPopulateScope(
            IPopulateScope<MemberDefinition> parameterDefinition, 
            ILocalStaticScope methodScope, 
            IPopulateScope<ICodeElement>[] elements, 
            NameKey outputTypeName, 
            Func<MemberDefinition, IBox<IReturnable>, IEnumerable<ICodeElement>, IResolvableScope, IEnumerable<ICodeElement>, MethodDefinition> make
            )
        {
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.outputTypeName = outputTypeName ?? throw new ArgumentNullException(nameof(outputTypeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));

        }

        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IResolveReference<MethodDefinition> Run(IPopulateScopeContext context)
        {

            var nextContext = context.Child(this,methodScope);
            return new MethodDefinitionResolveReferance(parameterDefinition.Run(nextContext), methodScope.ToResolvable(), elements.Select(x => x.Run(nextContext)).ToArray(), outputTypeName, make, box);
        }
    }

    public class MethodDefinitionResolveReferance : IResolveReference<MethodDefinition>
    {
        private readonly IResolveReference<MemberDefinition> parameter;
        private readonly IResolvableScope methodScope;
        private readonly IResolveReference<ICodeElement>[] lines;
        private readonly NameKey outputTypeName;
        private readonly Func<MemberDefinition, IBox<IReturnable>, IEnumerable<ICodeElement>, IResolvableScope, IEnumerable<ICodeElement>, MethodDefinition> make;
        private readonly Box<IReturnable> box;

        public MethodDefinitionResolveReferance(
            IResolveReference<MemberDefinition> parameter, 
            IResolvableScope methodScope, 
            IResolveReference<ICodeElement>[] resolveReferance2, 
            NameKey outputTypeName, 
            Func<MemberDefinition, IBox<IReturnable>, IEnumerable<ICodeElement>, IResolvableScope, IEnumerable<ICodeElement>, MethodDefinition> make, 
            Box<IReturnable> box)
        {
            this.parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
            lines = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            this.outputTypeName = outputTypeName ?? throw new ArgumentNullException(nameof(outputTypeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public MethodDefinition Run(IResolveReferanceContext context)
        {
            var nextContext = context.Child(this, methodScope);
            return box.Fill(make(parameter.Run(nextContext), context.GetTypeDefintion(outputTypeName), lines.Select(x => x.Run(nextContext)).ToArray(), methodScope, new ICodeElement[0]));
        }
    }
}