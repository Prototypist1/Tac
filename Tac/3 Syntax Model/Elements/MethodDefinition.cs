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
        public delegate MethodDefinition Make(
            IBox<IReturnable> outputType,
            IBox<MemberDefinition> parameterDefinition,
            ICodeElement[] body,
            IResolvableScope scope,
            IEnumerable<ICodeElement> staticInitializers);

        public MethodDefinition(
            IBox<IReturnable> outputType, 
            IBox<MemberDefinition> parameterDefinition,
            ICodeElement[] body,
            IResolvableScope scope,
            IEnumerable<ICodeElement> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }
        


        public IBox<IReturnable> InputType
        {
            get
            {
                return ParameterDefinition.GetValue().Type;
            }
        }
        public IBox<IReturnable> OutputType { get; }
        public IBox<MemberDefinition> ParameterDefinition { get; }
    }


    public class MethodDefinitionMaker : IMaker<MethodDefinition>
    {
        public MethodDefinitionMaker(
            MethodDefinition.Make make,
            IElementBuilders elementBuilders)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        private MethodDefinition.Make Make { get; }
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

                var (methodScope,stack) = matchingContext.ScopeStack.LocalStaticScope();

                var newMatchingContext = matchingContext.Child(stack);
                var elements = newMatchingContext.ParseBlock(body);
                
                var parameterDefinition = new MemberDefinitionPopulateScope(
                        parameterName?.Item ?? "input",
                        false,
                         new NameKey(inputType.Item),
                        matchingContext.Builders.MemberDefinition
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
        private readonly MethodDefinition.Make make;
        private readonly Box<IReturnable> box = new Box<IReturnable>();

        public MethodDefinitionPopulateScope(
            IPopulateScope<MemberDefinition> parameterDefinition, 
            ILocalStaticScope methodScope, 
            IPopulateScope<ICodeElement>[] elements, 
            NameKey outputTypeName,
            MethodDefinition.Make make
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
            return new MethodDefinitionResolveReferance(
                parameterDefinition.Run(nextContext), 
                methodScope.ToResolvable(), 
                elements.Select(x => x.Run(nextContext)).ToArray(), 
                outputTypeName, 
                make, 
                box);
        }
    }

    public class MethodDefinitionResolveReferance : IResolveReference<MethodDefinition>
    {
        private readonly IResolveReference<MemberDefinition> parameter;
        private readonly IResolvableScope methodScope;
        private readonly IResolveReference<ICodeElement>[] lines;
        private readonly NameKey outputTypeName;
        private readonly MethodDefinition.Make make;
        private readonly Box<IReturnable> box;

        public MethodDefinitionResolveReferance(
            IResolveReference<MemberDefinition> parameter, 
            IResolvableScope methodScope, 
            IResolveReference<ICodeElement>[] resolveReferance2, 
            NameKey outputTypeName,
            MethodDefinition.Make make, 
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
            return box.Fill(
                make(
                    context.GetTypeDefintion(outputTypeName),
                    new Box<MemberDefinition>(parameter.Run(nextContext)), 
                    lines.Select(x => x.Run(nextContext)).ToArray(),
                    methodScope,
                    new ICodeElement[0]));
        }
    }
}