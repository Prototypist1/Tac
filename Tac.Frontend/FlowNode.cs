using Prototypist.Toolbox;
using Prototypist.Toolbox.Dictionary;
using Prototypist.Toolbox.IEnumerable;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Frontend.New.CrzayNamespace
{

    internal partial class Tpn
    {
        // ok so next a try they shared virtual representation
        // it is an or of and of primitive/concrete nodes

        // you can create it by merging together a bunch of nodes in an or-manner or an and-manner

        public class CombinedTypesAnd
        {
            public IEnumerable<KeyValuePair<IKey, VirtualNode>> VirtualMembers()
            {
                return VirtualMembers(And);
            }

            public static IEnumerable<KeyValuePair<IKey, VirtualNode>> VirtualMembers(HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>> backing)
            {
                return backing.SelectMany(x => x.GetValueAs(out IVirtualFlowNode _).VirtualMembers())
                        .GroupBy(x => x.Key)
                        .Where(x=>x.Count() == backing.Count)
                        .Select(x =>
                        {
                            if (x.Count() == 1)
                            {
                                return new KeyValuePair<IKey, VirtualNode>(x.Key, x.First().Value);
                            }

                            return new KeyValuePair<IKey, VirtualNode>(x.Key, VirtualNode.IsAll(x.Select(y => y.Value).ToHashSet()));
                        });
            }

            public IIsPossibly<VirtualNode> VirtualInput()
            {
                return VirtualInput(And);
            }

            public static IIsPossibly<VirtualNode> VirtualInput(HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>> backing)
            {
                var set = backing.Select(x => x.GetValueAs(out IVirtualFlowNode _).VirtualInput())
                        .OfType<IIsDefinately<VirtualNode>>().Select(x => x.Value).ToList();

                if (set.Count == 1)
                {
                    return Possibly.Is(set.First());
                }
                if (set.Count == backing.Count)
                {
                    return Possibly.Is(VirtualNode.IsAll(set.ToHashSet()));
                }

                return Possibly.IsNot<VirtualNode>();
            }

            public IIsPossibly<VirtualNode> VirtualOutput()
            {
                return VirtualOutput(And);
            }

            public static IIsPossibly<VirtualNode> VirtualOutput(HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>> backing)
            {
                var set = backing.Select(x => x.GetValueAs(out IVirtualFlowNode _).VirtualOutput())
                        .OfType<IIsDefinately<VirtualNode>>().Select(x => x.Value).ToList();

                if (set.Count == 1)
                {
                    return Possibly.Is(set.First());
                }
                if (set.Count == backing.Count)
                {
                    return Possibly.Is(VirtualNode.IsAll(set.ToHashSet()));
                }

                return Possibly.IsNot<VirtualNode>();
            }


            public override bool Equals(object? obj)
            {
                return obj is CombinedTypesAnd and &&
                       And.SetEqual(and.And);
            }

            public override int GetHashCode()
            {
                return And.Select(x => x.GetHashCode()).Sum();
            }

            // please don't add to this, it will change the HashCode
            public HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>> And { get; }

            public CombinedTypesAnd(HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>> and)
            {
                And = and ?? throw new ArgumentNullException(nameof(and));
            }

            internal CombinedTypesAnd AddAsNew(OrType<ConcreteFlowNode, PrimitiveFlowNode> orType)
            {
                var set = new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>();
                foreach (var item in And)
                {
                    set.Add(item);
                }
                set.Add(orType);
                return new CombinedTypesAnd(set);
            }

        }


        public class VirtualNode: IVirtualFlowNode
        {

            public HashSet<CombinedTypesAnd> Or = new HashSet<CombinedTypesAnd>();


            public static VirtualNode IsAll(IEnumerable<IVirtualFlowNode> toMerge) {
                return new VirtualNode(toMerge.Select(x => x.ToRep()).Aggregate((a, b) => Union(a, b)).Distinct().ToHashSet());
                
            }

            public static VirtualNode IsAny(IEnumerable<IVirtualFlowNode> toMerge)
            {
                return new VirtualNode(toMerge.SelectMany(x => x.ToRep()).Distinct().ToHashSet());
            }

            public VirtualNode(HashSet<CombinedTypesAnd> or)
            {
                Or = or ?? throw new ArgumentNullException(nameof(or));
            }

            private static HashSet<CombinedTypesAnd> Union(HashSet<CombinedTypesAnd> left, HashSet<CombinedTypesAnd> right)
            {
                var res = new List<CombinedTypesAnd>();
                foreach (var leftEntry in left)
                {
                    foreach (var rightEntry in right)
                    {
                        res.Add(Merge(leftEntry, rightEntry));
                    }
                }
                return res.Distinct().ToHashSet();
            }

            private static CombinedTypesAnd Merge(CombinedTypesAnd left, CombinedTypesAnd right)
            {
                return new CombinedTypesAnd(left.And.Union(right.And).ToHashSet());
            }

            public override bool Equals(object? obj)
            {
                return obj is VirtualNode node &&
                       Or.SetEquals( node.Or);
            }

            public override int GetHashCode()
            {
                return Or.Sum(x=>x.GetHashCode());
            }

            public IEnumerable<KeyValuePair<IKey, VirtualNode>> VirtualMembers()
            {
                return VirtualMembers(Or);
            }

            public static IEnumerable<KeyValuePair<IKey, VirtualNode>> VirtualMembers(HashSet<CombinedTypesAnd> backing)
            {
                return backing.SelectMany(x => x.VirtualMembers())
                        .GroupBy(x => x.Key)
                        .Select(x =>
                        {
                            if (x.Count() == 1)
                            {
                                return new KeyValuePair<IKey, VirtualNode>(x.Key, x.First().Value);
                            }

                            return new KeyValuePair<IKey, VirtualNode>(x.Key, IsAny(x.Select(y => y.Value).ToHashSet()));
                        });
            }

            public IIsPossibly<VirtualNode> VirtualInput()
            {
                return VirtualInput(Or);
            }

            public static IIsPossibly<VirtualNode> VirtualInput(HashSet<CombinedTypesAnd> backing)
            {
                var set = backing.Select(x => x.VirtualInput())
                        .OfType<IIsDefinately<VirtualNode>>().Select(x => x.Value).ToList();

                if (!set.Any())
                {
                    return Possibly.IsNot<VirtualNode>();
                }
                if (set.Count == 1)
                {
                    return Possibly.Is(set.First());
                }

                return Possibly.Is(IsAny(set.ToHashSet()));
            }

            public IIsPossibly<VirtualNode> VirtualOutput()
            {
                return VirtualOutput(Or);
            }

            public static IIsPossibly<VirtualNode> VirtualOutput(HashSet<CombinedTypesAnd> backing)
            {
                var set = backing.Select(x => x.VirtualOutput())
                        .OfType<IIsDefinately<VirtualNode>>().Select(x => x.Value).ToList();

                if (!set.Any())
                {
                    return Possibly.IsNot<VirtualNode>();
                }
                if (set.Count == 1)
                {
                    return Possibly.Is(set.First());
                }

                return Possibly.Is(IsAny(set.ToHashSet()));
            }

            public HashSet<CombinedTypesAnd> ToRep() => Or;
        }


        // -----------------------------------------------------------------------------------------------------------------

        // ok so you have prime types and you have generated types
        // prime types are primitives and concrete
        // 
        // generated are ors and ands
        // generated types need to be cashed 
        // or maybe just have some good idea of equality

        // these generated types are going to need to be convertable as well

        // no more big or
        // virtual or type + virtual and type with good ideas of equality 


        // SkipItCache sucks
        // I think I am going to end up just not allow a flow to be called inside itself
        // I will always catch it not the next round
        // I can always try flowing again (and again) until there are no changes 

        // --------------------------------------------------------------------------------------------------------------------


        // I wish the members of these to be visible to things in Tpn 
        // like TypeProblem2 and TypeSolution
        // but not to the outside world
        // I could not figure out how to do that
        // so the members are just public

        // the only way I can think of is redickulous
        // solution and tpn would have to be inside a chain of nested classes 
        // containing all the classes I want them to access

        // or they could be in there own project but I tired that and it sucked


        public interface IFlowNode<TSource> : IFlowNode
        {
            IIsPossibly<TSource> Source { get; }
        }


        // I am not really sure this works 😖 
        // I mean, you don't have to have to change to change
        // if one of your constituents changes you also change
        public class SkipItCache {

            private readonly Dictionary<IFlowNode, List<IFlowNode>> backing = new Dictionary<IFlowNode, List<IFlowNode>>();
            public void Clear(IFlowNode source) {
                if (backing.ContainsKey(source)) {
                    backing[source] = new List<IFlowNode>();
                }
            }

            /// returns true if already added
            public bool CheckOrAdd(IFlowNode source, IFlowNode target)
            {
                var list = backing.GetOrAdd(source, new List<IFlowNode>());

                if (list.Contains(target)) {
                    return true;
                }
                list.Add(target);
                return false;
            }
        }

        public interface IFlowNode: IVirtualFlowNode
        {

            bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from, List<(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> assumeTrue);
            bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from, SkipItCache skipItCache);
        }

        public interface IVirtualFlowNode {

            IIsPossibly<VirtualNode> VirtualOutput();
            IIsPossibly<VirtualNode> VirtualInput();
            IEnumerable<KeyValuePair<IKey, VirtualNode>> VirtualMembers();
            HashSet<CombinedTypesAnd> ToRep();
        }

        //public class VirtualOrNode : IVirtualFlowNode
        //{
        //    private readonly HashSet<IVirtualFlowNode> backing;

        //    public VirtualOrNode(HashSet<IVirtualFlowNode> backing)
        //    {
        //        this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
        //    }

        //    public override bool Equals(object? obj)
        //    {
        //        return obj is VirtualOrNode node &&
        //               backing.SetEquals(node.backing);
        //    }

        //    public override int GetHashCode()
        //    {
        //        return backing.Sum(x => x.GetHashCode());
        //    }

        //    public IEnumerable<KeyValuePair<IKey, IVirtualFlowNode>> VirtualMembers()
        //    {
        //        return VirtualMembers(backing);
        //    }

        //    public static IEnumerable<KeyValuePair<IKey, IVirtualFlowNode>> VirtualMembers(HashSet<IVirtualFlowNode> backing)
        //    {
        //        return backing.SelectMany(x => x.VirtualMembers())
        //                .GroupBy(x => x.Key)
        //                .Where(x => x.Count() == backing.Count)
        //                .Select(x =>
        //                {


        //                    return new KeyValuePair<IKey, IVirtualFlowNode>(x.Key, new VirtualOrNode(x.Select(y => y.Value).ToHashSet()));
        //                });
        //    }

        //    public IIsPossibly<IVirtualFlowNode> VirtualInput()
        //    {
        //        return VirtualInput(backing);

        //    }

        //    public static IIsPossibly<IVirtualFlowNode> VirtualInput(HashSet<IVirtualFlowNode> backing)
        //    {
        //        var set = backing.Select(x => x.VirtualInput())
        //                .OfType<IIsDefinately<IVirtualFlowNode>>().Select(x => x.Value).ToList();


        //        if (set.Count == backing.Count)
        //        {
        //            return Possibly.Is(new VirtualOrNode(set.ToHashSet()));
        //        }
        //        return Possibly.IsNot<IVirtualFlowNode>();
        //    }

        //    public IIsPossibly<IVirtualFlowNode> VirtualOutput()
        //    {
        //        return VirtualOutput(backing);
        //    }

        //    public static IIsPossibly<IVirtualFlowNode> VirtualOutput(HashSet<IVirtualFlowNode> backing)
        //    {
        //        var set = backing.Select(x => x.VirtualOutput())
        //                .OfType<IIsDefinately<IVirtualFlowNode>>().Select(x => x.Value).ToList();

        //        if (set.Count == backing.Count)
        //        {
        //            return Possibly.Is(new VirtualOrNode(set.ToHashSet()));
        //        }
        //        return Possibly.IsNot<IVirtualFlowNode>();
        //    }
        //}

        //public class VirtualAndNode: IVirtualFlowNode
        //{
        //    private readonly HashSet<IVirtualFlowNode> backing;

        //    public VirtualAndNode(HashSet<IVirtualFlowNode> backing)
        //    {
        //        this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
        //    }

        //    public override bool Equals(object? obj)
        //    {
        //        return obj is VirtualAndNode node &&
        //               backing.SetEquals(node.backing);
        //    }

        //    public override int GetHashCode()
        //    {
        //        return backing.Sum(x=>x.GetHashCode());
        //    }

        //    public IEnumerable<KeyValuePair<IKey, IVirtualFlowNode>> VirtualMembers()
        //    {
        //        return VirtualMembers(backing);
        //    }

        //    public static IEnumerable<KeyValuePair<IKey, IVirtualFlowNode>> VirtualMembers(HashSet<IVirtualFlowNode> backing)
        //    {
        //        return backing.SelectMany(x => x.VirtualMembers())
        //                .GroupBy(x => x.Key)
        //                .Select(x =>
        //                {
        //                    if (x.Count() == 1)
        //                    {
        //                        return new KeyValuePair<IKey, IVirtualFlowNode>(x.Key, x.First().Value);
        //                    }

        //                    return new KeyValuePair<IKey, IVirtualFlowNode>(x.Key, new VirtualAndNode(x.Select(y => y.Value).ToHashSet()));
        //                });
        //    }

        //    public IIsPossibly<IVirtualFlowNode> VirtualInput()
        //    {
        //        return VirtualInput(backing);
        //    }

        //    public static IIsPossibly<IVirtualFlowNode> VirtualInput(HashSet<IVirtualFlowNode> backing)
        //    {
        //        var set = backing.Select(x => x.VirtualInput())
        //                .OfType<IIsDefinately<IVirtualFlowNode>>().Select(x => x.Value).ToList();

        //        if (set.Any())
        //        {
        //            return Possibly.IsNot<IVirtualFlowNode>();
        //        }
        //        if (set.Count == 1)
        //        {
        //            return Possibly.Is(set.First());
        //        }

        //        return Possibly.Is(new VirtualAndNode(set.ToHashSet()));
        //    }

        //    public IIsPossibly<IVirtualFlowNode> VirtualOutput()
        //    {
        //        return VirtualOutput(backing);
        //    }

        //    public static IIsPossibly<IVirtualFlowNode> VirtualOutput(HashSet<IVirtualFlowNode> backing)
        //    {
        //        var set = backing.Select(x => x.VirtualOutput())
        //                .OfType<IIsDefinately<IVirtualFlowNode>>().Select(x => x.Value).ToList();

        //        if (set.Any())
        //        {
        //            return Possibly.IsNot<IVirtualFlowNode>();
        //        }
        //        if (set.Count == 1)
        //        {
        //            return Possibly.Is(set.First());
        //        }

        //        return Possibly.Is(new VirtualAndNode(set.ToHashSet()));
        //    }
        //}

        public class PrimitiveFlowNode: IFlowNode<Tpn.TypeProblem2.Type>
        {
            public PrimitiveFlowNode(TypeProblem2.Type source, Guid guid)
            {
                Source = Possibly.Is(source ?? throw new ArgumentNullException(nameof(source)));
                Guid = guid;
            }

            public IIsPossibly<Tpn.TypeProblem2.Type> Source { get; }
            public Guid Guid { get; }

            public HashSet<CombinedTypesAnd> ToRep()
            {
                return new HashSet<CombinedTypesAnd>
                {
                    new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this) })
                };
            }

            public bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from, List<(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> assumeTrue) {
                var me = (from, ToOr(this));
                if (assumeTrue.Contains(me))
                {
                    return true;
                }
                assumeTrue.Add(me);

                return from.Is3(out var v3) && Equals(v3, this);
            }
            public bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from, SkipItCache skipItCache) {
                return false;
            }

            public IIsPossibly<IVirtualFlowNode> VirtualInput() 
            {
                return Possibly.IsNot<IVirtualFlowNode>();
            }

            public IIsPossibly<IVirtualFlowNode> VirtualOutput()
            {
                return Possibly.IsNot<IVirtualFlowNode>();
            }

            public IEnumerable<KeyValuePair<IKey, IVirtualFlowNode>> VirtualMembers() {
                return new Dictionary<IKey, IVirtualFlowNode>();
            }
        }

        public class ConcreteFlowNode<TSource>: ConcreteFlowNode, IFlowNode<TSource>
        {
            public ConcreteFlowNode(TSource source)
            {
                Source = Possibly.Is( source);
            }

            public IIsPossibly<TSource> Source { get; }
        }

        public abstract class ConcreteFlowNode : IFlowNode
        {

            public Dictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Members = new Dictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();

            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Input = Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Output = Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();

            public IEnumerable<KeyValuePair<IKey, IVirtualFlowNode>> VirtualMembers() => Members.Select(x=>new KeyValuePair<IKey, IVirtualFlowNode>(x.Key,x.Value.TransformInner(x => x.GetValueAs(out IVirtualFlowNode _))));
            public IIsPossibly<IVirtualFlowNode> VirtualInput() => Input.TransformInner(x=>x.GetValueAs(out IVirtualFlowNode _));
            public IIsPossibly<IVirtualFlowNode> VirtualOutput() => Output.TransformInner(x => x.GetValueAs(out IVirtualFlowNode _));

            public HashSet<CombinedTypesAnd> ToRep()
            {
                return new HashSet<CombinedTypesAnd>
                {
                    new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this) })
                };
            }


            public bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from, List<(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> assumeTrue) {
                var me = (from, ToOr(this));
                if (assumeTrue.Contains(me)) {
                    return true;
                }
                assumeTrue.Add(me);
                
                if (from.Is3(out var _))
                {
                    return false;
                }

                if (Members.Any() && from.GetValueAs(out IFlowNode _).VirtualInput().Is(out var _))
                {
                    return false;
                }

                if (Input.Is(out var _) && from.GetValueAs(out IFlowNode _).VirtualMembers().Any()) {
                    return false;
                }

                foreach (var fromMember in from.GetValueAs(out IFlowNode _).VirtualMembers())
                {
                    if (!CanFlowMember(fromMember, assumeTrue.ToList()))
                    {
                        return false;
                    }
                }

                if (Input.Is(out var input) && from.GetValueAs(out IFlowNode _).VirtualInput().Is(out var theirInput)) {
                    if (!input.GetValueAs(out IFlowNode _).CanFlow(theirInput, assumeTrue.ToList())) {
                        return false;
                    }
                }

                if (Output.Is(out var output) && from.GetValueAs(out IFlowNode _).VirtualOutput().Is(out var theirOutput))
                {
                    if (!output.GetValueAs(out IFlowNode _).CanFlow(theirOutput, assumeTrue.ToList()))
                    {
                        return false;
                    }
                }

                return true;
            }
            public bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from, SkipItCache skipItCache) {

                if (skipItCache.CheckOrAdd(from.GetValueAs(out IFlowNode _), this)) {
                    return false;
                }

                if (from.Is3(out var _))
                {
                    throw new Exception("actually don't flow");
                }

                var changes = false;
                foreach (var fromMember in from.GetValueAs(out IFlowNode _).VirtualMembers())
                {
                    changes |= FlowMember(fromMember, skipItCache);
                }

                if (Input.Is(out var input) && from.GetValueAs(out IFlowNode _).VirtualInput().Is(out var theirInput))
                {
                    changes |= input.GetValueAs(out IFlowNode _).Flow(theirInput, skipItCache);
                }

                if (Output.Is(out var output) && from.GetValueAs(out IFlowNode _).VirtualOutput().Is(out var theirOutput))
                {
                    changes |= output.GetValueAs(out IFlowNode _).Flow(theirOutput, skipItCache);
                }

                return changes;
            }

            private bool CanFlowMember( KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> fromMember, List<(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> assumeTrue)
            {
                if (this.Members.TryGetValue(fromMember.Key, out var toMember))
                {
                    return toMember.GetValueAs(out IFlowNode _).CanFlow(fromMember.Value, assumeTrue.ToList());
                }
                else
                {
                    return false;
                }
            }

            private bool FlowMember(KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> fromMember, SkipItCache skipItCache)
            {
                var changes = false;
                if (this.Members.TryGetValue(fromMember.Key, out var toMember))
                {
                    changes |= toMember.GetValueAs(out IFlowNode _).Flow(fromMember.Value, skipItCache);
                }
                return changes;
            }
        }

        public class OrFlowNode : IFlowNode<TypeProblem2.OrType>
        {

            public HashSet<CombinedTypesAnd> ToRep()
            {
                return this.Or.SelectMany(x => x.SwitchReturns(y => y.ToRep(), y => y.ToRep(), y => y.ToRep(), y => y.ToRep())).Distinct().ToHashSet();
            }


            public bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from, List<(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> assumeTrue)
            {
                var me = (from, ToOr(this));
                if (assumeTrue.Contains(me))
                {
                    return true;
                }
                assumeTrue.Add(me);

                if (from.Is3(out var _)) {
                    return false;
                }

                return Or.All(x => x.GetValueAs(out IFlowNode _).CanFlow(from, assumeTrue.ToList()));
            }
            public bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from, SkipItCache skipItCache)
            {
                if (skipItCache.CheckOrAdd(from.GetValueAs(out IFlowNode _), this))
                {
                    return false;
                }

                if (from.Is3(out var _))
                {
                    throw new Exception("actually don't flow");
                }

                var changes = false;
                foreach (var item in this.Or)
                {
                    changes |= item.GetValueAs(out IFlowNode _).Flow(from, skipItCache);
                }
                return changes;
            }
            public OrFlowNode(IReadOnlyList<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> or, IIsPossibly< TypeProblem2.OrType> source)
            {
                Or = or ?? throw new ArgumentNullException(nameof(or));
                Source = source;
            }

            public IReadOnlyList<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Or { get; }

            public IIsPossibly<TypeProblem2.OrType> Source {get;}

            // these should only be used for from
            public IEnumerable<KeyValuePair<IKey, IVirtualFlowNode>> VirtualMembers()
            {
                return VirtualOrNode.VirtualMembers(Or.Select(x=>x.GetValueAs(out IVirtualFlowNode _)).ToHashSet());
            }


            public IIsPossibly<IVirtualFlowNode> VirtualInput()
            {
                return VirtualOrNode.VirtualInput(Or.Select(x => x.GetValueAs(out IVirtualFlowNode _)).ToHashSet());
            }

            public IIsPossibly<IVirtualFlowNode> VirtualOutput()
            {
                return VirtualOrNode.VirtualOutput(Or.Select(x => x.GetValueAs(out IVirtualFlowNode _)).ToHashSet());
            }

        }

        public class InferredFlowNode : IFlowNode<TypeProblem2.InferredType>
        {
            public List<CombinedTypesAnd> Or = new List<CombinedTypesAnd>();

            public InferredFlowNode(IIsPossibly<TypeProblem2.InferredType> source)
            {
                Source = source ?? throw new ArgumentNullException(nameof(source));
            }

            public IIsPossibly<TypeProblem2.InferredType> Source
            {
                get;
            }

            public HashSet<CombinedTypesAnd> ToRep()
            {
                return this.Or.ToHashSet();
            }


            public bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from, List<(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> assumeTrue)
            {
                var me = (from, ToOr(this));
                if (assumeTrue.Contains(me))
                {
                    return true;
                }
                assumeTrue.Add(me);

                return from.SwitchReturns(
                    x => CanMerge(ToRep(this),ToRep(x)),
                    x => CanMerge(ToRep(this), ToRep(x)),
                    x => CanMerge(ToRep(this), ToRep(x)),
                    x => CanMerge(ToRep(this), ToRep(x)));
            }
            public bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from, SkipItCache skipItCache)
            {
                if (skipItCache.CheckOrAdd(from.GetValueAs(out IFlowNode _), this))
                {
                    return false;
                }

                var merged = from.SwitchReturns(
                    x => Union(ToRep(this), ToRep(x)),
                    x => Union(ToRep(this), ToRep(x)),
                    x => Union(ToRep(this), ToRep(x)),
                    x => Union(ToRep(this), ToRep(x)));

                if (!this.Or.SetEqual(merged)) {
                    this.Or = merged;
                    skipItCache.Clear(this);
                    return true;
                }

                return false;
            }

            // this is pretty much the same method as OrFlowNode has
            public IEnumerable<KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>> VirtualMembers()
            {
                var count = Or.Count();
                return Or.SelectMany(x => x.VirtualMembers())
                        .GroupBy(x => x.Key)
                        .Where(x=>x.Count() == count)
                        .Select(x =>
                        {
                            return new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(x.Key, IntersectReduce(x.Select(y => y.Value).ToList()));
                        });
            }

            // this is pretty much the same method as OrFlowNode has
            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualInput()
            {
                var count = Or.Count();
                var set = Or.Select(x => x.VirtualInput())
                        .OfType<IIsDefinately<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>>().Select(x => x.Value).ToList();

                if (set.Count < count)
                {
                    return Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
                }
                return Possibly.Is(IntersectReduce(set));
            }

            // this is pretty much the same method as OrFlowNode has
            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualOutput()
            {
                var count = Or.Count();
                var set = Or.Select(x => x.VirtualOutput())
                        .OfType<IIsDefinately<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>>().Select(x => x.Value).ToList();

                if (set.Count < count)
                {
                    return Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
                }
                return Possibly.Is(IntersectReduce(set));
            }

            private static List<CombinedTypesAnd> ToRep(InferredFlowNode from) {
                return from.Or.ToList();
            }

            private static List<CombinedTypesAnd> ToRep(ConcreteFlowNode from)
            {
                return new List<CombinedTypesAnd>
                {
                    new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(from) })
                };
            }

            private static List<CombinedTypesAnd> ToRep(PrimitiveFlowNode from)
            {
                return new List<CombinedTypesAnd>
                {
                    new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(from) })
                };
            }

            private static List<CombinedTypesAnd> ToRep(OrFlowNode from)
            {
                return from.Or.SelectMany(x => x.SwitchReturns(y => ToRep(y), y => ToRep(y), y => ToRep(y), y => ToRep(y))).Distinct().ToList();
            }

            private static List<CombinedTypesAnd> Union(IReadOnlyList<CombinedTypesAnd> left, List<CombinedTypesAnd> right) {
                var res = new List<CombinedTypesAnd>();
                foreach (var leftEntry in left)
                {
                    foreach (var rightEntry in right)
                    {
                        res.Add(Merge(leftEntry, rightEntry));
                    }
                }
                return res.Distinct().ToList();
            }

            private static CombinedTypesAnd Merge(CombinedTypesAnd left, CombinedTypesAnd right) {
                return new CombinedTypesAnd(left.And.Union(right.And).ToHashSet());
            }

            private static bool CanMerge(List<CombinedTypesAnd> left, List<CombinedTypesAnd> right) {
                return left.All(l => right.All(r => CanMerge(r,l)));
            }

            private static bool CanMerge(CombinedTypesAnd left, CombinedTypesAnd right ) {
                return left.And.All(l => right.And.All(r => CanMerge(r, l)));
            }

            private static bool CanMerge(IOrType<ConcreteFlowNode,PrimitiveFlowNode> left, IOrType<ConcreteFlowNode, PrimitiveFlowNode> right)
            {
                return left.SwitchReturns(
                    leftConcrete => right.SwitchReturns(
                        rightConcrete => {


                            if (leftConcrete.Input.Is(out var _) && rightConcrete.Members.Any()) {
                                return false;
                            }

                            if (leftConcrete.Members.Any() && rightConcrete.Input.Is(out var _))
                            {
                                return false;
                            }

                            foreach (var leftMember in leftConcrete.Members)
                            {
                                if (rightConcrete.Members.TryGetValue(leftMember.Key, out var rightMember)) {
                                    var leftMemberRep = leftMember.Value.SwitchReturns(z => ToRep(z), z => ToRep(z), z => ToRep(z), z => ToRep(z));
                                    var rightMemberRep = rightMember.SwitchReturns(z => ToRep(z), z => ToRep(z), z => ToRep(z), z => ToRep(z));
                                    if (!CanMerge(leftMemberRep, rightMemberRep)) {
                                        return false;
                                    }
                                }
                            }

                            if (leftConcrete.Input.Is(out var leftInput) && rightConcrete.Input.Is(out var rightInput))
                            {
                                var leftMemberRep = leftInput.SwitchReturns(z => ToRep(z), z => ToRep(z), z => ToRep(z), z => ToRep(z));
                                var rightMemberRep = rightInput.SwitchReturns(z => ToRep(z), z => ToRep(z), z => ToRep(z), z => ToRep(z));
                                if (!CanMerge(leftMemberRep, rightMemberRep))
                                {
                                    return false;
                                }
                            }

                            if (leftConcrete.Output.Is(out var leftOutput) && rightConcrete.Output.Is(out var rightOutput))
                            {
                                var leftMemberRep = leftOutput.SwitchReturns(z => ToRep(z), z => ToRep(z), z => ToRep(z), z => ToRep(z));
                                var rightMemberRep = rightOutput.SwitchReturns(z => ToRep(z), z => ToRep(z), z => ToRep(z), z => ToRep(z));
                                if (!CanMerge(leftMemberRep, rightMemberRep))
                                {
                                    return false;
                                }
                            }

                            return true;
                        },
                        rightPrimitve => {
                            return false;
                        }),
                    leftPrimitive => right.SwitchReturns(
                        rightConcrete => {
                            return false;
                        },
                        rightPrimitve => {
                            return leftPrimitive.Guid == rightPrimitve.Guid;
                        }));
            }

            private static IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> UnionReduce(List<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> list)
            {
                if (list.Count == 1)
                {
                    return list.First();
                }
                return OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(new InferredFlowNode(Possibly.IsNot<TypeProblem2.InferredType>())
                {
                    Or = list.Select(x => x.SwitchReturns(y => ToRep(y), y => ToRep(y), y => ToRep(y), y => ToRep(y))).Aggregate((a, b) => Union(a, b)).Distinct().ToList()
                });
            }

            private  static IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> IntersectReduce(List<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> list)
            {
                if (list.Count == 1)
                {
                    return list.First();
                }
                return OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(new OrFlowNode(list.Distinct().ToList(), Possibly.IsNot<TypeProblem2.OrType>()));
            }

            // TODO you are here!
            // the things in the AND should really really only be concrete types
            // everything else can be broken down in to concrete types
            // if they are not concrete types 
            // say one is an or type
            // then the virtual members really don't capture the whole picture

            // if these are all concrete types 
            // then we just need one compatable check 
            // it is between concrete types and other concrete types

            //public class CombinedTypesAnd
            //{
            //    internal IEnumerable<KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>> VirtualMembers()
            //    {
            //        var count = And.Count();
            //        return And.SelectMany(x => x.SwitchReturns(
            //             y => y.VirtualMembers(),
            //             y => y.VirtualMembers()))
            //                .GroupBy(x => x.Key)
            //                .Select(x =>
            //                {
            //                    return new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> (x.Key, UnionReduce(x.Select(y=>y.Value).ToList()));
            //                });
            //    }

            //    public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualInput()
            //    {
            //        var set = And.Select(x => x.SwitchReturns(
            //             y => y.Input,
            //             y => y.VirtualOutput())).OfType<IIsDefinately<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>>().Select(x => x.Value).ToList();

            //        if (!set.Any())
            //        {
            //            return Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
            //        }
            //        return Possibly.Is(UnionReduce(set));
            //    }

            //    public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualOutput()
            //    {
            //        var set = And.Select(x => x.SwitchReturns(
            //             y => y.Output,
            //             y => y.VirtualOutput())).OfType<IIsDefinately<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>>().Select(x => x.Value).ToList();
                    
            //        if (!set.Any())
            //        {
            //            return Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
            //        }
            //        return Possibly.Is(UnionReduce(set));
            //    }


            //    public override bool Equals(object? obj)
            //    {
            //        return obj is CombinedTypesAnd and &&
            //               And.SetEqual(and.And);
            //    }

            //    public override int GetHashCode()
            //    {
            //        return And.Select(x=>x.GetHashCode()).Sum();
            //    }

            //    // please don't add to this, it will change the HashCode
            //    public HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>> And { get; }

            //    public CombinedTypesAnd(HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>> and)
            //    {
            //        And = and ?? throw new ArgumentNullException(nameof(and));
            //    }

            //    internal CombinedTypesAnd AddAsNew(OrType<ConcreteFlowNode, PrimitiveFlowNode> orType)
            //    {
            //        var set = new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>();
            //        foreach (var item in And)
            //        {
            //            set.Add(item);
            //        }
            //        set.Add(orType);
            //        return new CombinedTypesAnd(set);
            //    }

            //}
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
