using Prototypist.Toolbox;
using System;
using System.Diagnostics.CodeAnalysis;
using Tac.Model;

namespace Tac.Frontend.New.CrzayNamespace
{
    internal partial class Tpn
    {
        internal abstract class TypeRequirementWithOr : OrType<
           MustHave,
           MustBePrimitive,
           GivenPathThen,
           OrConstraint,
           HasMembers,
           IsGeneric>
        {
            public static TypeRequirementWithOr Make(MustHave contents)
            {
                return new Inner<MustHave>(contents);
            }
            public static TypeRequirementWithOr Make(MustBePrimitive contents)
            {
                return new Inner<MustBePrimitive>(contents);
            }
            public static TypeRequirementWithOr Make(GivenPathThen contents)
            {
                return new Inner<GivenPathThen>(contents);
            }
            public static TypeRequirementWithOr Make(OrConstraint contents)
            {
                return new Inner<OrConstraint>(contents);
            }
            public static TypeRequirementWithOr Make(HasMembers contents)
            {
                return new Inner<HasMembers>(contents);
            }
            public static TypeRequirementWithOr Make(IsGeneric contents)
            {
                return new Inner<IsGeneric>(contents);
            }

            public static TypeRequirementWithOr Make(IError contents)
            {
                return new Inner<IError>(contents);
            }

            private class Inner<T> : TypeRequirementWithOr, IIsDefinately<T>
            {
                public Inner(T value)
                {
                    this.Value = value ?? throw new ArgumentNullException(nameof(value));
                }

                [NotNull]
                public T Value { get; }

                public override object Representative() => Value;
            }
        }
    }
}


// this generic thing is pretty complex
// I think they do flow

// there is sort of two contexts
// 1 - 
//
// method [T] [T,T] a;
// c =: a;
//
// c is method [T] [T,T] but it has it's own T
// 
// 2 - 
// 
// method [T] [T,T] input {
//  a =: input
//  return input;
// }
//
// a is T
//
// in both cases the genericness flows

// are these two case different constraint?
//
// I need to think through a few more cases...
//
// method [T1] [T1, method [T] [T, T1]] input {
//      method[T][T, T1] a;
//      c =: a;
//      a return;
// }
//
// c is method [T] [T,T1] it has it's own T but T1 is shared
// so..
// if I am generic, I make other things the same generic
// if I have a generic I make other things have their own gernic
// but those are just different ways of saying the same thing...
// from the prospective of a's input, I'm generic
// from the prospective of a, I have a generic
//
// I need to name my cases "has" vs "is"
// 
// I think the key this is how they look up where they come from
// it is sort of a inverse path
// a's input looks up to [input] and then takes the first generic
// a's output looks up to [output, a] and then takes the first generic ... except it is actually just a different kind of constraint
//
// so really we have relationships
// like a's input is it's first generic
// maybe the constrait defining this relationship live on "a"?
// the thing about how it works now is: it can't flow very far
// a generic method has to be realized for befor you can call it
// so it can only flow by assigning to "a"
// 
// a's output is just T1




// ugh, idk
// method[T][int, method [T1][T,T1]] x
// method[T][string, method [T1][T,T1]] y
// 
// the inner method here combine
// but the outer methods don't 
// and so the inner methods shouldn't 
// 
// if we had
// method[T][int, method [T1][T,T1]] x
// method[T][int, method [T1][T,T1]] y
// then everythig combines but probably nothing would break if it didn't 
// it is going to be possible to assigne them to each other, even if they aren't the same object
// 
// only generics should be context dependent
// they can exist on many paths
// but all paths should look up to the same thing
// 
// so the source is one of the features that defines identity for generic constraints 












// what about external types?
// 









// Yolo's aren't 1-1 with types
// it's Yolo + context -> type
// context is a stack of Yolos
// 
// when you look something up you do it like: what is Yolo-X in Yolo-Y?
// which might turn around and do deeper looks like Yolo-Z in [Yolo-X, Yolo-Y]
// sometimes you can't figure it out maybe Yolo-Z in Yolo-X isn't well defined
// and that's ok
// at somepoint we'll look at Yolo-Y and that will give us the context we need 
//
// but.. that is going to break a lot of these apis... GetMethodType, GetObjectType
// I think I just have to pass it on and hope the call-e knowns the context




// oof generics again
// so two things can look up to the same generic type
// and they can have two different paths to owner
// often you know exactly who generic you are
//
// method [t] [t,t] a
// b =: a
//
// here flowing the way I do makes a lot of sense
//
// ...
// 
// ok so the plan now is paired constrains generic and generic source
// generic and generic source are paired 
// 
// but I really already have what I need 
// 