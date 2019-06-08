using Prototypist.LeftToRight;
using System;
using System.Linq;
using System.Reflection;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal interface IInterpetedMember : IInterpetedAnyType { }

    internal interface IInterpetedMember<out T> : IInterpetedMember
        where T: IInterpetedAnyType
    {
        T Value { get;  }
    }

    public interface IInterpetedMemberSet<in T>
    {
        void Set(T o);
    }



    internal static partial class TypeManager
    {

        internal static IInterpetedMember MakeMember(IVerifiableType type)
        {
            var method = typeof(TypeManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(x =>
              x.Name == nameof(Member) && x.IsGenericMethod);
            var made = method.MakeGenericMethod(new Type[] { TypeMap.MapType(type) });
            return made.Invoke(null,new object[] { }).Cast<IInterpetedMember>();
        }

        public static IInterpetedMember<T> Member<T>(T t)
            where T : IInterpetedAnyType 
            => new RunTimeAnyRoot(new Func<RunTimeAnyRoot, IInterpetedAnyType>[] { MemberIntention<T>(t) }).Has<IInterpetedMember<T>>();

        public static IInterpetedMember<T> Member<T>()
            where T : IInterpetedAnyType
            => new RunTimeAnyRoot(new Func<RunTimeAnyRoot, IInterpetedAnyType>[] { MemberIntention<T>() }).Has<IInterpetedMember<T>>();

        public static Func<RunTimeAnyRoot, IInterpetedMember<T>> MemberIntention<T>()
            where T : IInterpetedAnyType
            => root => new InterpetedMember<T>(root);

        public static Func<RunTimeAnyRoot, IInterpetedMember<T>> MemberIntention<T>(T t)
            where T : IInterpetedAnyType
            => root => new InterpetedMember<T>(t,root);


        // is this really a type?
        // yeah, I think this is really like ref x
        // ref x is exactly a type
        private class InterpetedMember<T> : RootedTypeAny, IInterpetedMember<T>, IInterpetedMemberSet<T>
            where T : IInterpetedAnyType
        {
            private T _value;

            public InterpetedMember(RunTimeAnyRoot root) : base(root)
            {
            }

            public InterpetedMember(T value, RunTimeAnyRoot root) : base(root)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                Value = value;
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

        }

    }

}