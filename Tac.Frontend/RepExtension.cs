using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tac.Model;

namespace Tac.Frontend.New.CrzayNamespace
{
    // extension because that could make these context dependent
    // you shoul only use this after the problem is solved
    static class RepExtension
    {

        // this is basically
        // A and B and (C or D) to (A and B and C) or (A and B and D)
        // returns OR AND
        public static EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>> Flatten(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>> self)
        {
            // AND OR AND 
            var andOrAnd = self
                .Where(x => x.Is4(out var _))
                .Select(x => x.Is4OrThrow())
                .Select(x => x.source.or
                    // OR AND
                    .SelectMany(y => y.GetValueAs(out IConstraintSoruce _).GetExtendedConstraints().Flatten())
                    .ToArray())
                .ToHashSet();

            var orAndRes = new List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>>() {
                new List<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>()
            };


            foreach (var orAnd in andOrAnd)
            {

                var nextOrAndRes = new List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>>() { };

                foreach (var andRes in orAndRes)
                {
                    foreach (var and in orAnd)
                    {
                        if (andRes.All(andResItem => and.All(andItem => andResItem.GetValueAs(out IConstraint _).ExtendedIsCompatible(andItem))))
                        {
                            var list = andRes.ToList();
                            list.AddRange(and);
                            nextOrAndRes.Add(list);
                        }
                    }
                    orAndRes = nextOrAndRes;
                }
            }

            var shared = self.Where(x => !x.Is4(out var _))
                .Select(x => x.SwitchReturns(
                    y => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>(y),
                    y => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>(y),
                    y => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>(y),
                    _ => throw new Exception("I just said not that!"),
                    y => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>(y),
                    y => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>(y),
                    y => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>(y)))
                .ToList();
            {
                var nextOrAndRes = new List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>>() { };

                foreach (var andRes in orAndRes)
                {
                    if (andRes.All(andResItem => shared.All(sharedItem => andResItem.GetValueAs(out IConstraint _).ExtendedIsCompatible(sharedItem))))
                    {
                        andRes.AddRange(shared);
                        nextOrAndRes.Add(andRes);
                    }
                }
                orAndRes = nextOrAndRes;
            }

            return new EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>>(
                orAndRes.Select(x => new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>(x.ToHashSet())).ToHashSet());
        }

