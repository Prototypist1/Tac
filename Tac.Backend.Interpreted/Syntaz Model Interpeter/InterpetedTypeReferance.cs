//using System;
//using Tac.Model;
//using Tac.Model.Elements;
//using Tac.Backend.Interpreted.SyntazModelInterpeter;
//using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;

//namespace Tac.Backend.Interpreted.SyntazModelInterpeter
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