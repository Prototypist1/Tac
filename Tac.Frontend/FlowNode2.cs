using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tac.Frontend.New.CrzayNamespace;

namespace Tac.Frontend
{


    // assumptions:
    //  - if a node flows something, it will always flow that thing. once "A" has a member nothing can take that member away from "A"
    // 

    interface IConstraint {
        bool IsCompatible(IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> constraint, List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>> assumeTrue);
    }

    // this originates at a ConcreteFlowNode2 
    // but it's constraints code from the element at the path
    //
    // this could also come from an or node
    class MustHave :IConstraint{
        public readonly IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic> path;
        public readonly ConcreteFlowNode2 source;
        public readonly IOrType<PrimitiveFlowNode2, ConcreteFlowNode2, InferredFlowNode2, OrFlowNode2> dependent;

        public MustHave(IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic> path, ConcreteFlowNode2 source, IOrType<PrimitiveFlowNode2, ConcreteFlowNode2, InferredFlowNode2, OrFlowNode2> dependent)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.dependent = dependent ?? throw new ArgumentNullException(nameof(dependent));
        }

        public override bool Equals(object? obj)
        {
            return obj is MustHave have &&
                   EqualityComparer<IOrType<PrimitiveFlowNode2, ConcreteFlowNode2, InferredFlowNode2, OrFlowNode2>>.Default.Equals(dependent, have.dependent);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(dependent);
        }

