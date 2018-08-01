namespace Tac.Semantic_Model
{
    public interface IReferanced
    {
    }

    public interface IReferanced<TKey>: IReferanced
    {
        TKey Key { get; }
    }
}