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
        private ICodeElement CodeElement { get; }

        public override bool Equals(object obj) => obj is ImplicitTypeReferance && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public ITypeDefinition GetTypeDefinition(ScopeStack scope)
        {
            return CodeElement.ReturnType(scope);
        }

        public ITypeDefinition ReturnType(ScopeStack scope) => RootScope.TypeType;
    }

    // TODO we also have types that are defined inline "annonymous types"
    // and types that are the result of operations &|! "calculated types"
    
    public class ExplicitTypeName : ITypeSource {
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
        }

        public ITypeDefinition ReturnType(ScopeStack scope) => RootScope.TypeType;
    }
    
    public class GenericExplicitTypeName : ExplicitTypeName
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
    
    public class ExplicitMemberName : IMemberSource
    {
        public ExplicitMemberName(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        public string Name { get; }

        public override bool Equals(object obj)
        {
            return obj is ExplicitMemberName name &&
                   Name == name.Name;
        }

        public override int GetHashCode() => 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        public MemberDefinition GetMemberDefinition(ScopeStack scopeStack)
        {
        }
        public ITypeDefinition ReturnType(ScopeStack scope)
        {
        }
    }

}
