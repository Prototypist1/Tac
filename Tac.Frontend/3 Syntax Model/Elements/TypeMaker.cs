using System;
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
            {

                if (tokenMatching.Has(new TypeDefinitionMaker(), out var type)
                         is IMatchedTokenMatching matched)
                {
                    return TokenMatching<ISetUp<IBox<IFrontendType>,Tpn.TypeProblem2.TypeReference>>.MakeMatch(
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

            // TODO

            // I need a more well-rounded approach to parenthesis 
            // I don't think this will work generally at all

            // gosh, what to do? I don't really want to kick the can down the road,
            // but I don't feel like I understand it well enough to make changes

            // I mean for the short term I am just going to jam all the type operators in here 
            // maybe that is ok
            
            if (tokenMatching.AllTokens[tokenMatching.EndIndex] is ParenthesisToken parenthesisToken) {
                if (TokenMatching<ISetUp<IFrontendType, Tpn.IExplicitType>>.MakeStart(parenthesisToken.Tokens.ToArray(), tokenMatching.Context, 0).Has(new TypeOrOperationMaker(), out var type)
                        is IMatchedTokenMatching matched)
                {
                    return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                            tokenMatching.AllTokens,
                            matched.Context,
                            type,
                            tokenMatching.EndIndex,
                            tokenMatching.EndIndex+1);

                }
            }
            
            return TokenMatching<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>.MakeNotMatch(
                    tokenMatching.Context);
        }
    }
}
