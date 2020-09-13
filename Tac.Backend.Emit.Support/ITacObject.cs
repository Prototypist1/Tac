namespace Tac.Backend.Emit.Support
{
    interface ITacObject
    {
        TacCastObject GetComplexMember(int position);
        T GetSimpleMember<T>(int position);
        void SetComplexMember(int position, TacCastObject tacCastObject);
        void SetSimpleMember(int position, object value);


        TacCastObject GetComplexReadonlyMember(int position);
        void SetComplexWriteonlyMember(int position, TacCastObject tacCastObject);
    }
}