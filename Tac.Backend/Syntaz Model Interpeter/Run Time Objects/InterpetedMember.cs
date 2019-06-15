using Prototypist.LeftToRight;
using System;
using System.Linq;
using System.Reflection;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    public interface IInterpetedMember : IInterpetedAnyType { }

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
              x.Name == nameof(Member) && x.IsGenericMethod && x.GetParameters().Count() == 0);
            var made = method.MakeGenericMethod(new Type[] { TypeMap.MapType(type) });
            return made.Invoke(null,new object[] { }).Cast<IInterpetedMember>();
        }

        public static IInterpetedMember<T> Member<T>(IVerifiableType type,T t)
            where T : IInterpetedAnyType 
            => Root(new Func<IRunTimeAnyRoot, IInterpetedAnyType>[] { MemberIntention<T>(type,t) }).Has<IInterpetedMember<T>>();

        public static IInterpetedMember<T> Member<T>(IVerifiableType type)
            where T : IInterpetedAnyType
            => Root(new Func<IRunTimeAnyRoot, IInterpetedAnyType>[] { MemberIntention<T>(type) }).Has<IInterpetedMember<T>>();

        public static Func<IRunTimeAnyRoot, IInterpetedMember<T>> MemberIntention<T>(IVerifiableType type)
            where T : IInterpetedAnyType
            => root => new InterpetedMember<T>(type,root);

        public static Func<IRunTimeAnyRoot, IInterpetedMember<T>> MemberIntention<T>(IVerifiableType type,T t)
            where T : IInterpetedAnyType
            => root => new InterpetedMember<T>(type,t, root);


        // is this really a type?
        // yeah, I think this is really like ref x
        // ref x is exactly a type
        private class InterpetedMember<T> : RootedTypeAny, IInterpetedMember<T>, IInterpetedMemberSet<T>
            where T : IInterpetedAnyType
        {

            // you are here!
            // I don't think this is really a verifiable type!
            // it does have to be a type of some sort
            private readonly IVerifiableType verifiableType;

            private T _value;

            public InterpetedMember(IVerifiableType verifiableType,IRunTimeAnyRoot root) : base(root)
            {
                this.verifiableType = verifiableType ?? throw new ArgumentNullException(nameof(verifiableType));
            }

            public InterpetedMember(IVerifiableType verifiableType, T value, IRunTimeAnyRoot root) : base(root)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                Value = value;
                this.verifiableType = verifiableType ?? throw new ArgumentNullException(nameof(verifiableType));
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
                Value = o.Cast<T>();
            }

            public bool TrySet(IInterpetedAnyType o)
            {
                throw new NotImplementedException();
            }
        }

    }

}