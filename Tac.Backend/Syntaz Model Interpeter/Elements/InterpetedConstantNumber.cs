using System;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedConstantNumber : IInterpetedOperation<double>
    {
        public void Init(double value) {
            this.Value = value;
        }

        public double Value { get; private set; }

        public IInterpetedResult<IInterpetedMember<double>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new RuntimeNumber(Value));
        }
        
        void IInterpetedOperation.Interpet(InterpetedContext interpetedContext)
        {
            Interpet(interpetedContext);
        }
    }
}