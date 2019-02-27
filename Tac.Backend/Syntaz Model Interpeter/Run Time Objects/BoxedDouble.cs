namespace Tac.Syntaz_Model_Interpeter
{
    internal class BoxedDouble: RunTimeAny
    {
        public BoxedDouble(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }
}