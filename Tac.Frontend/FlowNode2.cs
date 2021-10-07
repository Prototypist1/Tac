using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tac.Frontend.New.CrzayNamespace;

namespace Tac.Frontend
{


    class MustHave {
        public readonly IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic> path;
        public readonly EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraints;

        public MustHave(IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic> path, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraints)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.constraints = constraints ?? throw new ArgumentNullException(nameof(constraints));
        }

        public override bool Equals(object? obj)
        {
            return obj is MustHave have &&
                   EqualityComparer<IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>>.Default.Equals(path, have.path) &&
                   EqualityComparer<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>>.Default.Equals(constraints, have.constraints);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(path, constraints);
        }
    }

    class MustBePrimitive {
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
    }

    class GivenPathThen {
        public readonly IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>[] path;
        public readonly EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraints;

        public GivenPathThen(IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>[] path, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraints)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.constraints = constraints ?? throw new ArgumentNullException(nameof(constraints));
        }

        public override bool Equals(object? obj)
        {
            return obj is GivenPathThen then &&
                   EqualityComparer<IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>[]>.Default.Equals(path, then.path) &&
                   EqualityComparer<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>>.Default.Equals(constraints, then.constraints);
        }



        public override int GetHashCode()
        {
            return HashCode.Combine(path, constraints);
        }
    }

    class DisjointConstraint {
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

    }

    interface IFlowNode2 {
        IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> GetConstraints();
        /// <summary>
        /// when flowing down stream (given a =: b, downstream would be from a to b)
        /// only pass GivenPathThen and DisjointConstraint of GivenPathThen
        /// see {95C8B654-3AF5-42FD-A42B-A94165BEF7A3}
        /// </summary>
        bool AcceptConstraints(EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraints);
        bool CouldApplyToMe(IEnumerable<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraint);
    }

    class PrimitiveFlowNode2: IFlowNode2
    {
        private readonly Guid guid;

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
        public bool AcceptConstraints(EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> newConstraints) {
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
        public bool AcceptConstraints(EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> newConstraints)
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
                            return dependent.GetValueAs(out IFlowNode2 _).AcceptConstraints(mustHave.constraints);
                        }
                        return false;
                    },
                    mustBePrimitve => false,
                    givenPathThen =>
                    {
                        if (dependents.TryGetValue(givenPathThen.path.First(), out var dependent))
                        {
                            if (givenPathThen.path.Length == 1) {
                                return dependent.GetValueAs(out IFlowNode2 _).AcceptConstraints(givenPathThen.constraints);
                            }
                            var next = new GivenPathThen(givenPathThen.path.Skip(1).ToArray(), givenPathThen.constraints);
                            return dependent.GetValueAs(out IFlowNode2 _).AcceptConstraints(
                                new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>(
                                    new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> { 
                                        OrType.Make<MustHave, MustBePrimitive, GivenPathThen,DisjointConstraint>(next)}));

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
                            return false;
                        }

                        if (couldApply.Length == 1) {
                            return AcceptConstraints(
                                new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>(
                                    couldApply.Single().Select(x => ConstraintUtils.Broaden(x)).ToHashSet()));
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
                                new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>(
                                    new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> {
                                        OrType.Make<MustHave, MustBePrimitive, GivenPathThen,DisjointConstraint>(next)}));
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
                        return mustHave.constraints.ToArray();//??
                    }
                    return Array.Empty<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>();
                },
                mustBePrimitive => Array.Empty<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>(),
                givenPathThen => {
                    if (dependent.Key.Equals(givenPathThen.path.First()))
                    {
                        if (givenPathThen.path.Length == 1)
                        {
                            return givenPathThen.constraints.ToArray();
                        }
                        var next = new GivenPathThen(givenPathThen.path.Skip(1).ToArray(), givenPathThen.constraints);
                        return new[] {OrType.Make<MustHave, MustBePrimitive, GivenPathThen,DisjointConstraint>(next)};
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
                            return dependent.GetValueAs(out IFlowNode2 _).CouldApplyToMe(mustHave.constraints);
                        }
                        return false;
                    },
                    mustBePrimitve => false,
                    givenPathThen =>
                    {
                        if (dependents.TryGetValue(givenPathThen.path.First(), out var dependent))
                        {
                            if (givenPathThen.path.Length == 1)
                            {
                                return givenPathThen.constraints.All(x => dependent.GetValueAs(out IFlowNode2 _).CouldApplyToMe(new[] { x }));
                            }
                            var next = new GivenPathThen(givenPathThen.path.Skip(1).ToArray(), givenPathThen.constraints);
                            return dependent.GetValueAs(out IFlowNode2 _).CouldApplyToMe(new[] { OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(next) });

                        }
                        return false;
                    },
                    disjointConstraint => disjointConstraint.constraintSets.Any(x => CouldApplyToMe(x.Select(y=>ConstraintUtils.Broaden(y)))));
        }
    }

    class InferredFlowNode2 : IFlowNode2
    {
        private readonly EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraints = new (new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>());

        public bool AcceptConstraints(EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> newConstraints)
        {
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


        public bool AcceptConstraints(EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> newConstraints)
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

}
