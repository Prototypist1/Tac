using System.Collections.Generic;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IBox<out T> where T : class, ICodeElement
    {

    }

    public class Box<T> : IBox<T> where T : class, ICodeElement
    {

        public Box(T innerType)
        {
            InnerType = innerType ?? throw new System.ArgumentNullException(nameof(innerType));
        }

        private T InnerType { get; }
    }

    // TODO split scopes out in to socpes an scope builders
    public interface IScope
    {
        IReadOnlyList<IBox<MemberDefinition>> Members { get; }

        bool TryGetType(NameKey name, out IBox<ITypeDefinition> type);
        bool TryGetGenericType(NameKey name, IEnumerable<ITypeDefinition> genericTypeParameters, out IBox<GenericTypeDefinition> typeDefinition);
        bool TryGetMember(NameKey name, bool staticOnly, out IBox<MemberDefinition>
            member);
    }
}