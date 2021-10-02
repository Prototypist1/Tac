using Prototypist.TaskChain;
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

    internal class EqualibleHashSet<T> : IEnumerable<T>
    {
        public readonly HashSet<T> backing;

        public EqualibleHashSet(HashSet<T> backing)
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

        public bool Add(T t) {
            return backing.Add(t);
        }

        public override string? ToString()
        {
            return $"EqualibleHashSet<{typeof(T).Name}>({string.Join(", ", backing.Take(10).Select(x => x.ToString())) + ((backing.Count > 10) ? "..." : "")})";
        }
    }

    internal static class IVirtualFlowNodeExtensions
    {
        public static IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError> ToRep(this Tpn.IVirtualFlowNode self)
        {
            return self.ToRep(new IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>[] { }).SwitchReturns(
                    x => OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(x),
                    x => OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(x),
                    _ => throw new Exception("if you don't have a path, you have to exist, without a path you are ToRep of this"));
        }

        public static IIsPossibly<IOrType<Tpn.VirtualNode, IError>> VirtualOutput(this Tpn.IVirtualFlowNode self)
        {
            return self.ToRep(new[] { OrType.Make<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>(new Tpn.Output()) }).SwitchReturns(
                x => Possibly.Is(OrType.Make<Tpn.VirtualNode, IError>(new Tpn.VirtualNode(self.SourcePath().Output(), x))),
                x => Possibly.Is(OrType.Make<Tpn.VirtualNode, IError>(x)),
                x => Possibly.IsNot<IOrType<Tpn.VirtualNode, IError>>());
        }
        public static IIsPossibly<IOrType<Tpn.VirtualNode, IError>> VirtualInput(this Tpn.IVirtualFlowNode self)
        {
            return self.ToRep(new[] { OrType.Make<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>(new Tpn.Input()) }).SwitchReturns(
                x => Possibly.Is(OrType.Make<Tpn.VirtualNode, IError>(new Tpn.VirtualNode(self.SourcePath().Input(), x))),
                x => Possibly.Is(OrType.Make<Tpn.VirtualNode, IError>(x)),
                x => Possibly.IsNot<IOrType<Tpn.VirtualNode, IError>>());
        }

        public static IIsPossibly<IOrType<Tpn.VirtualNode, IError>> VirtualMember(this Tpn.IVirtualFlowNode self, IKey key)
        {
            return self.ToRep(new[] { OrType.Make<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>(new Tpn.Member(key)) }).SwitchReturns(
                x => Possibly.Is(OrType.Make<Tpn.VirtualNode, IError>(new Tpn.VirtualNode(self.SourcePath().Input(), x))),
                x => Possibly.Is(OrType.Make<Tpn.VirtualNode, IError>(x)),
                x => Possibly.IsNot<IOrType<Tpn.VirtualNode, IError>>());
        }

        public static IIsPossibly<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>> VirtualOutput(this EqualibleHashSet<Tpn.CombinedTypesAnd> Or)
        {
            return Or.ToRep(new[] { OrType.Make<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>(new Tpn.Output()) }).SwitchReturns(
                x => Possibly.Is(OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(x)),
                x => Possibly.Is(OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(x)),
                x => Possibly.IsNot<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>>());
        }
        public static IIsPossibly<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>> VirtualInput(this EqualibleHashSet<Tpn.CombinedTypesAnd> Or)
        {
            return Or.ToRep(new[] { OrType.Make<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>(new Tpn.Input()) }).SwitchReturns(
                x => Possibly.Is(OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(x)),
                x => Possibly.Is(OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(x)),
                x => Possibly.IsNot<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>>());
        }

        // OR of ANDs
        public static IIsPossibly<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>> VirtualMember(this EqualibleHashSet<Tpn.CombinedTypesAnd> Or, IKey key)
        {
            return Or.ToRep(new[] { OrType.Make<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>(new Tpn.Member(key)) }).SwitchReturns(
                x => Possibly.Is(OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(x)),
                x => Possibly.Is(OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(x)),
                x => Possibly.IsNot<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>>());
        }

        public static IOrType<ICollection<KeyValuePair<IKey, Lazy<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>>>>, IError> VirtualMembers(this EqualibleHashSet<Tpn.CombinedTypesAnd> Or)
        {
            var couldBeErrors = Or.backing.Select(x => x.VirtualMembers()).ToArray();

            var error = couldBeErrors.SelectMany(x => {
                if (x.Is2(out var error))
                {
                    return new IError[] { error };
                }
                return new IError[] { };
            }).ToArray();

            if (error.Any())
            {
                return OrType.Make<ICollection<KeyValuePair<IKey, Lazy<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>>>>, IError>(Error.Cascaded("", error));
            }

            var set = couldBeErrors.SelectMany(x => x.Is1OrThrow()).ToArray();

            return OrType.Make<ICollection<KeyValuePair<IKey, Lazy<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>>>>, IError>(
                    set.GroupBy(x => x.Key)
                    .Where(x => x.Count() == Or.backing.Count) // everyone in the or has to have it
                    .Select(x => new KeyValuePair<IKey, Lazy<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>>>(x.Key, new Lazy<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>>(() => {
                        var errors = x.SelectMany(y =>
                        {
                            if (y.Value.Value.Is2(out var error))
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
                                return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(errors.First());
                            }
                            if (errors.Length > 1)
                            {
                                return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(Error.Cascaded("", errors));
                            }
                        }

                        var notError = x.Select(y => y.Value.Value.Is1OrThrow()).ToArray();

                        if (notError.Count() == 1)
                        {
                            return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(notError.First());
                        }
                        return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(Tpn.VirtualNode.IsAny(notError));
                    })))
                .ToArray());
        }

        // this looks too much like this
        // {736F2203-BF89-4975-8BFE-7B28406E74C6}
        public static IOrType<IReadOnlyList<Lazy<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>>>, IError> VirtualGenerics(this EqualibleHashSet<Tpn.CombinedTypesAnd> Or)
        {
            var couldBeErrors = Or.backing.Select(x => x.VirtualGenerics()).ToArray();

            var error = couldBeErrors.SelectMany(x => {
                if (x.Is2(out var error))
                {
                    return new IError[] { error };
                }
                return new IError[] { };
            }).ToArray();

            if (error.Any())
            {
                return OrType.Make<IReadOnlyList<Lazy<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>>>, IError>(Error.Cascaded("", error));
            }

            var set = couldBeErrors.Select(x => x.Is1OrThrow()).SelectMany(x => {
                var i = 0;
                return x.Select(y => (Key: i++, Value: y)).ToArray();
            }).ToArray();

            return OrType.Make<IReadOnlyList<Lazy<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>>>, IError>(
                    set
                    .GroupBy(x => x.Key)
                    .OrderBy(x => x.Key)
                    .Where(x => x.Count() == Or.backing.Count) // everyone in the or has to have it
                    .Select(x => new Lazy<IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>>(() => {
                        var errors = x.SelectMany(y =>
                        {
                            if (y.Value.Value.Is2(out var error))
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
                                return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(errors.First());
                            }
                            if (errors.Length > 1)
                            {
                                return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(Error.Cascaded("", errors));
                            }
                        }

                        var notError = x.Select(y => y.Value.Value.Is1OrThrow()).ToArray();

                        if (notError.Count() == 1)
                        {
                            return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(notError.First());
                        }
                        return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(Tpn.VirtualNode.IsAny(notError));
                    }))
                    .ToArray());
        }

        public static IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError, DoesNotExist> ToRep(this EqualibleHashSet<Tpn.CombinedTypesAnd> Or, IReadOnlyList<IOrType<Tpn.Member, Tpn.Input, Tpn.Output, Tpn.Generic>> path)
        {
            if (!path.Any())
            {
                return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError, DoesNotExist>(Or);
            }

            var errorCheck = Or.backing.Select(x => x.ToRep(path)).ToArray();

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
                    return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError, DoesNotExist>(errors.First());
                }
                if (errors.Length > 1)
                {
                    return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError, DoesNotExist>(Error.Cascaded("", errors));
                }
            }

            var set = errorCheck.SelectMany(x => {
                if (x.Is1(out var value))
                {
                    return new[] { value };
                }
                return new EqualibleHashSet<Tpn.CombinedTypesAnd>[] { };

            }).ToArray();

            // if any of the components don't have it than we don't have it
            if (set.Length != errorCheck.Length)
            {
                return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
            }

            if (!set.Any())
            {
                return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
            }

            return OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError, DoesNotExist>(Tpn.VirtualNode.IsAny(set));

        }

        public static IOrType<IIsPossibly<Guid>, IError> Primitive(this EqualibleHashSet<Tpn.CombinedTypesAnd> Or)
        {
            var first = Or.backing.FirstOrDefault();
            if (first != null && Or.backing.Count == 1)
            {
                return first.Primitive();
            }
            return OrType.Make<IIsPossibly<Guid>, IError>(Possibly.IsNot<Guid>());
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

            // this is lazy because we don't what to have to fully calculate all our members
            // it is a lot of wasted work
            // also if we flow in to one of our members we get a stack overflow
            IOrType<ICollection<KeyValuePair<IKey, Lazy<IOrType<VirtualNode, IError>>>>, IError> VirtualMembers();

            // so much work 
            IOrType<IReadOnlyList<Lazy<IOrType<VirtualNode, IError>>>, IError> VirtualGenerics();

            IOrType<IIsPossibly<Guid>, IError> Primitive();

            /// <summary>
            /// Source Path is used as a way to express identity
            /// virtual nodes are defined in terms of their replationship to a "real" node
            /// </summary>
            SourcePath SourcePath();

            IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> ToRep(IReadOnlyList<IOrType<Member, Input, Output, Generic>> path);
        }


        public interface IFlowNode : IVirtualFlowNode
        {
            bool MustAccept(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> from, List<(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing);
            bool MustReturn(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> from, List<(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing);
            // AND of ORs of ANDs
            EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> UpStreamMustReturn();
            EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> DownStreamMustAccept();
        }


        public class PrimitiveFlowNode : IFlowNode<Tpn.TypeProblem2.Type>
        {
            private readonly SourcePath.SourcePathCache sourcePathCache;
            public PrimitiveFlowNode(TypeProblem2.Type source, Guid guid, SourcePath.SourcePathCache sourcePathCache)
            {
                Source = Possibly.Is(source ?? throw new ArgumentNullException(nameof(source)));
                Guid = guid;
                this.sourcePathCache = sourcePathCache ?? throw new ArgumentNullException(nameof(sourcePathCache));
            }

            public IIsPossibly<Tpn.TypeProblem2.Type> Source { get; }
            public Guid Guid { get; }

            public IOrType<IIsPossibly<Guid>, IError> Primitive()
            {
                return OrType.Make<IIsPossibly<Guid>, IError>(Possibly.Is(Guid));
            }

            public bool MustAccept(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> from, List<(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                return false;
            }
            public bool MustReturn(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> from, List<(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                return false;
            }

            public IOrType<ICollection<KeyValuePair<IKey, Lazy<IOrType<VirtualNode, IError>>>>, IError> VirtualMembers()
            {
                return OrType.Make<ICollection<KeyValuePair<IKey, Lazy<IOrType<VirtualNode, IError>>>>, IError>(new Dictionary<IKey, Lazy<IOrType<VirtualNode, IError>>>());
            }

            public IOrType<IReadOnlyList<Lazy<IOrType<VirtualNode, IError>>>, IError> VirtualGenerics()
            {
                return OrType.Make<IReadOnlyList<Lazy<IOrType<VirtualNode, IError>>>, IError>(Array.Empty<Lazy<IOrType<VirtualNode, IError>>>());
            }

            public SourcePath SourcePath()
            {
                return sourcePathCache.CreateSourcePath(OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(this), new List<IOrType<Member, Input, Output, Generic>>());
            }

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> ToRep(IReadOnlyList<IOrType<Member, Input, Output, Generic>> path)
            {
                if (!path.Any())
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd>
                    {
                        new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this) })
                    }));
                }

                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
            }


            public IIsPossibly<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>> Flatten(HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> except, IReadOnlyList<IOrType<Member, Input, Output, Generic>> pathParts, bool _)
            {

                if (pathParts.Any())
                {
                    throw new Exception("....a primitive can't have a type...");
                }

                var key = OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(this);
                if (except.Contains(key))
                {
                    return Possibly.IsNot<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>();
                }
                except.Add(key);

                return Possibly.Is(
                    OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(
                        new EqualibleHashSet<CombinedTypesAnd>(
                            new HashSet<CombinedTypesAnd>{
                                new CombinedTypesAnd(
                                    new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{
                                        OrType.Make<ConcreteFlowNode, PrimitiveFlowNode > (this) })})));
            }

            public override string? ToString()
            {
                return $"{nameof(PrimitiveFlowNode)}({Source})";
            }

            public EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> UpStreamMustReturn() =>
                new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>(
                    new HashSet<EqualibleHashSet<CombinedTypesAnd>> {
                        new EqualibleHashSet<CombinedTypesAnd>(
                            new HashSet<CombinedTypesAnd> {
                                new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{
                                    Prototypist.Toolbox.OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this)})})});

            public EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> DownStreamMustAccept() =>
                new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>(
                    new HashSet<EqualibleHashSet<CombinedTypesAnd>> {
                        new EqualibleHashSet<CombinedTypesAnd>(
                            new HashSet<CombinedTypesAnd> {
                                new CombinedTypesAnd(
                                    new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{
                                        Prototypist.Toolbox.OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this)})})});
        }

        public class ConcreteFlowNode<TSource> : ConcreteFlowNode, IFlowNode<TSource>
        {
            public ConcreteFlowNode(TSource source, SourcePath.SourcePathCache sourcePathCache):base(sourcePathCache)
            {
                Source = Possibly.Is(source);
            }

            public IIsPossibly<TSource> Source { get; }

            public override string? ToString()
            {
                return $"{nameof(ConcreteFlowNode)}<{typeof(TSource).Name}>({Source})";
            }
        }

        public abstract class ConcreteFlowNode : IFlowNode
        {

            private readonly SourcePath.SourcePathCache sourcePathCache;

            protected ConcreteFlowNode(SourcePath.SourcePathCache sourcePathCache)
            {
                this.sourcePathCache = sourcePathCache;
            }

            private Dictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> members = new Dictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
            private IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> input = Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
            private IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> output = Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
            private IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>[] generics = Array.Empty<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();


            public IReadOnlyDictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Members => members;
            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Input { get {
                    return input;
                }
                set {
                    if (input.Is(out var _)) {
                        throw new Exception("I don't think this should be set twice...");
                    }
                    sourcePathCache.AddInput(this, value.GetOrThrow()/*I'm not getting this to a not*/);
                    input = value;
                }
            }
            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Output
            {
                get
                {
                    return output;
                }
                set
                {
                    if (output.Is(out var _))
                    {
                        throw new Exception("I don't think this should be set twice...");
                    }
                    sourcePathCache.AddOutput(this, value.GetOrThrow()/*I'm not getting this to a not*/);
                    output = value;
                }
            }

            public IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>[] Generics
            {
                get
                {
                    return generics;
                }
                set
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        sourcePathCache.AddGeneric(this, value[i], i);
                    }
                    generics = value;
                }
            }

            public void AddMember(IKey key, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> value) {
                members.Add(key, value);
                sourcePathCache.AddMember(this, key, value);
            }


            public IOrType<ICollection<KeyValuePair<IKey, Lazy<IOrType<VirtualNode, IError>>>>, IError> VirtualMembers()
            {
                return OrType.Make<ICollection<KeyValuePair<IKey, Lazy<IOrType<VirtualNode, IError>>>>, IError>(
                    Members
                        .Select(x => new KeyValuePair<IKey, Lazy<IOrType<VirtualNode, IError>>>(
                            x.Key,
                            new Lazy<IOrType<VirtualNode, IError>>(() => x.Value.GetValueAs(out IVirtualFlowNode _)
                                .ToRep()
                                .TransformInner(y => new VirtualNode(sourcePathCache.CreateSourcePath(x.Value, Array.Empty<IOrType<Member, Input, Output, Generic>>()), y)))))
                        .ToArray());
            }

            public IOrType<IReadOnlyList<Lazy<IOrType<VirtualNode, IError>>>, IError> VirtualGenerics()
            {
                var i = 0;
                return OrType.Make<IReadOnlyList<Lazy<IOrType<VirtualNode, IError>>>, IError>(
                    Generics
                        .Select(x =>
                        new Lazy<IOrType<VirtualNode, IError>>(() => x.GetValueAs(out IVirtualFlowNode _)
                            .ToRep()
                            .TransformInner(y => new VirtualNode(sourcePathCache.CreateSourcePath(x, Array.Empty<IOrType<Member, Input, Output, Generic>>()), y))))
                        .ToArray());
            }


            public IOrType<IIsPossibly<Guid>, IError> Primitive()
            {
                return OrType.Make<IIsPossibly<Guid>, IError>(Possibly.IsNot<Guid>());
            }

            public bool MustAccept(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> sourcePath, List<(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {

                var me = (sourcePath, ToOr(this));
                if (alreadyFlowing.Contains(me))
                {
                    return false;
                }
                alreadyFlowing.Add(me);


                //if (sourcePath.source.Is3(out var _))
                //{
                //    throw new Exception("actually don't flow");
                //}

                //var walked = sourcePath.Walk();

                //if (!walked.Is1(out var from)) {
                //    return false;
                //}

                //
                // TODO 
                // you are here,
                // we can lead with our members
                // when I flow, if I am flowing from an infered node I shouldn't flow it's path 
                // I should just flow the stuff flowing into it
                // so in "MustAcceptMember(fromMember.Value.Value.Is1OrThrow().SourcePath()/*lazy*/, alreadyFlowing);" I need to replace "fromMember.Value.Value.Is1OrThrow().SourcePath()" with what source paths to flow, if any  
                // 
                //{
                //    if (from.VirtualMembers().Is1(out var fromMembers))
                //    {
                //        foreach (var fromMember in fromMembers)
                //        {
                //            // 
                //            changes |= MustAcceptMember(fromMember.Value.Value.Is1OrThrow().SourcePath()/*lazy*/, alreadyFlowing);
                //        }
                //    }
                //}

                var changes = false;

                foreach (var ourMember in members)
                {
                    var memberFlow = new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>(sourcePath.Select(x => x.VirtualMember(ourMember.Key)).SelectMany(x =>
                    {
                        if (x.Is(out var exists) && exists.Is1(out var notError))
                        {
                            return new[] { notError };
                        }
                        return Array.Empty<EqualibleHashSet<CombinedTypesAnd>>();
                    }).ToHashSet());

                    changes |= ourMember.Value.GetValueAs(out IFlowNode _).MustAccept(memberFlow, alreadyFlowing);
                }


                if (Input.Is(out var input))
                {
                    // method [{a,b,c},empty] =: x
                    // method [{a,b},empty] =: x
                    // method [{a},empty] =: x
                    // x better be metohd[{a},empty]

                    var inputFlow = new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>(sourcePath.Select(x => x.VirtualInput()).SelectMany(x =>
                    {
                        if (x.Is(out var exists) && exists.Is1(out var notError))
                        {
                            return new[] { notError };
                        }
                        return Array.Empty<EqualibleHashSet<CombinedTypesAnd>>();
                    }).ToHashSet());

                    changes |= input.GetValueAs(out IFlowNode _).MustAccept(inputFlow, alreadyFlowing.ToList());
                }

                if (Output.Is(out var output))
                {
                    // method [empty,{a,b,c}] =: x
                    // method [empty,{a,b}] =: x
                    // method [empty,{a}] =: x
                    // x better be metohd[empty,{a}]


                    var outputFlow = new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>(sourcePath.Select(x => x.VirtualOutput()).SelectMany(x =>
                    {
                        if (x.Is(out var exists) && exists.Is1(out var notError))
                        {
                            return new[] { notError };
                        }
                        return Array.Empty<EqualibleHashSet<CombinedTypesAnd>>();
                    }).ToHashSet());


                    changes |= output.GetValueAs(out IFlowNode _).MustAccept(outputFlow, alreadyFlowing.ToList());
                }

                foreach (var from in sourcePath)
                {
                    if (from.VirtualGenerics().Is1(out var theirGenerics))
                    {
                        foreach (var (ours, theirs) in Generics.Zip(theirGenerics, (x, y) => (x, y)))
                        {
                            if (theirs.Value.Is1(out var thiersConcrete))
                            {
                                changes |= ours.GetValueAs(out IFlowNode _).MustAccept(
                                    new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>(
                                        new HashSet<EqualibleHashSet<CombinedTypesAnd>> { thiersConcrete }), alreadyFlowing.ToList()/*do I need to always copy this list?*/);
                            }
                        }
                    }
                }

                return changes;
            }

            public bool MustReturn(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> sourcePath, List<(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                var me = (sourcePath, ToOr(this));
                if (alreadyFlowing.Contains(me))
                {
                    return false;
                }
                alreadyFlowing.Add(me);


                //if (sourcePath.source.Is3(out var _))
                //{
                //    throw new Exception("actually don't flow");
                //}


                //var walked = sourcePath.Walk();

                //if (!walked.Is1(out var from))
                //{
                //    return false;
                //}

                //if (from.VirtualMembers().Is1(out var members))
                //{
                //    foreach (var fromMember in members)
                //    {
                //        changes |= MustReturnMember(fromMember.Value.Value.Is1OrThrow()/*lazy*/.SourcePath(), alreadyFlowing);
                //    }
                //}

                var changes = false;
                foreach (var ourMember in members)
                {
                    var memberFlow = new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>(sourcePath.Select(x => x.VirtualMember(ourMember.Key)).SelectMany(x =>
                    {
                        if (x.Is(out var exists) && exists.Is1(out var notError))
                        {
                            return new[] { notError };
                        }
                        return Array.Empty<EqualibleHashSet<CombinedTypesAnd>>();
                    }).ToHashSet());

                    changes |= ourMember.Value.GetValueAs(out IFlowNode _).MustReturn(memberFlow, alreadyFlowing);
                }


                if (Input.Is(out var input))
                {
                    // x =: method [{a,b,c},empty]
                    // x =: method [{a,b},empty]
                    // x =: method [{a},empty]
                    // x better be metohd[{a,b,c},empty]

                    // must return 
                    // x =: {a,b,c} x1
                    // x =: {a,b} x2 
                    // x =: {a} x3
                    // x is {a,b,c}

                    // must accept 
                    // {a,b,c} x1 =: x 
                    // {a,b} x2 =: x 
                    // {a} x3 =: x 
                    // x is {a}

                    // yeah must return

                    var inputFlow = new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>(sourcePath.Select(x => x.VirtualInput()).SelectMany(x =>
                    {
                        if (x.Is(out var exists) && exists.Is1(out var notError))
                        {
                            return new[] { notError };
                        }
                        return Array.Empty<EqualibleHashSet<CombinedTypesAnd>>();
                    }).ToHashSet());

                    changes |= input.GetValueAs(out IFlowNode _).MustReturn(inputFlow, alreadyFlowing.ToList());
                }

                if (Output.Is(out var output))
                {
                    // x =: method [empty,{a,b,c}]
                    // x =: method [empty,{a,b}]
                    // x =: method [empty,{a}]
                    // x better be metohd[empty,{a,b,c}]


                    var outputFlow = new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>(sourcePath.Select(x => x.VirtualOutput()).SelectMany(x =>
                    {
                        if (x.Is(out var exists) && exists.Is1(out var notError))
                        {
                            return new[] { notError };
                        }
                        return Array.Empty<EqualibleHashSet<CombinedTypesAnd>>();
                    }).ToHashSet());

                    changes |= output.GetValueAs(out IFlowNode _).MustReturn(outputFlow, alreadyFlowing.ToList());
                }

                foreach (var from in sourcePath)
                {
                    if (from.VirtualGenerics().Is1(out var theirGenerics))
                    {
                        foreach (var (ours, theirs) in Generics.Zip(theirGenerics, (x, y) => (x, y)))
                        {
                            if (theirs.Value.Is1(out var thiersConcrete))
                            {
                                changes |= ours.GetValueAs(out IFlowNode _).MustReturn(
                                    new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>(
                                        new HashSet<EqualibleHashSet<CombinedTypesAnd>> { thiersConcrete }), alreadyFlowing.ToList()/*do I need to always copy this list?*/);
                            }
                        }
                    }
                }

                return changes;
            }
            ////KeyValuePair<IKey, Lazy<IOrType<VirtualNode, IError>>>
            //private bool MustAcceptMember(SourcePath fromMember, List<(SourcePath, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            //{
            //    //if (fromMember.Value.Value.Is2(out var _))
            //    //{
            //    //    return false;
            //    //}

            //    var changes = false;
            //    if (this.Members.TryGetValue(fromMember.path.Last().Is1OrThrow().key, out var toMember))
            //    {
            //        changes |= toMember.GetValueAs(out IFlowNode _).MustAccept(fromMember, alreadyFlowing.ToList());
            //    }
            //    else
            //    {
            //        // we end up here because we try to flow all the values in an or type
            //        // below are two code snipits:

            //        // in this one, A's "num" and "a" have to be numbers

            //        // type A { num; a;}
            //        // A a;
            //        // a =: type {number num; number a} | type {number a; number b;} c


            //        // in this one, A's "b" and "a" have to be numbers

            //        // type A { b; a;}
            //        // A a;
            //        // a =: type {number num; number a} | type {number a; number b;} c

            //        // for them both to work the or type has to flow all of it's member definitions
            //    }
            //    return changes;
            //}

            //private bool MustReturnMember(SourcePath fromMember, List<(SourcePath, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            //{
            //    //if (fromMember.Value.Value.Is2(out var _))
            //    //{
            //    //    return false;
            //    //}

            //    var changes = false;
            //    if (this.Members.TryGetValue(fromMember.path.Last().Is1OrThrow().key, out var toMember))
            //    {
            //        changes |= toMember.GetValueAs(out IFlowNode _).MustReturn(fromMember, alreadyFlowing.ToList());
            //    }
            //    else
            //    {

            //    }
            //    return changes;
            //}

            public SourcePath SourcePath()
            {
                return sourcePathCache.CreateSourcePath(OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(this), new List<IOrType<Member, Input, Output, Generic>>());
            }

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> ToRep(IReadOnlyList<IOrType<Member, Input, Output, Generic>> path)
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
                        if (Members.TryGetValue(member.key, out var value))
                        {
                            return value.GetValueAs(out IVirtualFlowNode _).ToRep(path.Skip(1).ToArray());
                        }
                        return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
                    },
                    input => {
                        if (Input.Is(out var value))
                        {
                            return value.GetValueAs(out IVirtualFlowNode _).ToRep(path.Skip(1).ToArray());
                        }
                        return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
                    },
                    output => {
                        if (Output.Is(out var value))
                        {
                            return value.GetValueAs(out IVirtualFlowNode _).ToRep(path.Skip(1).ToArray());
                        }
                        return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
                    },
                    generic => {
                        if (Generics.Length > generic.index)
                        {
                            return Generics[generic.index].GetValueAs(out IVirtualFlowNode _).ToRep(path.Skip(1).ToArray());
                        }
                        return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new DoesNotExist());
                    });
            }

            public IIsPossibly<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>> Flatten(HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> except, IReadOnlyList<IOrType<Member, Input, Output, Generic>> pathParts, bool accepted)
            {

                if (pathParts.Any())
                {

                    return pathParts.First().SwitchReturns(
                        member =>
                        {
                            if (Members.TryGetValue(member.key, out var value))
                            {
                                return value.SwitchReturns(
                                    x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted),
                                    x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted),
                                    x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted),
                                    x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted));
                            }
                            return Possibly.IsNot<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>();
                        },
                        input =>
                        {
                            if (Input.Is(out var myInput))
                            {
                                return myInput.SwitchReturns(
                                    x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted),
                                    x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted),
                                    x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted),
                                    x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted));
                            }
                            return Possibly.IsNot<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>();
                        },
                        output =>
                        {
                            if (Output.Is(out var myOutput))
                            {
                                return myOutput.SwitchReturns(
                                    x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted),
                                    x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted),
                                    x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted),
                                    x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted));
                            }
                            return Possibly.IsNot<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>();
                        },
                        generic =>
                        {
                            return Generics[generic.index].SwitchReturns(
                                x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted),
                                x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted),
                                x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted),
                                x => x.Flatten(except.ToHashSet(), pathParts.Skip(1).ToArray(), accepted));
                        });
                }

                var key = OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(this);
                if (except.Contains(key))
                {
                    return Possibly.IsNot<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>();
                }
                except.Add(key);

                return Possibly.Is(
                    OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(
                        new EqualibleHashSet<CombinedTypesAnd>(
                            new HashSet<CombinedTypesAnd>{
                                new CombinedTypesAnd(
                                    new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{
                                        OrType.Make<ConcreteFlowNode, PrimitiveFlowNode > (this) })})));
            }

            public EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> UpStreamMustReturn() =>
                new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> (
                    new HashSet<EqualibleHashSet<CombinedTypesAnd>> {
                        new EqualibleHashSet<CombinedTypesAnd>(
                            new HashSet<CombinedTypesAnd> {
                                new CombinedTypesAnd(new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{
                                    Prototypist.Toolbox.OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this)})})});
            public EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> DownStreamMustAccept() =>
                new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> (
                    new HashSet<EqualibleHashSet<CombinedTypesAnd>> {
                        new EqualibleHashSet<CombinedTypesAnd>(
                            new HashSet<CombinedTypesAnd> {
                                new CombinedTypesAnd(
                                    new HashSet<IOrType<ConcreteFlowNode, PrimitiveFlowNode>>{
                                        Prototypist.Toolbox.OrType.Make<ConcreteFlowNode, PrimitiveFlowNode>(this)})})});
        }

        public class OrFlowNode : IFlowNode<TypeProblem2.OrType>
        {
            private readonly SourcePath.SourcePathCache sourcePathCache;

            private IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> InnerToRep()
            {
                var couldBeErrors = this.Or.Select(x => x.GetValueAs(out IVirtualFlowNode _).ToRep());

                var errors = couldBeErrors.SelectMany(x =>
                {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                if (errors.Any())
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Cascaded("", errors));
                }

                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(new EqualibleHashSet<CombinedTypesAnd>(couldBeErrors.SelectMany(x => x.Is1OrThrow().backing).Distinct().ToHashSet()));
            }

            public bool MustAccept(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> from, List<(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                var me = (from, ToOr(this));
                if (alreadyFlowing.Contains(me))
                {
                    return false;
                }
                alreadyFlowing.Add(me);

                //if (from.source.Is3(out var _))
                //{
                //    return false;
                //    //throw new Exception("actually don't flow");
                //}

                var changes = false;
                foreach (var item in this.Or)
                {
                    changes |= item.GetValueAs(out IFlowNode _).MustAccept(from, alreadyFlowing.ToList());
                }
                return changes;
            }

            public bool MustReturn(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> from, List<(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                var me = (from, ToOr(this));
                if (alreadyFlowing.Contains(me))
                {
                    return false;
                }
                alreadyFlowing.Add(me);

                //if (from.source.Is3(out var _))
                //{
                //    return false;
                //    //throw new Exception("actually don't flow");
                //}

                var changes = false;
                foreach (var item in this.Or)
                {
                    changes |= item.GetValueAs(out IFlowNode _).MustReturn(from, alreadyFlowing.ToList());
                }
                return changes;
            }

            public OrFlowNode(IReadOnlyList<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> or, IIsPossibly<TypeProblem2.OrType> source, SourcePath.SourcePathCache sourcePathCache)
            {
                Or = or ?? throw new ArgumentNullException(nameof(or));
                Source = source;
                this.sourcePathCache= sourcePathCache ?? throw new ArgumentNullException(nameof(sourcePathCache));
            }

            public IReadOnlyList<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Or { get; }

            public IIsPossibly<TypeProblem2.OrType> Source { get; }

            public IOrType<IIsPossibly<Guid>, IError> Primitive()
            {
                return OrType.Make<IIsPossibly<Guid>, IError>(Possibly.IsNot<Guid>());
            }

            public IOrType<ICollection<KeyValuePair<IKey, Lazy<IOrType<VirtualNode, IError>>>>, IError> VirtualMembers()
            {
                return this.ToRep().SwitchReturns(
                    x => new VirtualNode(SourcePath(), x).VirtualMembers(),
                    x => OrType.Make<ICollection<KeyValuePair<IKey, Lazy<IOrType<VirtualNode, IError>>>>, IError>(x));
            }


            public IOrType<IReadOnlyList<Lazy<IOrType<VirtualNode, IError>>>, IError> VirtualGenerics()
            {
                return this.ToRep().SwitchReturns(
                    x => new VirtualNode(SourcePath(), x).VirtualGenerics(),
                    x => OrType.Make<IReadOnlyList<Lazy<IOrType<VirtualNode, IError>>>, IError>(x));
            }

            //public IIsPossibly<IOrType<VirtualNode, IError>> VirtualInput()
            //{
            //    return ToRep().SwitchReturns(x => new VirtualNode(x, SourcePath()).VirtualInput(), y => Possibly.Is<IOrType<VirtualNode, IError>>(OrType.Make<VirtualNode, IError>(y)));
            //}

            //public IIsPossibly<IOrType<VirtualNode, IError>> VirtualOutput()
            //{
            //    return ToRep().SwitchReturns(x => new VirtualNode(x, SourcePath()).VirtualOutput(), y => Possibly.Is<IOrType<VirtualNode, IError>>(OrType.Make<VirtualNode, IError>(y)));
            //}

            public SourcePath SourcePath()
            {
                return sourcePathCache.CreateSourcePath(OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(this), new List<IOrType<Member, Input, Output, Generic>>());
            }

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> ToRep(IReadOnlyList<IOrType<Member, Input, Output, Generic>> path)
            {
                if (!path.Any())
                {
                    return InnerToRep().SwitchReturns(x => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(x), x => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(x));
                }

                var couldBeErrors = this.Or.Select(x => x.GetValueAs(out IVirtualFlowNode _).ToRep(path));

                if (couldBeErrors.Any(x => x.Is3(out var _)))
                {
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

            public IIsPossibly<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>> Flatten(HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> except, IReadOnlyList<IOrType<Member, Input, Output, Generic>> pathParts, bool accepted)
            {
                if (!pathParts.Any())
                {
                    var key = OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(this);
                    if (except.Contains(key))
                    {
                        return Possibly.IsNot<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>();
                    }
                    except.Add(key);
                }

                var list = Or.Select(y =>
                   y.SwitchReturns(
                       x => x.Flatten(except.ToHashSet(), pathParts, accepted),
                       x => x.Flatten(except.ToHashSet(), pathParts, accepted),
                       x => x.Flatten(except.ToHashSet(), pathParts, accepted),
                       x => x.Flatten(except.ToHashSet(), pathParts, accepted)))
                    .SelectMany(x =>
                    {
                        if (x.Is(out var orType))
                        {
                            return new[] { orType };
                        }
                        return Array.Empty<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>();
                    }).Distinct().ToArray();

                var error = list.SelectMany(x => { if (x.Is2(out var error)) { return new[] { error }; } return Array.Empty<IError>(); }).ToArray();

                if (error.Length == 1)
                {
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(error[0]));
                }

                if (error.Length > 1)
                {

                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Cascaded("errors", error)));
                }

                return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(new EqualibleHashSet<CombinedTypesAnd>(list.SelectMany(x => x.Is1OrThrow()).ToHashSet())));
            }
            public EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> UpStreamMustReturn() => new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>( Or.SelectMany(x => x.GetValueAs(out IFlowNode _).UpStreamMustReturn()).ToHashSet());
            public EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> DownStreamMustAccept() => new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>(Or.SelectMany(x => x.GetValueAs(out IFlowNode _).DownStreamMustAccept()).ToHashSet());
        }

        
        public class InferredFlowNode : IFlowNode<TypeProblem2.InferredType>
        {
            private readonly SourcePath.SourcePathCache sourcePathCache;

            public readonly HashSet<SourcePath> AcceptedSources = new HashSet<SourcePath>();
            public readonly HashSet<SourcePath> ReturnedSources = new HashSet<SourcePath>();

            public readonly EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> Acceped2 = new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>(new HashSet<EqualibleHashSet<CombinedTypesAnd>> ());
            public readonly EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> Returned2 = new EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>(new HashSet<EqualibleHashSet<CombinedTypesAnd>>());

            public InferredFlowNode(IIsPossibly<TypeProblem2.InferredType> source, SourcePath.SourcePathCache sourcePathCache)
            {
                Source = source ?? throw new ArgumentNullException(nameof(source));
                this.sourcePathCache = sourcePathCache ?? throw new ArgumentNullException(nameof(sourcePathCache));
            }

            public IIsPossibly<TypeProblem2.InferredType> Source
            {
                get;
            }

            private IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> ToRepReturns(IReadOnlyList<IOrType<Member, Input, Output, Generic>> pathParts)
            {
                if (Flatten(new HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> { }, pathParts, false).Is(out var res))
                {
                    return res;
                }
                else
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd>()));
                }
            }

            private IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> ToRepAccepts(IReadOnlyList<IOrType<Member, Input, Output, Generic>> pathParts)
            {
                if (Flatten(new HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> { }, pathParts, true).Is(out var res))
                {
                    return res;
                }
                else
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd>()));
                }
            }

            private static IEnumerable<IOrType<Member, Input, Output, Generic>> Combine(IEnumerable<IOrType<Member, Input, Output, Generic>> left, IEnumerable<IOrType<Member, Input, Output, Generic>> right)
            {
                foreach (var item in left)
                {
                    yield return item;
                }
                foreach (var item in right)
                {
                    yield return item;
                }
            }

            //public HashSet<EqualibleHashSet<CombinedTypesAnd>> FlattenReturn(HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> except, IEnumerable<IOrType<Member, Input, Output, Generic>> pathParts)
            //{
            //    return Flatten(except, pathParts, false);
            //}

            //public HashSet<EqualibleHashSet<CombinedTypesAnd>> FlattenAccepts(HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> except, IEnumerable<IOrType<Member, Input, Output, Generic>> pathParts)
            //{
            //    return Flatten(except, pathParts, true);
            //}

            // I think "or nothing" might be a good idea
            // I think maybe Flatten should just flatten and we can walk after 

            // this "except" song and dance is to avoid stack overflows
            // when you flow in to something and it flows in to you bad things can happen

            // the IIsPossibly here is just used for stuff we have already seen
            public IIsPossibly<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>> Flatten(HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> except, IReadOnlyList<IOrType<Member, Input, Output, Generic>> pathParts, bool accepted)
            {
                if (!pathParts.Any())
                {
                    var key = OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(this);
                    if (except.Contains(key))
                    {
                        return Possibly.IsNot<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>();
                    }
                    except.Add(key);
                }

                var hashSetSet = new HashSet<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>();
                foreach (var item in accepted ? AcceptedSources : ReturnedSources)
                {
                    var flattened = item.source.SwitchReturns(
                         primitive => primitive.Flatten(except.ToHashSet(), Combine(item.path, pathParts).ToArray(), accepted),
                         concrete => concrete.Flatten(except.ToHashSet(), Combine(item.path, pathParts).ToArray(), accepted),
                         or => or.Flatten(except.ToHashSet(), Combine(item.path, pathParts).ToArray(), accepted),
                         inferred => inferred.Flatten(except.ToHashSet(), Combine(item.path, pathParts).ToArray(), accepted));

                    if (flattened.Is(out var orType))
                    {
                        hashSetSet.Add(orType);
                    }
                }

                var setsSet = hashSetSet.ToArray();

                //var setsSet = (accepted ? AcceptedSources : ReturnedSources)
                //    .Select(item => item.source.SwitchReturns(
                //         primitive => primitive.Flatten(except.ToHashSet(), Combine(item.path,  pathParts).ToArray(), accepted),
                //         concrete => concrete.Flatten(except.ToHashSet(), Combine(item.path, pathParts).ToArray(), accepted),
                //         or => or.Flatten(except.ToHashSet(), Combine(item.path, pathParts).ToArray(), accepted),
                //         inferred => inferred.Flatten(except.ToHashSet(), Combine(item.path, pathParts).ToArray(), accepted)))
                //    .SelectMany(x =>
                //         {
                //             if (x.Is(out var orType))
                //             {
                //                 return new[] { orType };
                //             }
                //             return Array.Empty<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>();
                //         })
                //    .Distinct()
                //    .ToArray();

                var errors = setsSet.SelectMany(x => { if (x.Is2(out var error)) { return new[] { error }; } return Array.Empty<IError>(); }).ToArray();

                if (errors.Length == 1)
                {
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(errors[0]));
                }

                if (errors.Length > 1)
                {

                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Cascaded("errors", errors)));
                }

                var sets = setsSet.Select(x => x.Is1OrThrow()).ToArray();

                if (!accepted)
                {
                    if (sets.Length == 0)
                    {
                        return Possibly.IsNot<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>();
                    }

                    if (sets.Length == 1)
                    {
                        return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(sets.First()));
                    }

                    var at = sets.First();
                    foreach (var item in sets.Skip(1))
                    {
                        var or = Union(at, item);
                        if (or.Is2(out var error))
                        {
                            return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(error));
                        }
                        at = or.Is1OrThrow();
                    }
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(at));
                }
                else
                {
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(new EqualibleHashSet<CombinedTypesAnd>(sets.SelectMany(x => x).ToHashSet())));
                }
            }

            public bool MustAccept(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> from, List<(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
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
                //if (from.source.Is3(out PrimitiveFlowNode _))
                //{
                //    return false;
                //}
                // this ^ comment doesn't make any sense
                // and I don't like the code
                // when I get things stable I would like to comment it out
                // TODO

                var me = (from, ToOr(this));
                if (alreadyFlowing.Contains(me))
                {
                    return false;
                }
                alreadyFlowing.Add(me);


                // don't flow in to your self
                //if (from.Walk().Is1(out var v1) && v1 == this)
                //{
                //    return false;
                //}
                var res = false;
                foreach (var item in from)
                {
                    res |= Acceped2.Add(item); 
                }

                return res;
            }

            public bool MustReturn(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> from, List<(EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)> alreadyFlowing)
            {
                var me = (from, ToOr(this));
                if (alreadyFlowing.Contains(me))
                {
                    return false;
                }
                alreadyFlowing.Add(me);


                // don't flow in to your self
                //if (from.Walk().Is1(out var v1) && v1 == this)
                //{
                //    return false;
                //}
                var res = false;
                foreach (var item in from)
                {
                    res |= Returned2.Add(item);
                }

                return res;
            }

            public IOrType<IIsPossibly<Guid>, IError> Primitive()
            {
                return this.ToRep().SwitchReturns(x => new VirtualNode(SourcePath(), x).Primitive(), y => OrType.Make<IIsPossibly<Guid>, IError>(y));
            }
            public IOrType<ICollection<KeyValuePair<IKey, Lazy<IOrType<VirtualNode, IError>>>>, IError> VirtualMembers()
            {
                return this.ToRep().SwitchReturns(
                    x => new VirtualNode(SourcePath(), x).VirtualMembers(),
                    x => OrType.Make<ICollection<KeyValuePair<IKey, Lazy<IOrType<VirtualNode, IError>>>>, IError>(x));
            }

            public IOrType<IReadOnlyList<Lazy<IOrType<VirtualNode, IError>>>, IError> VirtualGenerics()
            {
                return this.ToRep().SwitchReturns(
                    x => new VirtualNode(SourcePath(), x).VirtualGenerics(),
                    x => OrType.Make<IReadOnlyList<Lazy<IOrType<VirtualNode, IError>>>, IError>(x));
            }

            public SourcePath SourcePath()
            {
                return sourcePathCache.CreateSourcePath(OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(this), new List<IOrType<Member, Input, Output, Generic>>());
            }

            internal static IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> Union(EqualibleHashSet<CombinedTypesAnd> left, EqualibleHashSet<CombinedTypesAnd> right)
            {
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

                if (!res.Any())
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Other("nothing could union!"));
                }

                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(new EqualibleHashSet<CombinedTypesAnd>(res.Distinct().ToHashSet()));
            }

            private static CombinedTypesAnd Union(CombinedTypesAnd left, CombinedTypesAnd right)
            {
                var start = left.And.Union(right.And).Distinct().ToArray();

                // remove empties
                var v2 = start.Where(x => x.SwitchReturns(y => y.Input.Is(out var _) || y.Output.Is(out var _) || y.Members.Any(), y => true)).ToList();
                // but if you end up removing them all, put one back
                if (!v2.Any())
                {
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

            private static IError[] CanUnion(CombinedTypesAnd left, CombinedTypesAnd right, List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)> assumeTrue, List<(CombinedTypesAnd, CombinedTypesAnd)> assumeTrueInner)
            {
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
            private static IError[] CanUnion(IOrType<ConcreteFlowNode, PrimitiveFlowNode> left, IOrType<ConcreteFlowNode, PrimitiveFlowNode> right, List<(HashSet<CombinedTypesAnd>, HashSet<CombinedTypesAnd>)> assumeTrue, List<(CombinedTypesAnd, CombinedTypesAnd)> assumeTrueInner)
            {
                return left.SwitchReturns(
                    leftConcrete => right.SwitchReturns(
                        rightConcrete => {
                            if (leftConcrete.Input.Is(out var _) && rightConcrete.Members.Any())
                            {
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
                            if (rightConcrete.Members.Any() || rightConcrete.Input.Is(out var _) || rightConcrete.Output.Is(out var _))
                            {

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

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> ToRep(IReadOnlyList<IOrType<Member, Input, Output, Generic>> pathParts)
            {
                // I have some doubt about this...
                // this used to get full to reps of thing we have to return/accept
                // there was some stack overflows there
                // so I split it out.. I now just get what the things we accept accept/ what the thing we return return
                // and I just bring those together one time here
                // but I am not sure it is the same
                //
                // {3C5701EB-2CC9-4D58-8336-D5877F6661EF}
                // TODO test this!
                // I need something with a above+below type inference flowing in to something else with about+below type inference
                // 
                // I think I could have a toRep context with all the cached work for all the node, not just the current one
                // to stop stack overflows
                //
                // let's think this out
                // A must return B, B must accept A 
                // this is clearly one requirement
                // 
                var returns = ToRepReturns(pathParts);
                var accepts = ToRepAccepts(pathParts);

                return returns.SwitchReturns(
                    returnsAnds => accepts.SwitchReturns(
                        acceptsAnds => {

                            {
                                // when there are no constraints on what to return
                                // the constrains on what we accept arn't interstesting
                                // we're an any
                                if (!returnsAnds.Any())
                                {
                                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(returnsAnds);
                                }

                                if (returnsAnds.Any(x=>x.Primitive().Is1(out var possiblyGuid) && possiblyGuid.IsNot()) &&
                                    returnsAnds.VirtualMembers().Is1(out var members) && !members.Any() &&
                                    returnsAnds.VirtualInput().IsNot() &&
                                    returnsAnds.VirtualOutput().IsNot())
                                {
                                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(returnsAnds);
                                }
                            }

                            {
                                // if there are no contraints on what we accept
                                // we just follow the constraints on what we return 
                                if (!acceptsAnds.Any()) {
                                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(returnsAnds);
                                }
                                
                                if (acceptsAnds.Any(x => x.Primitive().Is1(out var possiblyGuid) && possiblyGuid.IsNot()) &&
                                    acceptsAnds.VirtualMembers().Is1(out var members) && !members.Any() &&
                                    acceptsAnds.VirtualInput().IsNot() &&
                                    acceptsAnds.VirtualOutput().IsNot())
                                {
                                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(returnsAnds);
                                }
                            }

                            // must accept A | B
                            // must return C | D
                            // A&C | A&D | B&C | B&D

                            return InferredFlowNode.Union(returnsAnds, acceptsAnds).SwitchReturns(
                                x => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(x),
                                x => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(x));
                        },
                        acceptsError => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(acceptsError)),
                    returnsError => accepts.SwitchReturns(
                        acceptsAnds => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(returnsError),
                        acceptsError => OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(Error.Cascaded("", new[] { returnsError, acceptsError }))));
            }

            public override string? ToString()
            {
                return $"{nameof(InferredFlowNode)}({Source})";
            }


            public EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> UpStreamMustReturn()
            {
                return Returned2;
            }
            public EqualibleHashSet<EqualibleHashSet<CombinedTypesAnd>> DownStreamMustAccept()
            {
                return Acceped2;
            }

        }


        // we don't have a SourcePath so we are not really an IVirtualFlowNode
        public class CombinedTypesAnd// : IVirtualFlowNode
        {
            // we have an output if any of our elements have an output  
            public IIsPossibly<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>> VirtualOutput()
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
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(errors.First()));
                }
                if (errors.Length > 1)
                {
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Cascaded("", errors)));
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
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(errors.First()));
                }
                if (errors.Length > 1)
                {
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Cascaded("", errors)));
                }

                var set = errorCheck2.Select(x => x.Is1OrThrow()).ToArray();

                if (!set.Any())
                {
                    return Possibly.IsNot<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>();
                }

                if (set.Length == 1)
                {
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(set.First()));
                }
                return Possibly.Is(VirtualNode.IsAll(set));
            }

            public IIsPossibly<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>> VirtualInput()
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
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(errors.First()));
                }
                if (errors.Length > 1)
                {
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Cascaded("", errors)));
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
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(errors.First()));
                }
                if (errors.Length > 1)
                {
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Cascaded("", errors)));
                }

                var set = errorCheck2.Select(x => x.Is1OrThrow()).ToArray();

                if (!set.Any()) {
                    return Possibly.IsNot<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>();
                }

                if (set.Length == 1) {
                    return Possibly.Is(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(set.First()));
                }
                return Possibly.Is(VirtualNode.IsAll(set));
            }

            public IOrType<IEnumerable<KeyValuePair<IKey, Lazy<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>>>, IError> VirtualMembers()
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
                    return OrType.Make<IEnumerable<KeyValuePair<IKey, Lazy<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>>>, IError>(Error.Cascaded("", error));
                }

                var set = couldBeErrors.SelectMany(x => x.Is1OrThrow()).ToArray();


                return OrType.Make<IEnumerable<KeyValuePair<IKey, Lazy<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>>>, IError>(set
                        .GroupBy(x => x.Key)
                        .Select(x => new KeyValuePair<IKey, Lazy<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>>(x.Key, new Lazy<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>(()=> {

                            var errors = x.SelectMany(y => {
                                if (y.Value.Value.Is2(out var error))
                                {
                                    return new IError[] { error };
                                }
                                return new IError[] { };
                            }).ToArray();

                            if (errors.Length == 1)
                            {
                                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(errors.First());
                            }
                            if (errors.Length > 1)
                            {
                                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Cascaded("", errors));
                            }

                            var errorCheck = x.Select(y => y.Value.Value.Is1OrThrow().ToRep()).ToArray();

                            errors = errorCheck.SelectMany(y => {
                                if (y.Is2(out var error))
                                {
                                    return new IError[] { error };
                                }
                                return new IError[] { };
                            }).ToArray();

                            if (errors.Length == 1)
                            {
                                return  OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(errors.First());
                            }
                            if (errors.Length > 1)
                            {
                                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Cascaded("", errors));
                            }
                            var set = errorCheck.Select(y => y.Is1OrThrow()).ToArray();

                            return VirtualNode.IsAll(set);
                        })))
                        .ToArray());
            }

            // this looks too much like this
            // {736F2203-BF89-4975-8BFE-7B28406E74C6}
            public IOrType<IReadOnlyList<Lazy<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>>, IError> VirtualGenerics()
            {
                var couldBeErrors = And.Select(x => x.GetValueAs(out IVirtualFlowNode _).VirtualGenerics()).ToArray();

                var error = couldBeErrors.SelectMany(x => {
                    if (x.Is2(out var error))
                    {
                        return new IError[] { error };
                    }
                    return new IError[] { };
                }).ToArray();

                if (error.Any())
                {
                    return OrType.Make<IReadOnlyList<Lazy<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>>, IError>(Error.Cascaded("", error));
                }

                var set = couldBeErrors.Select(x => x.Is1OrThrow()).SelectMany(x => {
                    var i = 0;
                    return x.Select(y => (Key:i++, Value:y)).ToArray();
                }).ToArray();


                return OrType.Make<IReadOnlyList<Lazy<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>>, IError>(set
                        .GroupBy(x => x.Key)
                        .OrderBy(x => x.Key)
                        .Select(x => new Lazy<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>>(() => {

                            var errors = x.SelectMany(y => {
                                if (y.Value.Value.Is2(out var error))
                                {
                                    return new IError[] { error };
                                }
                                return new IError[] { };
                            }).ToArray();

                            if (errors.Length == 1)
                            {
                                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(errors.First());
                            }
                            if (errors.Length > 1)
                            {
                                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Cascaded("", errors));
                            }

                            var errorCheck = x.Select(y => y.Value.Value.Is1OrThrow().ToRep()).ToArray();

                            errors = errorCheck.SelectMany(y => {
                                if (y.Is2(out var error))
                                {
                                    return new IError[] { error };
                                }
                                return new IError[] { };
                            }).ToArray();

                            if (errors.Length == 1)
                            {
                                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(errors.First());
                            }
                            if (errors.Length > 1)
                            {
                                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(Error.Cascaded("", errors));
                            }
                            var set = errorCheck.Select(y => y.Is1OrThrow()).ToArray();

                            return VirtualNode.IsAll(set);
                        }))
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

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> ToRep()
            {
                return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd> { this }));
            }

            //public IIsPossibly<SourcePath> SourcePath()
            //{
            //    // we don't have one
            //    return Possibly.IsNot<SourcePath>();
            //}


            // this shares a lot of code with VirtualOutput and it's friends 
            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> ToRep(IReadOnlyList<IOrType<Member, Input, Output, Generic>> path)
            {
                if (!path.Any())
                {
                    return OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist>(new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd> { this }));
                }

                var errorCheck = And.Select(element => element.GetValueAs(out IVirtualFlowNode _).ToRep(path)).ToArray();

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

                var set = errorCheck.SelectMany(x => {
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
                return $"{nameof(CombinedTypesAnd)}({string.Join(", ", And.Take(10).Select(x => x.ToString())) + (And.Count > 10 ? "..." : "")})";
            }
        }

        public class VirtualNode : IVirtualFlowNode
        {

            public readonly EqualibleHashSet<CombinedTypesAnd> Or;
            private readonly SourcePath sourcePath;

            public VirtualNode(SourcePath sourcePath,EqualibleHashSet<CombinedTypesAnd> or)
            {
                Or = or ?? throw new ArgumentNullException(nameof(or));
                this.sourcePath = sourcePath ?? throw new ArgumentNullException(nameof(sourcePath));
            }

            // you would think we would do equality bases off sourcePath
            // but we use the Or
            // this is used for already flowing
            // we don't want to flow the same representation twice
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
                return Or.Primitive();
            }

            public IOrType<ICollection<KeyValuePair<IKey, Lazy<IOrType<VirtualNode, IError>>>>, IError> VirtualMembers()
            {
                return Or.VirtualMembers().TransformInner(dict => (ICollection<KeyValuePair<IKey, Lazy<IOrType<VirtualNode, IError>>>>)(dict.Select(pair => KeyValuePair.Create(pair.Key, new Lazy<IOrType<VirtualNode, IError>>(()=> pair.Value.Value.TransformInner(ands => new VirtualNode( SourcePath().Member(pair.Key), ands))))).ToArray()));
            }


            public IOrType<IReadOnlyList<Lazy<IOrType<VirtualNode, IError>>>, IError> VirtualGenerics()
            {
                return Or.VirtualGenerics()
                    .TransformInner(list => {
                        int i = 0;
                        return (IReadOnlyList<Lazy<IOrType<VirtualNode, IError>>>)(list.Select(value => new Lazy<IOrType<VirtualNode, IError>>(()=>value.Value.TransformInner(ands => new VirtualNode(SourcePath().Generic(i++), ands)))).ToArray());
                    });
            }

            public SourcePath SourcePath()
            {
                return sourcePath;
            }

            public IOrType<EqualibleHashSet<CombinedTypesAnd>, IError, DoesNotExist> ToRep(IReadOnlyList<IOrType<Member, Input, Output, Generic>> path)
            {
                return Or.ToRep(path);
            }


            // this does not really blong here 
            public static IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> IsAll(IEnumerable<EqualibleHashSet<CombinedTypesAnd>> toMerge)
            {
                if (toMerge.Count() == 0)
                {
                    throw new Exception("well that is unexpected!");
                }

                if (toMerge.Count() == 1)
                {
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

            // this does not really blong here 
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

        public class Generic
        {
            public readonly int index;

            public Generic(int index)
            {
                this.index = index;
            }

            public override bool Equals(object? obj)
            {
                return obj != null && obj is Generic generic && generic.index.Equals(this.index);
            }

            public override int GetHashCode()
            {
                return index;
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





        public class SourcePath
        {
            public readonly IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> source;
            public readonly IReadOnlyList<IOrType<Member, Input, Output, Generic>> path;
            private readonly SourcePathCache cache;

            private SourcePath(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> source, IReadOnlyList<IOrType<Member, Input, Output, Generic>> path, SourcePathCache cache)
            {
                this.source = source ?? throw new ArgumentNullException(nameof(source));
                this.path = path ?? throw new ArgumentNullException(nameof(path));
                this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            }

            public override bool Equals(object? obj)
            {
                return obj is SourcePath inflow &&
                        source.Equals(inflow.source) &&
                        path.SequenceEqual(inflow.path);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var res = source.GetHashCode();
                    foreach (var pathPart in path)
                    {
                        res += pathPart.GetHashCode();
                    }
                    return res;
                }
            }

            public SourcePath Member(IKey key)
            {
                var newList = path.ToList();
                newList.Add(OrType.Make<Member, Input, Output, Generic>(new Member(key)));
                return cache.CreateSourcePath(source, newList);
            }

            // are generics really a "path"?
            // I think it is. virtual flow nodes are defined by thier relation to their parent
            // some are generics defined off their parent 
            public SourcePath Generic(int index)
            {
                var newList = path.ToList();
                newList.Add(OrType.Make<Member, Input, Output, Generic>(new Generic(index)));
                return cache.CreateSourcePath(source, newList);
            }

            public SourcePath Input()
            {
                var newList = path.ToList();
                newList.Add(OrType.Make<Member, Input, Output, Generic>(new Input()));
                return cache.CreateSourcePath(source, newList);

            }

            public SourcePath Output()
            {
                var newList = path.ToList();
                newList.Add(OrType.Make<Member, Input, Output, Generic>(new Output()));
                return cache.CreateSourcePath(source, newList);
            }

            public IOrType<IVirtualFlowNode, IError> Walk()
            {

                var result = source.GetValueAs(out IVirtualFlowNode _);
                foreach (var pathElement in path)
                {
                    var couldBeError = pathElement.SwitchReturns(
                            x => result.ToRep(new IOrType<Member, Input, Output, Generic>[] { OrType.Make<Member, Input, Output, Generic>(new Member(x.key)) }).SwitchReturns(
                                inner => (IOrType<IVirtualFlowNode, IError>)OrType.Make<IVirtualFlowNode, IError>(new VirtualNode(result.SourcePath().Member(x.key), inner)),
                                error => (IOrType<IVirtualFlowNode, IError>)OrType.Make<IVirtualFlowNode, IError>(error),
                                _ => (IOrType<IVirtualFlowNode, IError>)OrType.Make<IVirtualFlowNode, IError>(Error.Other($"member does not exist {x.key}"))),  //.VirtualMembers().SwitchReturns(inner=> inner.Where(y => y.Key.Equals(x.key)).Single().Value, error=> (IOrType<IVirtualFlowNode, IError>) OrType.Make< IVirtualFlowNode, IError >(error)),
                            x => result.VirtualInput().GetOrThrow(),
                            x => result.VirtualOutput().GetOrThrow(),
                            x => result.VirtualGenerics().SwitchReturns(
                                inner => inner[x.index].Value,
                                error => (IOrType<IVirtualFlowNode, IError>)OrType.Make<IVirtualFlowNode, IError>(error)
                                ));
                    if (couldBeError.Is2(out var error))
                    {
                        return OrType.Make<IVirtualFlowNode, IError>(error);
                    }
                    else
                    {
                        result = couldBeError.Is1OrThrow();
                    }
                }
                return OrType.Make<IVirtualFlowNode, IError>(result);
            }

            public override string? ToString()
            {
                return $"SourcePath({source}, {string.Join(", ", path.Take(10).Select(x => x.ToString())) + (path.Count > 10 ? "..." : "")})";
            }

            // we need to manage SourcePath creation
            public class SourcePathCache
            {
                Dictionary<SourcePath, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> cache = new Dictionary<SourcePath, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();

                public SourcePath CreateSourcePath(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> source, IReadOnlyList<IOrType<Member, Input, Output, Generic>> path)
                {
                    for (int i = path.Count; i > 0; i--)
                    {
                        if (cache.TryGetValue(new SourcePath(source, path.Take(i).ToArray(), this), out var res))
                        {
                            if (i == path.Count)
                            {
                                return new SourcePath(res, Array.Empty<IOrType<Member, Input, Output, Generic>>(), this);
                            }
                            return CreateSourcePath(res, path.Skip(i).ToArray());
                        }
                    }
                    {
                        return new SourcePath(source, path, this);
                    }
                }

                internal void AddGeneric(ConcreteFlowNode concreteFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> value, int i)
                {
                    var path = new SourcePath(OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(concreteFlowNode), new IOrType<Member, Input, Output, Generic>[] { OrType.Make<Member, Input, Output, Generic>(new Tpn.Generic(i)) }, this);
                    cache[path] = value;
                }

                internal void AddInput(ConcreteFlowNode concreteFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> value)
                {
                    var path = new SourcePath(OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(concreteFlowNode), new IOrType<Member, Input, Output, Generic>[] { OrType.Make<Member, Input, Output, Generic>(new Tpn.Input()) }, this);
                    cache[path] = value;
                }

                internal void AddMember(ConcreteFlowNode source, IKey key, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> value)
                {
                    var path = new SourcePath(OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(source), new IOrType<Member, Input, Output, Generic>[] { OrType.Make<Member, Input, Output, Generic>(new Tpn.Member(key)) }, this);
                    cache[path] = value;
                }

                internal void AddOutput(ConcreteFlowNode concreteFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> value)
                {
                    var path = new SourcePath(OrType.Make<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>(concreteFlowNode), new IOrType<Member, Input, Output, Generic>[] { OrType.Make<Member, Input, Output, Generic>(new Tpn.Output()) }, this);
                    cache[path] = value;
                }
            }
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



// I don't think anything need to be virtual
// I can create nodes for anything I need
// and keep track of them off what I have 
// and then I can flow up stream
// this is possibly because we are really just building constrains... right?
// 

// maybe things aren't so dynamic
// must returns go up
// must acceps go up
// but we jsut flow primitives and concretes 
// this is possibly because we are really just building constrains... right?

// infered nodes are created as needed when they must accept a constraint
// constraints are just concrete and primitive 
// ... or maybe just primitive ... constrtaint would just build nextwrok
// ... concrete nodes will force members and IO to exist

// that's a big change 
// if I do it concretely like this it's going to get werid
// a =: a.x
//
//
// infered a
// concrete with x
// infered x
// infered a <- concrete a
// infered a -> infered x
//      which flows infered a.x -> infered x.x
//      
// we create an x off the infered
//
//
// I need to be able to flow must accept up stream
// but I can't because upstream is often in to Virtual nodes
//  x =: type { bool|string a;} z
//  x =: type { a;} y
//  5 =: y.a
//  x.a must accept 5 and must retunr bool|string
//  it is heavily constrained and virtual
//
// hmmmmmmmmm