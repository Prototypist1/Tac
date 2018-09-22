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
    public class MethodDefinition : AbstractBlockDefinition,  ITypeDefinition
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
            return new ScopeStack(scopes,Scope).GetGenericType(new GenericExplicitTypeName(RootScope.MethodType.Name, InputType, OutputType));
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

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out Steps.PopulateScope<MethodDefinition> result)
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

                result = PopulateScope( parameterDefinition, methodScope, elements, outputTypeName);
                return true;
            }

            result = default;
            return false;
        }

        private Steps.PopulateScope<MethodDefinition> PopulateScope(
            Steps.PopulateScope<MemberDefinition> parameterDefintion,
            LocalStaticScope scope, Steps.PopulateScope<ICodeElement>[] elements,
            ExplicitTypeName outputTypeName)
        {
            return () =>
            {
                return DetermineInferedTypes(parameterDefintion(), scope, elements.Select(x => x()).ToArray(), outputTypeName);
            };
        }

        private Steps.DetermineInferedTypes<MethodDefinition> DetermineInferedTypes(
            Steps.DetermineInferedTypes<MemberDefinition> parameterDefintion,
            LocalStaticScope scope, Steps.DetermineInferedTypes<ICodeElement>[] elements,
            ExplicitTypeName outputTypeName)
        {
            return () =>
            {

                return ResolveReferance(parameterDefintion(), scope, elements.Select(x => x()).ToArray(), outputTypeName);
            };
        }

        private Steps.ResolveReferance<MethodDefinition> ResolveReferance(
            Steps.ResolveReferance<MemberDefinition> parameterDefintion,
            LocalStaticScope scope, Steps.ResolveReferance<ICodeElement>[] elements,
            ExplicitTypeName outputTypeName)
        {
            return (tree) =>
            {
                return Make(parameterDefintion(tree), new ScopeStack(tree, scope).GetType(outputTypeName), elements.Select(x => x(tree)).ToArray(), scope, new ICodeElement[0]);
            };
        }
    }
}