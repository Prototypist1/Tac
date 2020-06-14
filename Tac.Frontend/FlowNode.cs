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
        public interface IFlowNode<TSource>: IFlowNode
        {

            IIsPossibly<TSource> Source { get; }
        }

        public interface IFlowNode {

            bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from);
            bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from);
            IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualOutput();
            IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualInput();
            IEnumerable<KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>> VirtualMembers();
        }

        public class PrimitiveFlowNode: IFlowNode
        {

            public bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from) {
                return from.Is3(out var v3) && Equals(v3, this);
            }
            public bool Flow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from) {
                return false;
            }

            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualInput() 
            {
                return Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
            }

            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualOutput()
            {
                return Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
            }

            public IEnumerable<KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>> VirtualMembers() {
                return new Dictionary<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
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

            public IEnumerable<KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>> VirtualMembers() => Members;
            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualInput() => Input;
            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualOutput() => Output;

            public bool CanFlow(IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> from) {
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
                    if (!CanFlowMember(fromMember))
                    {
                        return false;
                    }
                }

                if (Input.Is(out var input) && from.GetValueAs(out IFlowNode _).VirtualInput().Is(out var theirInput)) {
                    if (!input.GetValueAs(out IFlowNode _).CanFlow(theirInput)) {
                        return false;
                    }
                }

                if (Output.Is(out var output) && from.GetValueAs(out IFlowNode _).VirtualOutput().Is(out var theirOutput))
                {
                    if (!output.GetValueAs(out IFlowNode _).CanFlow(theirOutput))
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
                foreach (var fromMember in from.GetValueAs(out IFlowNode _).VirtualMembers())
                {
                    changes |= FlowMember(fromMember);
                }

                if (Input.Is(out var input) && from.GetValueAs(out IFlowNode _).VirtualInput().Is(out var theirInput))
                {
                    changes |= input.GetValueAs(out IFlowNode _).Flow(theirInput);
                }

                if (Output.Is(out var output) && from.GetValueAs(out IFlowNode _).VirtualOutput().Is(out var theirOutput))
                {
                    changes |= output.GetValueAs(out IFlowNode _).Flow(theirOutput);
                }

                return changes;
            }

            private bool CanFlowMember( KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> fromMember)
            {
                if (this.Members.TryGetValue(fromMember.Key, out var toMember))
                {
                    return toMember.GetValueAs(out IFlowNode _).CanFlow(fromMember.Value);
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
                    changes |= toMember.GetValueAs(out IFlowNode _).Flow(fromMember.Value);
                }
                return changes;
            }
        }

        public class OrFlowNode : IFlowNode<TypeProblem2.OrType>
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
            public OrFlowNode(IReadOnlyList<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> or, IIsPossibly< TypeProblem2.OrType> source)
            {
                Or = or ?? throw new ArgumentNullException(nameof(or));
                Source = source;
            }

            public IReadOnlyList<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> Or { get; }

            public IIsPossibly<TypeProblem2.OrType> Source {get;}

            // these should only be used for from
            public IEnumerable<KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>> VirtualMembers()
            {
                // this is the intersection 
                var count = Or.Count();
                return Or.SelectMany(x => x.GetValueAs(out IFlowNode _).VirtualMembers())
                        .GroupBy(x => x.Key).Where(x => x.Count() == count)
                        .Select(x =>
                        {
                            var res = new OrFlowNode(x.Select(y => y.Value).ToList(), Possibly.IsNot<TypeProblem2.OrType>());
                            return new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(x.Key, ToOr(res));
                        });
            }

            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualInput()
            {
                var list = Or.Select(x => x.SwitchReturns(
                             y => y.Input,
                             y => y.VirtualInput(),
                             y => y.VirtualInput(),
                             y => y.VirtualInput())).OfType<IIsDefinately<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>>().Select(x=> x.Value).ToArray();

                if (list.Length != Or.Count) {
                    return Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
                }
                return Possibly.Is(ToOr(new OrFlowNode(list, Possibly.IsNot<TypeProblem2.OrType>() )));
            }

            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualOutput()
            {
                var list = Or.Select(x => x.SwitchReturns(
                     y => y.Output,
                     y => y.VirtualOutput(),
                     y => y.VirtualOutput(),
                     y => y.VirtualOutput())).OfType<IIsDefinately<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>>().Select(x => x.Value).ToArray();

                if (list.Length != Or.Count)
                {
                    return Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
                }
                return Possibly.Is(ToOr(new OrFlowNode(list, Possibly.IsNot<TypeProblem2.OrType>())));
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
            public IEnumerable<KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>> VirtualMembers()
            {

                // this is the intersection 
                return Or.SelectMany(x => x.VirtualMembers())
                    .GroupBy(x => x.Key).Where(x => x.Count() == Or.Count)
                    .Select(x =>
                    {
                        var res = new InferredFlowNode();
                        res.Or = x.Select(y => y.Value).ToList();

                        return new KeyValuePair<IKey, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>(x.Key, ToOr(res));
                    });
            }

            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualInput()
            {
                var list = Or.Select(x => x.VirtualInput()).OfType<IIsDefinately<CombinedTypesAnd>>().Select(x => x.Value).ToList();

                if (list.Count != Or.Count)
                {
                    return Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
                }
                var res = new InferredFlowNode();
                res.Or = list;

                return Possibly.Is(ToOr(res));
            }

            public IIsPossibly<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> VirtualOutput()
            {
                var list = Or.Select(x => x.VirtualOutput()).OfType<IIsDefinately<CombinedTypesAnd>>().Select(x => x.Value).ToList();

                if (list.Count != Or.Count)
                {
                    return Possibly.IsNot<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
                }
                var res = new InferredFlowNode();
                res.Or = list;

                return Possibly.Is(ToOr(res));
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
                    return And.SelectMany(x => x.GetValueAs(out IFlowNode _).VirtualMembers())
                            .GroupBy(x => x.Key).Where(x => x.Count() == count)
                            .Select(x =>
                            {
                                var res = new CombinedTypesAnd(x.Select(y => y.Value).ToHashSet());
                                return new KeyValuePair<IKey, CombinedTypesAnd>(x.Key, res);
                            });
                }


                public IIsPossibly<CombinedTypesAnd> VirtualInput()
                {
                    var set = And.Select(x => x.SwitchReturns(
                                 y => y.Input,
                                 y => y.VirtualInput(),
                                 y => y.VirtualInput(),
                                 y => y.VirtualInput())).OfType<IIsDefinately<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>>().Select(x => x.Value).ToHashSet();

                    if (set.Count != And.Count())
                    {
                        return Possibly.IsNot<CombinedTypesAnd>();
                    }
                    return Possibly.Is(new CombinedTypesAnd(set));
                }

                public IIsPossibly<CombinedTypesAnd> VirtualOutput()
                {
                    var set = And.Select(x => x.SwitchReturns(
                         y => y.Output,
                         y => y.VirtualOutput(),
                         y => y.VirtualOutput(),
                         y => y.VirtualOutput())).OfType<IIsDefinately<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>>().Select(x => x.Value).ToHashSet();

                    if (set.Count != And.Count())
                    {
                        return Possibly.IsNot<CombinedTypesAnd>();
                    }
                    return Possibly.Is(new CombinedTypesAnd(set));
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

                // please don't add to this, it will change the HashCode
                public HashSet<IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> And { get; }

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

        public static void Solve(IReadOnlyList<IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>> ors) {
            var orsToFlowNodes = new Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();

            foreach (var methodType in ors.Select(x => (x.Is1(out var v), v)).Where(x => x.Item1).Select(x => x.v))
            {
                orsToFlowNodes.Add(Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(methodType), ToOr(new ConcreteFlowNode<Tpn.TypeProblem2.MethodType>(methodType)));
            }
            foreach (var type in ors.Select(x => (x.Is2(out var v), v)).Where(x => x.Item1).Select(x => x.v))
            {
                orsToFlowNodes.Add(Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(type), ToOr(new ConcreteFlowNode<Tpn.TypeProblem2.Type>(type)));
            }
            foreach (var @object in ors.Select(x => (x.Is3(out var v), v)).Where(x => x.Item1).Select(x => x.v))
            {
                orsToFlowNodes.Add(Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(@object), ToOr(new ConcreteFlowNode<Tpn.TypeProblem2.Object>(@object)));
            }
            foreach (var inferred in ors.Select(x => (x.Is5(out var v), v)).Where(x => x.Item1).Select(x => x.v))
            {
                orsToFlowNodes.Add(Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(inferred), ToOr(new ConcreteFlowNode<Tpn.TypeProblem2.InferredType>(inferred)));
            }
            foreach (var error in ors.Select(x => (x.Is6(out var v), v)).Where(x => x.Item1).Select(x => x.v))
            {
                orsToFlowNodes.Add(Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(error), ToOr(new ConcreteFlowNode<IError>(error)));
            }

            var todo = ors.Select(x => (x.Is4(out var v), v)).Where(x => x.Item1).Select(x => x.v).ToArray();
            var excapeValve = 0;

            // or types are a bit of a project because they might depend on each other
            while (todo.Any())
            {
                excapeValve++;
                var nextTodo = new List<TypeProblem2.OrType>();
                foreach (var or in todo)
                {
                    if (TryToOuterFlowNode(orsToFlowNodes, or, out var res))
                    {
                        orsToFlowNodes[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(or)] = res;
                    }
                    else
                    {
                        nextTodo.Add(or);
                    }
                }
                todo = nextTodo.ToArray();
                if (excapeValve > 100000)
                {
                    throw new Exception("we are probably stuck");
                }
            }


            // we create members on our new representation
            foreach (var hasPublicMembers in ors.Select(x => (x.Is(out IHavePublicMembers members), members)).Where(x => x.Item1).Select(x => x.members))
            {
                foreach (var member in hasPublicMembers.PublicMembers)
                {
                    orsToFlowNodes[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(hasPublicMembers)].Is1OrThrow().Members.Add(
                        member.Key,
                        orsToFlowNodes[member.Value.LooksUp.GetOrThrow().SwitchReturns(
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))]);
                }
            }

            // we create input and output on our new implmentation
            foreach (var hasInputAndOutput in ors.Select(x => (x.Is(out IHaveInputAndOutput io), io)).Where(x => x.Item1).Select(x => x.io))
            {
                if (hasInputAndOutput.Input.Is(out var input))
                {
                    orsToFlowNodes[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(hasInputAndOutput)].Is1OrThrow().Input = 
                                Possibly.Is( orsToFlowNodes[input.LooksUp.GetOrThrow().SwitchReturns(
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))]);
                }
                if (hasInputAndOutput.Returns.Is(out var output))
                {
                    orsToFlowNodes[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(hasInputAndOutput)].Is1OrThrow().Output = Possibly.Is(
                            orsToFlowNodes[output.LooksUp.GetOrThrow().SwitchReturns(
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))]);
                }
            }

            List<(ILookUpType, ILookUpType)> assignments = null;

            excapeValve = 0;

            bool go;
            do
            {
                go = false;

                foreach (var (from, to) in assignments)
                {
                    var toType = orsToFlowNodes[to.LooksUp.GetOrThrow().SwitchReturns(
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))];
                    var fromType = orsToFlowNodes[from.LooksUp.GetOrThrow().SwitchReturns(
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))];

                    if (fromType.GetValueAs(out IFlowNode _).CanFlow(toType))
                    {
                        go |= fromType.GetValueAs(out IFlowNode _).Flow(toType);
                    }
                }

                excapeValve++;
                if (excapeValve > 1000000)
                {
                    throw new Exception("probably stuck in a loop");
                }

            } while (go);


        }


        private static bool TryToOuterFlowNode(Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> orsToFlowNodes, Tpn.TypeProblem2.OrType or, out IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> res) {
            if (orsToFlowNodes.TryGetValue(GetType(or.Left.GetOrThrow()).SwitchReturns(
                                       x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                       x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                       x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                       x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                       x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                       x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x)), out var left) &&
                       orsToFlowNodes.TryGetValue(GetType(or.Right.GetOrThrow()).SwitchReturns(
                                       x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                       x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                       x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                       x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                       x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                       x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x)), out var right))
            {

                res = ToOr( new OrFlowNode(new[] { left,right}, Possibly.Is(or)));
                return true;
            }
            res = default;
            return false;
        }

        static IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError> GetType(ITypeProblemNode value) { 
        
        }
    }
}
