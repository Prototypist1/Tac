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
            public bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from) {
                return from.SwitchReturns(x => CanFlow(x), x => CanFlow(x), x => CanFlow(x), x => CanFlow(x));
            }
            public bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from) {

                return from.SwitchReturns(x => Flow(x), x => Flow(x), x => Flow(x), x => Flow(x));
            }

            public Dictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Members = new Dictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();

            public bool CanFlow(ConcreteFlowNode from)
            {
                foreach (var fromMember in from.Members)
                {
                    if (!CanFlowMember(fromMember))
                    {
                        return false;
                    }
                }
                return true;
            }
            public bool Flow(ConcreteFlowNode from)
            {
                var changes = false;
                foreach (var fromMember in from.Members)
                {
                    changes |= FlowMember(fromMember);
                }
                return changes;
            }

            private bool CanFlow(OrFlowNode from)
            {
                foreach (var member in from.VirtualMembers())
                {
                    if (!CanFlowMember( new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(member.Key, OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(member.Value))))
                    {
                        return false;
                    }
                }
                return true;
            }
            private bool Flow(OrFlowNode from)
            {
                var changes = false;
                foreach (var member in from.VirtualMembers())
                {
                    changes |= FlowMember( new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(member.Key, OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(member.Value)));
                }
                return changes;
            }

            public bool CanFlow(PrimitiveFlowNode from) => false;
            private bool Flow(PrimitiveFlowNode from) => throw new Exception("actually don't flow");

            public bool CanFlowMember( KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> fromMember)
            {
                if (this.Members.TryGetValue(fromMember.Key, out var toMember))
                {

                    return !fromMember.Value.SwitchReturns(
                        fromMemberValue => toMember.SwitchReturns(
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue))),
                        fromMemberValue => toMember.SwitchReturns(
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue))),
                        fromMemberValue => toMember.SwitchReturns(
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue))),
                        fromMemberValue => toMember.SwitchReturns(
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.CanFlow(ToOr(fromMemberValue))));
                }
                else
                {
                    return false;
                }
            }

            public bool FlowMember(KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> fromMember)
            {
                var changes = false;
                if (this.Members.TryGetValue(fromMember.Key, out var toMember))
                {

                    changes |= fromMember.Value.SwitchReturns(
                        fromMemberValue => toMember.SwitchReturns(
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue))),
                        fromMemberValue => toMember.SwitchReturns(
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue))),
                        fromMemberValue => toMember.SwitchReturns(
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue))),
                        fromMemberValue => toMember.SwitchReturns(
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMemberValue))));
                }
                return changes;
            }
            public bool CanFlow(InferredFlowNode from)
            {
                foreach (var fromMember in from.VirtualMembers())
                {
                    if (!CanFlowMember(new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(fromMember.Key, OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(fromMember.Value))))
                    {
                        return false;
                    }
                }
                return true;
            }
            public bool Flow(InferredFlowNode from)
            {
                var changes = false;
                foreach (var fromMember in from.VirtualMembers())
                {
                    if (this.Members.TryGetValue(fromMember.Key, out var toMember))
                    {
                        changes |= toMember.SwitchReturns(
                            toMemberValue => toMemberValue.Flow(ToOr(fromMember.Value)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMember.Value)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMember.Value)),
                            toMemberValue => toMemberValue.Flow(ToOr(fromMember.Value)));
                    }
                }
                return changes;
            }
        }

        public class OrFlowNode : IFlowNode
        {
            public bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from)
            {
                return from.SwitchReturns(x => CanFlow(x), x => CanFlow(x), x => CanFlow(x), x => CanFlow(x));
            }
            public bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from)
            {
                return from.SwitchReturns(x => Flow(x), x => Flow(x), x => Flow(x), x => Flow(x));
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

            // A | B ab =: C | D cd;
            // this is the same as
            private bool CanFlow( OrFlowNode from)
            {
                return Or.All(x => x.SwitchReturns(y => y.CanFlow(ToOr(from)), y => y.CanFlow(ToOr(from)), y => y.CanFlow(ToOr(from)), y => y.CanFlow(ToOr(from))));
            }

            private bool Flow(OrFlowNode from)
            {
                var changes = false;
                foreach (var item in this.Or)
                {
                    changes |= item.SwitchReturns(y => y.Flow(ToOr(from)), y => y.Flow(ToOr(from)), y => y.Flow(ToOr(from)), y => y.Flow(ToOr(from)));
                }
                return changes;
            }


            // A|B ab =: int i;
            private bool CanFlow( PrimitiveFlowNode from)
            {
                return false;
            }
            private bool Flow( PrimitiveFlowNode from)
            {
                throw new Exception("actually don't flow");
            }

            // A | B ab =: i;
            private bool CanFlow(InferredFlowNode from)
            {
                return Or.All(x => x.SwitchReturns(y => y.CanFlow(ToOr(from)), y => y.CanFlow(ToOr(from)), y => y.CanFlow(ToOr(from)), y => y.CanFlow(ToOr(from))));
            }
            private bool Flow( InferredFlowNode from)
            {
                var changes = false;
                foreach (var item in Or)
                {
                    changes |= item.GetValueAs(out IFlowNode _).Flow(ToOr(from));
                }
                return changes;
            }

            private bool CanFlow(ConcreteFlowNode from)
            {
                return Or.All(x => x.GetValueAs(out IFlowNode _).CanFlow(ToOr(from)));
            }
            private bool Flow( ConcreteFlowNode from)
            {
                var changes = false;
                foreach (var item in Or)
                {
                    changes |= item.GetValueAs(out IFlowNode _).Flow(ToOr(from));
                }
                return changes;
            }



        }

        public class InferredFlowNode : IFlowNode
        {
            public bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from)
            {
                return from.SwitchReturns(x => CanFlow(x), x => CanFlow(x), x => CanFlow(x), x => CanFlow(x));
            }
            public bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from)
            {
                return from.SwitchReturns(x => Flow(x), x => Flow(x), x => Flow(x), x => Flow(x));
            }
            public List<CombinedTypesAnd> Or = new List<CombinedTypesAnd>();

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

            public bool CanFlow(PrimitiveFlowNode from)
            {
                return !this.Or.Any();
            }
            private bool Flow( PrimitiveFlowNode from)
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
                    throw new Exception("actually don't flow");
                }
            }
            public bool CanFlow( ConcreteFlowNode from)
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
            public bool Flow( ConcreteFlowNode from)
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
            public bool CanFlow( InferredFlowNode from)
            {
                foreach (var fromItem in from.Or)
                {
                    foreach (var element in fromItem.And)
                    {
                        foreach (var toItem in this.Or)
                        {
                            if (!toItem.And.All(x =>
                                element.SwitchReturns(
                                    fromValue => x.SwitchReturns(
                                        toValue => toValue.CanFlow(ToOr(fromValue)),
                                        toValue => toValue.CanFlow(ToOr(fromValue)),
                                        toValue => toValue.CanFlow(ToOr(fromValue)),
                                        toValue => toValue.CanFlow(ToOr(fromValue))),
                                    fromValue => x.SwitchReturns(
                                        toValue => toValue.CanFlow(ToOr(fromValue)),
                                        toValue => toValue.CanFlow(ToOr(fromValue)),
                                        toValue => toValue.CanFlow(ToOr(fromValue)),
                                        toValue => toValue.CanFlow(ToOr(fromValue))),
                                    fromValue => x.SwitchReturns(
                                        toValue => toValue.CanFlow(ToOr(fromValue)),
                                        toValue => toValue.CanFlow(ToOr(fromValue)),
                                        toValue => toValue.CanFlow(ToOr(fromValue)),
                                        toValue => toValue.CanFlow(ToOr(fromValue))),
                                    fromValue => x.SwitchReturns(
                                        toValue => toValue.CanFlow(ToOr(fromValue)),
                                        toValue => toValue.CanFlow(ToOr(fromValue)),
                                        toValue => toValue.CanFlow(ToOr(fromValue)),
                                        toValue => toValue.CanFlow(ToOr(fromValue))))))
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
            public bool Flow(InferredFlowNode from)
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
