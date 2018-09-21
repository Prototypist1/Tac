using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model.Names
{
    public interface IKey { }

    public class NameKey : IKey
    {
        public NameKey(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public override bool Equals(object obj)
        {
            return obj is NameKey key &&
                   Name == key.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }

    public class ExplicitTypeName 
    {
        public ExplicitTypeName(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
        
        public NameKey Key
        {
            get
            {
                return new NameKey(Name);
            }
        }
        
    }

    public class GenericExplicitTypeName : ExplicitTypeName
    {
        public GenericExplicitTypeName(string name, params ITypeDefinition[] types) : base(name)
        {
            Types = types ?? throw new System.ArgumentNullException(nameof(types));
        }

        public ITypeDefinition[] Types { get; }

        public virtual bool TryGetTypeDefinition(ScopeStack scope, out ITypeDefinition typeDefinition)
        {
            typeDefinition = scope.GetGenericType(this);
            return typeDefinition == default;
        }
    }
    
    public class ExplicitMemberName 
    {
        public ExplicitMemberName(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
        
        public NameKey Key
        {
            get
            {
                return new NameKey(Name);
            }
        }
    }

}