        public static bool ExtendedIsCompatible(this IConstraint constraint, IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal> item)
        {
            return item.SwitchReturns(
                x => constraint.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x), new List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>>>()),
                x => constraint.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x), new List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>>>()),
                x => constraint.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x), new List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>>>()),
                x => constraint.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x), new List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>>>()),
                x => true,
                x => true);
        }

        //public static IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric> Broaden(this IOrType<MustHave, MustBePrimitive, GivenPathThen,  HasMembers, IsGenericRestraintFor, IsExternal> self) {
        //    return self.SwitchReturns(
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x),
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x),
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x),
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x),
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x),
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x));
        //}

        public static IOrType<IIsPossibly<Guid>, IError> Primitive(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self)
        {
            var primitives = self.SelectMany(x =>
            {
                if (x.Is2(out var v2))
                {
                    return new[] { v2 };
                }
                return Array.Empty<MustBePrimitive>();
            }).ToArray();

            if (primitives.Length == self.Count())
            {
                var groupedPrimitives = primitives.GroupBy(x => x.primitive).ToArray();
                if (groupedPrimitives.Length == 1)
                {
                    return OrType.Make<IIsPossibly<Guid>, IError>(Possibly.Is(groupedPrimitives.First().Key));
                }
                return OrType.Make<IIsPossibly<Guid>, IError>(Error.Other("multiple primitives types..."));
            }

            if (primitives.Any())
            {
                return OrType.Make<IIsPossibly<Guid>, IError>(Error.Other("primitives and non primitives"));
            }
            return OrType.Make<IIsPossibly<Guid>, IError>(Possibly.IsNot<Guid>());
        }

        public static IOrType<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>, IError> Members(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self)
        {
            if (self.ErrorCheck(out var error))
            {
                return OrType.Make<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>, IError>(error);
            }

            var givenPathDictionary = self
                .SelectMany(x =>
                {
                    if (x.Is3(out var v3))
                    {
                        return new[] { v3 };
                    }
                    return Array.Empty<GivenPathThen>();
                })
                .Where(x => x.path.Is1(out var _))
                .GroupBy(x => x.path.Is1OrThrow()).ToDictionary(x => x.Key, x => x);

            var mustHaveGroup = self
                .SelectMany(x =>
                {
                    if (x.Is1(out var v1))
                    {
                        return new[] { v1 };
                    }
                    return Array.Empty<MustHave>();
                })
                .Where(x => x.path.Is1(out var _))
                .GroupBy(x => x.path.Is1OrThrow());

            var list = new List<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>();
            foreach (var mustHaves in mustHaveGroup)
            {
                var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>();

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in  mustHave.dependent.GetExtendedConstraints())
                    {
                        set.Add(constraint);
                    }
                }
                {
                    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                    {
                        foreach (var givenPath in givenPaths)
                        {
                            foreach (var constraint in givenPath.dependent.GetExtendedConstraints())
                            {
                                set.Add(constraint);
                            }
                        }
                    }
                }
                var equalableSet = new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>(set);

                list.Add(new KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>(mustHaves.Key.key, equalableSet));
            }

            return OrType.Make<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>, IError>(list);
        }


        public static IOrType<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>, IError> PrivateMembers(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self)
        {
            if (self.ErrorCheck(out var error))
            {
                return OrType.Make<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>, IError>(error);
            }

            var givenPathDictionary = self
                .SelectMany(x =>
                {
                    if (x.Is3(out var v3))
                    {
                        return new[] { v3 };
                    }
                    return Array.Empty<GivenPathThen>();
                })
                .Where(x => x.path.Is5(out var _))
                .GroupBy(x => x.path.Is5OrThrow()).ToDictionary(x => x.Key, x => x);

            var mustHaveGroup = self
                .SelectMany(x =>
                {
                    if (x.Is1(out var v1))
                    {
                        return new[] { v1 };
                    }
                    return Array.Empty<MustHave>();
                })
                .Where(x => x.path.Is5(out var _))
                .GroupBy(x => x.path.Is5OrThrow());

            var list = new List<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>();
            foreach (var mustHaves in mustHaveGroup)
            {
                var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>();

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in mustHave.dependent.GetExtendedConstraints())
                    {
                        set.Add(constraint);
                    }
                }
                {
                    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                    {
                        foreach (var givenPath in givenPaths)
                        {
                            foreach (var constraint in givenPath.dependent.GetExtendedConstraints())
                            {
                                set.Add(constraint);
                            }
                        }
                    }
                }
                var equalableSet = new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>(set);

                list.Add(new KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>(mustHaves.Key.key, equalableSet));
            }

            return OrType.Make<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>, IError>(list);
        }

        public static IIsPossibly<IOrType<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>> Input(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self)
        {
            if (self.ErrorCheck(out var error))
            {
                return Possibly.Is(OrType.Make<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>(error));
            }

            var mustHaves = self
                .SelectMany(x =>
                {
                    if (x.Is1(out var v1))
                    {
                        return new[] { v1 };
                    }
                    return Array.Empty<MustHave>();
                })
                .Where(x => x.path.Is2(out var _))
                .ToArray();

            if (mustHaves.Any())
            {
                var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>();

                var givenPaths = self
                    .SelectMany(x =>
                    {
                        if (x.Is3(out var v3))
                        {
                            return new[] { v3 };
                        }
                        return Array.Empty<GivenPathThen>();
                    })
                    .Where(x => x.path.Is2(out var _));

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in  mustHave.dependent.GetExtendedConstraints())
                    {
                        set.Add(constraint);
                    }
                }


                foreach (var givenPath in givenPaths)
                {
                    foreach (var constraint in givenPath.dependent.GetExtendedConstraints())
                    {
                        set.Add(constraint);
                    }
                }
                return Possibly.Is(OrType.Make<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>(new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>(set)));

            }

            return Possibly.IsNot<IOrType<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>>();
        }


        public static IIsPossibly<IOrType<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>> Output(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self)
        {
            if (self.ErrorCheck(out var error))
            {
                return Possibly.Is(OrType.Make<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>(error));
            }

            var mustHaves = self
                .SelectMany(x =>
                {
                    if (x.Is1(out var v1))
                    {
                        return new[] { v1 };
                    }
                    return Array.Empty<MustHave>();
                })
                .Where(x => x.path.Is3(out var _))
                .ToArray();

            if (mustHaves.Any())
            {
                var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>();


                var givenPaths = self
                    .SelectMany(x =>
                    {
                        if (x.Is3(out var v3))
                        {
                            return new[] { v3 };
                        }
                        return Array.Empty<GivenPathThen>();
                    })
                    .Where(x => x.path.Is3(out var _));

                //var extened = false;
                //{
                //    var sources = mustHaves.Select(x => x.dependent).ToHashSet();

                //    foreach (var givenPath in givenPaths)
                //    {
                //        sources.Add(givenPath.dependent);
                //    }

                //    if (sources.Count() == 1)
                //    {
                //        extened = true;
                //    }
                //}

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in mustHave.dependent.GetExtendedConstraints())
                    {
                        set.Add(constraint);
                    }
                }


                foreach (var givenPath in givenPaths)
                {
                    foreach (var constraint in  givenPath.dependent.GetExtendedConstraints())
                    {
                        set.Add(constraint);
                    }
                }
                return Possibly.Is(OrType.Make<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>(new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>(set)));

            }
            return Possibly.IsNot<IOrType<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>>();
        }

        public static IOrType<IReadOnlyList<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>, IError> Generics(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self)
        {

            if (self.ErrorCheck(out var error))
            {
                return OrType.Make<IReadOnlyList<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>, IError>(error);
            }

            var givenPathDictionary = self
                .SelectMany(x =>
                {
                    if (x.Is3(out var v3))
                    {
                        return new[] { v3 };
                    }
                    return Array.Empty<GivenPathThen>();
                })
                .Where(x => x.path.Is4(out var _))
                .GroupBy(x => x.path.Is4OrThrow()).ToDictionary(x => x.Key, x => x);

            var mustHaveGroup = self
                .SelectMany(x =>
                {
                    if (x.Is1(out var v1))
                    {
                        return new[] { v1 };
                    }
                    return Array.Empty<MustHave>();
                })
                .Where(x => x.path.Is4(out var _))
                .GroupBy(x => x.path.Is4OrThrow());

            var pairs = new List<(int, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>)>();
            foreach (var mustHaves in mustHaveGroup)
            {
                var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>();

                //var extened = false;
                //{
                //    var sources = mustHaves.Select(x => x.dependent).ToHashSet();
                //    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                //    {
                //        foreach (var givenPath in givenPaths)
                //        {
                //            sources.Add(givenPath.dependent);
                //        }
                //    }

                //    if (sources.Count() == 1)
                //    {
                //        extened = true;
                //    }
                //}

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in mustHave.dependent.GetExtendedConstraints())
                    {
                        set.Add(constraint);
                    }
                }
                {
                    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                    {
                        foreach (var givenPath in givenPaths)
                        {
                            foreach (var constraint in givenPath.dependent.GetExtendedConstraints())
                            {
                                set.Add(constraint);
                            }
                        }
                    }
                }
                var equalableSet = new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>(set);

                pairs.Add((mustHaves.Key.index, equalableSet));
            }

            if (!pairs.Any())
            {
                return OrType.Make<IReadOnlyList<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>, IError>(Array.Empty<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>());
            }

            if (pairs.Select(x => x.Item1).Max() != pairs.Count() - 1)
            {
                // I think this is an exception and not an IError
                // you really shouldn't be able to have disconunious generic constraints
                throw new Exception("the generic constriants are discontinious...");
            }

            return OrType.Make<IReadOnlyList<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>, IError>(pairs.OrderBy(x => x.Item1).Select(x => x.Item2).ToArray());
        }

        private static bool ErrorCheck(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self, [NotNullWhen(true)] out IError? error)
        {
            if (self.Any(x => x.Is2(out var _)) && !self.All(x => x.Is2(out var _)))
            {
                error = Error.Other("primitives and non primitives");
                return true;
            }

            if (self.Any(x => x.Is1(out var mustHave) && mustHave.path.Is1(out var _)) &&
                (
                    self.Any(x => x.Is1(out var mustHave) && mustHave.path.Is2(out var _) ||
                    self.Any(x => x.Is1(out var mustHave) && mustHave.path.Is3(out var _)))
                ))
            {
                error = Error.Other("is it a method or is it an object");
                return true;
            }
            error = default;
            return false;
        }
    }
}


