using System;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class LocalDefinition : IReferanced
    {
        public LocalDefinition(bool readOnly, TypeReferance type, AbstractName key)
        {
            ReadOnly = readOnly;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public bool ReadOnly { get; }
        public TypeReferance Type { get; }
        public AbstractName Key { get; }
    }

}