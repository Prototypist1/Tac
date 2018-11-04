using System;
using Prototypist.LeftToRight;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedPathOperation : InterpetedBinaryOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var scope = Left.Interpet(interpetedContext).Cast<InterpetedMember>().Value.Cast<IInterpetedScope>();

            // I think not call Interpet on Right is a smell
            // that said, InterpetedMemberReferance does different things in different contexts

            // x := 5       we want x to end up being a member so we can set it
            // x + 3        we want x to be evaulated all the way down to a 5
            // this . x     we just want the referance so we can look in this 

            // maybe the above somehow asks for what it wants?

            // maybe it is the callers job to manage
            // x returns a member referance
            // and the consumer resolves that to what it needs

            // so member definition (int x) returns a member object
            // and member referance (x) returns a member definition
            // 
            // these are both used by assign
            //
            // int x := 5
            // x := 5
            // 
            // AssignOperation wants a member, and does not put it in the context at all
            // this is fair, it would not know where to put it
            //
            // what a mess
            //
            //
            // TODO you are here 
            //
            // so:
            // 1 - caller should handle whatever type it gets
            // 2 - member definiton and member refereance should both return member definition
            
            return InterpetedResult.Create(scope.GetMember(Right.Interpet(interpetedContext).Cast<InterpetedMemberDefinition>().Key));
        }
    }
}