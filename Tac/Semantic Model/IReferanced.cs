namespace Tac.Semantic_Model
{
    internal interface IReferanced
    {
    }

    internal interface IReferanced<TKey> {
        TKey Key { get; }
    }
}