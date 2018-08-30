namespace Tac.Semantic_Model
{
    public interface IScoped<out TScope> where TScope : IScope
    {
        IScope Scope { get; }
    }
}