using System;

namespace Tac.Syntaz_Model_Interpeter
{
    internal abstract class InterpetedBinaryOperation: IInterpeted {
        public void Init(IInterpeted left, IInterpeted right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public abstract InterpetedResult Interpet(InterpetedContext interpetedContext);

        public IInterpeted Left { get; private set; }
        public IInterpeted Right { get; private set; }
    }
}