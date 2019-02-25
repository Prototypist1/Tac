namespace Tac.Syntaz_Model_Interpeter
{
    internal class BoxedString: IInterpetedAnyType
    {
        public BoxedString(string value)
        {
            Value = value ?? throw new System.ArgumentNullException(nameof(value));
        }

        public string Value { get; }

    }
}