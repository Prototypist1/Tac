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

        public static ScopeScope GrowScopeStack(this IScoped<IScope> scope, ScopeScope stack)
        {
            return new ScopeScope(stack, scope.Scope);
        }
    }


    public class ScopeScope
    {
        public ScopeScope(ScopeScope scopes, IScope newScope)
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
        public ScopeScope(IEnumerable<IScope> scopes) => Scopes = scopes?.ToArray() ?? throw new ArgumentNullException(nameof(scopes));

        public IScope[] Scopes { get; }

        public ITypeDefinition<IScope> GetType(IEnumerable<AbstractName> names)
        {

            var (staticOnly, at) = GetStart();

            (bool, ITypeDefinition<IScope>) GetStart()
            {
                var name = names.First();
                foreach (var scope in Scopes)
                {
                    if (scope.TryGetType(name, out var typeDefinition))
                    {
                        return (true, typeDefinition);
                    }
                    if (scope.TryGetMember(name, false, out var memberDefinition))
                    {
                        if (memberDefinition.Type.TryGetTypeDefinition(this, out var memberType))
                        {
                            return (false, memberType);
                        }
                        else
                        {
                            throw new Exception("");
                        }
                    }
                }
                throw new Exception("");
            }

            foreach (var name in names.Skip(1))
            {

                (staticOnly, at) = Continue();

                (bool, ITypeDefinition<IScope>) Continue()
                {
                    if (at.Scope.TryGetType(name, out var typeDefinition))
                    {
                        return (true, typeDefinition);
                    }
                    if (at.Scope.TryGetMember(name, staticOnly, out var memberDefinition))
                    {
                        if (memberDefinition.Type.TryGetTypeDefinition(this, out var memberType))
                        {
                            return (false, memberType);
                        }
                        else
                        {
                            throw new Exception("");
                        }
                    }
                    throw new Exception("");
                }
            }

            return at;
        }


        public MemberDefinition GetMember(IEnumerable<AbstractName> names)
        {

            if (names.Count() > 1)
            {
                var (staticOnly, at) = GetStart();

                (bool, ITypeDefinition<IScope>) GetStart()
                {
                    var name = names.First();
                    foreach (var scope in Scopes)
                    {
                        if (scope.TryGetType(name, out var typeDefinition))
                        {
                            return (true, typeDefinition);
                        }
                        if (scope.TryGetMember(name, false, out var memberDefinition))
                        {
                            if (memberDefinition.Type.TryGetTypeDefinition(this, out var memberType))
                            {
                                return (false, memberType);
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

                    (bool, ITypeDefinition<IScope>) Continue()
                    {
                        if (at.Scope.TryGetType(name, out var typeDefinition))
                        {
                            return (true, typeDefinition);
                        }
                        if (at.Scope.TryGetMember(name, staticOnly, out var memberDefinition))
                        {
                            if (memberDefinition.Type.TryGetTypeDefinition(this, out var memberType))
                            {
                                return (false, memberType);
                            }
                            else
                            {
                                throw new Exception("");
                            }
                        }
                        throw new Exception("");
                    }
                }

                return GetFinal();

                MemberDefinition GetFinal()
                {
                    var name = names.Last();

                    if (at.Scope.TryGetMember(name, false, out var memberDefinition))
                    {
                        if (memberDefinition.Type.TryGetTypeDefinition(this, out var memberType))
                        {
                            return memberType;
                        }
                        else
                        {
                            throw new Exception("");
                        }
                    }

                    throw new Exception("");
                }

            }
            else
            {
                return SimpleLookUp();

                MemberDefinition SimpleLookUp()
                {
                    var name = names.First();
                    foreach (var scope in Scopes)
                    {
                        if (scope.TryGetMember(name, false, out var memberDefinition))
                        {
                            if (memberDefinition.Type.TryGetTypeDefinition(this, out var memberType))
                            {
                                return memberType;
                            }
                            else
                            {
                                throw new Exception("");
                            }
                        }
                    }
                    throw new Exception("");
                }

            }
        }
    }
}

