using Prototypist.Toolbox;
using Prototypist.Toolbox.IEnumerable;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;

namespace Tac.Frontend.New.CrzayNamespace
{

    internal partial class Tpn
    {

        public interface IFlowNode {
            bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from);
            bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from);
        }

        public class PrimitiveFlowNode: IFlowNode
        {

            public bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from) {
                return from.Is3(out var v3) && Equals(v3, this);
            }
            public bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from) {
                return false;
            }
        }


        public class ConcreteFlowNode : IFlowNode
        {

            public Dictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Members = new Dictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();

            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Input = Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Output = Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();

            public bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from) {
                if (from.Is3(out var _))
                {
                    return false;
                }

                foreach (var fromMember in from.SwitchReturns(
                            y => y.Members,
                            y => y.VirtualMembers().Select(z => new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(z.Key, OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(z.Value))),
                            y => new Dictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(),
                            y => y.VirtualMembers().Select(z => new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(z.Key, OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(z.Value)))))
                {
                    if (!CanFlowMember(fromMember))
                    {
                        return false;
                    }
                }
                return true;
            }
            public bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from) {

                if (from.Is3(out var _))
                {
                    throw new Exception("actually don't flow");
                }

                var changes = false;
                foreach (var fromMember in from.SwitchReturns(
                          y => y.Members,
                          y => y.VirtualMembers().Select(z => new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(z.Key, OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(z.Value))),
                          y => new Dictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(),
                          y => y.VirtualMembers().Select(z => new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(z.Key, OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(z.Value)))))
                {
                    changes |= FlowMember(fromMember);
                }
                return changes;
            }

            private bool CanFlowMember( KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> fromMember)
            {
                if (this.Members.TryGetValue(fromMember.Key, out var toMember))
                {
                    return toMember.SwitchReturns(
                           toMemberValue => toMemberValue.CanFlow(fromMember.Value),
                           toMemberValue => toMemberValue.CanFlow(fromMember.Value),
                           toMemberValue => toMemberValue.CanFlow(fromMember.Value),
                           toMemberValue => toMemberValue.CanFlow(fromMember.Value));
                }
                else
                {
                    return false;
                }
            }

            private bool FlowMember(KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> fromMember)
            {
                var changes = false;
                if (this.Members.TryGetValue(fromMember.Key, out var toMember))
                {
                    changes |= toMember.SwitchReturns(
                            toMemberValue => toMemberValue.Flow(fromMember.Value),
                            toMemberValue => toMemberValue.Flow(fromMember.Value),
                            toMemberValue => toMemberValue.Flow(fromMember.Value),
                            toMemberValue => toMemberValue.Flow(fromMember.Value));
                }
                return changes;
            }
        }

        public class OrFlowNode : IFlowNode
        {
            public bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from)
            {
                if (from.Is3(out var _)) {
                    return false;
                }

                return Or.All(x => x.GetValueAs(out IFlowNode _).CanFlow(from));
            }
            public bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from)
            {
                if (from.Is3(out var _))
                {
                    throw new Exception("actually don't flow");
                }

                var changes = false;
                foreach (var item in this.Or)
                {
                    changes |= item.GetValueAs(out IFlowNode _).Flow(from);
                }
                return changes;
            }
            public OrFlowNode(IReadOnlyList<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> or)
            {
                Or = or ?? throw new ArgumentNullException(nameof(or));
            }

            public IReadOnlyList<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Or { get; }

            // these should only be used for from
            internal IEnumerable<KeyValuePair<IKey, OrFlowNode>> VirtualMembers()
            {
                // this is the intersection 
                var count = Or.Count();
                return Or.SelectMany(x => x.SwitchReturns(
                            y => y.Members,
                            y => y.VirtualMembers().Select(z => new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(z.Key, OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(z.Value))),
                            y => new Dictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(),
                            y => y.VirtualMembers().Select(z => new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(z.Key, OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(z.Value)))))
                        .GroupBy(x => x.Key).Where(x => x.Count() == count)
                        .Select(x =>
                        {
                            var res = new OrFlowNode(x.Select(y => y.Value).ToList());
                            return new KeyValuePair<IKey, OrFlowNode>(x.Key, res);
                        });
            }
        }

        public class InferredFlowNode : IFlowNode
        {
            public List<CombinedTypesAnd> Or = new List<CombinedTypesAnd>();
            public IIsPossibly<CombinedTypesAnd> Input = Possibly.IsNot<CombinedTypesAnd>();
            public IIsPossibly<CombinedTypesAnd> Output = Possibly.IsNot<CombinedTypesAnd>();

            public bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from)
            {
                return from.SwitchReturns(x => CanFlow(x), x => CanFlow(x), x => CanFlow(x), x => CanFlow(x));
            }
            public bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from)
            {
                return from.SwitchReturns(x => Flow(x), x => Flow(x), x => Flow(x), x => Flow(x));
            }

            // these should only be used for from
            internal IEnumerable<KeyValuePair<IKey, InferredFlowNode>> VirtualMembers()
            {

                // this is the intersection 
                return Or.SelectMany(x => x.VirtualMembers())
                    .GroupBy(x => x.Key).Where(x => x.Count() == Or.Count)
                    .Select(x =>
                    {
                        var res = new InferredFlowNode();
                        res.Or = x.Select(y => y.Value).ToList();

                        return new KeyValuePair<IKey, InferredFlowNode>(x.Key, res);
                    });
            }

            private bool CanFlow( OrFlowNode from)
            {
                if (!this.Or.Any())
                {
                    return true;
                }
                else
                {
                    return this.Or.All(x => x.And.All(y => y.SwitchReturns(
                        z => z.CanFlow(ToOr(from)),
                        z => z.CanFlow(ToOr(from)),
                        z => z.CanFlow(ToOr(from)),
                        z => z.CanFlow(ToOr(from)))));
                }
            }
            private bool Flow( OrFlowNode from)
            {
                if (!this.Or.Any())
                {
                    var toAdd = new InferredFlowNode.CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> {
                    OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode,OrFlowNode>(from)
                });
                    this.Or.Add(toAdd);
                    return true;
                }
                else
                {
                    var changes = false;
                    var nextList = new List<InferredFlowNode.CombinedTypesAnd>();
                    foreach (var element in this.Or)
                    {
                        if (!element.And.Contains(OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(from)))
                        {
                            nextList.Add(element.AddAsNew(OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(from)));
                            // this is really only possibly a change what you added could be a subset
                            changes = true;
                        }
                        else
                        {
                            nextList.Add(element);
                        }
                    }

                    this.Or = nextList;

                    return changes;
                }
            }

            private bool CanFlow(PrimitiveFlowNode from)
            {
                return !Or.Any();
            }
            private bool Flow( PrimitiveFlowNode from)
            {
                if (!Or.Any())
                {
                    var toAdd = new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> {
                    OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode,OrFlowNode>(from)
                });
                    Or.Add(toAdd);
                    return true;
                }
                else
                {
                    throw new Exception("actually don't flow");
                }
            }
            private bool CanFlow( ConcreteFlowNode from)
            {
                if (!Or.Any())
                {
                    return true;
                }
                else
                {
                    return Or.All(x => x.And.All(y => y.SwitchReturns(
                        z => z.CanFlow(ToOr(from)),
                        z => z.CanFlow(ToOr(from)),
                        z => z.CanFlow(ToOr(from)),
                        z => z.CanFlow(ToOr(from)))));
                }
            }
            private bool Flow( ConcreteFlowNode from)
            {
                if (!Or.Any())
                {
                    var toAdd = new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> {
                        OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode,OrFlowNode>(from)
                    });
                    Or.Add(toAdd);
                    return true;
                }
                else
                {
                    var changes = false;
                    var nextList = new List<InferredFlowNode.CombinedTypesAnd>();
                    foreach (var element in this.Or)
                    {
                        if (!element.And.Contains(OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(from)))
                        {
                            nextList.Add(element.AddAsNew(OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(from)));
                            // this is really only possibly a change what you added could be a subset
                            changes = true;
                        }
                        else
                        {
                            nextList.Add(element);
                        }
                    }

                    this.Or = nextList;

                    return changes;
                }
            }
            private bool CanFlow(InferredFlowNode from)
            {
                foreach (var fromItem in from.Or)
                {
                    foreach (var element in fromItem.And)
                    {
                        foreach (var toItem in this.Or)
                        {
                            if (!toItem.And.All(x =>

                                 x.SwitchReturns(
                                        toValue => toValue.CanFlow(element),
                                        toValue => toValue.CanFlow(element),
                                        toValue => toValue.CanFlow(element),
                                        toValue => toValue.CanFlow(element))))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            // ab =: A | B _ 
            // cd =: C | D _
            // ab =: cd
            // ab is A&C | A&D | B&C | B&D
            private bool Flow(InferredFlowNode from)
            {
                if (!this.Or.Any())
                {
                    foreach (var item in from.Or)
                    {
                        this.Or.Add(item);
                    }
                    return from.Or.Any();
                }
                else
                {

                    var newList = new List<InferredFlowNode.CombinedTypesAnd>();
                    foreach (var fromItem in from.Or)
                    {
                        foreach (var toItem in this.Or)
                        {
                            var toAdd = new InferredFlowNode.CombinedTypesAnd(fromItem.And.Union(toItem.And).ToHashSet());
                            newList.Add(toAdd);
                        }
                    }
                    if (!this.Or.SetEqual(newList))
                    {
                        this.Or = newList;
                        return true;
                    }
                    return false;
                }
            }

            public class CombinedTypesAnd
            {

                internal IEnumerable<KeyValuePair<IKey, CombinedTypesAnd>> VirtualMembers()
                {
                    var count = And.Count();
                    return And.SelectMany(x => x.SwitchReturns(
                                y => y.Members,
                                y => y.VirtualMembers().Select(z => new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(z.Key, OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(z.Value))),
                                y => new Dictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(),
                                y => y.VirtualMembers().Select(z => new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(z.Key, OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(z.Value)))))
                            .GroupBy(x => x.Key).Where(x => x.Count() == count)
                            .Select(x =>
                            {
                                var res = new CombinedTypesAnd(x.Select(y => y.Value).ToHashSet());
                                return new KeyValuePair<IKey, CombinedTypesAnd>(x.Key, res);
                            });
                }

                public override bool Equals(object? obj)
                {
                    return obj is CombinedTypesAnd and &&
                           And.SetEqual(and.And);
                }

                public override int GetHashCode()
                {
                    return HashCode.Combine(And);
                }

                public IEnumerable<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> And { get; }

                public CombinedTypesAnd(HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> and)
                {
                    And = and ?? throw new ArgumentNullException(nameof(and));
                }

                internal CombinedTypesAnd AddAsNew(OrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> orType)
                {
                    var set = new HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
                    foreach (var item in And)
                    {
                        set.Add(item);
                    }
                    set.Add(orType);
                    return new CombinedTypesAnd(set);
                }

            }
        }


        public static IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> ToOr(ConcreteFlowNode node) {
            return OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(node);
        }
        public static IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> ToOr(InferredFlowNode node)
        {
            return OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(node);
        }
        public static IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> ToOr(PrimitiveFlowNode node)
        {
            return OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(node);
        }
        public static IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> ToOr(OrFlowNode node)
        {
            return OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(node);
        }
    }
}
