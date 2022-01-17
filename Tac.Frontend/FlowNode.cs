using Prototypist.TaskChain;
using Prototypist.Toolbox;
using Prototypist.Toolbox.Dictionary;
using Prototypist.Toolbox.IEnumerable;
using Prototypist.Toolbox.Object;
using System;
using System.Dynamic;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Frontend.New.CrzayNamespace
{
    internal partial class Tpn
    {
        public static IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2> ToOr2(ConcreteFlowNode2 node)
        {
            return OrType.Make<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>(node);
        }
        public static IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2> ToOr2(InferredFlowNode2 node)
        {
            return OrType.Make<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>(node);
        }
        public static IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2> ToOr2(PrimitiveFlowNode2 node)
        {
            return OrType.Make<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>(node);
        }
        public static IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2> ToOr2(OrFlowNode2 node)
        {
            return OrType.Make<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>(node);
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

            public override string ToString()
            {
                return nameof(Input)+"()"; 
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

            public override string ToString()
            {
                return nameof(Output) + "()";
            }
        }

        public class Generic
        {
            /// <summary>
            /// zero indexed
            /// </summary>
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

            public override string ToString()
            {
                return $"{nameof(Generic)}({index})";
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

            public override string ToString()
            {
                return $"{nameof(Member)}({key})";
            }
        }

        public class PrivateMember
        {
            public readonly IKey key;

            public PrivateMember(IKey key)
            {
                this.key = key ?? throw new ArgumentNullException(nameof(key));
            }

            public override bool Equals(object? obj)
            {
                return obj != null && obj is PrivateMember member && member.key.Equals(this.key);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return key.GetHashCode() + Guid.Parse("{AE1C5BE0-C236-4D42-95FC-9108A71C702A}").GetHashCode();
                }
            }

            public override string ToString()
            {
                return $"{nameof(PrivateMember)}({key})";
            }
        }

        public class Left
        {
            public override bool Equals(object? obj)
            {
                return obj != null && obj is Left;
            }

            public override int GetHashCode()
            {
                // why do I do this? it is probably really slow 
                return Guid.Parse("{5BACE1D7-5A10-4B9E-8565-4897D4929545}").GetHashCode();
            }

            public override string ToString()
            {
                return $"{nameof(Left)}()";
            }
        }

        public class Right
        {

            public Right()
            {
            }

            public override bool Equals(object? obj)
            {
                return obj != null && obj is Right;
            }

            public override int GetHashCode()
            {
                return Guid.Parse("{F4CE4BAA-93D1-4D6D-986B-D543DD5DD6E7}").GetHashCode();
            }
            public override string ToString()
            {
                return $"{nameof(Right)}()";
            }
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