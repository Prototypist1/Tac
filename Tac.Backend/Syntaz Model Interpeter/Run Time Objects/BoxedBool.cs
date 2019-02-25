namespace Tac.Syntaz_Model_Interpeter
{
    internal class BoxedBool: IInterpetedAnyType
    {
        public BoxedBool(bool value)
        {
            Value = value;
        }

        public bool Value { get; }
    }
}