using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;

namespace Tac.Syntaz_Model_Interpeter
{

    internal interface IInterpetedTryAssignOperation : IInterpetedOperation<IBoxedBool>
    {

    }

    internal interface IInterpetedTryAssignOperation<out TLeft, in TRight, in TBlock> : IInterpetedTryAssignOperation
        where TLeft : IInterpetedAnyType
        where TRight : IInterpetedAnyType
        where TBlock : IInterpedEmpty
    { }

    internal class InterpetedTryAssignOperation<TLeft, TRight, TBlock> : IInterpetedTryAssignOperation<TLeft, TRight,TBlock>
        where TLeft : IInterpetedAnyType
        where TRight : IInterpetedAnyType
        where TBlock : IInterpedEmpty
    {

        public void Init(IInterpetedOperation<TLeft> left, IInterpetedOperation<TRight> right, IInterpetedOperation<TBlock> block, IInterpetedScopeTemplate scope)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
            Block = block ?? throw new ArgumentNullException(nameof(block));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }


        private IInterpetedOperation<TLeft>? left;
        public IInterpetedOperation<TLeft> Left { get => left ?? throw new NullReferenceException(nameof(left)); private set => left = value ?? throw new NullReferenceException(nameof(value)); }
        private IInterpetedOperation<TRight>? right;
        public IInterpetedOperation<TRight> Right { get => right ?? throw new NullReferenceException(nameof(right)); private set => right = value ?? throw new NullReferenceException(nameof(value)); }
        
        private IInterpetedOperation<TBlock>? block;
        public IInterpetedOperation<TBlock> Block { get => block ?? throw new NullReferenceException(nameof(right)); private set => block = value ?? throw new NullReferenceException(nameof(value)); }

        public IInterpetedScopeTemplate? scope;
        public IInterpetedScopeTemplate Scope { get => scope ?? throw new NullReferenceException(nameof(scope)); private set => scope = value ?? throw new NullReferenceException(nameof(value)); }


        public IInterpetedResult<IInterpetedMember<IBoxedBool>> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<IBoxedBool>>(leftReturned!);
            }

            var scope = interpetedContext.Child(Scope.Create());

            var rightResult = Right.Interpet(scope);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember<IBoxedBool>>(rightReturned!);
            }

            var res =TypeManager.Bool(rightValue!.CastTo<IInterpetedMemberSet<TRight>>().TrySet(leftValue!.Value));

            if (res.Value) {

                var blockResult = Block.Interpet(scope);

                if (blockResult.IsReturn(out var blockReturned, out var _))
                {
                    return InterpetedResult.Return<IInterpetedMember<IBoxedBool>>(blockReturned!);
                }

            }

            return InterpetedResult.Create(TypeManager.BoolMember(res));
        }
    }
}