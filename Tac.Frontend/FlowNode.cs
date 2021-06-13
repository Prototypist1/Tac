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

    internal class EqualibleHashSet<T>: IEnumerable<T>
    {
        public readonly IReadOnlySet<T> backing;

        public EqualibleHashSet(IReadOnlySet<T> backing)
        {
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        public override bool Equals(object? obj)
        {
            return obj is EqualibleHashSet<T> set &&
                set.backing.SetEquals(backing);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return backing.GetEnumerator();
        }

        public override int GetHashCode()
        {
            return backing.Sum(x => x.GetHashCode());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string? ToString()
        {
            return $"EqualibleHashSet<{typeof(T).Name}>({string.Join(", ", backing.Take(10).Select(x => x.ToString())) + ((backing.Count > 10)? "..." : "")})";
        }

    }
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

            IIsPossibly<IOrType<VirtualNode, IError>> VirtualOutput();
            IIsPossibly<IOrType<VirtualNode, IError>> VirtualInput();
            IOrType<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError> VirtualMembers();

            //... I probably need to remove this...
            //... you can use Walk with an empty ienum
            IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> ToRep();
            IOrType<IIsPossibly<Guid>, IError> Primitive();
            // this is a bit of a stinker
            // CombinedAndNode does not have one 
            // but everyone else does

            /// <summary>
            /// Source Path is used as a way to express identity
            /// virtual nodes are defined in terms of their replationship to a "real" node
            /// </summary>
            IIsPossibly<SourcePath> SourcePath();

            IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> Walk(IEnumerable<IOrType<Member, Input, Output>> path);
        }


        public interface IFlowNode : IVirtualFlowNode
        {
            bool MustAccept(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing);
            bool MustReturn(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing);
        }

        public static class IVirtualFlowNodeExtensions {
            public static IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> ToRep(IVirtualFlowNode self) {
                return self.Walk(new IOrType<Member, Input, Output>[] { }).SwitchReturns(
                        x => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(x),
                        x => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(x),
                        _ => throw new Exception("if you don't have a path, you have to exist, without a path you are ToRep of this"));
            }
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

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> ToRep()
            {
                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd>
                {
                    new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this) })
                }));
            }

            public bool MustAccept(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing) {
                return false;
            }
            public bool MustReturn(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
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

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> Walk(IEnumerable<IOrType<Member, Input, Output>> path)
            {
                if (!path.Any()) {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd>
                    {
                        new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this) })
                    }));
                }

                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
            }

            public override string? ToString()
            {
                return $"{nameof(PrimitiveFlowNode)}({Source})";
            }
        }

        public class ConcreteFlowNode<TSource>: ConcreteFlowNode, IFlowNode<TSource>
        {
            public ConcreteFlowNode(TSource source)
            {
                Source = Possibly.Is( source);
            }

            public IIsPossibly<TSource> Source { get; }

            public override string? ToString()
            {
                return $"{nameof(ConcreteFlowNode)}<{typeof(TSource).Name}>({Source})";
            }
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

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> ToRep()
            {
                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd>
                {
                    new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this) })
                }));
            }


            public bool MustAccept(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing) {

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
                        changes |= MustAcceptMember(fromMember, alreadyFlowing);
                    }
                }

                if (Input.Is(out var input) && from.VirtualInput().Is(out var theirInputOr) && theirInputOr.Is1(out var theirInput))
                {
                    // for a seond I thought this would be mustReturn but..
                    // method [{a,b,c},empty] =: x
                    // method [{a,b},empty] =: x
                    // method [{a},empty] =: x
                    // x better be metohd[{a},empty]
                    changes |= input.GetValueAs(out IFlowNode _).MustAccept(theirInput, alreadyFlowing.ToList());
                }

                if (Output.Is(out var output) && from.VirtualOutput().Is(out var theirOutputOr) && theirOutputOr.Is1(out var theirOutput))
                {
                    // for a seond I thought this would be mustReturn but..
                    // method [empty,{a,b,c}] =: x
                    // method [empty,{a,b}] =: x
                    // method [empty,{a}] =: x
                    // x better be metohd[empty,{a}]
                    changes |= output.GetValueAs(out IFlowNode _).MustAccept(theirOutput, alreadyFlowing.ToList());
                }

                return changes;
            }

            public bool MustReturn(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
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
                        changes |= MustReturnMember(fromMember, alreadyFlowing);
                    }
                }

                if (Input.Is(out var input) && from.VirtualInput().Is(out var theirInputOr) && theirInputOr.Is1(out var theirInput))
                {
                    changes |= input.GetValueAs(out IFlowNode _).MustReturn(theirInput, alreadyFlowing.ToList());
                }

                if (Output.Is(out var output) && from.VirtualOutput().Is(out var theirOutputOr) && theirOutputOr.Is1(out var theirOutput))
                {
                    changes |= output.GetValueAs(out IFlowNode _).MustReturn(theirOutput, alreadyFlowing.ToList());
                }

                return changes;
            }

            private bool MustAcceptMember(KeyValuePair<IKey, IOrType< VirtualNode,IError>> fromMember, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                if (fromMember.Value.Is2(out var _)) {
                    return false;
                }

                var changes = false;
                if (this.Members.TryGetValue(fromMember.Key, out var toMember))
                {
                    changes |= toMember.GetValueAs(out IFlowNode _).MustAccept(fromMember.Value.Is1OrThrow(), alreadyFlowing.ToList());
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

            private bool MustReturnMember(KeyValuePair<IKey, IOrType<VirtualNode, IError>> fromMember, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                if (fromMember.Value.Is2(out var _))
                {
                    return false;
                }

                var changes = false;
                if (this.Members.TryGetValue(fromMember.Key, out var toMember))
                {
                    changes |= toMember.GetValueAs(out IFlowNode _).MustReturn(fromMember.Value.Is1OrThrow(), alreadyFlowing.ToList());
                }
                else
                {

                }
                return changes;
            }


            public IIsPossibly<SourcePath> SourcePath()
            {
                return Possibly.Is(new SourcePath(OrType.Make<PrimitiveFlowNode, ConcreteFlowNode, OrFlowNode, InferredFlowNode>(this), new List<IOrType<Member, Input, Output>>()));
            }

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> Walk(IEnumerable<IOrType<Member, Input, Output>> path)
            {
                if (!path.Any())
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd>
                    {
                        new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this) })
                    }));
                }

                return path.First().SwitchReturns(
                    member => {
                        if (Members.TryGetValue(member.key, out var value)) {
                            return value.GetValueAs(out IVirtualFlowNode _).Walk(path.Skip(1));
                        }
                        return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
                    },
                    input => {
                        if (Input.Is(out var value))
                        {
                            return value.GetValueAs(out IVirtualFlowNode _).Walk(path.Skip(1));
                        }
                        return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
                    },
                    output => {
                        if (Output.Is(out var value))
                        {
                            return value.GetValueAs(out IVirtualFlowNode _).Walk(path.Skip(1));
                        }
                        return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
                    });
            }

        }

        public class OrFlowNode : IFlowNode<TypeProblem2.OrType>
        {

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> ToRep()
            {
                var couldBeErrors = this.Or.Select(x => x.GetValueAs(out IVirtualFlowNode _).ToRep());

                var errors = couldBeErrors.SelectMany(x => {
                    if (x.Is2(out var error)) {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                if (errors.Any()) {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Cascaded("", errors));
                }

                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(new EqualibleHashSet<CombinedTypesAnd>(couldBeErrors.SelectMany(x => x.Is1OrThrow().backing).Distinct().ToHashSet()));
            }


            public bool MustAccept(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
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
                    changes |= item.GetValueAs(out IFlowNode _).MustAccept(from, alreadyFlowing.ToList());
                }
                return changes;
            }

            public bool MustReturn(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
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
                    changes |= item.GetValueAs(out IFlowNode _).MustReturn(from, alreadyFlowing.ToList());
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

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> Walk(IEnumerable<IOrType<Member, Input, Output>> path)
            {
                if (!path.Any())
                {
                    return ToRep().SwitchReturns(x=> OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(x), x => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(x));
                }

                var couldBeErrors = this.Or.Select(x => x.GetValueAs(out IVirtualFlowNode _).Walk(path));

                if (couldBeErrors.Any(x => x.Is3(out var _))) {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
                }

                var errors = couldBeErrors.SelectMany(x => {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                if (errors.Any())
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(Error.Cascaded("", errors));
                }

                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new EqualibleHashSet<CombinedTypesAnd>(couldBeErrors.SelectMany(x => x.Is1OrThrow().backing).Distinct().ToHashSet()));
            }

            public override string? ToString()
            {
                return $"{nameof(OrFlowNode)}({Source})";
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

                var result = source.GetValueAs(out IVirtualFlowNode _);
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

            public override string? ToString()
            {
                return $"SourcePath({source}, {string.Join(", ", path.Take(10).Select(x=>x.ToString())) +(path.Count > 10? "...": "" )})";
            }
        }

        public class InferredFlowNode : IFlowNode<TypeProblem2.InferredType>
        {

            public readonly HashSet<SourcePath> AcceptedSources = new HashSet<SourcePath>();
            public readonly HashSet<SourcePath> ReturnedSources = new HashSet<SourcePath>();


            public InferredFlowNode(IIsPossibly<TypeProblem2.InferredType> source)
            {
                Source = source ?? throw new ArgumentNullException(nameof(source));
            }

            public IIsPossibly<TypeProblem2.InferredType> Source
            {
                get;
            }

            public IOrType<EqualibleHashSet<CombinedTypesAnd>,IError> ToRep()
            {
                return Walk(new IOrType<Member, Input, Output>[] { }).SwitchReturns(
                    x=> OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(x),
                    x => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(x),
                    _ => throw new Exception("if you don't have a path, you have to exist, without a path you are ToRep of this"));
            }

            private IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> ToRepReturns(IEnumerable<IOrType<Member, Input, Output>> pathParts)
            {
                var nodeOrError = FlattenReturn(new HashSet<SourcePath> { }, pathParts);
                var errors = nodeOrError.SelectMany(x =>
                {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };

                }).ToArray();

                if (errors.Any())
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(Error.Cascaded("", errors));
                }

                if (nodeOrError.All(x => x.Is3(out var _))) { 
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
                }

                var setOrError = nodeOrError.Where(x=> x.Is1(out var _)).Select(x => x.Is1OrThrow().ToRep()).ToArray();

                errors = setOrError.SelectMany(x =>
                {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };

                }).ToArray();

                if (errors.Any())
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(Error.Cascaded("", errors));
                }

                var sets = setOrError.Select(x => x.Is1OrThrow()).ToArray();

                if (sets.Length == 0)
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd>()));
                }

                if (sets.Length == 1)
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(sets.First());
                }

                var at = sets.First();
                foreach (var item in sets.Skip(1))
                {
                    var or = Union(at, item);
                    if (or.Is2(out var error))
                    {
                        return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(error);
                    }
                    at = or.Is1OrThrow();
                }
                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(at);
            }

            private IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> ToRepAccepts(IEnumerable<IOrType<Member, Input, Output>> pathParts)
            {
                var nodeOrError = FlattenAccepts(new HashSet<SourcePath> { }, pathParts);

                var errors = nodeOrError.SelectMany(x =>
                {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };

                }).ToArray();

                if (errors.Any())
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(Error.Cascaded("", errors));
                }

                if (nodeOrError.Any(x => x.Is3(out var _)))
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
                }

                var setOrError = nodeOrError.Select(x => x.Is1OrThrow().ToRep()).ToArray();

                errors = setOrError.SelectMany(x =>
                {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };

                }).ToArray();

                if (errors.Any())
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(Error.Cascaded("", errors));
                }

                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new EqualibleHashSet<CombinedTypesAnd>(setOrError.SelectMany(x => x.Is1OrThrow()).Distinct().ToHashSet()));
            }

            public HashSet<IOrType<IVirtualFlowNode, IError, DoesNotExist>> FlattenReturn(HashSet<SourcePath> except, IEnumerable<IOrType<Member, Input, Output>> pathParts)
            {
                return Flatten(except, pathParts, false);
            }

            public HashSet<IOrType<IVirtualFlowNode, IError, DoesNotExist>> FlattenAccepts(HashSet<SourcePath> except, IEnumerable<IOrType<Member, Input, Output>> pathParts)
            {
                return Flatten(except, pathParts, true);
            }

            // I think "or nothing" might be a good idea
            // I think maybe Flatten should just flatten and we can walk after 

            // this "except" song and dance is to avoid stack overflows
            // when you flow in to something and it flows in to you bad things can happen
            public HashSet<IOrType<IVirtualFlowNode, IError, DoesNotExist>> Flatten(HashSet<SourcePath> except, IEnumerable<IOrType<Member, Input, Output>> pathParts, bool accepted)
            {
                var res = new HashSet<IOrType<IVirtualFlowNode, IError, DoesNotExist>>();

                foreach (var item in (accepted ? AcceptedSources : ReturnedSources))
                {
                    var fullPath = new List<IOrType<Member, Input, Output>>();
                    fullPath.AddRange(item.path);
                    fullPath.AddRange(pathParts);
                    var effectiveSourceNode = new SourcePath(item.source, fullPath);

                    if (except.Contains(effectiveSourceNode)) {
                        continue;
                    }

                    except.Add(effectiveSourceNode);

                    var walked = item.source.SwitchReturns(
                                        primitive => new[] { primitive.Walk(fullPath).SwitchReturns(
                                            x => OrType.Make<IVirtualFlowNode, IError, DoesNotExist>( new VirtualNode(x, Possibly.Is( effectiveSourceNode))),
                                            x => OrType.Make <IVirtualFlowNode, IError, DoesNotExist > (x) ,
                                            x => OrType.Make < IVirtualFlowNode, IError, DoesNotExist > (x)) }, 
                                        concrete => new[] { concrete.Walk(fullPath).SwitchReturns(
                                            x=> OrType.Make<IVirtualFlowNode, IError, DoesNotExist>( new VirtualNode(x, Possibly.Is( effectiveSourceNode))),
                                            x => OrType.Make <IVirtualFlowNode, IError, DoesNotExist > (x) ,
                                            x => OrType.Make < IVirtualFlowNode, IError, DoesNotExist > (x)) }, 
                                        or => new[] { or.Walk(fullPath).SwitchReturns(
                                            x=> OrType.Make<IVirtualFlowNode, IError, DoesNotExist>( new VirtualNode(x, Possibly.Is( effectiveSourceNode))),
                                            x => OrType.Make <IVirtualFlowNode, IError, DoesNotExist > (x) ,
                                            x => OrType.Make < IVirtualFlowNode, IError, DoesNotExist > (x)) },
                                        inferred => inferred.Flatten(except.ToHashSet(), fullPath, accepted).ToArray()); 

                    foreach (var walkedItem in walked)
                    {
                        res.Add(walkedItem);
                    }
                }

                return res; 


                //return (accepted ? AcceptedSources : ReturnedSources).SelectMany(x =>
                //{
                //    //var fullPath = new List<IOrType<Member, Input, Output>>();
                //    //fullPath.AddRange(x.path);
                //    //fullPath.AddRange(pathParts);

                //    //return x.source.GetValueAs(out IVirtualFlowNode _).Walk(fullPath);

                //    //var walked = 

                //    var walked = x.Walk();

                //    //if (walked.Is1(out var virtualFlowNode)) { 

                //    //}

                //    //walked = WalkPath(pathParts, walked);

                //    if (walked.Is1(out var virtualFlowNode) && virtualFlowNode.SafeIs(out InferredFlowNode node))
                //    {
                //        if (except.Contains(node))
                //        {
                //            return new HashSet<IOrType<IVirtualFlowNode, IError, DoesNotExist>> { };
                //        }
                //        except.Add(node);
                //        return node.Flatten(except, new IOrType<Member, Input, Output>[] { }, accepted);
                //    }

                //    return new HashSet<IOrType<IVirtualFlowNode, IError, DoesNotExist>> {
                //        walked.SwitchReturns(
                //            x => OrType.Make<IVirtualFlowNode, IError, DoesNotExist>(x),
                //            x => OrType.Make<IVirtualFlowNode, IError, DoesNotExist>(x),
                //            x => OrType.Make<IVirtualFlowNode, IError, DoesNotExist>(x)) };
                //}).ToHashSet();
            }


            // Maybe extend IOrType<Member,Input,Output> to give it a name
            // and then extend it with this swtich? 
            //                        pathPart.SwitchReturns(
            //x => virtualFlowNode.VirtualMembers().SwitchReturns(inner => inner.Where(y => y.Key.Equals(x.key)).Single().Value, error => (IOrType<IVirtualFlowNode, IError>) OrType.Make<IVirtualFlowNode, IError>(error)),
            //                x => virtualFlowNode.VirtualInput().GetOrThrow(),
            //                x => virtualFlowNode.VirtualOutput().GetOrThrow());
            // the switch is also used in Walk()
            //private static IOrType<IVirtualFlowNode, IError> WalkPath(IEnumerable<IOrType<Member, Input, Output>> pathParts, IOrType<IVirtualFlowNode, IError> walked)
            //{
            //    var enumerator = pathParts.GetEnumerator();
            //    while (walked.Is1(out var virtualFlowNode) && enumerator.MoveNext())
            //    {
            //        walked = enumerator.Current.SwitchReturns(
            //            x => virtualFlowNode.VirtualMembers().SwitchReturns(inner => inner.Where(y => y.Key.Equals(x.key)).Single().Value, error => (IOrType<IVirtualFlowNode, IError>)OrType.Make<IVirtualFlowNode, IError>(error)),
            //            x => virtualFlowNode.VirtualInput().GetOrThrow(),
            //            x => virtualFlowNode.VirtualOutput().GetOrThrow());
            //    }

            //    return walked;
            //}

            public bool MustAccept(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                // consider:
                //
                // 5 =: c
                // c =: number | string b
                // c should be number | string
                //
                // if we accept primitives
                // we would say this must accept number
                // and must return number | string 
                // so it is a number
                // but that's not right 
                //
                // I think primitive are always driven from must return because they don't have any member or inputs
                if (from.SafeIs(out PrimitiveFlowNode _)) 
                {
                    return false;
                }

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

                if (!AcceptedSources.Contains(thing)) {
                    AcceptedSources.Add(thing);
                    return true;
                }

                return false;
            }

            public bool MustReturn(IVirtualFlowNode from, List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                var me = (from, ToOr(this));
                if (alreadyFlowing.Contains(me))
                {
                    return false;
                }
                alreadyFlowing.Add(me);

                var thing = from.SourcePath().GetOrThrow();

                // don't flow in to your self
                if (thing.Walk() == this)
                {
                    return false;
                }

                if (!ReturnedSources.Contains(thing))
                {
                    ReturnedSources.Add(thing);
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

            internal static IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> Union(EqualibleHashSet<CombinedTypesAnd> left, EqualibleHashSet<CombinedTypesAnd> right) {
                var res = new List<CombinedTypesAnd>();
                foreach (var leftEntry in left.backing)
                {
                    foreach (var rightEntry in right.backing)
                    {
                        var canMergeResult = CanUnion(leftEntry, rightEntry, new List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)>(), new List<(CombinedTypesAnd, CombinedTypesAnd)>());
                        if (!canMergeResult.Any())
                        {
                            res.Add(Union(leftEntry, rightEntry));
                        }
                    }
                }

                if (!res.Any()) {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Other("nothing could union!"));
                }

                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(new EqualibleHashSet<CombinedTypesAnd>(res.Distinct().ToHashSet()));
            }

            private static CombinedTypesAnd Union(CombinedTypesAnd left, CombinedTypesAnd right) {
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

            //private static IError[] CanMerge(HashSet<CombinedTypesAnd> left, HashSet<CombinedTypesAnd> right, List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)> assumeTrue, List<(CombinedTypesAnd, CombinedTypesAnd)> assumeTrueInner) {
            //    var ours = (left, right);
            //    if (assumeTrue.Contains(ours)) {
            //        return new IError[] { };
            //    }
            //    assumeTrue.Add(ours);

            //    foreach (var l in left)
            //    {
            //        foreach (var r in right)
            //        {
            //            if (!CanUnion(r, l, assumeTrue, assumeTrueInner).Any())
            //            {
            //                return new IError[] { };
            //            }
            //        }
            //    }
            //    return new IError[] { Error.Other("No valid combinations") };

            //    //return left.Any(l => right.Any(r => CanMerge(r,l,assumeTrue,assumeTrueInner)));
            //}

            private static IError[] CanUnion(CombinedTypesAnd left, CombinedTypesAnd right , List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)> assumeTrue, List<(CombinedTypesAnd, CombinedTypesAnd)> assumeTrueInner) {
                var ours = (left, right);
                if (assumeTrueInner.Contains(ours))
                {
                    return new IError[] { };
                }
                assumeTrueInner.Add(ours);

                return left.And.SelectMany(l => right.And.SelectMany(r => CanUnion(r, l, assumeTrue, assumeTrueInner))).ToArray();
            }


            // TODO
            // TODO compatiblity does not need to be deep!
            // 
            private static IError[] CanUnion(IOrType<ConcreteFlowNode,PrimitiveFlowNode> left, IOrType<ConcreteFlowNode, PrimitiveFlowNode> right, List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)> assumeTrue, List<(CombinedTypesAnd, CombinedTypesAnd)> assumeTrueInner)
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

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> Walk(IEnumerable<IOrType<Member, Input, Output>> pathParts)
            {
                var returns = ToRepReturns(pathParts);
                var accepts = ToRepAccepts(pathParts);

                return returns.SwitchReturns(
                    returnsAnds => accepts.SwitchReturns(
                        acceptsAnds => {

                            {
                                // when there are no constraints on what to return
                                // the constrains on what we accept arn't interstesting
                                // we're an any
                                var returnsAsVirtualNode = new VirtualNode(returnsAnds, Possibly.IsNot<SourcePath>());
                                if (returnsAsVirtualNode.Primitive().Is1(out var possiblyGuid) && possiblyGuid.IsNot() &&
                                    returnsAsVirtualNode.VirtualMembers().Is1(out var members) && !members.Any() &&
                                    returnsAsVirtualNode.VirtualInput().IsNot() &&
                                    returnsAsVirtualNode.VirtualOutput().IsNot())
                                {
                                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(returnsAnds);
                                }
                            }

                            {
                                // if there are no contraints on what we accept
                                // we just follow the constraints on what we return 
                                var acceptsAsVirtualNode = new VirtualNode(acceptsAnds, Possibly.IsNot<SourcePath>());
                                if (acceptsAsVirtualNode.Primitive().Is1(out var possiblyGuid) && possiblyGuid.IsNot() &&
                                    acceptsAsVirtualNode.VirtualMembers().Is1(out var members) && !members.Any() &&
                                    acceptsAsVirtualNode.VirtualInput().IsNot() &&
                                    acceptsAsVirtualNode.VirtualOutput().IsNot())
                                {
                                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(returnsAnds);
                                }
                            }

                            // must accept A | B
                            // must return C | D
                            // A&C | A&D | B&C | B&D

                            return InferredFlowNode.Union( returnsAnds , acceptsAnds ).SwitchReturns(
                                x=> OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(x), 
                                x => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(x));
                        },
                        acceptsError => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(acceptsError),
                        acceptsDoesNotExist => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(returnsAnds)),
                    returnsError => accepts.SwitchReturns(
                        acceptsAnds => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(returnsError),
                        acceptsError => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(Error.Cascaded("", new[] { returnsError, acceptsError })),
                        acceptsDoesNotExist => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(returnsError)),
                    returnDoesNotExist => accepts.SwitchReturns(
                        acceptsAnds => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd>())),// if there are no constraints on the return side it's basically an any
                        acceptsError => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(acceptsError),
                        acceptsDoesNotExist => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist())));
            }

            public override string? ToString()
            {
                return $"{nameof(InferredFlowNode)}({Source})";
            }

        }

        
        public class CombinedTypesAnd : IVirtualFlowNode
        {
            // we have an output if any of our elements have an output  
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

            public IOrType<EqualibleHashSet<CombinedTypesAnd>,IError> ToRep()
            {
                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError> (new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd> { this }));
            }

            public IIsPossibly<SourcePath> SourcePath()
            {
                // we don't have one
                return Possibly.IsNot<SourcePath>();
            }


            // this shares a lot of code with VirtualOutput and it's friends 
            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> Walk(IEnumerable<IOrType<Member, Input, Output>> path)
            {
                if (!path.Any())
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd> { this }));
                }

                var errorCheck =  And.Select(element => element.GetValueAs(out IVirtualFlowNode _).Walk(path)).ToArray();

                var errors = errorCheck.SelectMany(x => {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                if (errors.Length == 1)
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(errors.First());
                }
                if (errors.Length > 1)
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(Error.Cascaded("", errors));
                }

                var set =  errorCheck.SelectMany(x => {
                    if (x.Is1(out var value)) {
                        return new[] { value };
                    }
                    return new EqualibleHashSet<CombinedTypesAnd>[] { };

                }).ToArray();

                if (!set.Any()) {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
                }

                //var errorCheck2 = set.Select(x => x.ToRep()).ToArray();

                //errors = errorCheck2.SelectMany(x => {
                //    if (x.Is2(out var error))
                //    {
                //        return new IError[] { error };
                //    }
                //    return new IError[] { };
                //}).ToArray();

                //if (errors.Length == 1)
                //{
                //    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(errors.First());
                //}
                //if (errors.Length > 1)
                //{
                //    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(Error.Cascaded("", errors));
                //}

                // ((a&b) | (c&d) | (e&f)) & ((g&h) | (i&j)) => (a&b&g&h) | (c&d&g&h) | (e&f&g&h) | (a&b&i&j) | (c&d&i&j) | (e&f&i&j)
                return VirtualNode.IsAll(set).SwitchReturns(
                    x => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(x), 
                    x => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(x));
            }

            public override string? ToString()
            {
                return $"{nameof(CombinedTypesAnd)}({string.Join(", ", And.Take(10).Select(x=>x.ToString())) + (And.Count > 10 ? "..." : "")})";
            }
        }


        public class VirtualNode : IVirtualFlowNode
        {

            public EqualibleHashSet<CombinedTypesAnd> Or;

            public static IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> IsAll(IEnumerable<EqualibleHashSet<CombinedTypesAnd>> toMerge)
            {
                if (toMerge.Count() == 0) {
                    throw new Exception("well that is unexpected!");
                }

                if (toMerge.Count() == 1) {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(toMerge.First());
                }

                var at = toMerge.First();
                foreach (var item in toMerge.Skip(1))
                {
                    var or = InferredFlowNode.Union(at, item);
                    if (or.Is2(out var error))
                    {
                        return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(error);
                    }
                    at = or.Is1OrThrow();
                }
                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(at);

            }

            public static EqualibleHashSet<CombinedTypesAnd> IsAny(IEnumerable<EqualibleHashSet<CombinedTypesAnd>> toMerge)
            {
                if (toMerge.Count() == 0)
                {
                    throw new Exception("well that is unexpected!");
                }

                if (toMerge.Count() == 1)
                {
                    return toMerge.First();
                }

                return new EqualibleHashSet<CombinedTypesAnd>(toMerge.SelectMany(x => x.backing).Distinct().ToHashSet());
            }

            public VirtualNode(EqualibleHashSet<CombinedTypesAnd> or, IIsPossibly<SourcePath> sourcePath)
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
                       Or.Equals(node.Or);
            }

            public override int GetHashCode()
            {
                return Or.GetHashCode();
            }


            public override string? ToString()
            {
                return $"{nameof(VirtualNode)}({Or})";
            }

            public IOrType<IIsPossibly<Guid>, IError> Primitive()
            {
                var first = Or.backing.FirstOrDefault();
                if (first != null && Or.backing.Count == 1)
                {
                    return first.Primitive();
                }
                return OrType.Make<IIsPossibly<Guid>, IError>(Possibly.IsNot<Guid>());
            }

            public IOrType<IEnumerable<KeyValuePair<IKey, IOrType<VirtualNode, IError>>>, IError> VirtualMembers()
            {
                var couldBeErrors = Or.backing.Select(x => x.VirtualMembers()).ToArray();

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
                var errorCheck = Or.backing.Select(x => x.VirtualInput()).OfType<IIsDefinately<IOrType<VirtualNode, IError>>>().Select(x=>x.Value).ToArray();

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
                var errorCheck = Or.backing.Select(x => x.VirtualOutput()).OfType<IIsDefinately<IOrType<VirtualNode, IError>>>().Select(x => x.Value).ToArray();


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

            public IOrType<EqualibleHashSet<CombinedTypesAnd>,IError> ToRep() => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Or);

            private readonly IIsPossibly<SourcePath> sourcePath;

            public IIsPossibly<SourcePath> SourcePath() {
                return sourcePath;
            }

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> Walk(IEnumerable<IOrType<Member, Input, Output>> path)
            {
                if (!path.Any())
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(Or);
                }

                var errorCheck = Or.backing.Select(x => x.Walk(path)).ToArray();

                var errors = errorCheck.SelectMany(x => {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                // we only actually error our if everything is invalid
                // why?
                if (errorCheck.Length == errors.Length)
                {

                    if (errors.Length == 1)
                    {
                        return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(errors.First());
                    }
                    if (errors.Length > 1)
                    {
                        return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(Error.Cascaded("", errors));
                    }
                }

                var set = errorCheck.SelectMany(x => {
                    if (x.Is1(out var value))
                    {
                        return new[] { value };
                    }
                    return new EqualibleHashSet<CombinedTypesAnd>[] { };

                }).ToArray();

                // if any of the components don't have it than we don't have it
                if (set.Length != errorCheck.Length)
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
                }

                if (!set.Any())
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
                }


                //var errorCheck2 = set.Select(x => x.ToRep()).ToArray();

                //errors = errorCheck2.SelectMany(x => {
                //    if (x.Is2(out var error))
                //    {
                //        return new IError[] { error };
                //    }
                //    return new IError[] { };
                //}).ToArray();

                //if (errors.Length == 1)
                //{
                //    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(errors.First());
                //}
                //if (errors.Length > 1)
                //{
                //    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(Error.Cascaded("", errors));
                //}

                return  OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(VirtualNode.IsAny(set));

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

    // we use a struct so we get equality and hashcodes for free 
    // plus, am empty struct is probably a really preformant
    struct DoesNotExist
    {
        public override string? ToString()
        {
            return nameof(DoesNotExist);
        }
    }
}
