using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IScoped<out TScope> where TScope : IScope
    {
        TScope Scope { get; }
    }

    public static class ScopedExtensions
    {

        public static ScopeStack GrowScopeStack(this IScoped<IScope> scope, ScopeStack stack)
        {
            return new ScopeStack(stack, scope.Scope);
        }
    }
    
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

        public ITypeDefinition<IScope> GetType(IEnumerable<AbstractName> names)
        {

            if (names.Count() > 1)
            {

                var (staticOnly, at) = GetStart();


                (bool, IScope) GetStart()
                {
                    var name = names.First();
                    foreach (var scope in Scopes)
                    {
                        if (scope.TryGetType(name, out var typeDefinition))
                        {
                            return (true, typeDefinition.Scope);
                        }
                        if (scope.TryGetMember(name, false, out var memberDefinition))
                        {
                            if (memberDefinition.Type.TryGetTypeDefinition(this, out var memberType))
                            {
                                return (false, memberType.Scope);
                            }
                            else
                            {
                                throw new Exception("");
                            }
                        }
                    }
                    throw new Exception("");
                }

                foreach (var name in names.Skip(1).Take(names.Count() - 2))
                {

                    (staticOnly, at) = Continue();

                    (bool, IScope) Continue()
                    {
                        if (at.TryGetType(name, out var typeDefinition))
                        {
                            return (true, typeDefinition.Scope);
                        }
                        if (at.TryGetMember(name, staticOnly, out var memberDefinition))
                        {
                            if (memberDefinition.Type.TryGetTypeDefinition(this, out var memberType))
                            {
                                return (false, memberType.Scope);
                            }
                            else
                            {
                                throw new Exception("");
                            }
                        }
                        throw new Exception("");
                    }
                }

                return Finsh();

                // finally we make sure we get a type
                ITypeDefinition<IScope> Finsh()
                {
                    if (at.TryGetType(names.Last(), out var typeDefinition))
                    {
                        return typeDefinition;
                    }
                    throw new Exception("");
                }

            }
            else
            {
                return SimpleLookUp();

                // in the simple case we just search for up the stack for types
                ITypeDefinition<IScope> SimpleLookUp()
                {
                    foreach (var scope in Scopes)
                    {
                        if (scope.TryGetType(names.Last(), out var typeDefinition))
                        {
                            return typeDefinition;
                        }
                    }
                    throw new Exception("");
                }
            }
        }

        internal ITypeDefinition<IScope> GetType(object names) => throw new NotImplementedException();

        public MemberDefinition GetMember(AbstractName name)
        {

            //if (names.Count() > 1)
            //{
            //    var (staticOnly, at) = GetStart();

            //    // the first is specail because it searches the stack
            //    (bool, ITypeDefinition<IScope>) GetStart()
            //    {
            //        var name = names.First();
            //        foreach (var scope in Scopes)
            //        {
            //            if (scope.TryGetType(name, out var typeDefinition))
            //            {
            //                return (true, typeDefinition);
            //            }
            //            if (scope.TryGetMember(name, false, out var memberDefinition))
            //            {
            //                if (memberDefinition.Type.TryGetTypeDefinition(this, out var memberType))
            //                {
            //                    return (false, memberType);
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

            //        // when we contune we only search the scope we are at
            //        (bool, ITypeDefinition<IScope>) Continue()
            //        {
            //            if (at.Scope.TryGetType(name, out var typeDefinition))
            //            {
            //                return (true, typeDefinition);
            //            }
            //            if (at.Scope.TryGetMember(name, staticOnly, out var memberDefinition))
            //            {
            //                if (memberDefinition.Type.TryGetTypeDefinition(this, out var memberType))
            //                {
            //                    return (false, memberType);
            //                }
            //                else
            //                {
            //                    throw new Exception("");
            //                }
            //            }
            //            throw new Exception("");
            //        }
            //    }

            //    return GetFinal();

            //    // in the final entry we only look for members
            //    MemberDefinition GetFinal()
            //    {
            //        var name = names.Last();

            //        if (at.Scope.TryGetMember(name, false, out var memberDefinition))
            //        {
            //            return memberDefinition;
            //        }

            //        throw new Exception("");
            //    }

            //}
            //else
            //{
                return SimpleLookUp();

                // if ther is only one we search the whole stack
                MemberDefinition SimpleLookUp()
                {
                    foreach (var scope in Scopes)
                    {
                        if (scope.TryGetMember(name, false, out var memberDefinition))
                        {
                            return memberDefinition;
                        }
                    }
                    throw new Exception("");
                }

            //}
        }
    }
}

