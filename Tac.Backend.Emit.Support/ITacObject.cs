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
    }
}