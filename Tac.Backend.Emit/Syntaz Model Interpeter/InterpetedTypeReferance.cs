//using System;
//using Tac.Model;
//using Tac.Model.Elements;
//using Tac.Backend.Emit.SyntazModel;
//using Tac.Backend.Emit.SyntazModel.Run_Time_Objects;

//namespace Tac.Backend.Emit.SyntazModel
//{
//    internal class InterpetedTypeReferance : IInterpetedOperation<IInterpetedAnyType>, ITypeReferance
//    {
//        public IVerifiableType VerifiableType
//        {
//            get; private set;
//        }
//        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
//            where TBacking : IBacking
//        {
//            return context.TypeReferance(this);
//        }

//        public IInterpetedResult<IInterpetedMember<IInterpetedAnyType>> Interpet(InterpetedContext interpetedContext)
//        {
//            return InterpetedResult.Create<IInterpetedMember<IInterpetedAnyType>>(new InterpetedMember<IInterpetedAnyType>( new RunTimeType()));
//        }

//        public IVerifiableType Returns()
//        {
//            return VerifiableType;
//        }

//        internal void Init(IVerifiableType typeDefinition)
//        {
//            VerifiableType = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
//        }
//    }
//}