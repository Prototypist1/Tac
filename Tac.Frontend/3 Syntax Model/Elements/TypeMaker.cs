﻿using System;
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
            var matching = tokenMatching.HasOne(
                new Func<ITokenMatching, ITokenMatching<IPopulateScope<IWeakTypeReference>>>[] {
                    w => w.Has(new TypeReferanceMaker(), out var _).Has(new DoneMaker()),
                    w => w.Has(new TypeDefinitionMaker(), out var _).Has(new DoneMaker()),
                    w => w.Has(new TypeOrOperationMaker(), out var _).Has(new DoneMaker())
                },
                out var type);

            if (matching
                     is IMatchedTokenMatching matched)
            {
                TokenMatching<IPopulateScope<IWeakTypeReference>>.MakeMatch(
                        matched.Tokens,
                        matched.Context,
                        type);

            }

            return TokenMatching<IPopulateScope<IWeakTypeReference>>.MakeNotMatch(
                    matching.Context);
        }
    }
}
