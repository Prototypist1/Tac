using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;
using Tac.Model.Operations;

//namespace Tac.Model.Instantiated.Operations
//{
//    public class TypeOrOperation : ITypeOrOperation, IBinaryOperationBuilder
//    {
//        private readonly Buildable<IVerifiableType> buildableLeft = new Buildable<IVerifiableType>();
//        private readonly Buildable<IVerifiableType> buildableRight = new Buildable<IVerifiableType>();

//        public void Build(IVerifiableType left, IVerifiableType right)
//        {
//            buildableLeft.Set(left);
//            buildableRight.Set(right);
//        }

//        public IVerifiableType Left => buildableLeft.Get();
//        public IVerifiableType Right => buildableRight.Get();
//        public IVerifiableType[] Operands => new[] { Left, Right };

//        private TypeOrOperation() { }

//        public static (ITypeOrOperation, IBinaryOperationBuilder) Create()
//        {
//            var res = new TypeOrOperation();
//            return (res, res);
//        }

//        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
//            where TBacking : IBacking
//        {
//            return context.TypeOrOperation(this);
//        }

//        public IVerifiableType Returns()
//        {
//            return new ();
//        }

//        public static ITypeOrOperation CreateAndBuild(ICodeElement left, ICodeElement right)
//        {
//            var (x, y) = Create();
//            y.Build(left, right);
//            return x;
//        }
//    }

//    public interface IBinaryOperationBuilder
//    {
//        void Build(ICodeElement left, ICodeElement right);
//    }
//}
