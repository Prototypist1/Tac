namespace Tac.Backend.Emit.Support
{
    public interface ITacObject
    {
        ITacObject GetComplexMember(int position);
        T GetSimpleMember<T>(int position);
        ITacObject GetComplexReadonlyMember(int position);

        void SetComplexMember(ITacObject tacCastObject, int position);
        void SetSimpleMember(object value,int position);
        void SetComplexWriteonlyMember(ITacObject tacCastObject,int position);

        ITacObject SetComplexMemberReturn(ITacObject tacCastObject, int position);
        object SetSimpleMemberReturn(object value, int position);
        ITacObject SetComplexWriteonlyMemberReturn(ITacObject tacCastObject, int position);

        ITacObject Call_Complex_Complex(ITacObject input);
        ITacObject Call_Simple_Complex<Tin>(Tin input);
        Tout Call_Complex_Simple<Tout>(ITacObject input);
        Tout Call_Simple_Simple<Tin, Tout>(Tin input);
    }
}