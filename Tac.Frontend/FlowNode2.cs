using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Frontend
{


    // assumptions:
    //  - if a node flows something, it will always flow that thing. once "A" has a member nothing can take that member away from "A"
    // 

    interface IConstraint {
        bool IsCompatible(IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource> constraint, List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>> assumeTrue);
    }

    interface IConstraintSoruce {
        EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> GetConstraints();
        EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>> GetExtendedConstraints();
    }

    // this originates at a ConcreteFlowNode2 
    // but it's constraints code from the element at the path
    //
    // this could also come from an or node
    class MustHave : IConstraint {
        public readonly IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic, Tpn.PrivateMember> path;
        public readonly IConstraintSoruce dependent;

        public MustHave(IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic, Tpn.PrivateMember> path, IConstraintSoruce dependent)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.dependent = dependent ?? throw new ArgumentNullException(nameof(dependent));
        }

        public override bool Equals(object? obj)
        {
            return obj.SafeIs(out MustHave have) &&
                   path.NullSafeEqual(have.path) &&
                   dependent.NullSafeEqual( have.dependent);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(path, dependent);
        }

        public bool IsCompatible(
            IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource> constraint,
            List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>> assumeTrue)
        {
            var pair = new UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), constraint);
            if (assumeTrue.Contains(pair)) {
                return true;
            }
            var nextAssumeTrue = new Lazy<List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>>>(() => {
                var list = assumeTrue.ToList();
                list.Add(pair);
                return list;
            });

            return constraint.SwitchReturns(
                    mustHave => {
                        if (path.Is1(out var _) && (mustHave.path.Is2(out var _) || mustHave.path.Is3(out var _))) {
                            return false;
                        }

                        if (mustHave.path.Is1(out var _) && (path.Is2(out var _) || path.Is3(out var _)))
                        {
                            return false;
                        }

                        if (path != mustHave.path)
                        {
                            return true;
                        }
                        if (dependent == mustHave.dependent)
                        {
                            return true;
                        }

                        return dependent.GetConstraints().All(myConstraint =>
                            mustHave.dependent.GetConstraints().All(theirConstraint => theirConstraint.GetValueAs(out IConstraint _).IsCompatible(myConstraint, nextAssumeTrue.Value)));
                    },
                    prim => false,
                    givenPathThen => {
                        if (path != givenPathThen.path)
                        {
                            return true;
                        }
                        if (dependent == givenPathThen.dependent)
                        {
                            return true;
                        }
                        return dependent.GetConstraints().All(myConstraint =>
                            givenPathThen.dependent.GetConstraints().All(theirConstraint => theirConstraint.GetValueAs(out IConstraint _).IsCompatible(myConstraint, nextAssumeTrue.Value)));
                    },
                    or => or.source.or.Select(x=> x.GetValueAs(out IConstraintSoruce _).GetConstraints()).Any(oneOf => oneOf.All(item => this.IsCompatible(item, nextAssumeTrue.Value))),
                    hasMembers => {

                        if (path.Is2(out var _) || path.Is3(out var _)) {
                            return false;
                        }

                        return true;

                    },
                    generic => {
                        return true;
                    },
                    isGenericSource => true);
        }
        //public readonly EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor>> constraints;

        public override string ToString()
        {
            return $"{nameof(MustHave)}({path}, {dependent})";
        }

    }

    // this orginates at a primitive flow node
    //
    // but it might also come from a very pointless or node: number | number
    //
    // but it doesn't really matter where it comes from
    class MustBePrimitive : IConstraint
    {
        public readonly Guid primitive;
        public readonly PrimitiveFlowNode2 primitiveFlowNode2;

        public MustBePrimitive(Guid primitive, PrimitiveFlowNode2 primitiveFlowNode2)
        {
            this.primitive = primitive;
            this.primitiveFlowNode2 = primitiveFlowNode2 ?? throw new ArgumentNullException(nameof(primitiveFlowNode2));
        }

        public override bool Equals(object? obj)
        {
            return obj is MustBePrimitive primitive &&
                   this.primitive.Equals(primitive.primitive);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(primitive);
        }
        public bool IsCompatible(
            IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource> constraint,
            List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>> assumeTrue)
        {
            var pair = new UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), constraint);
            if (assumeTrue.Contains(pair))
            {
                return true;
            }
            var nextAssumeTrue = new Lazy<List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>>>(() => {
                var list = assumeTrue.ToList();
                list.Add(pair);
                return list;
            });

            return constraint.SwitchReturns(
                mustHave => mustHave.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue), // reverse it  - use og assumeTrue
                prim => primitive == prim.primitive,
                givenPathThen => false,
                or => or.source.or.Select(x => x.GetValueAs(out IConstraintSoruce _).GetConstraints()).Any(oneOf => oneOf.All(item => this.IsCompatible(item, nextAssumeTrue.Value))),
                HasMembers => false,
                generic => false,
                isGenericSource => false);
        }

        public override string ToString()
        {
            return $"{nameof(MustBePrimitive)}({primitive})";
        }
    }

    // this comes from concrete and flow down and up
    // but it's constraints code from the element at the path
    //
    // I don't think I really need path, just chain them together...
    // if .x.y then why is an int
    // .x.y implies .x
    // so (GivenPathThen .x.y implies int ) is (GivenPathThen .x implies  GivenPathThen .y implies int) 
    class GivenPathThen : IConstraint
    {
        public readonly IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic, Tpn.PrivateMember> path;
        public readonly IConstraintSoruce dependent;

        public GivenPathThen(IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic, Tpn.PrivateMember> path, IConstraintSoruce dependent)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.dependent = dependent ?? throw new ArgumentNullException(nameof(dependent));
        }

        public override bool Equals(object? obj)
        {
            return obj.SafeIs(out MustHave have) &&
                   path.NullSafeEqual(have.path) &&
                   dependent.NullSafeEqual(have.dependent);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(path, dependent);
        }

        public bool IsCompatible(
           IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource> constraint,
           List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>> assumeTrue)
        {
            var pair = new UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), constraint);
            if (assumeTrue.Contains(pair))
            {
                return true;
            }
            var nextAssumeTrue = new Lazy<List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>>>(() => {
                var list = assumeTrue.ToList();
                list.Add(pair);
                return list;
            });

            return constraint.SwitchReturns(
                mustHave => mustHave.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue), // reverse it - use og assumeTrue
                primitive => primitive.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue), // reverse it - use og assumeTrue
                givenPathThen => {
                    if (path != givenPathThen.path)
                    {
                        return true;
                    }
                    if (dependent == givenPathThen.dependent)
                    {
                        return true;
                    }
                    return dependent.GetConstraints().All(myConstraint =>
                        givenPathThen.dependent.GetConstraints().All(theirConstraint => theirConstraint.GetValueAs(out IConstraint _).IsCompatible(myConstraint, nextAssumeTrue.Value)));
                },
                or => or.source.or.Select(x => x.GetValueAs(out IConstraintSoruce _).GetConstraints()).Any(oneOf => oneOf.All(item => this.IsCompatible(item, nextAssumeTrue.Value))),
                HasMembers => {

                    if (path.Is2(out var _) || path.Is3(out var _))
                    {
                        return false;
                    }

                    return true;
                },
                isGeneric => {
                    return true;
                },
                isGenericSource => true);
        }

        public override string ToString()
        {
            return $"{nameof(GivenPathThen)}({path}, {dependent})";
        }
    }

    // this is for {x;} concrete =: int | {{int a; } x;}| {{int a; int b;} x;} or
    // concrete is obviously not int
    // it is concrete so it is not {{ a; } x;}| {{ a; int b;} x;}
    // but concrete.x must have a concrete.x.a
    // we can't say what type it is becuse both side of the or are inferred an don't have much info
    // the constraints end up flowing to concrete.x are:
    // - MustHave with a UnionConstraintSource of the two inferred node from the second and third elements of the or
    // - a OrConstraint { a; } | { a; int b;}
    class IntersectionsConstraintSource : IConstraintSoruce
    {
        public readonly EqualableHashSet<IConstraintSoruce> or;

        public IntersectionsConstraintSource(EqualableHashSet<IConstraintSoruce> or)
        {
            this.or = or ?? throw new ArgumentNullException(nameof(or));
        }

        public EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> GetConstraints()
        {
            // find if there is anything common

            var sets = or
                .Select(x => x
                    .GetConstraints()
                    .SelectMany(z=>z.SwitchReturns(
                        y=> new IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>[] { OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>(y) },
                        y => new IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>[] { OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>(y) },
                        y => new IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>[] { OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>(y) },
                        // we can actually ignore OrConstraints
                        // when ever you get constraints from an "or" it will contain the intersection constraints
                        // so we should have that and the intersection is the only part of the or that is useful to us 
                        y => new IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>[] {  },
                        y => new IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>[] { OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>(y) },
                        y => new IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>[] { OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>(y) },
                        y => new IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>[] { OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>(y) }))
                    .ToArray())
                .ToArray();

            // if any of the sets are empty return nothing
            if (sets.Any(x => !x.Any())) {
                return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>( new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> { });
            }

            // if all the sets are only MustBePrimitive
            // and they all have the same guid
            // then that is easy
            var primitiveGroups = sets.SelectMany(x => x.SelectMany(y =>
            {
                if (y.Is2(out var prim))
                {
                    return new[] { prim };
                }
                return Array.Empty<MustBePrimitive>();
            })).GroupBy(x => x.primitive).ToArray();

            if (primitiveGroups.Count() == 1 && primitiveGroups.First().Count() == sets.Sum(x=>x.Length)) {
                return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>( new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> {
                    OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(
                    primitiveGroups.First().First())
                });
            }

            // if any of the sets are MustBePrimitive
            if (sets.Any(x => x.Any(y=>y.Is2(out var _))))
            {
                return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>( new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> { });
            }

            var mustHaveGroups = sets.SelectMany(x => x.SelectMany(y =>
            {
                if (y.Is1(out var mustHave))
                {
                    return new[] { mustHave };
                }
                return Array.Empty<MustHave>();
            })).GroupBy(x => x.path).ToArray();

            if (mustHaveGroups.Any(x => x.Count() > sets.Count())) {
                // {49950614-10F1-4ABF-851F-E9D1EA0BF24C} relies on this assumption
                throw new Exception("there should only be must have per set");
            }

            var intersectionMustHaves = mustHaveGroups
                // if a member has a must have from every set
                .Where(group => sets.All(set => group.Any(groupMember => set.Contains(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>(groupMember)))))
                // {49950614-10F1-4ABF-851F-E9D1EA0BF24C}
                // we assume there is only 1 must have per group so we can just intersect them all 
                // if two came from the same source it would get more complex
                // (a union b) intersection (c) interesection (d)
                .Select(x => OrType.Make< MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(new MustHave(x.Key,new IntersectionsConstraintSource(new EqualableHashSet<IConstraintSoruce>(x.Select(x=>x.dependent).ToHashSet())))));

            var givePathThenGroups = sets.SelectMany(x => x.SelectMany(y =>
            {
                if (y.Is1(out var mustHave))
                {
                    return new[] { mustHave };
                }
                return Array.Empty<MustHave>();
            })).GroupBy(x => x.path).ToArray();

            if (givePathThenGroups.Any(x => x.Count() > sets.Count()))
            {
                // similar to {49950614-10F1-4ABF-851F-E9D1EA0BF24C} 
                throw new Exception("there should only be must have per set");
            }

            var intersectionGivenPathThens = givePathThenGroups
                .Where(group => sets.All(set => group.Any(groupMember => set.Contains(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsGenericSource>(groupMember)))))
                .Select(x => (IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>)OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(new GivenPathThen(x.Key, new IntersectionsConstraintSource(new EqualableHashSet<IConstraintSoruce>(x.Select(x => x.dependent).ToHashSet())))));

            return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>(intersectionMustHaves.Union(intersectionGivenPathThens).ToHashSet());
        }


        public EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>> GetExtendedConstraints() {
            return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>>(GetConstraints().Select(x => x.Broaden()).ToHashSet());
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(or);
        }

        public override bool Equals(object? obj)
        {
            return obj is IntersectionsConstraintSource source &&
                   EqualityComparer<EqualableHashSet<IConstraintSoruce>>.Default.Equals(or, source.or);
        }

        public override string ToString()
        {
            return $"{nameof(IntersectionsConstraintSource)}({or})";
        }
    }

    // this comes from on or
    // it's set really comes from a set of nodes of various types
    //
    class OrConstraint : IConstraint
    {
        public OrFlowNode2 source;

        public OrConstraint(OrFlowNode2 source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }


        public bool IsCompatible(
            IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource> constraint,
            List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>> assumeTrue)
        {
            var pair = new UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), constraint);
            if (assumeTrue.Contains(pair))
            {
                return true;
            }
            var nextAssumeTrue = new Lazy<List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>>>(() => {
                var list = assumeTrue.ToList();
                list.Add(pair);
                return list;
            });

            return constraint.SwitchReturns(
                mustHave => mustHave.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue), // reverse it  - use og assumeTrue
                primitive => primitive.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue), // reverse it  - use og assumeTrue
                givenPathThen => givenPathThen.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue), // reverse it  - use og assumeTrue
                or =>
                    // if any of our source is compatible with any of their any sources
                    source.or.Select(x=>x.GetValueAs(out IConstraintSoruce _).GetConstraints())
                        .Any(ourSet =>or.source.or.Select(x=>x.GetValueAs(out IConstraintSoruce _).GetConstraints())
                            .Any(thierSet => ourSet
                                .All(ourItem => thierSet
                                    .All(theirItem => ourItem.GetValueAs(out IConstraint _).IsCompatible(theirItem, nextAssumeTrue.Value))))),
                hasMembers => source.or.Select(x => x.GetValueAs(out IConstraintSoruce _).GetConstraints()).Any(x=>x.All(y=>y.GetValueAs(out IConstraint _).IsCompatible(constraint, assumeTrue))),
                generic => true,
                genericSource => true);
        }

        public override bool Equals(object? obj)
        {
            return obj is OrConstraint constraint &&
                   EqualityComparer<OrFlowNode2>.Default.Equals(source, constraint.source);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(source);
        }
        public override string ToString()
        {
            return $"{nameof(OrConstraint)}({source})";
        }
    }

    // x =: type {} y
    // x is {} not any
    class HasMembers : IConstraint
    {
        public override bool Equals(object? obj)
        {
            return obj.SafeIs(out HasMembers _);
        }

        public override int GetHashCode()
        {
            return 982347;
        }

        public bool IsCompatible(IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource> constraint, List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>> assumeTrue) =>
            constraint.SwitchReturns(
                mustHave => mustHave.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue), // reverse it - use og assumeTrue
                primitive => primitive.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue), // reverse it - use og assumeTrue
                givenPathThen => givenPathThen.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue),// reverse it - use og assumeTrue
                or => or.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue),// reverse it - use og assumeTrue
                HasMembers => true,
                generic => true,
                isGenericSource => true);

        public override string ToString()
        {
            return $"{nameof(HasMembers)}()";
        }

    }

    class IsGeneric: IConstraint
    {

        /// <summary>
        /// zero indexed
        /// </summary>
        public readonly IsGenericSource source;

        public IsGeneric(IsGenericSource source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public override bool Equals(object? obj)
        {
            return obj is IsGeneric generic &&
                   EqualityComparer<IsGenericSource>.Default.Equals(source, generic.source);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(source);
        }

        public bool IsCompatible(IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource> constraint, List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>> assumeTrue) =>
            constraint.SwitchReturns(
                mustHave => mustHave.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue), // reverse it - use og assumeTrue
                primitive => primitive.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue), // reverse it - use og assumeTrue
                givenPathThen => givenPathThen.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue),// reverse it - use og assumeTrue
                or => or.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue),// reverse it - use og assumeTrue
                HasMembers => HasMembers.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue),// reverse it - use og assumeTrue
                generic =>
                {
                    //if (!pathFromOwner.SequenceEqual(generic.pathFromOwner))
                    //{
                    //    // I don't think this can happen
                    //    // these only exist on generic methods
                    //    // and those those can only be assigned
                    //    // you can't call them or anything

                    //    // can you have two generics of different paths?
                    //    // I can't come up with an example of this happening
                    //    // throw if is happens so we can think about it 
                    //    throw new Exception("I don't know how this could happen");
                    //}
                    // I know same path different index is ok
                    // and clearly same path same index is ok
                    return true;
                },
                isGenericSource => true);

        public override string ToString()
        {
            return $"{nameof(IsGeneric)}(source)";
        }
    }

    // this is a none flowing constrain
    // it is used to uniquely identify types in the type solution
    // it just pass through here
    class IsExternal: IConstraint
    {
        public readonly Tac.Model.Elements.IInterfaceType interfaceType;

        public IsExternal(IInterfaceType external)
        {
            this.interfaceType = external ?? throw new ArgumentNullException(nameof(external));
        }

        public override bool Equals(object? obj)
        {
            return obj is IsExternal external &&
                   EqualityComparer<IInterfaceType>.Default.Equals(interfaceType, external.interfaceType);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(interfaceType);
        }

        public bool IsCompatible(IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource> constraint, List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>> assumeTrue) => true;


        public override string ToString()
        {
            return $"{nameof(IsExternal)}({interfaceType})";
        }
    }

    class IsGenericSource : IConstraint
    {
        /// <summary>
        /// zero indexed
        /// </summary>
        public readonly int index;

        public IsGenericSource(int index)
        {
            this.index = index;
        }

        public bool IsCompatible(IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource> constraint, List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>> assumeTrue)
        {
            return constraint.SwitchReturns(
                mustHave => mustHave.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue), // reverse it - use og assumeTrue
                primitive => primitive.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue), // reverse it - use og assumeTrue
                givenPathThen => givenPathThen.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue),// reverse it - use og assumeTrue
                or => or.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue),// reverse it - use og assumeTrue
                hasMembers => hasMembers.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue),// reverse it - use og assumeTrue
                isGeneric => isGeneric.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(this), assumeTrue),// reverse it - use og assumeTrue
                isGenericSource => true);
        }
    }

    //class IllegalActionException : Exception
    //{
    //    private object action;

    //    public IllegalActionException(object action)
    //    {
    //        this.action = action;
    //    }
    //}

    class NoChanges { }
    class Changes { }
    class FailedAction { }

    // this is probably a bad idea
    //class X : IOrType<NoChanges, Changes, FailedAction> {
    //    IOrType<NoChanges, Changes, FailedAction> inner;

    //    public override bool Equals(object? obj)
    //    {
    //        return obj is X x &&
    //               EqualityComparer<IOrType<NoChanges, Changes, FailedAction>>.Default.Equals(inner, x.inner);
    //    }

    //    public override int GetHashCode() => inner.GetHashCode();

    //    public bool Is<T>(out T res) => inner.Is(out res);
    //    public NoChanges Is1OrThrow() => inner.Is1OrThrow();
    //    public Changes Is2OrThrow() => inner.Is2OrThrow();
    //    public FailedAction Is3OrThrow() =>  inner.Is3OrThrow();
    //    public IIsPossibly<NoChanges> Possibly1() => inner.Possibly1();
    //    public IIsPossibly<Changes> Possibly2() => inner.Possibly2();
    //    public IIsPossibly<FailedAction> Possibly3() => inner.Possibly3();
    //    public void Switch(Action<NoChanges> a1, Action<Changes> a2, Action<FailedAction> a3) => inner.Switch(a1, a2, a3);
    //    public T SwitchReturns<T>(Func<NoChanges, T> f1, Func<Changes, T> f2, Func<FailedAction, T> f3) => inner.SwitchReturns(f1, f2, f3);

    //}

    static class TriStateExtensions {

        public static IOrType<NoChanges, Changes, FailedAction> CombineBothMustNotFail(this IOrType<NoChanges, Changes, FailedAction> self, IOrType<NoChanges, Changes, FailedAction> that) {
            if (self.Is3(out var _)) {
                return self;
            }
            if (that.Is3(out var _))
            {
                return that;
            }
            if (self.Is2(out var _))
            {
                return self;
            }
            if (that.Is2(out var _))
            {
                return that;
            }
            return self;
        }

        public static IOrType<NoChanges, Changes, FailedAction> CombineOneMustNotFail(this IOrType<NoChanges, Changes, FailedAction> self, IOrType<NoChanges, Changes, FailedAction> that)
        {
            if (self.Is3(out var _) && that.Is3(out var _))
            {
                return self;
            }
            if (self.Is2(out var _))
            {
                return self;
            }
            if (that.Is2(out var _))
            {
                return that;
            }
            return self;
        }
    }


    interface IFlowNode2 : IConstraintSoruce {
        /// <summary>
        /// when flowing down stream (given a =: b, downstream would be from a to b)
        /// only pass GivenPathThen and OrConstraint of GivenPathThen
        /// see {95C8B654-3AF5-42FD-A42B-A94165BEF7A3}
        /// </summary>
        IOrType<NoChanges, Changes, FailedAction> AcceptConstraints(IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> constraints);
        bool CouldApplyToMe(IEnumerable<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> constraint);
        //IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor>[] Retarget(IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic> path);

    }

    class PrimitiveFlowNode2: IFlowNode2
    {
        public readonly Guid guid;
        public readonly Tpn.TypeProblem2.Type type;
        private static int index = 0;
        private int myIndex = Interlocked.Increment(ref  index);

        public PrimitiveFlowNode2(Guid guid, Tpn.TypeProblem2.Type type)
        {
            this.guid = guid;
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> GetConstraints()
        {
            return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>(new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> {
                OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource> (new MustBePrimitive(guid,this))
            });
        }
        public IOrType<NoChanges, Changes, FailedAction> AcceptConstraints(IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> newConstraints) {
            return OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges());
        }

        public bool CouldApplyToMe(IEnumerable<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> constraint) =>
            constraint.All(x=>x.Is2(out var prim) && prim.primitive == guid);

        public EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>> GetExtendedConstraints()
        {
            return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>>(GetConstraints().Select(x => x.Broaden()).ToHashSet());
        }


        public override string ToString()
        {
            return $"{nameof(PrimitiveFlowNode2)}-{myIndex}({guid}, {type})";
        }

    }

    class ConcreteFlowNode2 : IFlowNode2
    { 
        // doesn't have OrConstraints
        private readonly EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers>> constraints = new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers>>(new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers>>());
        private readonly Dictionary<IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic, Tpn.PrivateMember>, IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>> dependents = new ();
        // private members don't flow
        private readonly List<MustHave> privateMembers = new List<MustHave>();

        private readonly HashSet<OrFlowNode2> perviouslyAccepted = new HashSet<OrFlowNode2>();
        private readonly HashSet<OrFlowNode2> perviouslyAcceptedDownstream= new HashSet<OrFlowNode2>();

        public IIsPossibly<IsExternal> isExternal = Possibly.IsNot<IsExternal>();

        private static int index = 0;
        private int myIndex = Interlocked.Increment(ref index);

        public EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> GetConstraints()
        {
            return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>(constraints.Select(x=>x.SwitchReturns(
                x => (IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>)OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(x))).ToHashSet());
        }
        public IOrType<NoChanges, Changes, FailedAction> AcceptConstraints(IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> newConstraints)
        {
            var sum = (IOrType<NoChanges, Changes, FailedAction>)OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges());

            foreach (var constraint in newConstraints)
            {

                sum = TriStateExtensions.CombineBothMustNotFail(
                    sum, 
                    constraint.SwitchReturns(
                        mustHave =>
                        {
                            // what happen here?
                            // type {x;} a =: {y;} | number b
                            // ...they wrote bad code
                            // y could flow if they had left it inferred but they didn't
                            if (dependents.TryGetValue(mustHave.path, out var dependent)) {
                                var constraints = mustHave.dependent.GetConstraints();
                                return dependent.GetValueAs(out IFlowNode2 _).AcceptConstraints(constraints);
                            }
                            return OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges());
                        },
                        mustBePrimitve => OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges()),
                        givenPathThen =>
                        {
                            if (dependents.TryGetValue(givenPathThen.path, out var dependent))
                            {
                                return dependent.GetValueAs(out IFlowNode2 _).AcceptConstraints(givenPathThen.dependent.GetConstraints());
                            }
                            return OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges());
                        },
                        orConstraint => {

                            // this isn't always right
                            // something could be upstream and all GivenPathThen
                            // but in that case it's upstream and downstream are the same 
                            var downstream = newConstraints.All(x => x.Is3(out GivenPathThen _));

                            // we try to find determine if we are definately one or the other of the disjoint options
                            // if we could be either, path the constraint on to our dependents
                            var couldApply = orConstraint.source.or.SelectMany(sourceOr =>
                            {
                                var set = sourceOr.GetValueAs(out IConstraintSoruce _).GetConstraints();
                                if (CouldApplyToMe(set)) {
                                    return new[] { sourceOr };
                                }
                                return Array.Empty<IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>>();
                            }).ToArray();

                            if (couldApply.Length == 0) {
                                // this is an error state
                                // and an upsetting one because we could have flowed earlier versions of the OrConstraint
                                // I think I probably want to restart the solve 
                                // not to flow certain pairs of nodes 
                                // 
                                // there are probably other cases
                                // like being asked to accept inconsistant constraints
                                //
                                // this really doesn't force a re-solve
                                // unless we previously accepted a different subset of it
                                //
                                // I am still not sure if this is right
                                // it is only a failed constraint is I have already flows some stuff 
                                // lot to think about see 
                                //
                                // once we know one of the elements in the or doesn't flow, it is never going to start flowing
                                // {x;} a =: test1 | test2 t
                                // test1 t1 =: {int y;}
                                //
                                // we are only going to accept something once it is on all applicable members of the or 
                                // {x;} a =: int | test1 | test2 t
                                // test1 t1 =: {int x;}
                                // test2 t2 =: {int x;}
                                //
                                // once we have accepted it only becomes a problem if we no longer accept any members of the or
                                // {x;} a =: int | test1 | test2 t
                                // test1 t1 =: {int x;}
                                // test2 t2 =: {int x;}
                                // test1 t1 =: {int y;}
                                // test2 t2 =: {int y;}

                                if (downstream)
                                {
                                    if (perviouslyAcceptedDownstream.Contains(orConstraint.source))
                                    {
                                        return OrType.Make<NoChanges, Changes, FailedAction>(new FailedAction());
                                    }
                                }
                                else {

                                    if (perviouslyAccepted.Contains(orConstraint.source))
                                    {
                                        return OrType.Make<NoChanges, Changes, FailedAction>(new FailedAction());
                                    }
                                }
                                return OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges());
                            }

                            if (couldApply.Length == 1) {
                                if (downstream)
                                {
                                    perviouslyAcceptedDownstream.Add(orConstraint.source);
                                }
                                else
                                {
                                    perviouslyAccepted.Add(orConstraint.source);
                                }
                                return AcceptConstraints(
                                        couldApply.Single().GetValueAs(out IConstraintSoruce _).GetConstraints().ToHashSet());
                            }

                            if (downstream)
                            {
                                perviouslyAcceptedDownstream.Add(orConstraint.source);
                            }
                            else
                            {
                                perviouslyAccepted.Add(orConstraint.source);
                            }

                            var intersectionsConstraintSource = new IntersectionsConstraintSource(new EqualableHashSet<IConstraintSoruce>(couldApply.Select(x => x.GetValueAs(out IConstraintSoruce _)).ToHashSet()));

                            return AcceptConstraints(intersectionsConstraintSource.GetConstraints().ToHashSet());
                            
                            // we need to create to approprate OrConstraint for each element
                            //var res = (IOrType<NoChanges, Changes, FailedAction>)OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges());
                            //foreach (var dependent in dependents)
                            //{
                            //    // this OrConstraint can have empty sets in it
                            //    // {a;b;} y =: {int a;}| {b;} x
                            //    // y's a could be an int or it could be unconstrainted
                            //    var next = new OrConstraint(new EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>>(
                            //        couldApply
                            //            .SelectMany(set=>
                            //                ConstraintUtils.Flatten(set
                            //                    .SelectMany(constraint => Retarget(constraint, dependent))
                            //                    .ToArray())
                            //                .Select(x=> new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>(x.ToHashSet())))
                            //            .ToHashSet()));
                            //    res = TriStateExtensions.CombineBothMustNotFail(
                            //            res,    
                            //            dependent.Value.GetValueAs(out IFlowNode2 _).AcceptConstraints(
                            //                new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor>> {
                            //                    OrType.Make<MustHave, MustBePrimitive, GivenPathThen,OrConstraint>(next)}));
                            //}
                            //return res;
                        },
                        HasMembers => {
                            if (dependents.Any(x => x.Key.Is1(out var _)))
                            {
                                return OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges());
                            }
                            if (dependents.Any(x => x.Key.Is2(out var _)|| x.Key.Is3(out var _)))
                            {
                                return OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges());
                            }
                            if (constraints.Any(x => x.Is4(out var _))) 
                            {
                                return OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges());
                            }

                            constraints.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers>(HasMembers));
                            return OrType.Make<NoChanges, Changes, FailedAction>(new Changes());

                        },
                        generic => OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges()),
                        genericSource => OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges())));
            }
            return sum;
        }

        // but this could contain OrConstraint so we need to split it out

        //private IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor>[] Retarget(IOrType<MustHave, MustBePrimitive, GivenPathThen> constraint, KeyValuePair<IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>, IOrType<PrimitiveFlowNode2, ConcreteFlowNode2, InferredFlowNode2, OrFlowNode2>> dependent)
        //{
        //    return constraint.SwitchReturns(
        //        mustHave =>
        //        {
        //            if (dependent.Key.Equals(mustHave.path))
        //            {
        //                return mustHave.dependent.GetConstraints().ToArray();//??
        //            }
        //            return Array.Empty<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor>>();
        //        },
        //        mustBePrimitive => throw new Exception("a constraint set with a MustBePrimitive shouldn't have applied to a ConcreteFlowNode2"),//Array.Empty<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor>>(),
        //        givenPathThen => {
        //            if (dependent.Key.Equals(givenPathThen.path))
        //            {
        //                return givenPathThen.dependent.GetConstraints().ToArray();
        //            }
        //            return Array.Empty<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor>>();
        //        });
        //}

        public EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>> GetExtendedConstraints()
        {
            var set = GetConstraints().Select(x => x.Broaden()).ToHashSet();
            //if (isGenericRestraintFor.Is(out var genericRestraintFor)) {
            //    set.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor, IsExternal>(genericRestraintFor));
            //}
            if (isExternal.Is(out var external))
            {
                set.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>(external));
            }
            foreach (var privateMember in privateMembers)
            {
                set.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>(privateMember));
            }
            return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>>(set);
        }

        public bool CouldApplyToMe(IEnumerable<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> constraints)
        {
            return constraints.All(constraint => CouldApplyToMe(constraint));
        }

        private bool CouldApplyToMe(IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource> constraint)
        {
            return constraint.SwitchReturns(
                    mustHave =>
                    {
                        // you can't have members and inputs/outputs
                        if (mustHave.path.Is1(out var _) && dependents.Any(x => x.Key.Is2(out var _) || x.Key.Is3(out var _))) {
                            return false;
                        }

                        if ((mustHave.path.Is2(out var _) || mustHave.path.Is3(out var _)) && dependents.Any(x => x.Key.Is1(out var _)))
                        {
                            return false;
                        }

                        if (dependents.TryGetValue(mustHave.path, out var dependent))
                        {
                            return dependent.GetValueAs(out IFlowNode2 _).CouldApplyToMe(mustHave.dependent.GetConstraints());
                        }
                        return false;
                    },
                    mustBePrimitve => false,
                    givenPathThen =>
                    {
                        if (dependents.TryGetValue(givenPathThen.path, out var dependent))
                        {
                            return dependent.GetValueAs(out IFlowNode2 _).CouldApplyToMe(givenPathThen.dependent.GetConstraints());
                        }
                        // does this stop the flow?
                        // no. see:
                        //
                        // {int x;} =: a
                        // {int y;} b =: a
                        // {x; int y;} c =: b
                        //
                        // c.x is an int

                        return true;
                    },
                    orConstraint => orConstraint.source.or.Any(x => CouldApplyToMe(x.GetValueAs(out IConstraintSoruce _).GetConstraints())),
                    HasMembers => {
                        if (dependents.Any(x => x.Key.Is2(out var _) || x.Key.Is3(out var _)))
                        {
                            return false;
                        }
                        return true;
                    },
                    generic => false,
                    genericSource => false);
        }

        internal void AddMember(IKey key, IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2> orType)
        {
            var path = OrType.Make<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic, Tpn.PrivateMember>(new Tpn.Member(key));
            dependents[path] = orType;
            constraints.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers>(new MustHave(path, orType.GetValueAs(out IConstraintSoruce _))));
            constraints.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers>(new GivenPathThen(path, orType.GetValueAs(out IConstraintSoruce _))));

        }
        internal void AddPrivateMember(IKey key, IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2> orType)
        {
            var path = OrType.Make<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic, Tpn.PrivateMember>(new Tpn.PrivateMember(key));
            dependents[path] = orType;
            privateMembers.Add(new MustHave(path, orType.GetValueAs(out IConstraintSoruce _)));
        }

        internal void AddInput(IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2> orType)
        {
            var path = OrType.Make<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic, Tpn.PrivateMember>(new Tpn.Input());
            dependents[path] = orType;
            constraints.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers>(new MustHave(path, orType.GetValueAs(out IConstraintSoruce _))));
            constraints.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers>(new GivenPathThen(path, orType.GetValueAs(out IConstraintSoruce _))));
        }

        internal void AddOutput(IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2> orType)
        {
            var path = OrType.Make<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic, Tpn.PrivateMember>(new Tpn.Output());
            dependents[path] = orType;
            constraints.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers>(new MustHave(path, orType.GetValueAs(out IConstraintSoruce _))));
            constraints.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers>(new GivenPathThen(path, orType.GetValueAs(out IConstraintSoruce _))));
        }

        internal void AddGenerics(IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>[] orTypes)
        {
            for (int i = 0; i < orTypes.Length; i++)
            {
                var orType = orTypes[i];
                var path = OrType.Make<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic, Tpn.PrivateMember>(new Tpn.Generic(i));
                dependents[path] = orType;
                constraints.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers>(new MustHave(path, orType.GetValueAs(out IConstraintSoruce _))));
                constraints.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers>(new GivenPathThen(path, orType.GetValueAs(out IConstraintSoruce _))));
            }
        }

        internal void IsExternal(IInterfaceType external)
        {
            if (isExternal.Is(out var _)) {
                throw new Exception("already set, is that ok?");
            }

            isExternal = Possibly.Is(new IsExternal(external));
        }

        internal void HasMembers()
        {
            constraints.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers>(new HasMembers()));
        }


        public override string ToString()
        {
            return $"{nameof(ConcreteFlowNode2)}-{myIndex}{{{String.Join(", ", GetExtendedConstraints().Select(x=>x.ToString()).ToArray())}}}";
        }
    }

    class InferredFlowNode2 : IFlowNode2
    {
        public readonly EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> constraints = new (new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>());

        public List<IsGeneric> isGenericRestraintFor = new List<IsGeneric>();
        //public IIsPossibly<IsExternal> isExternal = Possibly.IsNot<IsExternal>();

        private static int index = 0;
        private int myIndex = Interlocked.Increment(ref index);

        public IOrType<NoChanges, Changes, FailedAction> AcceptConstraints(IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> newConstraints)
        {
            // it might be ok for inferred nodes to become hot messes
            // for now I am just going to go with NoChanges
            // but it this could be a FailedAction - after all it did fail
            // but we could also just pile all the constraints on
            //
            // we could have accepted it eariler see:
            // x.a =: int z
            // x =: y
            // y =: {int b} _
            // y =: {string a} _        
            //
            // up to line 3 it is consistant, we sould accpet that x.b exists
            // but then in line 4 isn't good, x.a has to be a string and a int
            // so maybe this is really an error
            if (!constraints.All(existingItem => newConstraints
                    .All(newItem => existingItem.GetValueAs(out IConstraint _).IsCompatible(newItem, new List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>>()))))
            {
                return OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges());
            }

            var sum = (IOrType<NoChanges, Changes, FailedAction>)OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges());
            foreach (var newConstraint in newConstraints)
            {
                if (constraints.Add(newConstraint)) {
                    sum = OrType.Make<NoChanges, Changes, FailedAction>(new Changes());
                }
            }
            return sum;
        }
        public EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> GetConstraints()
        {
            var set = constraints.ToHashSet();
            foreach (var item in isGenericRestraintFor)
            {
                set.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(item));
            }
            //if (isExternal.Is(out var external))
            //{
            //    set.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor, IsExternal>(external));
            //}
            return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>(set);
        }

        public bool CouldApplyToMe(IEnumerable<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> constraint)
        {
            return true;
        }
        public EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>> GetExtendedConstraints()
        {
            return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>>(GetConstraints().Select(x => x.Broaden()).ToHashSet());
        }

        internal void IsConstraintFor(IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Left, Tpn.Right, Tpn.PrivateMember>[] pathFromOwner, int index, IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Method, Tpn.TypeProblem2.InferredType> owner)
        {
            //if (isGenericRestraintFor.Is(out var _)) {
            //    throw new Exception("already set? that's suprising and worth thinking agout");
            //}

            isGenericRestraintFor.Add(new IsGeneric(pathFromOwner, index, owner));
        }

        public override string ToString()
        {
            return $"{nameof(InferredFlowNode2)}-{myIndex}{{{String.Join(", ", GetExtendedConstraints().Select(x => x.ToString()).ToArray())}}}";
        }
    }

    class OrFlowNode2 : IFlowNode2
    {
        // we have shared constrains
        // and disjoin constraints
        // do we calculate them from our sources?

        private static int index = 0;
        private int myIndex = Interlocked.Increment(ref index);

        public readonly EqualableHashSet<IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>> or;

        public OrFlowNode2(IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2> left, IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2> right)
        {
            or =  new (new HashSet<IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>> { left,right});
        }

        public IOrType<NoChanges, Changes, FailedAction> AcceptConstraints(IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> newConstraints)
        {

            var sum = (IOrType<NoChanges, Changes, FailedAction>)OrType.Make<NoChanges, Changes, FailedAction>(new NoChanges());

            foreach (var item in or)
            {
                var accepted = item.GetValueAs(out IFlowNode2 _).AcceptConstraints(newConstraints);

                sum = TriStateExtensions.CombineBothMustNotFail(sum, accepted);
            }
            return sum;
        }

        public EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> GetConstraints()
        {
            var res = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>();
            // make our orConstraint
            res.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(new OrConstraint(this)));
            // ... and figure out the intersect
            var intersectSource = new IntersectionsConstraintSource(new EqualableHashSet<IConstraintSoruce>(or.Select(x => x.GetValueAs(out IConstraintSoruce _)).ToHashSet()));
            foreach (var source in intersectSource.GetConstraints()) {
                res.Add(source);
            }
            return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>>(res);

            //// each set is either:
            //// - has members
            //// - primitive
            //// - empty
            //// - inconsistant

            //// an example:
            //// {int x;} | {string x}
            //// {int|string x;}
            //// these are the same...
            //// but it is fair to say that it has an x



            //var unionSet = sets.First().ToArray();

            //// ugh! this union doesn't work
            //// MustHave equality is based on the dependend
            ////
            ////
            ////
            //foreach (var set in sets.Skip(1))
            //{
            //    unionSet = unionSet.Union(set).ToArray();
            //}


            //var res = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor>>();

            //foreach (var item in unionSet)
            //{
            //    res.Add(ConstraintUtils.Broaden(item));
            //}

            //var disjoint = 
            //    new OrConstraint(
            //        new EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>>(
            //            sets.Select(x => new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>(x.Except(unionSet).ToHashSet())).ToHashSet()));
            //res.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor>(disjoint));

            //return res;
        }

        public EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>> GetExtendedConstraints()
        {
            return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>>(GetConstraints().Select(x => x.Broaden()).ToHashSet());
        }

        public bool CouldApplyToMe(IEnumerable<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> constraints)
            => or.Any(x => x.GetValueAs(out IFlowNode2 _).CouldApplyToMe(constraints));


        public override string ToString()
        {
            return $"{nameof(OrFlowNode2)}-{myIndex}({or})";
        }
    }


    static class ConstraintUtils
    {
        /// <summary>
        /// take [A,B,C,(E|F)] to [[A,B,C,E],[A,B,C,F]]
        /// where (E|F) is a DisjointConstraint
        /// the result will contain no DisjointConstraints
        /// </summary>
        //public static List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen>>> Flatten(IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor>[] constraints)
        //{
        //    List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen>>> ress = new List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen>>> {
        //        new List<IOrType<MustHave, MustBePrimitive, GivenPathThen>>()
        //    };

        //    foreach (var constraint in constraints)
        //    {
        //        constraint.Switch(x =>
        //        {
        //            foreach (var res in ress)
        //            {
        //                res.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen>(x));
        //            }
        //        },
        //        x =>
        //        {
        //            foreach (var res in ress)
        //            {
        //                res.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen>(x));
        //            }
        //        },
        //        x =>
        //        {
        //            foreach (var res in ress)
        //            {
        //                res.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen>(x));
        //            }
        //        },
        //        x =>
        //        {
        //            var sourceRes = ress;
        //            ress = new List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen>>>();
        //            foreach (var set in x.source.or.Select(x=>x.GetValueAs(out IConstraintSoruce _).GetConstraints().Select(y
        //                =>
        //                    y.SwitchReturns(z => ))))
        //            {
        //                var newRess = sourceRes.Select(x => x.ToList()).ToList();
        //                foreach (var newRes in newRess)
        //                {
        //                    newRes.AddRange(set);
        //                }
        //                ress.AddRange(newRess);
        //            }
        //        });
        //    }
        //    return ress;
        //}


        //public static IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor> Broaden(IOrType<MustHave, MustBePrimitive, GivenPathThen> orType) =>
        //    orType.SwitchReturns(x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor>(x),
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor>(x),
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSourceRestraintFor>(x));

        public static IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource> Broaden(this IOrType<GivenPathThen, OrConstraint> orType) =>
                orType.SwitchReturns(x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(x),
                    x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>(x));


        public static IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal> Broaden(this IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource> self) =>
           (IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>)self.SwitchReturns(x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource, IsExternal>(x));

        // downstream we only send GivenPathThen and OrConstraint made entirely of GivenPathThen
        // {95C8B654-3AF5-42FD-A42B-A94165BEF7A3}
        public static IReadOnlySet<IOrType<GivenPathThen, OrConstraint>> ToDownStream(this IEnumerable<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsGenericSource>> constraints) {
            return constraints.SelectMany(x =>
            {
                if (x.Is3(out var givenPath))
                {
                    return new[] { OrType.Make < GivenPathThen, OrConstraint > (givenPath) };
                }
                // or's don't flow down stream
                // instead they make intersect that flow downstream

                //if (x.Is4(out var or)) {
                //    var sets = new EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>>(or.source.or.Select(sourceOr =>
                //        new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>(sourceOr.GetValueAs(out IConstraintSoruce _).GetConstraints().SelectMany(y =>
                //       {
                //           if (y.Is3(out var innerGivenPath))
                //           {
                //               return new[] { OrType.Make<MustHave, MustBePrimitive, GivenPathThen>(innerGivenPath) };
                //           }
                //           return Array.Empty<IOrType<MustHave, MustBePrimitive, GivenPathThen>>();
                //       }).ToHashSet())).ToHashSet());

                //    if (sets.Any(x => x.Any())) {
                //        return new[] { OrType.Make<GivenPathThen, OrConstraint>(new OrConstraint(sets)) };
                //    }
                //}
                return Array.Empty<IOrType<GivenPathThen, OrConstraint>>();
            }).ToHashSet();

        }
    }

    class UnorderedPair {
        private readonly object a, b;

        public UnorderedPair(object a, object b)
        {
            this.a = a ?? throw new ArgumentNullException(nameof(a));
            this.b = b ?? throw new ArgumentNullException(nameof(b));
        }

        public override bool Equals(object? obj)
        {
            return obj is UnorderedPair pair &&
                   ((a.NullSafeEqual(pair.a) && b.NullSafeEqual(pair.b)) ||
                   (b.NullSafeEqual(pair.a) && a.NullSafeEqual(pair.b)));
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() + b.GetHashCode();
        }
    }

    class UnorderedPair<T>: UnorderedPair
    {
        public UnorderedPair(T a, T b): base(a,b){}
    }

    // Tests to write
    //
    // something where an or flow a OrConstraint of GivenPathThen down stream
    // is it even possible? I can't come up with an example...
    //
    // {int a;} | {int b} =: y
    // y.a := 5;
    //
    // y has an a
    // but y could still have a b
    // so we don't flow
    // y ends up being {a;}
    // 
    // {string|empty x;} | { int|empty x;} =: y
    // y.x := empty
    // 
    // y is {string|empty x;} | { int|empty x;}
    //
    // ...am I missing a constrint?
    // y.x ... exists, I've got that. but it also has to accept empty
    // that doesn't mean it is empty
    // 
    // here is the test:
    //
    // {int x} | int a =: b
    // {x} c =: b
    //
    // c.x is an int


    // flows from Or nodes can get a bit werid, mostly for broken code
    //
    // I think I need dependent constriants
    //
    // type test inferred // assuming you can do this, which seems like it would be nice...
    // {x;} a =: int | test t1
    // test t1 =: {int x;} b
    // test t2 =: {int y;} c
    //
    // fist this line flows and we conclude a.x is an int
    // test t1 =: {int x;} b
    // 
    // then this line flows and we conclude that {x;} a will accept neither side of (int | test) and there for a.x is not an int
    // test t2 =: {int y;} c
    //
    // but... it is mostly for broken code
    // so maybe I don't have to worry about it

    // maybe I always push or to the members
    // I would need: doesn't-have path | no-constraints | is int

    // can a OrConstraint ever add a member?
    // if it shows up on both sides...
    // {int x; int y;}|{int x; int b;}
    // ok, once it show up on both sides can it ever not show up?
    // 
    // type test inferred
    // {int x;} | test t1
    // test t2 =: {int x;} b
    // test t3 =: int c 
    // I mean, then inferred becomes an error


    // I think in this case we want {int x;} ?
    // {x;} a =: int | test t1
    // test t1 =: {int x;} b
    // test t2 =: {int y;} c
    //
    // I mean it comes down to:
    // {x;} a =: {int x; int y;} test
    // in this case we would flow the int....
    // 
    // so... extra members don't stop the flow
    // but they do...
    // {x;} a =: {int x; int y;} | {string x;}
    // ... in this case a.x is a string
    // 
    // so.. flow as far as you can and enter an error state <<< TODO

    // type test infer
    // test | int x;
    // if 
    //      {int a; int b} =: x;
    // else
    //      {b;} y =: x
    // 
    // if (x is test t)
    //      t.b := "yolo" // is this an error?... yes


    // I am thinking nodes enter an error state and they are done
    // that works for infered
    //
    // but this is still a problem:
    // type test infer
    // {x;} a =: int | test t0
    // test t1 =: {int x;} b
    // test t2 =: {int y;} c
    //
    // the final solution is wrong: {int x;} a
    // I do really need to real solution of throw and reflow with a black list....

    // or maybe I am looking for the best solution with no illegal moves
    // for:
    // type test infer
    // {x;} a =: int | test t0
    // test t1 =: {int x;} b
    // test t2 =: {int y;} c
    // maybe we don't flow c into t2 after we have flowed t0 into a 
    // ... 
    // yeah, reflow and black list is a way to get a solution with no illegal moves
    // and it is simpler I think
    // ...
    // but it is hard to know you have taken an illegal move
    // I just pass in an action with the inital flow 
    // and if anything goes wrong a blame that flow and don't do it next time
    // ...
    // only going wrong shouldn't be an exceptoin
    // I should return a tri-state


    // {x;} a =: test1 | test2 t
    // test1 t1 =: {int x;}
    // test2 t2 =: { y;}
    //
    // we can't really say infered doesn't apply 
    // {x;} a =: test1 | test2 t
    // test1 t1 =: {int x;}
    // test2 could get "x" at somepoint
    // we just don't know 
    //
    // however once we know one of the elements in the or doesn't flow, it is never going to start flowing
    // {x;} a =: test1 | test2 t
    // test1 t1 =: {int y;}
    //
    // we are only going to accept something once it is on all applicable members of the or 
    // {x;} a =: int | test1 | test2 t
    // test1 t1 =: {int x;}
    // test2 t2 =: {int x;}
    //
    // once we have accepted it only becomes a problem if we no longer accept any members of the or
    // {x;} a =: int | test1 | test2 t
    // test1 t1 =: {int x;}
    // test2 t2 =: {int x;}
    // test1 t1 =: {int y;}
    // test2 t2 =: {int y;}
    //
    // the problem it is hard to if "it is on all applicable members"
    // sure x is on all available members
    // but MustHave as written is drive by a concrete node
    // and these are driven by two different con
    // {x;} a =: int | test1 | test2 t
    // test1 t1 =: {infered1 x;}
    // test2 t2 =: {infered2 x;}
    // 
    // so... it has x and it has the intersection of the constraints infered1 and infered2 have 
    // that is fair to say 
    //
    // do OrConstraints really flow?
    // x =: int| string y 
    // x is a int|string of course they do
    //
    // but I do think they should look more like anything else where they are bound to their source node
    // how does rescoping work? there can be rescoped OrConstraints
    //

    // we need to be smart about intersect constraints
    // intersect of  A intersect B
    // is obviously stronger than
    // intersect of  A intersect B intersect C
    // 
    // but..
    // intersect of  A intersect B intersect C
    // vs
    // intersect of  A intersect B intersect (C union D) intersect (C union E)
    // get's more complex
    // we have added a constrait to C it is: C & (D | E)

}
