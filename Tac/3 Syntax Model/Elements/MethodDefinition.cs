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
    public class MethodDefinition : AbstractBlockDefinition, ITypeDefinition
    {
        public MethodDefinition(ITypeDefinition outputType, MemberDefinition parameterDefinition, ICodeElement[] body, MethodScope scope, IEnumerable<ICodeElement> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }

        public IBox<ITypeDefinition> InputType
        {
            get
            {
                return ParameterDefinition.Type;
            }
        }
        public ITypeDefinition OutputType { get; }
        public MemberDefinition ParameterDefinition { get; }

        public override IBox<ITypeDefinition> ReturnType(ScopeTree scopes)
        {
            return new ScopeStack(scopes, Scope).GetGenericType(new GenericExplicitTypeName(RootScope.MethodType.Name, InputType, OutputType));
        }

        public IBox<ITypeDefinition> GetTypeDefinition(ScopeTree scopes)
        {
            return new ScopeStack(scopes, Scope).GetGenericType(new GenericExplicitTypeName(RootScope.MethodType.Name, InputType, OutputType));
        }
    }


    public class MethodDefinitionMaker : IMaker<MethodDefinition>
    {
        public MethodDefinitionMaker(Func<MemberDefinition, IBox<ITypeDefinition>, IEnumerable<ICodeElement>, IScope, IEnumerable<ICodeElement>, MethodDefinition> make,
            IElementBuilders elementBuilders)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        private Func<MemberDefinition, IBox<ITypeDefinition>, IEnumerable<ICodeElement>, IScope, IEnumerable<ICodeElement>, MethodDefinition> Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<MethodDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("implementation"), out var _)
                .Has(ElementMatcher.Generic2, out AtomicToken inputType, out AtomicToken outputType)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken parameterName)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var methodScope = new MethodScope();

                var newMatchingContext = matchingContext.Child(methodScope);
                var elements = newMatchingContext.ParseBlock(body);

                var parameterDefinition = ElementBuilders.MemberDefinition.Make(
                        false,
                        new ExplicitMemberName(parameterName?.Item ?? "input"),
                        new ExplicitTypeName(inputType.Item)
                        );

                var outputTypeName = new ExplicitTypeName(outputType.Item);
                
                return ResultExtension.Good(new MethodDefinitionPopulateScope(parameterDefinition, methodScope, elements, outputTypeName, Make));
            }

            return ResultExtension.Bad<IPopulateScope<MethodDefinition>>();
        }

    }


    public class MethodDefinitionPopulateScope : IPopulateScope<MethodDefinition>
    {
        private readonly MemberDefinitionPopulateScope parameterDefinition;
        private readonly MethodScope methodScope;
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly ExplicitTypeName outputTypeName;
        private readonly Func<MemberDefinition, IBox<ITypeDefinition>, IEnumerable<ICodeElement>, IScope, IEnumerable<ICodeElement>, MethodDefinition> make;

        public MethodDefinitionPopulateScope(MemberDefinitionPopulateScope parameterDefinition, MethodScope methodScope, IPopulateScope<ICodeElement>[] elements, ExplicitTypeName outputTypeName, Func<MemberDefinition, IBox<ITypeDefinition>, IEnumerable<ICodeElement>, IScope, IEnumerable<ICodeElement>, MethodDefinition> make)
        {
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.outputTypeName = outputTypeName ?? throw new ArgumentNullException(nameof(outputTypeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReferance<MethodDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child(this, methodScope);
            return new MethodDefinitionResolveReferance(parameterDefinition.Run(nextContext), methodScope, elements.Select(x => x.Run(nextContext)).ToArray(), outputTypeName, make);
        }
    }

    public class MethodDefinitionResolveReferance : IResolveReferance<MethodDefinition>
    {
        private readonly MemberDefinitionResolveReferance parameter;
        private readonly MethodScope methodScope;
        private readonly IResolveReferance<ICodeElement>[] lines;
        private readonly ExplicitTypeName outputTypeName;
        private readonly Func<MemberDefinition, IBox<ITypeDefinition>, IEnumerable<ICodeElement>, IScope, IEnumerable<ICodeElement>, MethodDefinition> make;

        public MethodDefinitionResolveReferance(MemberDefinitionResolveReferance parameter, MethodScope methodScope, IResolveReferance<ICodeElement>[] resolveReferance2, ExplicitTypeName outputTypeName, Func<MemberDefinition, IBox<ITypeDefinition>, IEnumerable<ICodeElement>, IScope, IEnumerable<ICodeElement>, MethodDefinition> make)
        {
            this.parameter = parameter;
            this.methodScope = methodScope;
            lines = resolveReferance2;
            this.outputTypeName = outputTypeName;
            this.make = make;
        }

        public IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context)
        {
            return new ScopeStack(context.Tree, methodScope).GetType(new GenericExplicitTypeName(RootScope.ImplementationType, parameter.explicitTypeName, outputTypeName));
        }
        
        public MethodDefinition Run(IResolveReferanceContext context)
        {
            var nextContext = context.Child(this, methodScope);
            return make(parameter.Run(nextContext), new ScopeStack(context.Tree, methodScope).GetType(outputTypeName), lines.Select(x => x.Run(nextContext)).ToArray(), methodScope, new ICodeElement[0]);
        }
    }
}