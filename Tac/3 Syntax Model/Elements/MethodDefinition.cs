﻿using System;
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

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out IPopulateScope<MethodDefinition> result)
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

                result = new MethodDefinitionPopulateScope( parameterDefinition, methodScope, elements, outputTypeName,Make);
                return true;
            }

            result = default;
            return false;
        }

        private class MethodDefinitionPopulateScope : IPopulateScope<MethodDefinition>
        {
            private readonly IPopulateScope<MemberDefinition> parameterDefinition;
            private readonly MethodScope methodScope;
            private readonly IPopulateScope<ICodeElement>[] elements;
            private readonly ExplicitTypeName outputTypeName;
            private readonly Func<MemberDefinition, IBox<ITypeDefinition>, IEnumerable<ICodeElement>, IScope, IEnumerable<ICodeElement>, MethodDefinition> make;

            public MethodDefinitionPopulateScope(IPopulateScope<MemberDefinition> parameterDefinition, MethodScope methodScope, IPopulateScope<ICodeElement>[] elements, ExplicitTypeName outputTypeName, Func<MemberDefinition, IBox<ITypeDefinition>, IEnumerable<ICodeElement>, IScope, IEnumerable<ICodeElement>, MethodDefinition> make)
            {
                this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.outputTypeName = outputTypeName ?? throw new ArgumentNullException(nameof(outputTypeName));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public IResolveReferance<MethodDefinition> Run(ScopeTree tree)
            {
                return new MethodDefinitionResolveReferance(parameterDefinition.Run(tree), methodScope, elements.Select(x => x.Run(tree)).ToArray(), outputTypeName,make);
            }
        }

        private class MethodDefinitionResolveReferance : IResolveReferance<MethodDefinition>
        {
            private readonly IResolveReferance<MemberDefinition> parameter;
            private readonly MethodScope methodScope;
            private readonly IResolveReferance<ICodeElement>[] lines;
            private readonly ExplicitTypeName outputTypeName;
            private readonly Func<MemberDefinition, IBox<ITypeDefinition>, IEnumerable<ICodeElement>, IScope, IEnumerable<ICodeElement>, MethodDefinition> make;

            public MethodDefinitionResolveReferance(IResolveReferance<MemberDefinition> resolveReferance1, MethodScope methodScope, IResolveReferance<ICodeElement>[] resolveReferance2, ExplicitTypeName outputTypeName, Func<MemberDefinition, IBox<ITypeDefinition>, IEnumerable<ICodeElement>, IScope, IEnumerable<ICodeElement>, MethodDefinition> make)
            {
                this.parameter = resolveReferance1;
                this.methodScope = methodScope;
                this.lines = resolveReferance2;
                this.outputTypeName = outputTypeName;
                this.make = make;
            }

            public MethodDefinition Run(ScopeTree tree)
            {
                return make(parameter.Run(tree), new ScopeStack(tree, methodScope).GetType(outputTypeName), lines.Select(x => x.Run(tree)).ToArray(), methodScope, new ICodeElement[0]);
            }
        }
    }
}