// this generic thing is pretty complex
// I think they do flow

// there is sort of two contexts
// 1 - 
//
// method [T] [T,T] a;
// c =: a;
//
// c is method [T] [T,T] but it has it's own T
// 
// 2 - 
// 
// method [T] [T,T] input {
//  a =: input
//  return input;
// }
//
// a is T
//
// in both cases the genericness flows

// are these two case different constraint?
//
// I need to think through a few more cases...
//
// method [T1] [T1, method [T] [T, T1]] input {
//      method[T][T, T1] a;
//      c =: a;
//      a return;
// }
//
// c is method [T] [T,T1] it has it's own T but T1 is shared
// so..
// if I am generic, I make other things the same generic
// if I have a generic I make other things have their own gernic
// but those are just different ways of saying the same thing...
// from the prospective of a's input, I'm generic
// from the prospective of a, I have a generic
//
// I need to name my cases "has" vs "is"
// 
// I think the key this is how they look up where they come from
// it is sort of a inverse path
// a's input looks up to [input] and then takes the first generic
// a's output looks up to [output, a] and then takes the first generic ... except it is actually just a different kind of constraint
//
// so really we have relationships
// like a's input is it's first generic
// maybe the constrait defining this relationship live on "a"?
// the thing about how it works now is: it can't flow very far
// a generic method has to be realized for befor you can call it
// so it can only flow by assigning to "a"
// 
// a's output is just T1




