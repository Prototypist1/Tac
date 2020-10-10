using Tac.Model.Elements;

namespace Tac.Backend.Emit.Support
{
    public interface ITacObject
    {
        ITacObject GetComplexMember(int position);
        T GetSimpleMember<T>(int position);
        ITacObject GetComplexReadonlyMember(int position);

        void SetComplexMember(ITacObject tacCastObject, int position);
        // ugh, this totally needs to be generic 
        void SetSimpleMember<Tin>(Tin value,int position);
        void SetComplexWriteonlyMember(ITacObject tacCastObject,int position);

        ITacObject SetComplexMemberReturn(ITacObject tacCastObject, int position);
        // ugh, this totally needs to be generic 
        Tin SetSimpleMemberReturn<Tin>(Tin value, int position);
        ITacObject SetComplexWriteonlyMemberReturn(ITacObject tacCastObject, int position);

        ITacObject Call_Complex_Complex(ITacObject input);
        ITacObject Call_Simple_Complex<Tin>(Tin input);
        Tout Call_Complex_Simple<Tout>(ITacObject input);
        Tout Call_Simple_Simple<Tin, Tout>(Tin input);

        IVerifiableType TacType();
    }
}