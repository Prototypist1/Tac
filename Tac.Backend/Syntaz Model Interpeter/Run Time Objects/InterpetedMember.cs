using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Linq;
using System.Reflection;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    public interface IInterpetedMember : IInterpetedAnyType {
        IVerifiableType VerifiableType { get; }
    }

    public interface IInterpetedMember<out T> : IInterpetedMember
        where T: IInterpetedAnyType
    {
        T Value { get;  }
    }

    public interface IInterpetedMemberSet<in T>
    {
        bool TrySet(IInterpetedAnyType o);
        void Set(T o);
    }

    public static partial class TypeManager
    {

        internal static IInterpetedMember MakeMember(IVerifiableType type)
        {
            var method = typeof(TypeManager).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(x =>
              x.Name == nameof(Member) && x.IsGenericMethod && x.GetParameters().Count() == 1);
            var made = method.MakeGenericMethod(new Type[] { TypeMap.MapType(type) });
            return made.Invoke(null,new object[] { type }).CastTo<IInterpetedMember>();
        }

        public static IInterpetedMember<IInterpetedAnyType> AnyMember()
            => Member<IInterpetedAnyType>(new AnyType());

        public static IInterpetedMember<IInterpetedAnyType> AnyMember(IInterpetedAnyType t)
            => Member(new AnyType(), t);

        public static IInterpetedMember<IInterpedEmpty> EmptyMember()
            => Member<IInterpedEmpty>(new EmptyType());

        public static IInterpetedMember<IInterpedEmpty> EmptyMember(IInterpedEmpty t)
            => Member(new EmptyType(), t);

        public static IInterpetedMember<IBoxedDouble> NumberMember(IBoxedDouble t)
            => Member(new NumberType(), t);

        public static IInterpetedMember<IBoxedDouble> NumberMember()
            => Member<IBoxedDouble>(new NumberType());

        public static IInterpetedMember<IBoxedString> StringMember(IBoxedString t)
            => Member(new StringType(), t);

        public static IInterpetedMember<IBoxedString> StringMember()
            => Member<IBoxedString>(new StringType());

        public static IInterpetedMember<IBoxedBool> BoolMember(IBoxedBool t)
            => Member(new BooleanType(), t);

        public static IInterpetedMember<IBoxedBool> BoolMember()
            => Member<IBoxedBool>(new BooleanType());

        public static IInterpetedMember<T> Member<T>(IVerifiableType type,T t)
            where T : IInterpetedAnyType
            => Root(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { MemberIntention<T>(type,t) }).Has<IInterpetedMember<T>>();

        public static IInterpetedMember<T> Member<T>(IVerifiableType type)
            where T : IInterpetedAnyType
            => Root(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { MemberIntention<T>(type) }).Has<IInterpetedMember<T>>();

        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> MemberIntention<T>(IVerifiableType type)
            where T : IInterpetedAnyType
            // TODO check that IVerifiableType is T
            => root => new RunTimeAnyRootEntry(new InterpetedMember<T>(type,root), type);

        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> MemberIntention<T>(IVerifiableType type,T t)
            where T : IInterpetedAnyType
            // TODO check that IVerifiableType is T
            => root => new RunTimeAnyRootEntry( new InterpetedMember<T>(type,t, root), type);


        // is this really a type?
        // yeah, I think this is really like ref x
        // ref x is exactly a type
        private class InterpetedMember<T> : RootedTypeAny, IInterpetedMember<T>, IInterpetedMemberSet<T>
            where T : IInterpetedAnyType
        {

            // you are here!
            // I don't think this is really a verifiable type!
            // it does have to be a type of some sort
            public IVerifiableType VerifiableType { get; }

            private T _value;

            public InterpetedMember(IVerifiableType verifiableType,IRunTimeAnyRoot root) : base(root)
            {
                this.VerifiableType = verifiableType ?? throw new ArgumentNullException(nameof(verifiableType));
            }

            public InterpetedMember(IVerifiableType verifiableType, T value, IRunTimeAnyRoot root) : base(root)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                Value = value;
                this.VerifiableType = verifiableType ?? throw new ArgumentNullException(nameof(verifiableType));
            }

            public T Value
            {
                get
                {
                    if (_value == null)
                    {
                        throw new Exception($"members must be initialized before they are read");
                    }

                    return _value;
                }
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }

                    _value = value;
                }
            }

            public void Set(T o)
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
                if (VerifiableType.TheyAreUs(incommingType, false)) {
                    Set(o.CastTo<T>());
                    return true;
                }
                return false;
            }
        }

    }

}