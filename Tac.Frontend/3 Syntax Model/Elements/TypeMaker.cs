﻿using System;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.Parser;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendType>, Tpn.ITypeProblemNode>> StaticTypeMaker = AddTypeMaker(() => new TypeMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendType>, Tpn.ITypeProblemNode>> TypeMaker = StaticTypeMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}

namespace Tac.SemanticModel
{
    /// <summary>
    /// make general types
    /// contains several type makers 
    /// </summary>
    internal class TypeMaker : IMaker<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>
    {
        public ITokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            // {90C5FBAF-C5BC-4299-98C6-83A1B5109056}
            // TODO peal off ()


            if (tokenMatching.AllTokens[tokenMatching.StartIndex].SafeIs(out ParenthesisToken parenthesisToken))
            {
                var matching = TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeStart(parenthesisToken.Tokens, tokenMatching.Context ,0);
                var res = TryMake(matching);
                if (res is IMatchedTokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>> matched) {
                    return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                        tokenMatching.AllTokens,
                        tokenMatching.Context,
                        matched.Value,
                        tokenMatching.StartIndex,
                        tokenMatching.StartIndex+1);
                }
                else {
                    return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeNotMatch(
                            tokenMatching.Context);
                }
            }

            {

                if (tokenMatching.Has(new TypeDefinitionMaker(), out var type)
                         is IMatchedTokenMatching matched)
                {
                    return  TokenMatching<ISetUp<IBox<IFrontendType>,Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                            matched.AllTokens,
                            matched.Context,
                            type,
                            matched.StartIndex,
                            matched.EndIndex);
                }
            }

            {
                if (tokenMatching.Has(new TypeOrOperationMaker(), out var type)
                        is IMatchedTokenMatching matched)
                {
                    return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                            tokenMatching.AllTokens,
                            matched.Context,
                            type,
                            matched.StartIndex,
                            matched.EndIndex);

                }
            }

            {
                if (tokenMatching.Has(new TypeReferanceMaker(), out var type)
                         is IMatchedTokenMatching matched)
                {
                    return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                            matched.AllTokens,
                            matched.Context,
                            type,
                            matched.StartIndex,
                            matched.EndIndex);
                }
            }

            return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeNotMatch(
                    tokenMatching.Context);
        }
    }

    internal class TypeMakerAlreadyMatched : IMaker<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>
    {
        public ITokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {

            var at = tokenMatching.EndIndex;

            var or = tokenMatching.Context.Map.GetGreatestParent(tokenMatching.AllTokens[at]);

            if (or.Is1(out var v1) && v1.SafeIs(out ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference> mySetUp)) {

                var list = new List<int>();
                for (int i = 0; i < tokenMatching.AllTokens.Count; i++)
                {
                    var token = tokenMatching.AllTokens[i];
                    if (tokenMatching.Context.Map.GetGreatestParent(token).Is1(out var setUp) && setUp == mySetUp) {

                        list.Add(i);
                    }
                }

                return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                    tokenMatching.AllTokens,
                    tokenMatching.Context,
                    mySetUp,
                    list.Min(),
                    list.Max()+1);

            }
            return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeNotMatch(tokenMatching.Context);
        }
    }

    internal class TypeMakerNoOp : IMaker<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>
    {
        public ITokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            // {90C5FBAF-C5BC-4299-98C6-83A1B5109056}
            // TODO peal off ()

            if (tokenMatching.AllTokens[tokenMatching.StartIndex].SafeIs(out ParenthesisToken parenthesisToken))
            {
                var matching = TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeStart(parenthesisToken.Tokens, tokenMatching.Context, 0);
                var res = TryMake(matching);
                if (res is IMatchedTokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>> matched)
                {
                    return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                        tokenMatching.AllTokens,
                        tokenMatching.Context,
                        matched.Value,
                        tokenMatching.StartIndex,
                        tokenMatching.StartIndex + 1);
                }
                else
                {
                    return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeNotMatch(
                            tokenMatching.Context);
                }
            }


            {

                if (tokenMatching.Has(new TypeDefinitionMaker(), out var type)
                         is IMatchedTokenMatching matched)
                {
                    return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                            matched.AllTokens,
                            matched.Context,
                            type,
                            matched.StartIndex,
                            matched.EndIndex);
                }
            }

            {
                if (tokenMatching.Has(new TypeReferanceMaker(), out var type)
                         is IMatchedTokenMatching matched)
                {
                    return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                            matched.AllTokens,
                            matched.Context,
                            type,
                            matched.StartIndex,
                            matched.EndIndex);
                }
            }


            return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeNotMatch(
                    tokenMatching.Context);
        }
    }
}
