using Prototypist.Toolbox;
using System;
using System.Diagnostics.CodeAnalysis;
using Tac.Model;

namespace Tac.Frontend.New.CrzayNamespace
{
    internal partial class Tpn
    {
        internal abstract class TypeLikeOrError : OrType<
            TypeProblem2.MethodType, 
            TypeProblem2.Type, 
            TypeProblem2.Object, 
            TypeProblem2.OrType, 
            TypeProblem2.InferredType, 
            TypeProblem2.GenericTypeParameter, 
            IError>
        {
            public static TypeLikeOrError Make(TypeProblem2.MethodType contents)
            {
                return new Inner<TypeProblem2.MethodType>(contents);
            }
            public static TypeLikeOrError Make(TypeProblem2.Type contents)
            {
                return new Inner<TypeProblem2.Type>(contents);
            }
            public static TypeLikeOrError Make(TypeProblem2.Object contents)
            {
                return new Inner<TypeProblem2.Object>(contents);
            }
            public static TypeLikeOrError Make(TypeProblem2.OrType contents)
            {
                return new Inner<TypeProblem2.OrType>(contents);
            }
            public static TypeLikeOrError Make(TypeProblem2.InferredType contents)
            {
                return new Inner<TypeProblem2.InferredType>(contents);
            }
            public static TypeLikeOrError Make(TypeProblem2.GenericTypeParameter contents)
            {
                return new Inner<TypeProblem2.GenericTypeParameter>(contents);
            }
            public static TypeLikeOrError Make(IError contents)
            {
                return new Inner<IError>(contents);
            }

            private class Inner<T> : TypeLikeOrError, IIsDefinately<T>
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
