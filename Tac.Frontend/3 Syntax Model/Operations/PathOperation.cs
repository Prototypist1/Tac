﻿using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;

namespace Tac.SemanticModel.CodeStuff
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
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticPathMaker = AddOperationMatcher(() => new PathOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> PathMaker = StaticPathMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}

namespace Tac.SemanticModel.Operations
{
    internal class WeakPathOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IPathOperation>
    {
        public WeakPathOperation(OrType<IBox<IFrontendCodeElement>, IError> left, OrType<IBox<IFrontendCodeElement>, IError> right) : base(left, right)
        {
        }

        public override IBuildIntention<IPathOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = PathOperation.Create();
            return new BuildIntention<IPathOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.Convert(x => x.GetValue().ConvertElementOrThrow(context)),
                    Right.Convert(x => x.GetValue().ConvertElementOrThrow(context)));
            });
        }
    }


    internal class PathOperationMaker : IMaker<ISetUp<WeakPathOperation, Tpn.TypeProblem2.Member>>
    {

        public PathOperationMaker()
        {
        }


        public ITokenMatching<ISetUp<WeakPathOperation, Tpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .HasStruct(new BinaryOperationMatcher(SymbolsRegistry.StaticPathSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching)
            {
                if (tokenMatching.Tokens[tokenMatching.Tokens.Count - 1] is ElementToken elementToken &&
                    elementToken.Tokens.Single() is AtomicToken atomic)
                {
                    var left = matching.Context.ParseLine(match.perface);
                    //var right = matching.Context.ExpectPathPart(box).ParseParenthesisOrElement(match.rhs);

                    return TokenMatching<ISetUp<WeakPathOperation, Tpn.TypeProblem2.Member>>.MakeMatch(
                        Array.Empty<IToken>(),
                        matching.Context,
                        new WeakPathOperationPopulateScope(left, atomic.Item));
                }
            }

            return TokenMatching<ISetUp<WeakPathOperation, Tpn.TypeProblem2.Member>>.MakeNotMatch(
                    matching.Context);
        }


        private class WeakPathOperationPopulateScope : ISetUp<WeakPathOperation, Tpn.TypeProblem2.Member>
        {
            private readonly OrType<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>, IError> left;
            private readonly string name;

            public WeakPathOperationPopulateScope(OrType<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>, IError> left,
                string name)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.name = name ?? throw new ArgumentNullException(nameof(name));
            }

            public ISetUpResult<WeakPathOperation, Tpn.TypeProblem2.Member> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var nextLeft = left.Convert(x => x.Run(scope, context));

                OrType<Tpn.TypeProblem2.Member, IError> member;


                if (nextLeft.Is1(out var leftResult))
                {
                    if (leftResult.SetUpSideNode is Tpn.IValue value)
                    {
                        member = new OrType<Tpn.TypeProblem2.Member, IError>(context.TypeProblem.CreateHopefulMember(
                            value,
                            new NameKey(name),
                            new WeakMemberDefinitionConverter(false, new NameKey(name))));
                    }
                    else
                    {
                        member = new OrType<Tpn.TypeProblem2.Member, IError>(new Error(""));
                        // todo better error handling 
                        throw new Exception($"can not . off {leftResult.SetUpSideNode}");
                    }
                }
                else if (nextLeft.Is2(out var rightResult))
                {
                    member = new OrType<Tpn.TypeProblem2.Member, IError>(new Error("We needed ", rightResult));
                }
                else {
                    throw new Exception("uhh we are outside the else");
                }

                return new SetUpResult<WeakPathOperation, Tpn.TypeProblem2.Member>(new WeakPathOperationResolveReference(
                    nextLeft.Convert(x=>x.Resolve),
                    member), 
                    member);
            }
        }

        private class WeakPathOperationResolveReference : IResolve<WeakPathOperation>
        {
            readonly OrType<IResolve<IFrontendCodeElement>,IError> left;
            readonly OrType<Tpn.TypeProblem2.Member,IError> member;

            public WeakPathOperationResolveReference(
                OrType<IResolve<IFrontendCodeElement>, IError> resolveReference,
                OrType<Tpn.TypeProblem2.Member, IError> member)
            {
                left = resolveReference ?? throw new ArgumentNullException(nameof(resolveReference));
                this.member = member ?? throw new ArgumentNullException(nameof(member));
            }

            public IBox<WeakPathOperation> Run(Tpn.ITypeSolution context)
            {
                var res = new Box<WeakPathOperation>(new WeakPathOperation(
                    left.Convert(x => x.Run(context)),
                    member.SwitchReturns(
                        x => new OrType<IBox<IFrontendCodeElement>, IError>(new Box<WeakMemberReference>(new WeakMemberReference(context.GetMember(x)))),
                        y => new OrType<IBox<IFrontendCodeElement>, IError>(new Error("", y)))));
                return res;
            }
        }
    }
}