// ugh, idk
// method[T][int, method [T1][T,T1]] x
// method[T][string, method [T1][T,T1]] y
// 
// the inner method here combine
// but the outer methods don't 
// and so the inner methods shouldn't 
// 
// if we had
// method[T][int, method [T1][T,T1]] x
// method[T][int, method [T1][T,T1]] y
// then everythig combines but probably nothing would break if it didn't 
// it is going to be possible to assigne them to each other, even if they aren't the same object
// 
// only generics should be context dependent
// they can exist on many paths
// but all paths should look up to the same thing
// 
// so the source is one of the features that defines identity for generic constraints 












// what about external types?
// 









// Yolo's aren't 1-1 with types
// it's Yolo + context -> type
// context is a stack of Yolos
// 
// when you look something up you do it like: what is Yolo-X in Yolo-Y?
// which might turn around and do deeper looks like Yolo-Z in [Yolo-X, Yolo-Y]
// sometimes you can't figure it out maybe Yolo-Z in Yolo-X isn't well defined
// and that's ok
// at somepoint we'll look at Yolo-Y and that will give us the context we need 
//
// but.. that is going to break a lot of these apis... GetMethodType, GetObjectType
// I think I just have to pass it on and hope the call-e knowns the context




// oof generics again
// so two things can look up to the same generic type
// and they can have two different paths to owner
// often you know exactly who generic you are
//
// method [t] [t,t] a
// b =: a
//
// here flowing the way I do makes a lot of sense
//
// ...
// 
// ok so the plan now is paired constrains generic and generic source
// generic and generic source are paired 
// 
// but I really already have what I need 
// 