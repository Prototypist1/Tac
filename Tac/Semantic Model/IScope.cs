namespace Tac.Semantic_Model
{
    public interface IScope
    {
        IScope EnclosingScope { get; }
        TReferanced Get<TKey, TReferanced>(TKey key)
            where TReferanced : IReferanced<TKey>;
    }
}