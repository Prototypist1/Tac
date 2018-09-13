using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

// TODO you have names and type sources
// and it sucks to have both
// think about this 
// do we know when something is a type referance?
// pretty much yes...

namespace Tac.Semantic_Model.Names
{

    public interface IKeyd {
        IKey Key { get; }
    }

    public interface IKey { }
    
    public class NameKey: IKey
    {
        public NameKey(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        public string Name { get; }

        public override bool Equals(object obj)
        {
            return obj is NameKey key &&
                   Name == key.Name;
        }

        public override int GetHashCode() => 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
    }


    public interface ITypeSource : ICodeElement
    {
        ITypeDefinition GetTypeDefinition(ScopeStack scopeStack);
    }
    
    // var
    // TODO the way this works...
    // IScope does not need that weird backdoor for implicitTypes...
    // TODO remove!
    public sealed class ImplicitTypeReferance : ITypeSource
    {
        public ImplicitTypeReferance(ICodeElement codeElement) => CodeElement = codeElement ?? throw new ArgumentNullException(nameof(codeElement));

        private ICodeElement CodeElement { get; }

        public override bool Equals(object obj) => obj is ImplicitTypeReferance && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public ITypeDefinition GetTypeDefinition(ScopeStack scope)
        {
            return CodeElement.ReturnType(scope);
        }

        public ITypeDefinition ReturnType(ScopeStack scope) => RootScope.TypeType.GetTypeDefinition(scope);
    }

    // TODO we also have types that are defined inline "annonymous types"
    // and types that are the result of operations &|! "calculated types"
    
    public class ExplicitTypeName : ITypeSource, IKeyd
    {
        public ExplicitTypeName(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        public string Name { get; }

        public override bool Equals(object obj)
        {
            return obj is ExplicitMemberName name &&
                   Name == name.Name;
        }

        public override int GetHashCode() => 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);

        public ITypeDefinition GetTypeDefinition(ScopeStack scope)
        {
            return scope.GetType(this);
        }

        public IKey Key => new NameKey(Name);

        public ITypeDefinition ReturnType(ScopeStack scope) => RootScope.TypeType.GetTypeDefinition(scope);
    }
    
    public class GenericExplicitTypeName : ExplicitTypeName, IKeyd
    {
        public GenericExplicitTypeName(string name, params ITypeSource[] types) : base(name)
        {
            Types = types ?? throw new System.ArgumentNullException(nameof(types));
        }

        public ITypeSource[] Types { get; }

        public override bool Equals(object obj)
        {
            return obj is GenericExplicitTypeName name &&
                   base.Equals(obj) &&
                   Types.SequenceEqual(name.Types);
        }

        public override int GetHashCode()
        {
            var hashCode = -850890288;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Types.Sum(x=>x.GetHashCode());
            return hashCode;
        }

        public virtual bool TryGetTypeDefinition(ScopeStack scope, out ITypeDefinition typeDefinition)
        {
            typeDefinition = scope.GetGenericType(this);
            return typeDefinition == default;
        }
    }

    public interface IMemberSource : ICodeElement
    {
        MemberDefinition GetMemberDefinition(ScopeStack scopeStack);
    }
    
    public class ExplicitMemberName : IMemberSource, IKeyd
    {
        public ExplicitMemberName(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        public string Name { get; }

        public override bool Equals(object obj)
        {
            return obj is ExplicitMemberName name &&
                   Name == name.Name;
        }

        public override int GetHashCode() => 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        public IKey Key => new NameKey(Name);
        public MemberDefinition GetMemberDefinition(ScopeStack scope) => GetMemberDefinition(scope);
        public ITypeDefinition ReturnType(ScopeStack scope) => GetMemberDefinition(scope).Type.GetTypeDefinition(scope);
    }

}
