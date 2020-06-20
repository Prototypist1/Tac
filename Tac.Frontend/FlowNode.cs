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

        public class CombinedTypesAnd: IVirtualFlowNode
        {
            public IEnumerable<KeyValuePair<IKey, VirtualNode>> VirtualMembers()
            {
                return VirtualMembers(And);
            }

            public static IEnumerable<KeyValuePair<IKey, VirtualNode>> VirtualMembers(HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>> backing)
            {
                return backing.SelectMany(x => x.GetValueAs(out IVirtualFlowNode _).VirtualMembers())
                        .GroupBy(x => x.Key)
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


                if (!set.Any()) {

                    return Possibly.IsNot<VirtualNode>();
                }

                if (set.Count == 1)
                {
                    return Possibly.Is(set.First());
                }

                return Possibly.Is(VirtualNode.IsAll(set.ToHashSet()));
                
            }

            public IIsPossibly<VirtualNode> VirtualOutput()
            {
                return VirtualOutput(And);
            }

            public static IIsPossibly<VirtualNode> VirtualOutput(HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>> backing)
            {
                var set = backing.Select(x => x.GetValueAs(out IVirtualFlowNode _).VirtualOutput())
                        .OfType<IIsDefinately<VirtualNode>>().Select(x => x.Value).ToList();

                if (!set.Any())
                {
                    return Possibly.IsNot<VirtualNode>();
                }

                if (set.Count == 1)
                {
                    return Possibly.Is(set.First());
                }

                return Possibly.Is(VirtualNode.IsAll(set.ToHashSet()));
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

            public IIsPossibly<Guid> Primitive()
            {
                if (And.Count == 1) {
                    return And.First().GetValueAs(out IFlowNode _).Primitive();
                }
                return Possibly.IsNot<Guid>();
            }

            public HashSet<CombinedTypesAnd> ToRep()
            {
                return new HashSet<CombinedTypesAnd> { this };
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


            public IIsPossibly<Guid> Primitive()
            {
                var first = Or.FirstOrDefault();
                if (first != null && Or.Count == 1) {
                    return first.Primitive();
                }
                return Possibly.IsNot<Guid>();
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


        public interface IFlowNode<TSource> : IFlowNode
        {
            IIsPossibly<TSource> Source { get; }
        }


        // I am not really sure this works 😖 
        // I mean, you don't have to have to change to change
        // if one of your constituents changes you also change
        //public class SkipItCache {

        //    private readonly Dictionary<IVirtualFlowNode, List<IFlowNode>> backing = new Dictionary<IVirtualFlowNode, List<IFlowNode>>();
        //    public void Clear(IFlowNode source) {
        //        if (backing.ContainsKey(source)) {
        //            backing[source] = new List<IFlowNode>();
        //        }
        //    }

        //    /// returns true if already added
        //    public bool CheckOrAdd(IVirtualFlowNode source, IFlowNode target)
        //    {
        //        var list = backing.GetOrAdd(source, new List<IFlowNode>());

        //        if (list.Contains(target)) {
        //            return true;
        //        }
        //        list.Add(target);
        //        return false;
        //    }
        //}

        public interface IFlowNode: IVirtualFlowNode
        {

            bool CanFlow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> assumeTrue);
            // flow is a bit inperfect
            // alreadyFlowing prevents something A flowing in to B if A is already flowing in to B
            // if A is changed somewhere down the stack from where it is flowing in to B
            // that might not be caputered in the top level flow
            // you would think if A.Flow(B) returns true
            // then calling A.Flow(B) again would return false
            // but you can't count on that
            bool Flow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing);
        }

        public interface IVirtualFlowNode {

            IIsPossibly<VirtualNode> VirtualOutput();
            IIsPossibly<VirtualNode> VirtualInput();
            IEnumerable<KeyValuePair<IKey, VirtualNode>> VirtualMembers();
            HashSet<CombinedTypesAnd> ToRep();
            IIsPossibly<Guid> Primitive();
        }

        public class PrimitiveFlowNode: IFlowNode<Tpn.TypeProblem2.Type>
        {
            public PrimitiveFlowNode(TypeProblem2.Type source, Guid guid)
            {
                Source = Possibly.Is(source ?? throw new ArgumentNullException(nameof(source)));
                Guid = guid;
            }

            public IIsPossibly<Tpn.TypeProblem2.Type> Source { get; }
            public Guid Guid { get; }

            public IIsPossibly<Guid> Primitive() {
                return Possibly.Is(Guid);
            }

            public HashSet<CombinedTypesAnd> ToRep()
            {
                return new HashSet<CombinedTypesAnd>
                {
                    new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this) })
                };
            }

            public bool CanFlow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> assumeTrue) {
                var me = (from, ToOr(this));
                if (assumeTrue.Contains(me))
                {
                    return true;
                }
                assumeTrue.Add(me);

                return from.Primitive().Is(out var guid) && guid == this.Guid;
            }
            public bool Flow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing) {
                return false;
            }

            public IIsPossibly<VirtualNode> VirtualInput() 
            {
                return Possibly.IsNot<VirtualNode>();
            }

            public IIsPossibly<VirtualNode> VirtualOutput()
            {
                return Possibly.IsNot<VirtualNode>();
            }

            public IEnumerable<KeyValuePair<IKey, VirtualNode>> VirtualMembers() {
                return new Dictionary<IKey, VirtualNode>();
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

            public IEnumerable<KeyValuePair<IKey, VirtualNode>> VirtualMembers() => Members.Select(x=>new KeyValuePair<IKey, VirtualNode>(x.Key, new VirtualNode(x.Value.GetValueAs(out IVirtualFlowNode _).ToRep())));
            public IIsPossibly<VirtualNode> VirtualInput() => Input.TransformInner(x=>new VirtualNode( x.GetValueAs(out IVirtualFlowNode _).ToRep()));
            public IIsPossibly<VirtualNode> VirtualOutput() => Output.TransformInner(x => new VirtualNode(x.GetValueAs(out IVirtualFlowNode _).ToRep()));


            public IIsPossibly<Guid> Primitive()
            {
                return Possibly.IsNot<Guid>();
            }

            public HashSet<CombinedTypesAnd> ToRep()
            {
                return new HashSet<CombinedTypesAnd>
                {
                    new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this) })
                };
            }


            public bool CanFlow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> assumeTrue) {
                var me = (from, ToOr(this));
                if (assumeTrue.Contains(me)) {
                    return true;
                }
                assumeTrue.Add(me);
                
                if (from.Primitive().Is(out var _))
                {
                    return false;
                }

                if (Members.Any() && from.VirtualInput().Is(out var _))
                {
                    return false;
                }

                if (Input.Is(out var _) && from.VirtualMembers().Any()) {
                    return false;
                }

                foreach (var fromMember in from.VirtualMembers())
                {
                    if (!CanFlowMember(fromMember, assumeTrue.ToList()))
                    {
                        return false;
                    }
                }

                if (Input.Is(out var input) && from.VirtualInput().Is(out var theirInput)) {
                    if (!input.GetValueAs(out IFlowNode _).CanFlow(theirInput, assumeTrue.ToList())) {
                        return false;
                    }
                }

                if (Output.Is(out var output) && from.VirtualOutput().Is(out var theirOutput))
                {
                    if (!output.GetValueAs(out IFlowNode _).CanFlow(theirOutput, assumeTrue.ToList()))
                    {
                        return false;
                    }
                }

                return true;
            }
            public bool Flow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing) {

                var me = (from, ToOr(this));
                if (alreadyFlowing.Contains(me))
                {
                    return false;
                }
                alreadyFlowing.Add(me);


                if (from.Primitive().Is(out var _))
                {
                    throw new Exception("actually don't flow");
                }

                var changes = false;
                foreach (var fromMember in from.VirtualMembers())
                {
                    changes |= FlowMember(fromMember, alreadyFlowing);
                }

                if (Input.Is(out var input) && from.VirtualInput().Is(out var theirInput))
                {
                    changes |= input.GetValueAs(out IFlowNode _).Flow(theirInput, alreadyFlowing.ToList());
                }

                if (Output.Is(out var output) && from.VirtualOutput().Is(out var theirOutput))
                {
                    changes |= output.GetValueAs(out IFlowNode _).Flow(theirOutput, alreadyFlowing.ToList());
                }

                return changes;
            }

            private bool CanFlowMember( KeyValuePair<IKey, VirtualNode> fromMember, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> assumeTrue)
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

            private bool FlowMember(KeyValuePair<IKey, VirtualNode> fromMember, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                var changes = false;
                if (this.Members.TryGetValue(fromMember.Key, out var toMember))
                {
                    changes |= toMember.GetValueAs(out IFlowNode _).Flow(fromMember.Value, alreadyFlowing.ToList());
                }
                return changes;
            }
        }

        public class OrFlowNode : IFlowNode<TypeProblem2.OrType>
        {

            public HashSet<CombinedTypesAnd> ToRep()
            {
                return this.Or.SelectMany(x => x.GetValueAs(out IVirtualFlowNode _).ToRep()).Distinct().ToHashSet();
            }

            public IIsPossibly<Guid> Primitive()
            {
                return Possibly.IsNot<Guid>();
            }

            public bool CanFlow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> assumeTrue)
            {
                var me = (from, ToOr(this));
                if (assumeTrue.Contains(me))
                {
                    return true;
                }
                assumeTrue.Add(me);

                if (from.Primitive().Is(out var _)) {
                    return false;
                }

                return Or.All(x => x.GetValueAs(out IFlowNode _).CanFlow(from, assumeTrue.ToList()));
            }
            public bool Flow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                var me = (from, ToOr(this));
                if (alreadyFlowing.Contains(me))
                {
                    return false;
                }
                alreadyFlowing.Add(me);

                if (from.Primitive().Is(out var _))
                {
                    throw new Exception("actually don't flow");
                }

                var changes = false;
                foreach (var item in this.Or)
                {
                    changes |= item.GetValueAs(out IFlowNode _).Flow(from, alreadyFlowing.ToList());
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

            public IEnumerable<KeyValuePair<IKey, VirtualNode>> VirtualMembers()
            {
                return new VirtualNode(this.ToRep()).VirtualMembers();
            }

            public IIsPossibly<VirtualNode> VirtualInput()
            {
                return new VirtualNode(this.ToRep()).VirtualInput();
            }

            public IIsPossibly<VirtualNode> VirtualOutput()
            {
                return new VirtualNode(this.ToRep()).VirtualOutput();
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

            public IIsPossibly<Guid> Primitive()
            {
                return new VirtualNode(Or.ToHashSet()).Primitive();
            }
            public bool CanFlow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> assumeTrue)
            {
                var me = (from, ToOr(this));
                if (assumeTrue.Contains(me))
                {
                    return true;
                }
                assumeTrue.Add(me);

                return CanMerge(this.ToRep(), from.ToRep(), new List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)>(), new List<(CombinedTypesAnd, CombinedTypesAnd)>());
            }
            public bool Flow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                var me = (from, ToOr(this));
                if (alreadyFlowing.Contains(me))
                {
                    return false;
                }
                alreadyFlowing.Add(me);

                var merged = Union(this.ToRep(), from.ToRep());

                if (!this.Or.SetEqual(merged)) {
                    this.Or = merged;
                    return true;
                }

                return false;
            }

            public IEnumerable<KeyValuePair<IKey, VirtualNode>> VirtualMembers()
            {
                return new VirtualNode(Or.ToHashSet()).VirtualMembers();
            }

            public IIsPossibly<VirtualNode> VirtualInput()
            {
                return new VirtualNode(Or.ToHashSet()).VirtualInput();
            }

            public IIsPossibly<VirtualNode> VirtualOutput()
            {
                return new VirtualNode(Or.ToHashSet()).VirtualOutput();
            }

            //private static List<CombinedTypesAnd> ToRep(InferredFlowNode from) {
            //    return from.Or.ToList();
            //}

            //private static List<CombinedTypesAnd> ToRep(ConcreteFlowNode from)
            //{
            //    return new List<CombinedTypesAnd>
            //    {
            //        new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(from) })
            //    };
            //}

            //private static List<CombinedTypesAnd> ToRep(PrimitiveFlowNode from)
            //{
            //    return new List<CombinedTypesAnd>
            //    {
            //        new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(from) })
            //    };
            //}

            //private static List<CombinedTypesAnd> ToRep(OrFlowNode from)
            //{
            //    return from.Or.SelectMany(x => x.SwitchReturns(y => ToRep(y), y => ToRep(y), y => ToRep(y), y => ToRep(y))).Distinct().ToList();
            //}

            private static List<CombinedTypesAnd> Union(HashSet<CombinedTypesAnd> left, HashSet<CombinedTypesAnd> right) {
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
                var start = left.And.Union(right.And).Distinct().ToArray();

                // remove empties
                var v2 = start.Where(x => x.SwitchReturns(y => y.Input.Is(out var _) || y.Output.Is(out var _) || y.Members.Any(), y => true)).ToList();
                // but if you end up removing them all, put one back
                if (!v2.Any()) {
                    v2.Add(start.First());
                }
                // empties are kind of a weird thing 
                // why do I try to keep it to one?
                // why do I want to make sure I have one?
                // weird 

                return new CombinedTypesAnd(v2.ToHashSet());
            }

            private static bool CanMerge(HashSet<CombinedTypesAnd> left, HashSet<CombinedTypesAnd> right, List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)> assumeTrue, List<(CombinedTypesAnd, CombinedTypesAnd)> assumeTrueInner) {
                var ours = (left, right);
                if (assumeTrue.Contains(ours)) {
                    return true;
                }
                assumeTrue.Add(ours);


                return left.All(l => right.All(r => CanMerge(r,l,assumeTrue,assumeTrueInner)));
            }

            private static bool CanMerge(CombinedTypesAnd left, CombinedTypesAnd right , List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)> assumeTrue, List<(CombinedTypesAnd, CombinedTypesAnd)> assumeTrueInner) {
                var ours = (left, right);
                if (assumeTrueInner.Contains(ours))
                {
                    return true;
                }
                assumeTrueInner.Add(ours);

                return left.And.All(l => right.And.All(r => CanMerge(r, l, assumeTrue, assumeTrueInner)));
            }

            private static bool CanMerge(IOrType<ConcreteFlowNode,PrimitiveFlowNode> left, IOrType<ConcreteFlowNode, PrimitiveFlowNode> right, List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)> assumeTrue, List<(CombinedTypesAnd, CombinedTypesAnd)> assumeTrueInner)
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
                                    var leftMemberRep = leftMember.Value.GetValueAs(out IVirtualFlowNode _).ToRep();
                                    var rightMemberRep = rightMember.GetValueAs(out IVirtualFlowNode _).ToRep();
                                    if (!CanMerge(leftMemberRep, rightMemberRep, assumeTrue, assumeTrueInner)) {
                                        return false;
                                    }
                                }
                            }

                            if (leftConcrete.Input.Is(out var leftInput) && rightConcrete.Input.Is(out var rightInput))
                            {
                                var leftMemberRep = leftInput.GetValueAs(out IVirtualFlowNode _).ToRep();
                                var rightMemberRep = rightInput.GetValueAs(out IVirtualFlowNode _).ToRep();
                                if (!CanMerge(leftMemberRep, rightMemberRep, assumeTrue, assumeTrueInner))
                                {
                                    return false;
                                }
                            }

                            if (leftConcrete.Output.Is(out var leftOutput) && rightConcrete.Output.Is(out var rightOutput))
                            {
                                var leftMemberRep = leftOutput.GetValueAs(out IVirtualFlowNode _).ToRep();
                                var rightMemberRep = rightOutput.GetValueAs(out IVirtualFlowNode _).ToRep();
                                if (!CanMerge(leftMemberRep, rightMemberRep, assumeTrue, assumeTrueInner))
                                {
                                    return false;
                                }
                            }

                            return true;
                        },
                        rightPrimitve => {
                            return !leftConcrete.Members.Any() && !leftConcrete.Input.Is(out var _) && !leftConcrete.Output.Is(out var _);
                        }),
                    leftPrimitive => right.SwitchReturns(
                        rightConcrete => {
                            return !rightConcrete.Members.Any() && !rightConcrete.Input.Is(out var _) && !rightConcrete.Output.Is(out var _);
                        },
                        rightPrimitve => {
                            return leftPrimitive.Guid == rightPrimitve.Guid;
                        }));
            }
        }

        internal class Uhh { } 

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
