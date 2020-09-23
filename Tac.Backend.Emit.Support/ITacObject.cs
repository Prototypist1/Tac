namespace Tac.Backend.Emit.Support
{
    public interface ITacObject
    {
        ITacObject GetComplexMember(int position);
        T GetSimpleMember<T>(int position);
        void SetComplexMember(int position, ITacObject tacCastObject);
        void SetSimpleMember(int position, object value);
        ITacObject GetComplexReadonlyMember(int position);
        void SetComplexWriteonlyMember(int position, ITacObject tacCastObject);

        ITacObject Call_Complex_Complex(ITacObject input);
        ITacObject Call_Simple_Complex<Tin>(Tin input);
        Tout Call_Complex_Simple<Tout>(ITacObject input);
        Tout Call_Simple_Simple<Tin, Tout>(Tin input);
    }
}