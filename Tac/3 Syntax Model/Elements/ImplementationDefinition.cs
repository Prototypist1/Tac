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

    public class ImplementationDefinition: ITypeDefinition
    {
        public ImplementationDefinition(MemberDefinition contextDefinition, ITypeDefinition outputType, MemberDefinition parameterDefinition, IEnumerable<ICodeElement> metohdBody, IScope scope, IEnumerable<ICodeElement> staticInitializers)
        {
            ContextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            MethodBody = metohdBody ?? throw new ArgumentNullException(nameof(metohdBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialzers = staticInitializers ?? throw new ArgumentNullException(nameof(staticInitializers));
        }

        // dang! these could also be inline definitions 
        public ITypeDefinition ContextType
        {
            get
            {
                return ContextDefinition.Type;
            }
        }
        public ITypeDefinition InputType
        {
            get
            {
                return ParameterDefinition.Type;
            }
        }
        public ITypeDefinition OutputType { get; }
        public MemberDefinition ContextDefinition { get; }
        public MemberDefinition ParameterDefinition { get; }
        public IScope Scope { get; }
        public IEnumerable<ICodeElement> MethodBody { get; }
        public IEnumerable<ICodeElement> StaticInitialzers { get; }
        
        public ITypeDefinition ReturnType(ScopeStack scope) {
                return scope.GetGenericType(new GenericExplicitTypeName(RootScope.ImplementationType.Name,new ITypeDefinition[] { ContextType, InputType, OutputType }));
        }

        public ITypeDefinition GetTypeDefinition(ScopeStack scopeStack)
        {
            return scopeStack.GetGenericType(new GenericExplicitTypeName(RootScope.ImplementationType.Name, new ITypeDefinition[] { ContextType, InputType, OutputType }));
        }
    }

    public class ImplementationDefinitionMaker : IMaker<ImplementationDefinition>
    {
        public ImplementationDefinitionMaker(Func<MemberDefinition , ITypeDefinition , MemberDefinition , IEnumerable<ICodeElement> , IScope , IEnumerable<ICodeElement> , ImplementationDefinition> make,
            IElementBuilders elementBuilders)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        private Func<MemberDefinition, ITypeDefinition, MemberDefinition, IEnumerable<ICodeElement>, IScope, IEnumerable<ICodeElement>, ImplementationDefinition> Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out Steps.PopulateScope result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("implementation"), out var _)
                .Has(ElementMatcher.Generic3, out AtomicToken contextType, out AtomicToken inputType, out AtomicToken outputType)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken contextName)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken parameterName)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var methodScope = new MethodScope();

                var newMatchingContext = matchingContext.Child(methodScope);
                var elements = newMatchingContext.ParseBlock(body);

                var contextDefinition = ElementBuilders.MemberDefinition.Make(
                        false,
                        new ExplicitMemberName(parameterName?.Item ?? "context"),
                        new ExplicitTypeName(contextType.Item)
                        );
                

                var parameterDefinition = ElementBuilders.MemberDefinition.Make(
                        false,
                        new ExplicitMemberName(parameterName?.Item ?? "input"),
                        new ExplicitTypeName(inputType.Item)
                        );

                var outputTypeName= new ExplicitTypeName(outputType.Item);

                result = PopulateScope(contextDefinition, parameterDefinition, methodScope, elements, outputTypeName);
                return true;
            }

            result = default;
            return false;
        }

        private Steps.PopulateScope<ImplementationDefinition> PopulateScope(
            Steps.PopulateScope<MemberDefinition> contextDefinition,
            Steps.PopulateScope<MemberDefinition> parameterDefintion,
            LocalStaticScope scope, Steps.PopulateScope<ICodeElement>[] elements,
            ExplicitTypeName outputTypeName)
        {
            return () =>
            {
                return DetermineInferedTypes(contextDefinition(), parameterDefintion(),scope, elements.Select(x => x()).ToArray());
            };
        }

        private Steps.DetermineInferedTypes<ImplementationDefinition> DetermineInferedTypes(
            Steps.DetermineInferedTypes<MemberDefinition> contextDefinition,
            Steps.DetermineInferedTypes<MemberDefinition> parameterDefintion,
            LocalStaticScope scope, Steps.DetermineInferedTypes<ICodeElement>[] elements,
            ExplicitTypeName outputTypeName)
        {
            return () => ResolveReferance(contextDefinition(), parameterDefintion(), scope, elements.Select(x => x()).ToArray());
        }

        private Steps.ResolveReferance<ImplementationDefinition> ResolveReferance(Steps.ResolveReferance<MemberDefinition> contextDefinition,
            Steps.ResolveReferance<MemberDefinition> parameterDefintion,
            LocalStaticScope scope, Steps.ResolveReferance<ICodeElement>[] elements,
            ExplicitTypeName outputTypeName)
        {
            return () =>
            {
                return Make(contextDefinition(), parameterDefintion(), elements.Select(x => x()).ToArray(), scope, new ICodeElement[0]);
            };
        }
    }
}
