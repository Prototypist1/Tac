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

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>> StaticTypeMaker = AddTypeMaker(() => new TypeMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>> TypeMaker = StaticTypeMaker;
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
    internal class TypeMaker : IMaker<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>>
    {
        public ITokenMatching<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            {

                if (tokenMatching.Has(new TypeDefinitionMaker(), out var type)
                         is IMatchedTokenMatching matched)
                {
                    return TokenMatching<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                            matched.Tokens,
                            matched.Context,
                            type!);
                }
            }


            {
                if (tokenMatching.Has(new TypeReferanceMaker(), out var type)
                         is IMatchedTokenMatching matched)
                {
                    return TokenMatching<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                            matched.Tokens,
                            matched.Context,
                            type!);
                }
            }

            // TODO

            // I need a more well-rounded approach to parenthesis 
            // I don't think this will work generally at all

            // gosh, what to do? I don't really want to kick the can down the road,
            // but I don't feel like I understand it well enough to make changes

            // I mean for the short term I am just going to jam all the type operators in here 
            // maybe that is ok
            
            if (tokenMatching.Tokens[0] is ParenthesisToken parenthesisToken) {
                if (TokenMatching<ISetUp<IFrontendType, Tpn.IExplicitType>>.MakeStart(parenthesisToken.Tokens.ToArray(), tokenMatching.Context).Has(new TypeOrOperationMaker(), out var type)
                        is IMatchedTokenMatching matched)
                {
                    return TokenMatching<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                            tokenMatching.Tokens.Skip(1).ToArray(),
                            matched.Context,
                            type!);

                }
            }
            
            return TokenMatching<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>>.MakeNotMatch(
                    tokenMatching.Context);
        }
    }
}
