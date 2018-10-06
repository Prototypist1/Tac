using Prototypist.LeftToRight;
using System;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{
    public class PathOperation : BinaryOperation<Member, PathPart>
    {
        public PathOperation(Member left, PathPart right) : base(left, right)
        {
        }

        public override IBox<ITypeDefinition> ReturnType()
        {
            if (!left.Cast<IScoped>().Scope.TryGetMember(right.MemberDefinition.Key,false,out var check)){
                throw new Exception("Member should be defined");
            }

            if (!check.GetValue().Type.GetValue().Key.Equals(right.MemberDefinition.GetValue().Type.GetValue().Key)) {
                throw new Exception("we have two ways to get to the type, they better have the same value");
            }
            
            return right.MemberDefinition.GetValue().Type;
        }
    }

    // the matching on the LHS is different ??
    //public class PathOperationMaker : BinaryOperationMaker<PathOperation>
    //{
    //    public PathOperationMaker(Func<ICodeElement, ICodeElement, PathOperation> make, IElementBuilders elementBuilders) : base(".", make, elementBuilders)
    //    {
    //    }
    //}
}
