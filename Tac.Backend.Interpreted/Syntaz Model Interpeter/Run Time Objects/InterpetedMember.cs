using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Linq;
using System.Reflection;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;

namespace Tac.Backend.Interpreted.SyntazModelInterpeter
{

    public interface IInterpetedMember : IInterpetedAnyType {
        IVerifiableType VerifiableType { get; }
        IInterpetedAnyType Value { get; }
    }


    public interface IInterpetedMemberSet
    {
        bool TrySet(IInterpetedAnyType o);
        void Set(IInterpetedAnyType o);
    }

    public static partial class TypeManager
    {

        public static IInterpetedMember AnyMember()
            => Member(new AnyType());

        public static IInterpetedMember AnyMember(IInterpetedAnyType t)
            => Member(new AnyType(), t);

        public static IInterpetedMember EmptyMember()
            => Member(new EmptyType());

        public static IInterpetedMember EmptyMember(IInterpedEmpty t)
            => Member(new EmptyType(), t);

        public static IInterpetedMember NumberMember(IBoxedDouble t)
            => Member(new NumberType(), t);

        public static IInterpetedMember NumberMember()
            => Member(new NumberType());

        public static IInterpetedMember StringMember(IBoxedString t)
            => Member(new StringType(), t);

        public static IInterpetedMember StringMember()
            => Member(new StringType());

        public static IInterpetedMember BoolMember(IBoxedBool t)
            => Member(new BooleanType(), t);

        public static IInterpetedMember BoolMember()
            => Member(new BooleanType());

        public static IInterpetedMember Member(IVerifiableType type, IInterpetedAnyType t)
            => Root(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { MemberIntention(type,t) }).Has<IInterpetedMember>();

        public static IInterpetedMember Member(IVerifiableType type)
            => Root(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { MemberIntention(type) }).Has<IInterpetedMember>();

        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> MemberIntention(IVerifiableType type)
            // TODO check that IVerifiableType is T
            => root => new RunTimeAnyRootEntry(new InterpetedMember(type,root), type);

        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> MemberIntention(IVerifiableType type, IInterpetedAnyType t)
            // TODO check that IVerifiableType is T
            => root => new RunTimeAnyRootEntry( new InterpetedMember(type,t, root), type);


        // is this really a type?
        // yeah, I think this is really like ref x
        // ref x is exactly a type
        private class InterpetedMember : RootedTypeAny, IInterpetedMember, IInterpetedMemberSet
        {

            // 
            // I don't think this is really a verifiable type!
            // it does have to be a type of some sort
            public IVerifiableType VerifiableType { get; }


            private IIsPossibly<IInterpetedAnyType> _value = Possibly.IsNot<IInterpetedAnyType>();

            public InterpetedMember(IVerifiableType verifiableType,IRunTimeAnyRoot root) : base(root)
            {
                this.VerifiableType = verifiableType ?? throw new ArgumentNullException(nameof(verifiableType));
            }

            public InterpetedMember(IVerifiableType verifiableType, IInterpetedAnyType value, IRunTimeAnyRoot root) : base(root)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                Value = value;
                this.VerifiableType = verifiableType ?? throw new ArgumentNullException(nameof(verifiableType));
            }

            public IInterpetedAnyType Value
            {
                get
                {
                    if (_value is IIsDefinately<IInterpetedAnyType> definately)
                    {
                        return definately.Value;

                    }
                    throw new Exception($"members must be initialized before they are read");
                }
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }

                    _value = Possibly.Is(value);
                }
            }

            public void Set(IInterpetedAnyType o)
            {
                Value = o;
            }

            public bool TrySet(IInterpetedAnyType o)
            {
                //  having to find TransformerExtensions.NewConversionContext is cognative load
                // and I always use TransformerExtensions.NewConversionContext
                // what even is the point of passing it
                var incommingType = o.Convert(TransformerExtensions.NewConversionContext());
                // also don't like passing in false here
                // forces me to know too much about that is going on under the hood 



                if (VerifiableType.TheyAreUs(incommingType, new System.Collections.Generic.List<(IVerifiableType, IVerifiableType)>()))
                {
                    Set(o.CastTo<IInterpetedAnyType>());
                    return true;
                }
                return false;
            }
        }

    }

}