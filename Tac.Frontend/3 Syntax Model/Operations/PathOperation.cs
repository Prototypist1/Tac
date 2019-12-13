﻿using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticPathSymbol = StaticSymbolsRegistry.AddOrThrow(".");
        public readonly string PathSymbol = StaticPathSymbol;
    }
}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticPathMaker = AddOperationMatcher(() => new PathOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> PathMaker = StaticPathMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}

namespace Tac.Semantic_Model.Operations
{
    internal class WeakPathOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IPathOperation>
    {
        public WeakPathOperation(IBox<IFrontendCodeElement> left, IBox<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<IPathOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = PathOperation.Create();
            return new BuildIntention<IPathOperation>(toBuild, () =>
            {
                maker.Build(Left.GetValue().ConvertElementOrThrow(context), Right.GetValue().ConvertElementOrThrow(context));
            });
        }
    }


    internal class PathOperationMaker : IMaker<ISetUp<WeakPathOperation, LocalTpn.TypeProblem2.Member>>
    {

        public PathOperationMaker()
        {
        }


        public ITokenMatching<ISetUp<WeakPathOperation, LocalTpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(SymbolsRegistry.StaticPathSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var matching2 = matched.Has(new NameMaker(), out var first);
                if (matching2 is IMatchedTokenMatching matched2)
                {
                    
                    var left = matching.Context.ParseLine(match.perface);
                    //var right = matching.Context.ExpectPathPart(box).ParseParenthesisOrElement(match.rhs);

                    return TokenMatching<ISetUp<WeakPathOperation, LocalTpn.TypeProblem2.Member>>.MakeMatch(
                        matched2.Tokens,
                        matched2.Context,
                        new WeakPathOperationPopulateScope(left, first.Item));
                }
            }

            return TokenMatching<ISetUp<WeakPathOperation, LocalTpn.TypeProblem2.Member>>.MakeNotMatch(
                    matching.Context);
        }


        private class WeakPathOperationPopulateScope : ISetUp<WeakPathOperation, LocalTpn.TypeProblem2.Member>
        {
            private readonly ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode> left;
            private readonly string name;

            public WeakPathOperationPopulateScope(ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode> left,
                string name)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.name = name ?? throw new ArgumentNullException(nameof(name));
            }

            public ISetUpResult<WeakPathOperation, LocalTpn.TypeProblem2.Member> Run(LocalTpn.IScope scope, ISetUpContext context)
            {
                var nextLeft = left.Run(scope, context);
                var member = context.TypeProblem.CreateHopefulMember((LocalTpn.IHaveHopefulMembers)nextLeft, new NameKey(name), new WeakMemberDefinitionConverter(false,new NameKey(name)));

                return new SetUpResult<WeakPathOperation, LocalTpn.TypeProblem2.Member>(new WeakPathOperationResolveReferance(
                    left.Run(scope, context).Resolve,
                     member),member);
            }
        }

        private class WeakPathOperationResolveReferance : IResolve<WeakPathOperation>
        {
            readonly IResolve<IFrontendCodeElement> left;
            readonly Tpn<WeakBlockDefinition, Prototypist.Fluent.OrType<WeakTypeDefinition, WeakGenericTypeDefinition>, Prototypist.Fluent.OrType<WeakObjectDefinition, WeakModuleDefinition>, Frontend._3_Syntax_Model.Operations.WeakTypeOrOperation, Prototypist.Fluent.OrType<WeakMethodDefinition, WeakImplementationDefinition>, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Member member;

            public WeakPathOperationResolveReferance(
                IResolve<IFrontendCodeElement> resolveReferance1, 
                Tpn<WeakBlockDefinition, Prototypist.Fluent.OrType<WeakTypeDefinition, WeakGenericTypeDefinition>, Prototypist.Fluent.OrType<WeakObjectDefinition, WeakModuleDefinition>, Frontend._3_Syntax_Model.Operations.WeakTypeOrOperation, Prototypist.Fluent.OrType<WeakMethodDefinition, WeakImplementationDefinition>, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Member member)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                this.member = member ?? throw new ArgumentNullException(nameof(member));
            }

            public IBox<WeakPathOperation> Run(LocalTpn.ITypeSolution context)
            {
                var convertedMember = context.GetMember(member);

                var res = new Box<WeakPathOperation>(new WeakPathOperation(
                    left.Run(context),
                    new Box<WeakMemberReference>(new WeakMemberReference(convertedMember))));
                return res;
            }
        }
    }
}
