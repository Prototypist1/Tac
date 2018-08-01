namespace Tac.Semantic_Model
{
    public interface IScope
    {
        IScope EnclosingScope { get; }
        TReferanced Get<TReferanced>(IName key) where TReferanced : IReferanced;
    }
}