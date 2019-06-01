using System;
using Tac.Frontend._2_Parser;
using Tac.Frontend._3_Syntax_Model.Operations;
using Tac.New;
using Tac.Parser;

namespace Tac.Semantic_Model
{
    /// <summary>
    /// make general types
    /// contains several type makers 
    /// </summary>
    internal class TypeMaker : IMaker<IPopulateScope<IWeakTypeReference>>
    {
        public ITokenMatching<IPopulateScope<IWeakTypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            {

                if (tokenMatching.Has(new TypeDefinitionMaker(), out var type)
                         is IMatchedTokenMatching matched)
                {
                    return TokenMatching<IPopulateScope<IWeakTypeReference>>.MakeMatch(
                            matched.Tokens,
                            matched.Context,
                            type);

                }
            }


            {
                if (tokenMatching.Has(new TypeReferanceMaker(), out var type)
                         is IMatchedTokenMatching matched)
                {
                    return TokenMatching<IPopulateScope<IWeakTypeReference>>.MakeMatch(
                            matched.Tokens,
                            matched.Context,
                            type);

                }
            }

            {
                if (tokenMatching.Has(new TypeOrOperationMaker(), out var type)
                         is IMatchedTokenMatching matched)
                {
                    return TokenMatching<IPopulateScope<IWeakTypeReference>>.MakeMatch(
                            matched.Tokens,
                            matched.Context,
                            type);

                }
            }

            return TokenMatching<IPopulateScope<IWeakTypeReference>>.MakeNotMatch(
                    tokenMatching.Context);
        }
    }
}
