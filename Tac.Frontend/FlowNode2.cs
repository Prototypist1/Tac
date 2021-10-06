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
        EqualableHashSet<GivenPathThen> GetDownStreamConstraints();
        IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> GetUpStreamConstraints();
        bool AcceptConstraints(EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> constraints);
        bool CouldApplyToMe(IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> constraint)
    }

    class PrimitiveFlowNode2: IFlowNode2
    {
        private readonly Guid guid;

        public EqualableHashSet<GivenPathThen> GetDownStreamConstraints() {
            return new EqualableHashSet<GivenPathThen>(new HashSet<GivenPathThen>());
        }
        public IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> GetUpStreamConstraints()
        {
            return new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>>(new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> {
                OrType.Make<MustHave, MustBePrimitive, GivenPathThen,DisjointConstraint> (new MustBePrimitive(guid))
            });
        }
        public bool AcceptConstraints(EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> newConstraints) {
            return false;
        }

        public bool CouldApplyToMe(IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> constraint) { 
        
        }
    }

    class ConcreteFlowNode2 : IFlowNode2
    {
        // doesn't have DisjointConstraints
        private readonly EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>> constraints = new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>(new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>());
        private readonly Dictionary<IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>, IOrType<PrimitiveFlowNode2, ConcreteFlowNode2, InferredFlowNode2, OrFlowNode2>> dependents = new ();

        public EqualableHashSet<GivenPathThen> GetDownStreamConstraints()
        {
            return new EqualableHashSet<GivenPathThen>(
                constraints.SelectMany(x =>
                {
                    if (x.Is3(out var givenPath))
                    {
                        return new[] { givenPath };
                    }
                    return Array.Empty<GivenPathThen>();
                }).ToHashSet());
        }
        public IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> GetUpStreamConstraints()
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
                                couldApplyToMe |= CouldApplyToMe(Broaden(item));
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
                                    couldApply.Single().Select(x => Broaden(x)).ToHashSet()));
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
                                        Flatten(set
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
        private List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen>>> Flatten(IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>[] constraints ) {
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

        public IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> Broaden(IOrType<MustHave, MustBePrimitive, GivenPathThen> orType) =>
            orType.SwitchReturns(x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(x),
                x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>(x));

        public bool CouldApplyToMe(IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> constraint)
        {
            return constraint.SwitchReturns(
                    mustHave =>
                    {
                        if (dependents.TryGetValue(mustHave.path, out var dependent))
                        {
                            return mustHave.constraints.All(x=> dependent.GetValueAs(out IFlowNode2 _).CouldApplyToMe(x));
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
                                return givenPathThen.constraints.All(x => dependent.GetValueAs(out IFlowNode2 _).CouldApplyToMe(x));
                            }
                            var next = new GivenPathThen(givenPathThen.path.Skip(1).ToArray(), givenPathThen.constraints);
                            return dependent.GetValueAs(out IFlowNode2 _).CouldApplyToMe(OrType.Make<MustHave, MustBePrimitive, GivenPathThen,DisjointConstraint>(next));

                        }
                        return false;
                    },
                    disjointConstraint => disjointConstraint.constraintSets.Any(x => x.All(y=>CouldApplyToMe(Broaden(y)))));
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
        public EqualableHashSet<GivenPathThen> GetDownStreamConstraints()
        {
            return new EqualableHashSet<GivenPathThen>(
                constraints.SelectMany(x =>
                {
                    if (x.Is3(out var givenPath))
                    {
                        return new[] { givenPath };
                    }
                    return Array.Empty<GivenPathThen>();
                }).ToHashSet());
        }
        public IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> GetUpStreamConstraints()
        {
            return constraints;
        }

        public bool CouldApplyToMe(IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> constraint)
        {
            return true;
        }
    }

    class OrFlowNode2 : IFlowNode2
    {
        // we have shared constrains
        // and disjoin constraints
        // do we calculate them from our sources?

        public bool AcceptConstraints(EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> newConstraints)
        {
            throw new NotImplementedException();
        }

        public EqualableHashSet<GivenPathThen> GetDownStreamConstraints()
        {
            throw new NotImplementedException();
        }

        public IReadOnlySet<IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint>> GetUpStreamConstraints()
        {
            throw new NotImplementedException();
        }

        public bool CouldApplyToMe(IOrType<MustHave, MustBePrimitive, GivenPathThen, DisjointConstraint> constraint)
        {

        }
    }
}
