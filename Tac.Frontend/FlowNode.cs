using Prototypist.Toolbox;
using Prototypist.Toolbox.Dictionary;
using Prototypist.Toolbox.IEnumerable;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Dynamic;
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

        public interface IVirtualFlowNode
        {

            IIsPossibly<IOrType< VirtualNode,IError>> VirtualOutput();
            IIsPossibly<IOrType<VirtualNode, IError>> VirtualInput();
            IOrType<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError> VirtualMembers();
            IOrType<HashSet<CombinedTypesAnd>, IError> ToRep();
            IOrType<IIsPossibly<Guid>,IError> Primitive();
            // this is a bit of a stinker
            // CombinedAndNode does not have one 
            // but everyone else does
            IIsPossibly<SourcePath> SourcePath();
        }

        public interface IFlowNode: IVirtualFlowNode
        {

            bool Flow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing);
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

            public IOrType<IIsPossibly<Guid>, IError> Primitive() {
                return OrType.Make<IIsPossibly<Guid>, IError>( Possibly.Is(Guid));
            }

            public IOrType<HashSet<CombinedTypesAnd>, IError> ToRep()
            {
                return OrType.Make<HashSet<CombinedTypesAnd>, IError>(new HashSet<CombinedTypesAnd>
                {
                    new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this) })
                });
            }

            public bool Flow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing) {
                return false;
            }

            public IIsPossibly<IOrType<VirtualNode, IError>> VirtualInput() 
            {
                return Possibly.IsNot<IOrType<VirtualNode, IError>>();
            }

            public IIsPossibly<IOrType<VirtualNode, IError>> VirtualOutput()
            {
                return Possibly.IsNot<IOrType<VirtualNode, IError>>();
            }

            public IOrType<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError> VirtualMembers() {
                return OrType.Make<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError >( new Dictionary<IKey, IOrType<VirtualNode, IError>>());
            }

            public IIsPossibly< SourcePath> SourcePath()
            {
                return Possibly.Is( new SourcePath(OrType.Make<PrimitiveFlowNode, ConcreteFlowNode, OrFlowNode, InferredFlowNode>(this), new List<IOrType<Member, Input, Output>>()));
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

            public IOrType<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError> VirtualMembers()
            {
                return OrType.Make< IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError >(
                    Members.Select(x => new KeyValuePair<IKey, IOrType<VirtualNode, IError>>(x.Key, x.Value.GetValueAs(out IVirtualFlowNode _).ToRep().TransformInner(y => new VirtualNode(y, SourcePath().TransformInner(y => y.Member(x.Key)))))));
            }
            public IIsPossibly<IOrType<VirtualNode, IError>> VirtualInput()
            {
                return Input.TransformInner(x => x.GetValueAs(out IVirtualFlowNode _).ToRep().TransformInner(y => new VirtualNode(y, SourcePath().TransformInner(y => y.Input()))));
            }
            public IIsPossibly<IOrType<VirtualNode, IError>> VirtualOutput()
            {
                return Output.TransformInner(x => x.GetValueAs(out IVirtualFlowNode _).ToRep().TransformInner(y => new VirtualNode(y, SourcePath().TransformInner(y => y.Output()))));
            }


            public IOrType<IIsPossibly<Guid>, IError> Primitive()
            {
                return OrType.Make<IIsPossibly<Guid>, IError>(Possibly.IsNot<Guid>());
            }

            public IOrType<HashSet<CombinedTypesAnd>, IError> ToRep()
            {
                return OrType.Make<HashSet<CombinedTypesAnd>, IError>(new HashSet<CombinedTypesAnd>
                {
                    new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this) })
                });
            }


            public bool Flow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing) {

                var me = (from, ToOr(this));
                if (alreadyFlowing.Contains(me))
                {
                    return false;
                }
                alreadyFlowing.Add(me);


                if (from.Primitive().Is1(out var prim) && prim.Is(out var _))
                {
                    throw new Exception("actually don't flow");
                }

                var changes = false;
                if (from.VirtualMembers().Is1(out var members))
                {
                    foreach (var fromMember in members)
                    {
                        changes |= FlowMember(fromMember, alreadyFlowing);
                    }
                }

                if (Input.Is(out var input) && from.VirtualInput().Is(out var theirInputOr) && theirInputOr.Is1(out var theirInput))
                {
                    changes |= input.GetValueAs(out IFlowNode _).Flow(theirInput, alreadyFlowing.ToList());
                }

                if (Output.Is(out var output) && from.VirtualOutput().Is(out var theirOutputOr) && theirOutputOr.Is1(out var theirOutput))
                {
                    changes |= output.GetValueAs(out IFlowNode _).Flow(theirOutput, alreadyFlowing.ToList());
                }

                return changes;
            }

            private bool FlowMember(KeyValuePair<IKey, IOrType< VirtualNode,IError>> fromMember, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                if (fromMember.Value.Is2(out var _)) {
                    return false;
                }

                var changes = false;
                if (this.Members.TryGetValue(fromMember.Key, out var toMember))
                {
                    changes |= toMember.GetValueAs(out IFlowNode _).Flow(fromMember.Value.Is1OrThrow(), alreadyFlowing.ToList());
                }
                else {
                    // we end up here because we try to flow all the values in an or type
                    // below are two code snipits:

                    // in this one, A's "num" and "a" have to be numbers

                    // type A { num; a;}
                    // A a;
                    // a =: type {number num; number a} | type {number a; number b;} c


                    // in this one, A's "b" and "a" have to be numbers

                    // type A { b; a;}
                    // A a;
                    // a =: type {number num; number a} | type {number a; number b;} c

                    // for them both to work the or type has to flow all of it's member definitions
                }
                return changes;
            }

            public IIsPossibly<SourcePath> SourcePath()
            {
                return Possibly.Is(new SourcePath(OrType.Make<PrimitiveFlowNode, ConcreteFlowNode, OrFlowNode, InferredFlowNode>(this), new List<IOrType<Member, Input, Output>>()));
            }
        }

        public class OrFlowNode : IFlowNode<TypeProblem2.OrType>
        {

            public IOrType<HashSet<CombinedTypesAnd>, IError> ToRep()
            {
                var couldBeErrors = this.Or.Select(x => x.GetValueAs(out IVirtualFlowNode _).ToRep());

                var errors = couldBeErrors.SelectMany(x => {
                    if (x.Is2(out var error)) {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                if (errors.Any()) {
                    return OrType.Make<HashSet<CombinedTypesAnd>, IError>(Error.Cascaded("", errors));
                }

                return OrType.Make<HashSet<CombinedTypesAnd>, IError>(couldBeErrors.SelectMany(x => x.Is1OrThrow()).Distinct().ToHashSet());
            }

            public bool Flow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                var me = (from, ToOr(this));
                if (alreadyFlowing.Contains(me))
                {
                    return false;
                }
                alreadyFlowing.Add(me);

                if (from.Primitive().Is1(out var prim) && prim.Is(out var _))
                {
                    return false;
                    //throw new Exception("actually don't flow");
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

            public IOrType<IIsPossibly<Guid>, IError> Primitive()
            {
                return OrType.Make<IIsPossibly<Guid>, IError>(Possibly.IsNot<Guid>());
            }

            public IOrType<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError> VirtualMembers()
            {
                return this.ToRep().SwitchReturns(
                    x => new VirtualNode(x, SourcePath()).VirtualMembers(),
                    x => OrType.Make<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError>(x));
            }

            public IIsPossibly<IOrType<VirtualNode, IError>> VirtualInput()
            {
                return ToRep().SwitchReturns(x => new VirtualNode(x, SourcePath()).VirtualInput(), y => Possibly.Is<IOrType<VirtualNode, IError>>(OrType.Make<VirtualNode, IError>(y)));
            }

            public IIsPossibly<IOrType<VirtualNode, IError>> VirtualOutput()
            {
                return ToRep().SwitchReturns(x => new VirtualNode(x, SourcePath()).VirtualOutput(), y => Possibly.Is<IOrType<VirtualNode, IError>>(OrType.Make<VirtualNode, IError>(y)));
            }

            public IIsPossibly< SourcePath> SourcePath()
            {
                return Possibly.Is( new SourcePath(OrType.Make<PrimitiveFlowNode, ConcreteFlowNode, OrFlowNode, InferredFlowNode>(this), new List<IOrType<Member, Input, Output>>()));
            }
        }

        // what a mess 😭😭😭
        // if A | B flow in to empty
        // it will flow and you will get A | B
        // and it will flow again and you will get AA |AB |AB | BB aka A | AB | B

        // maybe I can simplify A | AB | B to A | B

        // the other option is tracking what flows in to a node
        // that is a little bit of work since inflows are often virtual
        // so maybe a path from a real node

        // can a node ever loose a member?
        // not right now

        // if we think of InferredFlowNode as a sum in flows (and calculate what they have at time of use) they they could
        // because a valid merge could become invald


        public class Input
        {
            public override bool Equals(object? obj)
            {
                return obj != null && obj is Input;
            }

            public override int GetHashCode()
            {
                return Guid.Parse("{41BEF862-F911-45AA-A7FA-BF53F455B6E5}").GetHashCode();
            }
        }
        public class Output
        {
            public override bool Equals(object? obj)
            {
                return obj != null && obj is Output;
            }

            public override int GetHashCode()
            {
                return Guid.Parse("{C3BA31B3-0779-4073-AB6F-E8965DD83F7A}").GetHashCode();
            }
        }
        public class Member
        {
            public readonly IKey key;

            public Member(IKey key)
            {
                this.key = key ?? throw new ArgumentNullException(nameof(key));
            }

            public override bool Equals(object? obj)
            {
                return obj != null && obj is Member member && member.key.Equals(this.key);
            }

            public override int GetHashCode()
            {
                return key.GetHashCode();
            }
        }

        public class SourcePath {
            public readonly IOrType<PrimitiveFlowNode, ConcreteFlowNode, OrFlowNode, InferredFlowNode> source;
            public readonly IReadOnlyList<IOrType<Member, Input, Output>> path;

            public SourcePath(IOrType<PrimitiveFlowNode, ConcreteFlowNode, OrFlowNode, InferredFlowNode> source, IReadOnlyList<IOrType<Member, Input, Output>> path)
            {
                this.source = source ?? throw new ArgumentNullException(nameof(source));
                this.path = path ?? throw new ArgumentNullException(nameof(path));
            }

            public override bool Equals(object? obj)
            {
                return obj is SourcePath inflow &&
                       source.Equals(inflow.source) &&
                       path.SequenceEqual(inflow.path);
            }

            public override int GetHashCode()
            {
                return source.GetHashCode() + path.Sum(x=>x.GetHashCode());
            }

            public SourcePath Member(IKey key) {
                var newList = path.ToList();
                newList.Add(OrType.Make<Member,Input, Output >(new Member(key)));
                return new SourcePath(source, newList);
            }

            public SourcePath Input()
            {
                var newList = path.ToList();
                newList.Add(OrType.Make<Member, Input, Output>(new Input()));
                return new SourcePath(source, newList);

            }

            public SourcePath Output()
            {
                var newList = path.ToList();
                newList.Add(OrType.Make<Member, Input, Output>(new Output()));
                return new SourcePath(source, newList);
            }

            public IOrType<IVirtualFlowNode, IError> Walk() {

               IVirtualFlowNode result = source.GetValueAs(out IVirtualFlowNode _);
                foreach (var pathElement in path)
                {
                    var couldBeError = pathElement.SwitchReturns(
                            x => result.VirtualMembers().SwitchReturns(inner=> inner.Where(y => y.Key.Equals(x.key)).Single().Value, error=> (IOrType<IVirtualFlowNode, IError>) OrType.Make< IVirtualFlowNode, IError >(error)),
                            x => result.VirtualInput().GetOrThrow(), 
                            x => result.VirtualOutput().GetOrThrow());
                    if (couldBeError.Is2(out var error))
                    {
                        return OrType.Make<IVirtualFlowNode, IError>(error);
                    }
                    else {
                        result = couldBeError.Is1OrThrow();
                    }
                }
                return OrType.Make<IVirtualFlowNode, IError>(result);
            }
        }

        public class InferredFlowNode : IFlowNode<TypeProblem2.InferredType>
        {

            public readonly HashSet<SourcePath> Sources = new HashSet<SourcePath>();


            public InferredFlowNode(IIsPossibly<TypeProblem2.InferredType> source)
            {
                Source = source ?? throw new ArgumentNullException(nameof(source));
            }

            public IIsPossibly<TypeProblem2.InferredType> Source
            {
                get;
            }

            public IOrType<HashSet<CombinedTypesAnd>,IError> ToRep()
            {
                var nodeOrError = Flatten(new List<InferredFlowNode> { this });

                var errors = nodeOrError.SelectMany(x => {
                    if (x.Is2(out var error)) {
                        return new IError[] { error };
                    }
                    return new IError[] { };

                }).ToArray();

                if (errors.Any()) {
                    return OrType.Make<HashSet<CombinedTypesAnd>, IError>(Error.Cascaded("", errors));
                }

                var setOrError = nodeOrError.Select(x => x.Is1OrThrow().ToRep()).ToArray();

                errors = setOrError.SelectMany(x => {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };

                }).ToArray();

                if (errors.Any())
                {
                    return OrType.Make<HashSet<CombinedTypesAnd>, IError>(Error.Cascaded("", errors));
                }

                var sets = setOrError.Select(x => x.Is1OrThrow()).ToArray();

                if (sets.Length == 0)
                {
                    return OrType.Make<HashSet<CombinedTypesAnd>, IError>(new HashSet<CombinedTypesAnd>());
                }

                if (sets.Length == 1)
                {
                    return OrType.Make<HashSet<CombinedTypesAnd>, IError>(sets.First());
                }

                var at = sets.First();
                foreach (var item in sets.Skip(1))
                {
                    var or = Union(at, item);
                    if (or.Is2(out var error)) {
                        return OrType.Make<HashSet<CombinedTypesAnd>, IError>(error);
                    }
                    at = or.Is1OrThrow();
                }
                return OrType.Make<HashSet<CombinedTypesAnd>, IError>(at);
            }

            // this song an dance is to avoid stack overflows
            // when you flow in to something and it flows in to you bad things can happen
            public HashSet<IOrType< IVirtualFlowNode,IError>> Flatten(List<InferredFlowNode> except) {
                return Sources.SelectMany(x =>
                {
                    var walked = x.Walk();
                    if (walked.Is1(out var virtualFlowNode) && virtualFlowNode is InferredFlowNode node)
                    {
                        if (except.Contains(node)) {
                            return new HashSet<IOrType<IVirtualFlowNode, IError>> { };
                        }
                        except.Add(node);
                        return node.Flatten(except);
                    }
                    return new HashSet<IOrType<IVirtualFlowNode, IError>> { walked };
                }).ToHashSet();
            }

            public bool Flow(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                var me = (from, ToOr(this));
                if (alreadyFlowing.Contains(me))
                {
                    return false;
                }
                alreadyFlowing.Add(me);
 
                var thing = from.SourcePath().GetOrThrow();

                // don't flow in to your self
                if (thing.Walk() == this) {
                    return false;
                }

                if (!Sources.Contains(thing)) {
                    Sources.Add(thing);
                    return true;
                }

                return false;
            }

            public IOrType<IIsPossibly<Guid>, IError> Primitive()
            {
                return ToRep().SwitchReturns(x => new VirtualNode(x, SourcePath()).Primitive(), y => OrType.Make<IIsPossibly<Guid>, IError>(y));
            }
            public IOrType<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>,IError> VirtualMembers()
            {
                return ToRep().SwitchReturns(x=> new VirtualNode(x, SourcePath()).VirtualMembers(), x=>OrType.Make<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError>(x)) ;
            }

            public IIsPossibly<IOrType<VirtualNode, IError>> VirtualInput()
            {
                return ToRep().SwitchReturns(x => new VirtualNode(x, SourcePath()).VirtualInput(), y => Possibly.Is<IOrType<VirtualNode, IError>>(OrType.Make<VirtualNode, IError>(y)));
            }

            public IIsPossibly<IOrType<VirtualNode, IError>> VirtualOutput()
            {
                return ToRep().SwitchReturns(x => new VirtualNode(x, SourcePath()).VirtualOutput(), y => Possibly.Is<IOrType<VirtualNode, IError>>(OrType.Make<VirtualNode, IError>(y)));
            }

            public IIsPossibly<SourcePath> SourcePath()
            {
                return Possibly.Is( new SourcePath(OrType.Make<PrimitiveFlowNode, ConcreteFlowNode, OrFlowNode, InferredFlowNode>(this), new List<IOrType<Member, Input, Output>>()));
            }

            internal static IOrType<HashSet<CombinedTypesAnd>, IError> Union(HashSet<CombinedTypesAnd> left, HashSet<CombinedTypesAnd> right) {
                var res = new List<CombinedTypesAnd>();
                foreach (var leftEntry in left)
                {
                    foreach (var rightEntry in right)
                    {
                        var canMergeResult = CanMerge(leftEntry, rightEntry, new List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)>(), new List<(CombinedTypesAnd, CombinedTypesAnd)>());
                        if (!canMergeResult.Any())
                        {
                            res.Add(Merge(leftEntry, rightEntry));
                        }
                    }
                }

                if (!res.Any()) {
                    return OrType.Make<HashSet<CombinedTypesAnd>, IError>(Error.Other("nothing could union!"));
                }

                return OrType.Make<HashSet<CombinedTypesAnd>, IError>(res.Distinct().ToHashSet());
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

            private static IError[] CanMerge(HashSet<CombinedTypesAnd> left, HashSet<CombinedTypesAnd> right, List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)> assumeTrue, List<(CombinedTypesAnd, CombinedTypesAnd)> assumeTrueInner) {
                var ours = (left, right);
                if (assumeTrue.Contains(ours)) {
                    return new IError[] { };
                }
                assumeTrue.Add(ours);

                foreach (var l in left)
                {
                    foreach (var r in right)
                    {
                        if (!CanMerge(r, l, assumeTrue, assumeTrueInner).Any())
                        {
                            return new IError[] { };
                        }
                    }
                }
                return new IError[] { Error.Other("No valid combinations") };

                //return left.Any(l => right.Any(r => CanMerge(r,l,assumeTrue,assumeTrueInner)));
            }

            private static IError[] CanMerge(CombinedTypesAnd left, CombinedTypesAnd right , List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)> assumeTrue, List<(CombinedTypesAnd, CombinedTypesAnd)> assumeTrueInner) {
                var ours = (left, right);
                if (assumeTrueInner.Contains(ours))
                {
                    return new IError[] { };
                }
                assumeTrueInner.Add(ours);

                return left.And.SelectMany(l => right.And.SelectMany(r => CanMerge(r, l, assumeTrue, assumeTrueInner))).ToArray();
            }


            // TODO
            // TODO compatiblity does not need to be deep!
            // 
            private static IError[] CanMerge(IOrType<ConcreteFlowNode,PrimitiveFlowNode> left, IOrType<ConcreteFlowNode, PrimitiveFlowNode> right, List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)> assumeTrue, List<(CombinedTypesAnd, CombinedTypesAnd)> assumeTrueInner)
            {
                return left.SwitchReturns(
                    leftConcrete => right.SwitchReturns(
                        rightConcrete => {


                            if (leftConcrete.Input.Is(out var _) && rightConcrete.Members.Any()) {
                                return new IError[] { Error.Other("can not merge something with members and something with I/O") };
                            }

                            if (leftConcrete.Members.Any() && rightConcrete.Input.Is(out var _))
                            {
                                return new IError[] { Error.Other("can not merge something with members and something with I/O") };
                            }

                            if (leftConcrete.Output.Is(out var _) && rightConcrete.Members.Any())
                            {
                                return new IError[] { Error.Other("can not merge something with members and something with I/O") };
                            }

                            if (leftConcrete.Members.Any() && rightConcrete.Output.Is(out var _))
                            {
                                return new IError[] { Error.Other("can not merge something with members and something with I/O") };
                            }

                            return new IError[] { };
                        },
                        rightPrimitve => {
                            if (leftConcrete.Members.Any() || leftConcrete.Input.Is(out var _) || leftConcrete.Output.Is(out var _))
                            {
                                return new IError[] { Error.Other("can not merge primitive and non-primitive") };
                            }
                            return new IError[] { };
                        }),
                    leftPrimitive => right.SwitchReturns(
                        rightConcrete => {
                            if (rightConcrete.Members.Any() || rightConcrete.Input.Is(out var _) || rightConcrete.Output.Is(out var _)) {

                                return new IError[] { Error.Other("can not merge primitive and non-primitive") };
                            }
                            return new IError[] { };
                        },
                        rightPrimitve => {
                            if (leftPrimitive.Guid != rightPrimitve.Guid)
                            {
                                return new IError[] { Error.Other("can not merge different primitives") };
                            }
                            return new IError[] { };
                        }));
            }
        }


        public class CombinedTypesAnd : IVirtualFlowNode
        {
            public IIsPossibly<IOrType<VirtualNode, IError>> VirtualOutput()
            {
                var errorCheck = And.Select(x => x.GetValueAs(out IVirtualFlowNode _).VirtualOutput())
                    .OfType<IIsDefinately<IOrType<VirtualNode, IError>>>().Select(x => x.Value).ToArray();

                var errors = errorCheck.SelectMany(x => {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                if (errors.Length == 1)
                {
                    return Possibly.Is(OrType.Make<VirtualNode, IError>(errors.First()));
                }
                if (errors.Length > 1)
                {
                    return Possibly.Is(OrType.Make<VirtualNode, IError>(Error.Cascaded("", errors)));
                }

                var errorCheck2 = errorCheck.Select(x => x.Is1OrThrow().ToRep()).ToArray();

                errors = errorCheck2.SelectMany(x => {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                if (errors.Length == 1)
                {
                    return Possibly.Is(OrType.Make<VirtualNode, IError>(errors.First()));
                }
                if (errors.Length > 1)
                {
                    return Possibly.Is(OrType.Make<VirtualNode, IError>(Error.Cascaded("", errors)));
                }

                var set = errorCheck2.Select(x => x.Is1OrThrow()).ToArray();

                if (!set.Any())
                {
                    return Possibly.IsNot<IOrType<VirtualNode, IError>>();
                }

                if (set.Length == 1)
                {
                    return Possibly.Is(OrType.Make<VirtualNode, IError>(new VirtualNode(set.First(), Possibly.IsNot<SourcePath>())));
                }
                return Possibly.Is(VirtualNode.IsAll(set).TransformInner(y=>new VirtualNode(y, Possibly.IsNot<SourcePath>())));
            }

            public IIsPossibly<IOrType<VirtualNode, IError>> VirtualInput()
            {
                var errorCheck = And.Select(x => x.GetValueAs(out IVirtualFlowNode _).VirtualInput())
                    .OfType<IIsDefinately<IOrType<VirtualNode, IError>>>().Select(x => x.Value).ToArray();

                var errors = errorCheck.SelectMany(x => {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                if (errors.Length == 1)
                {
                    return Possibly.Is(OrType.Make<VirtualNode, IError>(errors.First()));
                }
                if (errors.Length > 1)
                {
                    return Possibly.Is(OrType.Make<VirtualNode, IError>(Error.Cascaded("", errors)));
                }

                var errorCheck2 = errorCheck.Select(x => x.Is1OrThrow().ToRep()).ToArray();

                errors = errorCheck2.SelectMany(x => {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                if (errors.Length == 1)
                {
                    return Possibly.Is(OrType.Make<VirtualNode, IError>(errors.First()));
                }
                if (errors.Length > 1)
                {
                    return Possibly.Is(OrType.Make<VirtualNode, IError>(Error.Cascaded("", errors)));
                }

                var set = errorCheck2.Select(x => x.Is1OrThrow()).ToArray();

                if (!set.Any()) {
                    return Possibly.IsNot<IOrType<VirtualNode, IError>>();
                }

                if (set.Length == 1) {
                    return Possibly.Is(OrType.Make<VirtualNode, IError>(new VirtualNode(set.First(), Possibly.IsNot<SourcePath>())));
                }
                return Possibly.Is(VirtualNode.IsAll(set).TransformInner(y=>new VirtualNode(y, Possibly.IsNot<SourcePath>())));
            }

            public IOrType<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError> VirtualMembers()
            {
                var couldBeErrors = And.Select(x => x.GetValueAs(out IVirtualFlowNode _).VirtualMembers()).ToArray();

                var error = couldBeErrors.SelectMany(x => {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                if (error.Any())
                {
                    return OrType.Make<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError>(Error.Cascaded("", error));
                }

                var set = couldBeErrors.SelectMany(x => x.Is1OrThrow()).ToArray();


                return OrType.Make<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError >( set
                        .GroupBy(x => x.Key)
                        .Select(x => {

                            var errors = x.SelectMany(y => {
                                if (y.Value.Is2(out var error))
                                {
                                    return new IError[] { error };
                                }
                                return new IError[] { };
                            }).ToArray();

                            if (errors.Length == 1)
                            {
                                return new KeyValuePair<IKey, IOrType<VirtualNode, IError>>(x.Key, OrType.Make<VirtualNode, IError>(errors.First()));
                            }
                            if (errors.Length > 1)
                            {
                                return new KeyValuePair<IKey, IOrType<VirtualNode, IError>>(x.Key, OrType.Make<VirtualNode, IError>(Error.Cascaded("", errors)));
                            }

                            var errorCheck = x.Select(y => y.Value.Is1OrThrow().ToRep()).ToArray();

                            errors = errorCheck.SelectMany(y => {
                                if (y.Is2(out var error))
                                {
                                    return new IError[] { error };
                                }
                                return new IError[] { };
                            }).ToArray();

                            if (errors.Length == 1)
                            {
                                return new KeyValuePair<IKey, IOrType<VirtualNode, IError>>(x.Key, OrType.Make<VirtualNode, IError>(errors.First()));
                            }
                            if (errors.Length > 1)
                            {
                                return new KeyValuePair<IKey, IOrType<VirtualNode, IError>>(x.Key, OrType.Make<VirtualNode, IError>(Error.Cascaded("", errors)));
                            }
                            var set = errorCheck.Select(y => y.Is1OrThrow()).ToArray();

                            return new KeyValuePair<IKey, IOrType<VirtualNode, IError>>(x.Key, VirtualNode.IsAll(set).TransformInner(y=>new VirtualNode(y, Possibly.IsNot<SourcePath>())));
                            
                            })
                        .ToArray());
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

            public IReadOnlyCollection<IOrType<ConcreteFlowNode, PrimitiveFlowNode>> And { get; }

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

            public IOrType<IIsPossibly<Guid>, IError> Primitive()
            {
                if (And.Count == 1)
                {
                    return And.First().GetValueAs(out IFlowNode _).Primitive();
                }
                return OrType.Make<IIsPossibly<Guid>, IError>(Possibly.IsNot<Guid>());
            }

            public IOrType< HashSet<CombinedTypesAnd>,IError> ToRep()
            {
                return OrType.Make<HashSet<CombinedTypesAnd>, IError> (new HashSet<CombinedTypesAnd> { this });
            }

            public IIsPossibly<SourcePath> SourcePath()
            {
                // we don't have one
                return Possibly.IsNot<SourcePath>();
            }
        }


        public class VirtualNode : IVirtualFlowNode
        {

            public HashSet<CombinedTypesAnd> Or = new HashSet<CombinedTypesAnd>();


            public static IOrType<HashSet<CombinedTypesAnd>, IError> IsAll(IEnumerable<HashSet<CombinedTypesAnd>> toMerge)
            {
                if (toMerge.Count() == 0) {
                    throw new Exception("well that is unexpected!");
                }

                if (toMerge.Count() == 1) {
                    return OrType.Make<HashSet<CombinedTypesAnd>, IError>(toMerge.First());
                }

                var at = toMerge.First();
                foreach (var item in toMerge.Skip(1))
                {
                    var or = InferredFlowNode.Union(at, item);
                    if (or.Is2(out var error))
                    {
                        return OrType.Make<HashSet<CombinedTypesAnd>, IError>(error);
                    }
                    at = or.Is1OrThrow();
                }
                return OrType.Make<HashSet<CombinedTypesAnd>, IError>(at);

            }

            public static HashSet<CombinedTypesAnd> IsAny(IEnumerable<HashSet<CombinedTypesAnd>> toMerge)
            {
                if (toMerge.Count() == 0)
                {
                    throw new Exception("well that is unexpected!");
                }

                if (toMerge.Count() == 1)
                {
                    return toMerge.First();
                }

                return toMerge.SelectMany(x => x).Distinct().ToHashSet();
            }

            public VirtualNode(HashSet<CombinedTypesAnd> or, IIsPossibly<SourcePath> sourcePath)
            {
                Or = or ?? throw new ArgumentNullException(nameof(or));
                this.sourcePath = sourcePath ?? throw new ArgumentNullException(nameof(sourcePath));
            }

            //private static HashSet<CombinedTypesAnd> Union(HashSet<CombinedTypesAnd> left, HashSet<CombinedTypesAnd> right)
            //{
            //    var res = new List<CombinedTypesAnd>();
            //    foreach (var leftEntry in left)
            //    {
            //        foreach (var rightEntry in right)
            //        {
            //            res.Add(Merge(leftEntry, rightEntry));
            //        }
            //    }
            //    return res.Distinct().ToHashSet();
            //}

            //private static CombinedTypesAnd Merge(CombinedTypesAnd left, CombinedTypesAnd right)
            //{
            //    return new CombinedTypesAnd(left.And.Union(right.And).ToHashSet());
            //}

            public override bool Equals(object? obj)
            {
                return obj is VirtualNode node &&
                       Or.SetEquals(node.Or);
            }

            public override int GetHashCode()
            {
                return Or.Sum(x => x.GetHashCode());
            }


            public IOrType<IIsPossibly<Guid>, IError> Primitive()
            {
                var first = Or.FirstOrDefault();
                if (first != null && Or.Count == 1)
                {
                    return first.Primitive();
                }
                return OrType.Make<IIsPossibly<Guid>, IError>(Possibly.IsNot<Guid>());
            }

            public IOrType<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError> VirtualMembers()
            {
                var couldBeErrors = Or.Select(x => x.VirtualMembers()).ToArray();

                var error = couldBeErrors.SelectMany(x => {
                    if (x.Is2(out var error)) {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                if (error.Any()) {
                    return OrType.Make<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError>(Error.Cascaded("",error));
                }

                var set = couldBeErrors.SelectMany(x => x.Is1OrThrow()).ToArray();

                return OrType.Make< IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError > (
                        set.GroupBy(x => x.Key)
                        .Select(x =>
                        {
                            var errors = x.SelectMany(y => {
                                if (y.Value.Is2(out var error))
                                {
                                    return new IError[] { error };
                                }
                                return new IError[] { };
                            }).ToArray();

                            // we only actually error our if everything is invalid
                            if (x.Count() == errors.Length)
                            {

                                if (errors.Length == 1)
                                {
                                    return new KeyValuePair<IKey, IOrType<VirtualNode, IError>>(x.Key, OrType.Make<VirtualNode, IError>(errors.First()));
                                }
                                if (errors.Length > 1)
                                {
                                    return new KeyValuePair<IKey, IOrType<VirtualNode, IError>>(x.Key, OrType.Make<VirtualNode, IError>(Error.Cascaded("", errors)));
                                }
                            }

                            var notError = x.Select(y => y.Value.Is1OrThrow()).ToArray();

                            if (notError.Count() == 1)
                            {
                                return new KeyValuePair<IKey, IOrType<VirtualNode, IError>>(x.Key, OrType.Make<VirtualNode, IError>(new VirtualNode(notError.First().Or, sourcePath.TransformInner(y => y.Member(x.Key)))));
                            }
                            return new KeyValuePair<IKey, IOrType<VirtualNode, IError>>(x.Key, OrType.Make<VirtualNode, IError>(new VirtualNode(IsAny(notError.Select(y => y.Or)), sourcePath.TransformInner(y => y.Member(x.Key)))));
                        }));
            }

            public IIsPossibly<IOrType<VirtualNode, IError>> VirtualInput()
            {
                var errorCheck = Or.Select(x => x.VirtualInput()).OfType<IIsDefinately<IOrType<VirtualNode, IError>>>().Select(x=>x.Value).ToArray();

                var errors = errorCheck.SelectMany(x => {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                // we only actually error our if everything is invalid
                if (errorCheck.Length == errors.Length)
                {

                    if (errors.Length == 1)
                    {
                        return Possibly.Is(OrType.Make<VirtualNode, IError>(errors.First()));
                    }
                    if (errors.Length > 1)
                    {
                        return Possibly.Is(OrType.Make<VirtualNode, IError>(Error.Cascaded("", errors)));
                    }
                }

                var set = errorCheck.Select(x => x.Is1OrThrow()).ToArray();

                if (!set.Any())
                {
                    return Possibly.IsNot<IOrType<VirtualNode, IError>>();
                }
                if (set.Length == 1)
                {
                    return Possibly.Is(OrType.Make<VirtualNode, IError>(new VirtualNode(set.First().Or, sourcePath.TransformInner(x => x.Input()))));
                }

                return Possibly.Is(OrType.Make<VirtualNode, IError>(new VirtualNode(IsAny(set.Select(x => x.Or)), sourcePath.TransformInner(x => x.Input()))));
            }

            public IIsPossibly<IOrType<VirtualNode, IError>> VirtualOutput()
            {
                var errorCheck = Or.Select(x => x.VirtualOutput()).OfType<IIsDefinately<IOrType<VirtualNode, IError>>>().Select(x => x.Value).ToArray();


                var errors = errorCheck.SelectMany(x => {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                // we only actually error our if everything is invalid
                if (errorCheck.Length == errors.Length)
                {

                    if (errors.Length == 1)
                    {
                        return Possibly.Is(OrType.Make<VirtualNode, IError>(errors.First()));
                    }
                    if (errors.Length > 1)
                    {
                        return Possibly.Is(OrType.Make<VirtualNode, IError>(Error.Cascaded("", errors)));
                    }
                }

                var set = errorCheck.Select(x => x.Is1OrThrow()).ToArray();

                if (!set.Any())
                {
                    return Possibly.IsNot<IOrType<VirtualNode, IError>>();
                }
                if (set.Length == 1)
                {
                    return Possibly.Is(OrType.Make < VirtualNode, IError >( new VirtualNode(set.First().Or, sourcePath.TransformInner(x => x.Output()))));
                }

                return Possibly.Is(OrType.Make<VirtualNode, IError>(new VirtualNode(IsAny(set.Select(x => x.Or)), sourcePath.TransformInner(x => x.Output()))));
            }

            public IOrType< HashSet<CombinedTypesAnd>,IError> ToRep() => OrType.Make<HashSet<CombinedTypesAnd>, IError>( Or);

            private readonly IIsPossibly<SourcePath> sourcePath;

            public IIsPossibly<SourcePath> SourcePath() {
                return sourcePath;
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
