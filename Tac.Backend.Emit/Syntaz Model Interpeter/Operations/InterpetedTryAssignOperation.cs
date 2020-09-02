using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;

namespace Tac.Backend.Emit.SyntaxModel
{

    internal interface IInterpetedTryAssignOperation : IInterpetedOperation
    {

    }


    internal class InterpetedTryAssignOperation : IInterpetedTryAssignOperation
    {

        public void Init(IInterpetedOperation left, IInterpetedOperation right, IInterpetedOperation block, IInterpetedScopeTemplate scope)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
            Block = block ?? throw new ArgumentNullException(nameof(block));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }


        private IInterpetedOperation? left;
        public IInterpetedOperation Left { get => left ?? throw new NullReferenceException(nameof(left)); private set => left = value ?? throw new NullReferenceException(nameof(value)); }
        private IInterpetedOperation? right;
        public IInterpetedOperation Right { get => right ?? throw new NullReferenceException(nameof(right)); private set => right = value ?? throw new NullReferenceException(nameof(value)); }
        
        private IInterpetedOperation? block;
        public IInterpetedOperation Block { get => block ?? throw new NullReferenceException(nameof(right)); private set => block = value ?? throw new NullReferenceException(nameof(value)); }

        public IInterpetedScopeTemplate? scope;
        public IInterpetedScopeTemplate Scope { get => scope ?? throw new NullReferenceException(nameof(scope)); private set => scope = value ?? throw new NullReferenceException(nameof(value)); }


        public IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember>(leftReturned!);
            }

            var scope = interpetedContext.Child(Scope.Create());

            var rightResult = Right.Interpet(scope);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember>(rightReturned!);
            }

            var res =TypeManager.Bool(rightValue!.CastTo<IInterpetedMemberSet>().TrySet(leftValue!.Value));

            if (res.Value) {

                var blockResult = Block.Interpet(scope);

                if (blockResult.IsReturn(out var blockReturned, out var _))
                {
                    return InterpetedResult.Return<IInterpetedMember>(blockReturned!);
                }

            }

            return InterpetedResult.Create(TypeManager.BoolMember(res));
        }
    }
}