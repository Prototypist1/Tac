namespace Tac.Syntaz_Model_Interpeter
{
    public class BoxedBool: RunTimeAny
    {
        public BoxedBool(bool value)
        {
            Value = value;
        }

        public bool Value { get; }
    }
}