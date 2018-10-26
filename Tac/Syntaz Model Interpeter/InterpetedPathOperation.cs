using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedPathOperation : WeakPathOperation, IInterpeted
    {
        public InterpetedPathOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var scope = left.Cast<IInterpeted>().Interpet(interpetedContext).Cast<InterpetedMember>().Value.Cast<IInterpetedScope>();
            
            return  InterpetedResult.Create(scope.GetMember(right.Cast<WeakMemberReferance>().MemberDefinition.GetValue().Key));
        }

        internal static WeakPathOperation MakeNew(IWeakCodeElement left, IWeakCodeElement right)
        {
            return new InterpetedPathOperation(left, right);
        }
    }
}