        public bool IsCompatible(
            IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> constraint, 
            List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>> assumeTrue) 
        {
            var pair = new UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>( OrType.Make< MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> (this), constraint);
            if (assumeTrue.Contains(pair)) {
                return true;
            }
            var nextAssumeTrue = new Lazy<List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>>>(() => {
                var list = assumeTrue.ToList();
                list.Add(pair);
                return list;
            });

            return constraint.SwitchReturns(
                    mustHave => {
                        if (path != mustHave.path)
                        {
                            return true;
                        }
                        if (dependent == mustHave.dependent)
                        {
                            return true;
                        }
                        return dependent.GetValueAs(out IFlowNode2 _).GetConstraints().All(myConstraint =>
                            mustHave.dependent.GetValueAs(out IFlowNode2 _).GetConstraints().All(theirConstraint => theirConstraint.GetValueAs(out IConstraint _).IsCompatible(myConstraint, nextAssumeTrue.Value)));
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
                        return dependent.GetValueAs(out IFlowNode2 _).GetConstraints().All(myConstraint =>
                            givenPathThen.dependent.GetValueAs(out IFlowNode2 _).GetConstraints().All(theirConstraint => theirConstraint.GetValueAs(out IConstraint _).IsCompatible(myConstraint, nextAssumeTrue.Value)));
                    },
                    disjoint => disjoint.constraintSets.Any(oneOf => oneOf.All(item => this.IsCompatible(ConstraintUtils.Broaden(item), nextAssumeTrue.Value))));
        }
        //public readonly EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraints;

    }

    // this orginates at a primitive flow node
    //
    // but it might also come from a very pointless or node: number | number
    //
    // but it doesn't really matter where it comes from
    class MustBePrimitive: IConstraint
    {
        public readonly Guid primitive;

        public MustBePrimitive(Guid primitive)
        {
            this.primitive = primitive;
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
            IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> constraint,
            List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>> assumeTrue)
        {
            var pair = new UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(this), constraint);
            if (assumeTrue.Contains(pair))
            {
                return true;
            }
            var nextAssumeTrue = new Lazy<List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>>>(() => {
                var list = assumeTrue.ToList();
                list.Add(pair);
                return list;
            });

            return constraint.SwitchReturns(
                mustHave => mustHave.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(this), assumeTrue), // reverse it  - use og assumeTrue
                prim => primitive == prim.primitive, 
                givenPathThen => false,
                disjoint => disjoint.constraintSets.Any(oneOf => oneOf.All(item => this.IsCompatible(ConstraintUtils.Broaden(item), nextAssumeTrue.Value))));
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
        public readonly IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic> path;
        public readonly ConcreteFlowNode2 source;
        public readonly IOrType<PrimitiveFlowNode2, ConcreteFlowNode2, InferredFlowNode2, OrFlowNode2> dependent;

        public GivenPathThen(IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic> path, ConcreteFlowNode2 source, IOrType<PrimitiveFlowNode2, ConcreteFlowNode2, InferredFlowNode2, OrFlowNode2> dependent)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.dependent = dependent ?? throw new ArgumentNullException(nameof(dependent));
        }

        public override bool Equals(object? obj)
        {
            return obj is GivenPathThen then &&
                   EqualityComparer<IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>>.Default.Equals(path, then.path) &&
                   EqualityComparer<ConcreteFlowNode2>.Default.Equals(source, then.source);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(path, source);
        }

        public bool IsCompatible(
           IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> constraint,
           List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>> assumeTrue)
        {
            var pair = new UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(this), constraint);
            if (assumeTrue.Contains(pair))
            {
                return true;
            }
            var nextAssumeTrue = new Lazy<List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>>>(() => {
                var list = assumeTrue.ToList();
                list.Add(pair);
                return list;
            });

            return constraint.SwitchReturns(
                mustHave => mustHave.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(this), assumeTrue), // reverse it - use og assumeTrue
                primitive => primitive.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(this), assumeTrue), // reverse it - use og assumeTrue
                givenPathThen => {
                    if (path != givenPathThen.path)
                    {
                        return true;
                    }
                    if (dependent == givenPathThen.dependent)
                    {
                        return true;
                    }
                    return dependent.GetValueAs(out IFlowNode2 _).GetConstraints().All(myConstraint =>
                        givenPathThen.dependent.GetValueAs(out IFlowNode2 _).GetConstraints().All(theirConstraint => theirConstraint.GetValueAs(out IConstraint _).IsCompatible(myConstraint, nextAssumeTrue.Value)));
                },
                disjoint => disjoint.constraintSets.Any(oneOf => oneOf.All(item => this.IsCompatible(ConstraintUtils.Broaden(item), nextAssumeTrue.Value))));
        }
    }

    // this comes from on or
    // it's set really comes from a set of nodes of various types
    //
    class DisjointConstraint : IConstraint
    {
        // constraintSets does not containt DisjointConstraint
        // they are flattened out
        public readonly EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>> constraintSets;

        public DisjointConstraint(EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>> constraintSets)
        {
            if (constraintSets.Count == 1) {
                throw new Exception("you can't have an Or with only one thing!");
            }

            this.constraintSets = constraintSets ?? throw new ArgumentNullException(nameof(constraintSets));
        }

        public override bool Equals(object? obj)
        {
            return obj is DisjointConstraint constraint &&
                   EqualityComparer<EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>>>.Default.Equals(constraintSets, constraint.constraintSets);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(constraintSets);
        }

        public bool IsCompatible(
            IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> constraint,
            List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>> assumeTrue)
        {
            var pair = new UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(this), constraint);
            if (assumeTrue.Contains(pair))
            {
                return true;
            }
            var nextAssumeTrue = new Lazy<List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>>>(()=> { 
                var list = assumeTrue.ToList();
                list.Add(pair);
                return list;
                });

            return constraint.SwitchReturns(
                mustHave => mustHave.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(this), assumeTrue), // reverse it  - use og assumeTrue
                primitive => primitive.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(this), assumeTrue), // reverse it  - use og assumeTrue
                givenPathThen => givenPathThen.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(this), assumeTrue), // reverse it  - use og assumeTrue
                disjoint => 
                    constraintSets
                        .Any(ourSet => 
                            disjoint.constraintSets.Any(thierSet => 
                                ourSet.All(ourItem => 
                                    thierSet.All(theirItem => ourItem.GetValueAs(out IConstraint _).IsCompatible(ConstraintUtils.Broaden(theirItem), nextAssumeTrue.Value))))));
        }
    }

    interface IFlowNode2 {
        IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> GetConstraints();
        /// <summary>
        /// when flowing down stream (given a =: b, downstream would be from a to b)
        /// only pass GivenPathThen and DisjointConstraint of GivenPathThen
        /// see {95C8B654-3AF5-42FD-A42B-A94165BEF7A3}
        /// </summary>
        bool AcceptConstraints(IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraints);
        bool CouldApplyToMe(IEnumerable<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraint);
    }

    class PrimitiveFlowNode2: IFlowNode2
    {
        public readonly Guid guid;

        public PrimitiveFlowNode2(Guid guid)
        {
            this.guid = guid;
        }

        public IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> GetConstraints()
        {
            return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>(new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> {
                OrType.Make<MustHave, MustBePrimitive, GivenPathThen,DisjointConstraint> (new MustBePrimitive(guid))
            });
        }
        public bool AcceptConstraints(IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> newConstraints) {
            return false;
        }

        public bool CouldApplyToMe(IEnumerable<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraint) =>
            constraint.All(x=>x.Is2(out var prim) && prim.primitive == guid);
        
    }

    class ConcreteFlowNode2 : IFlowNode2
    { 
        // doesn't have DisjointConstraints
        private readonly EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>> constraints = new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>(new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>());
        private readonly Dictionary<IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>, IOrType<PrimitiveFlowNode2, ConcreteFlowNode2, InferredFlowNode2, OrFlowNode2>> dependents = new ();

        public IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> GetConstraints()
        {
            return constraints.Select(x=>x.SwitchReturns(
                x => (IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>)OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(x))).ToHashSet();
        }
        public bool AcceptConstraints(IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> newConstraints)
        {
            var res = false;
            foreach (var constraint in newConstraints)
            {
                res |= constraint.SwitchReturns(
                    mustHave =>
                    {
                        // what happen here?
                        // type {x;} a =: {y;} | number b
                        // ...they wrote bad code
                        // y could flow if they had left it inferred but they didn't
                        if (dependents.TryGetValue(mustHave.path, out var dependent)) {
                            var constraints = mustHave.dependent.GetValueAs(out IFlowNode2 _).GetConstraints();
                            return dependent.GetValueAs(out IFlowNode2 _).AcceptConstraints(constraints);
                        }
                        return false;
                    },
                    mustBePrimitve => false,
                    givenPathThen =>
                    {
                        if (dependents.TryGetValue(givenPathThen.path, out var dependent))
                        {
                            return dependent.GetValueAs(out IFlowNode2 _).AcceptConstraints(givenPathThen.dependent.GetValueAs(out IFlowNode2 _).GetConstraints());
                        }
                        return false;
                    },
                    disjointConstraint => {
                        // we try to find determine if we are definately one or the other of the disjoint options
                        // if we could be either, path the constraint on to our dependents
                        var couldApply = disjointConstraint.constraintSets.SelectMany(set =>
                        {
                            var couldApplyToMe = true;
                            foreach (var item in set)
                            {
                                couldApplyToMe |= CouldApplyToMe(ConstraintUtils.Broaden(item));
                            }
                            if (couldApplyToMe) {
                                return new[] { set };
                            }
                            return Array.Empty<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>>();
                        }).ToArray();

                        if (couldApply.Length == 0) {
                            // EDCECD4E-62DF-4216-BFDE-71083FF0C64A
                            // this is really an error state
                            // and an upsetting one because we could have flowed earlier versions of the disjointConstraint
                            // I think I probably want to restart the solve 
                            // not to flow certain pairs of nodes 
                            // 
                            // there are probably other cases
                            // like being asked to accept inconsistant constraints
                            return false;
                        }

                        if (couldApply.Length == 1) {
                            return AcceptConstraints(
                                    couldApply.Single().Select(x => ConstraintUtils.Broaden(x)).ToHashSet());
                        }

                        // we need to create to approprate disjointConstraint for each element
                        var res = false;
                        foreach (var dependent in dependents)
                        {
                            // this DisjointConstraint can have empty sets in it
                            // {a;b;} y =: {int a;}| {b;} x
                            // y's a could be an int or it could be unconstrainted
                            var next = new DisjointConstraint(new EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>>(
                                couldApply
                                    .SelectMany(set=>
                                        ConstraintUtils.Flatten(set
                                            .SelectMany(constraint => Retarget(constraint, dependent))
                                            .ToArray())
                                        .Select(x=> new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>(x.ToHashSet())))
                                    .ToHashSet()));
                            res |= dependent.Value.GetValueAs(out IFlowNode2 _).AcceptConstraints(
                                    new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> {
                                        OrType.Make<MustHave, MustBePrimitive, GivenPathThen,DisjointConstraint>(next)});
                        }
                        return res;
                    });
            }
            return false;
        }

        // but this could contain DisjointConstraint so we need to split it out
        
        private IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>[] Retarget(IOrType<MustHave, MustBePrimitive, GivenPathThen> constraint, KeyValuePair<IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>, IOrType<PrimitiveFlowNode2, ConcreteFlowNode2, InferredFlowNode2, OrFlowNode2>> dependent)
        {
            return constraint.SwitchReturns(
                mustHave =>
                {
                    if (dependent.Key.Equals(mustHave.path))
                    {
                        return mustHave.dependent.GetValueAs(out IFlowNode2 _).GetConstraints().ToArray();//??
                    }
                    return Array.Empty<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>();
                },
                mustBePrimitive => Array.Empty<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>(),
                givenPathThen => {
                    if (dependent.Key.Equals(givenPathThen.path))
                    {
                        return givenPathThen.dependent.GetValueAs(out IFlowNode2 _).GetConstraints().ToArray();
                    }
                    return Array.Empty<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>();
                });
        }

        public bool CouldApplyToMe(IEnumerable<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraints)
        {
            return constraints.All(constraint => CouldApplyToMe(constraint));
        }

        private bool CouldApplyToMe(IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> constraint)
        {
            return constraint.SwitchReturns(
                    mustHave =>
                    {
                        if (dependents.TryGetValue(mustHave.path, out var dependent))
                        {
                            return dependent.GetValueAs(out IFlowNode2 _).CouldApplyToMe(mustHave.dependent.GetValueAs(out IFlowNode2 _).GetConstraints());
                        }
                        return false;
                    },
                    mustBePrimitve => false,
                    givenPathThen =>
                    {
                        if (dependents.TryGetValue(givenPathThen.path, out var dependent))
                        {
                            return dependent.GetValueAs(out IFlowNode2 _).CouldApplyToMe(givenPathThen.dependent.GetValueAs(out IFlowNode2 _).GetConstraints());
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
                    disjointConstraint => disjointConstraint.constraintSets.Any(x => CouldApplyToMe(x.Select(y=>ConstraintUtils.Broaden(y)))));
        }
    }

    class InferredFlowNode2 : IFlowNode2
    {
        private readonly EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraints = new (new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>());
        private bool errorState = false;

        public bool AcceptConstraints(IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> newConstraints)
        {
            if (errorState) {
                return false;
            }

            if (!constraints.All(existingItem => newConstraints
                    .All(newItem => existingItem.GetValueAs(out IConstraint _).IsCompatible(newItem, new List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>>())))){
                errorState = true;
                return false;
            }

            var res = false;
            foreach (var newConstraint in newConstraints)
            {
                res |= constraints.Add(newConstraint);
            }
            return res;
        }
        public IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> GetConstraints()
        {
            return constraints;
        }

        public bool CouldApplyToMe(IEnumerable<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraint)
        {
            return true;
        }
    }

    class OrFlowNode2 : IFlowNode2
    {
        // we have shared constrains
        // and disjoin constraints
        // do we calculate them from our sources?
        private readonly EqualableHashSet<IOrType<PrimitiveFlowNode2, ConcreteFlowNode2, InferredFlowNode2>> or = new (new HashSet<IOrType<PrimitiveFlowNode2, ConcreteFlowNode2, InferredFlowNode2>>());


        public bool AcceptConstraints(IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> newConstraints)
        {
            var res = false;
            foreach (var item in or)
            {
                res |= item.GetValueAs(out IFlowNode2 _).AcceptConstraints(newConstraints);
            }
            return res;
        }

        public IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> GetConstraints()
        {
            var sets =  or.SelectMany(x => ConstraintUtils.Flatten(x.GetValueAs(out IFlowNode2 _).GetConstraints().ToArray())).ToArray();

            var unionSet = sets.First().ToArray();
            
            foreach (var set in sets.Skip(1))
            {
                unionSet = unionSet.Union(set).ToArray();
            }

            var res = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>();

            foreach (var item in unionSet)
            {
                res.Add(ConstraintUtils.Broaden(item));
            }

            var disjoint = 
                new DisjointConstraint(
                    new EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>>(
                        sets.Select(x => new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>(x.Except(unionSet).ToHashSet())).ToHashSet()));
            res.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(disjoint));

            return res;
        }

        public bool CouldApplyToMe(IEnumerable<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraints)
            => or.Any(x => x.GetValueAs(out IFlowNode2 _).CouldApplyToMe(constraints));
    }


    static class ConstraintUtils
    {
        /// <summary>
        /// take [A,B,C,(E|F)] to [[A,B,C,E],[A,B,C,F]]
        /// where (E|F) is a DisjointConstraint
        /// the result will contain no DisjointConstraints
        /// </summary>
        public static List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen>>> Flatten(IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>[] constraints)
        {
            List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen>>> ress = new List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen>>> {
                new List<IOrType<MustHave, MustBePrimitive, GivenPathThen>>()
            };

            foreach (var constraint in constraints)
            {
                constraint.Switch(x =>
                {
                    foreach (var res in ress)
                    {
                        res.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen>(x));
                    }
                },
                x =>
                {
                    foreach (var res in ress)
                    {
                        res.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen>(x));
                    }
                },
                x =>
                {
                    foreach (var res in ress)
                    {
                        res.Add(OrType.Make<MustHave, MustBePrimitive, GivenPathThen>(x));
                    }
                },
                x =>
                {
                    var sourceRes = ress;
                    ress = new List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen>>>();
                    foreach (var set in x.constraintSets)
                    {
                        var newRess = sourceRes.Select(x => x.ToList()).ToList();
                        foreach (var newRes in newRess)
                        {
                            newRes.AddRange(set);
                        }
                        ress.AddRange(newRess);
                    }
                });
            }
            return ress;
        }


        public static IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> Broaden(IOrType<MustHave, MustBePrimitive, GivenPathThen> orType) =>
            orType.SwitchReturns(x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(x));

        public static IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> Broaden(IOrType<GivenPathThen, DisjointConstraint> orType) =>
                orType.SwitchReturns(x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(x),
                    x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(x));


        // downstream we only send GivenPathThen and DisjointConstraint made entirely of GivenPathThen
        // {95C8B654-3AF5-42FD-A42B-A94165BEF7A3}
        public static IReadOnlySet<IOrType<GivenPathThen, DisjointConstraint>> ToDownStream(IEnumerable<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraints) {
            return constraints.SelectMany(x =>
            {
                if (x.Is3(out var givenPath))
                {
                    return new[] { OrType.Make < GivenPathThen, DisjointConstraint > (givenPath) };
                }
                if (x.Is4(out var disjoint)) {
                    var sets = new EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>>(disjoint.constraintSets.Select(set =>
                        new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>(set.SelectMany(y =>
                       {
                           if (y.Is3(out var innerGivenPath))
                           {
                               return new[] { OrType.Make<MustHave, MustBePrimitive, GivenPathThen>(innerGivenPath) };
                           }
                           return Array.Empty<IOrType<MustHave, MustBePrimitive, GivenPathThen>>();
                       }).ToHashSet())).ToHashSet());

                    if (sets.Any(x => x.Any())) {
                        return new[] { OrType.Make<GivenPathThen, DisjointConstraint>(new DisjointConstraint(sets)) };
                    }
                }
                return Array.Empty<IOrType<GivenPathThen, DisjointConstraint>>();
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
    // something where an or flow a DisjointConstraint of GivenPathThen down stream
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

    // can a disjoint constraint ever add a member?
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
}
