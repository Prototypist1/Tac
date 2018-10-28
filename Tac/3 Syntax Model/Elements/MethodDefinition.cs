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

    public class WeakMethodDefinition : WeakAbstractBlockDefinition, IType, IMethodDefinition
    {
        public WeakMethodDefinition(
            IBox<IType> outputType, 
            IBox<WeakMemberDefinition> parameterDefinition,
            ICodeElement[] body,
            IWeakFinalizedScope scope,
            IEnumerable<ICodeElement> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }
        


        public IBox<IType> InputType
        {
            get
            {
                return ParameterDefinition.GetValue().Type;
            }
        }
        public IBox<IType> OutputType { get; }
        public IBox<WeakMemberDefinition> ParameterDefinition { get; }
    }


    public class MethodDefinitionMaker : IMaker<WeakMethodDefinition>
    {
        public MethodDefinitionMaker()
        {
        }
        

        public IResult<IPopulateScope<WeakMethodDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("method"), out var _)
                .Has(ElementMatcher.Generic2, out AtomicToken inputType, out AtomicToken outputType)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken parameterName)
                .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                var elements = matchingContext.ParseBlock(body);
                
                var parameterDefinition = new MemberDefinitionPopulateScope(
                        parameterName?.Item ?? "input",
                        false,
                         new NameKey(inputType.Item)
                        );
                
                var outputTypeName = new NameKey(outputType.Item);
                
                return ResultExtension.Good(new MethodDefinitionPopulateScope(
                    parameterDefinition,
                    elements, 
                    outputTypeName));
            }

            return ResultExtension.Bad<IPopulateScope<WeakMethodDefinition>>();
        }
    }

    public class MethodDefinitionPopulateScope : IPopulateScope<WeakMethodDefinition>
    {
        private readonly IPopulateScope<WeakMemberReferance> parameterDefinition;
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly NameKey outputTypeName;
        private readonly Box<IType> box = new Box<IType>();

        public MethodDefinitionPopulateScope(
            IPopulateScope<WeakMemberReferance> parameterDefinition,
            IPopulateScope<ICodeElement>[] elements, 
            NameKey outputTypeName
            )
        {
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.outputTypeName = outputTypeName ?? throw new ArgumentNullException(nameof(outputTypeName));

        }

        public IBox<IType> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<WeakMethodDefinition> Run(IPopulateScopeContext context)
        {

            var nextContext = context.Child();
            return new MethodDefinitionResolveReferance(
                parameterDefinition.Run(nextContext),
                nextContext.GetResolvableScope(), 
                elements.Select(x => x.Run(nextContext)).ToArray(), 
                outputTypeName, 
                box);
        }
    }

    public class MethodDefinitionResolveReferance : IPopulateBoxes<WeakMethodDefinition>
    {
        private readonly IPopulateBoxes<WeakMemberReferance> parameter;
        private readonly IResolvableScope methodScope;
        private readonly IPopulateBoxes<ICodeElement>[] lines;
        private readonly NameKey outputTypeName;
        private readonly Box<IType> box;

        public MethodDefinitionResolveReferance(
            IPopulateBoxes<WeakMemberReferance> parameter, 
            IResolvableScope methodScope, 
            IPopulateBoxes<ICodeElement>[] resolveReferance2, 
            NameKey outputTypeName,
            Box<IType> box)
        {
            this.parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
            lines = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            this.outputTypeName = outputTypeName ?? throw new ArgumentNullException(nameof(outputTypeName));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IOpenBoxes<WeakMethodDefinition> Run(IResolveReferanceContext context)
        {
            var item = box.Fill(
                new WeakMethodDefinition(
                    methodScope.GetTypeOrThrow(outputTypeName),
                    parameter.Run(context).CodeElement.MemberDefinition, 
                    lines.Select(x => x.Run(context).CodeElement).ToArray(),
                    methodScope.GetFinalized(),
                    new ICodeElement[0]));
            return new MethodDefinitionOpenBoxes(item);
        }
    }

    internal class MethodDefinitionOpenBoxes : IOpenBoxes<WeakMethodDefinition>
    {
        public WeakMethodDefinition CodeElement { get; }

        public MethodDefinitionOpenBoxes(WeakMethodDefinition item)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
        }

        public T Run<T>(IOpenBoxesContext<T> context)
        {
            return context.MethodDefinition(CodeElement);
        }
    }
}