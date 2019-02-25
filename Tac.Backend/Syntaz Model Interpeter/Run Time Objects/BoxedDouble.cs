namespace Tac.Syntaz_Model_Interpeter
{
    internal class BoxedDouble: IInterpetedAnyType
    {
        public BoxedDouble(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }
}