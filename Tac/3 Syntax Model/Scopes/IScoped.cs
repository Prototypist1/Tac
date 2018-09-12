using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IScoped
    {
        IScope Scope { get; }
    }

    public static class ScopedExtensions
    {

        public static ScopeStack GrowScopeStack(this IScoped scope, ScopeStack stack)
        {
            return new ScopeStack(stack, scope.Scope);
        }
    }

    // I am really not sure about this,
    // I think it should be a scope tree
    // I mean if you call a method
    // we need to loop up in the scope of that method
    // I am really getting ahead of myself

    public class ScopeStack
    {
        public ScopeStack(ScopeStack scopes, IScope newScope)
        {
            if (scopes == null)
            {
                throw new ArgumentNullException(nameof(scopes));
            }

            if (newScope == null)
            {
                throw new ArgumentNullException(nameof(newScope));
            }

            var stack = scopes.Scopes.ToList();
            stack.Insert(0, newScope);
            Scopes = stack.ToArray();
        }

        public ScopeStack(IEnumerable<IScope> scopes) => Scopes = scopes?.ToArray() ?? throw new ArgumentNullException(nameof(scopes));

        public IScope[] Scopes { get; }

        public ITypeDefinition GetGenericType(GenericExplicitTypeName type) {
            return SimpleLookUp();

            // in the simple case we just search for up the stack for types
            ITypeDefinition SimpleLookUp()
            {
                foreach (var scope in Scopes)
                {
                    if (scope.TryGetGenericType(type, type.Types.Select(x=>x.GetTypeDefinition(this)).ToArray(), out var typeDefinition))
                    {
                        return typeDefinition;
                    }
                }
                throw new Exception("");
            }
        }

        public ITypeDefinition GetType(ExplicitTypeName name)
        {

            //if (names.Count() > 1)
            //{

            //    var (staticOnly, at) = GetStart();


            //    (bool, IScope) GetStart()
            //    {
            //        var name = names.First();
            //        foreach (var scope in Scopes)
            //        {
            //            if (scope.TryGetType(name, out var typeDefinition))
            //            {
            //                return (true, typeDefinition.Scope);
            //            }
            //            if (scope.TryGetMember(name, false, out var memberDefinition))
            //            {
            //                if (memberDefinition.Type.TryGetTypeDefinition(this, out var memberType))
            //                {
            //                    return (false, memberType.Scope);
            //                }
            //                else
            //                {
            //                    throw new Exception("");
            //                }
            //            }
            //        }
            //        throw new Exception("");
            //    }

            //    foreach (var name in names.Skip(1).Take(names.Count() - 2))
            //    {

            //        (staticOnly, at) = Continue();

            //        (bool, IScope) Continue()
            //        {
            //            if (at.TryGetType(name, out var typeDefinition))
            //            {
            //                return (true, typeDefinition.Scope);
            //            }
            //            if (at.TryGetMember(name, staticOnly, out var memberDefinition))
            //            {
            //                if (memberDefinition.Type.TryGetTypeDefinition(this, out var memberType))
            //                {
            //                    return (false, memberType.Scope);
            //                }
            //                else
            //                {
            //                    throw new Exception("");
            //                }
            //            }
            //            throw new Exception("");
            //        }
            //    }

            //    return Finsh();

            //    // finally we make sure we get a type
            //    ITypeDefinition<IScope> Finsh()
            //    {
            //        if (at.TryGetType(names.Last(), out var typeDefinition))
            //        {
            //            return typeDefinition;
            //        }
            //        throw new Exception("");
            //    }

            //}
            //else
            //{
                return SimpleLookUp();

                // in the simple case we just search for up the stack for types
                ITypeDefinition SimpleLookUp()
                {
                    foreach (var scope in Scopes)
                    {
                        if (scope.TryGetType(name, out var typeDefinition))
                        {
                            return typeDefinition;
                        }
                    }
                    throw new Exception("");
                }
            //}
        }
        
        public MemberDefinition GetMemberOrDefault(ExplicitMemberName name)
        {
            return SimpleLookUp();
            
            MemberDefinition SimpleLookUp()
            {
                foreach (var scope in Scopes)
                {
                    if (scope.TryGetMember(name, false, out var memberDefinition))
                    {
                        return memberDefinition;
                    }
                }
                return new MemberDefinition(false, name, new NameTypeSource(RootScope.AnyType));
            }
        }
    }
}

