//using System;
//using System.Collections.Generic;
//using System.Text;
//using Tac.Model.Elements;
//using Tac.Model.Operations;


// you dumbshit, you already have a visitory


//namespace Tac.Backend.Emit.Walkers
//{
//    abstract class ModelVisitor<T>
//    {
//        public T VisitBlockDefinition(IBlockDefinition blockDefinition)
//        {
//            return InnerVisitBlockDefinition(blockDefinition);
//        }

//        protected abstract T InnerVisitBlockDefinition(IBlockDefinition blockDefinition);

//        public T VisitConstantNumber(IConstantNumber constantNumber)
//        {
//            return InnerVisitConstantNumber(constantNumber);
//        }

//        protected abstract T InnerVisitConstantNumber(IConstantNumber constantNumber);

//        public T VisitEntryPointDefinition(IEntryPointDefinition entryPointDefinition)
//        {
//            return InnerVisitEntryPointDefinition(entryPointDefinition);
//        }

//        protected abstract T InnerVisitEntryPointDefinition(IEntryPointDefinition entryPointDefinition);

//        public T VisitImplementationDefinition(IImplementationDefinition implementationDefinition)
//        {
//            return InnerVisitImplementationDefinition(implementationDefinition);
//        }

//        protected abstract T  InnerVisitImplementationDefinition(IImplementationDefinition implementationDefinition);

//        public T VisitMemberDefinition(IMemberDefinition memberDefinition)
//        {
//            return InnerVisitMemberDefinition(memberDefinition);
//        }

//        protected abstract T InnerVisitMemberDefinition(IMemberDefinition memberDefinition);

//        public T VisitMemberReferance(IMemberReferance memberReferance)
//        {
//            return InnerVisitMemberReferance(memberReferance);
//        }

//        protected abstract T InnerVisitMemberReferance(IMemberReferance memberReferance);

//        public T VisitMethodDefinition(IInternalMethodDefinition methodDefinition)
//        {
//            return InnerVisitMethodDefinition(methodDefinition);
//        }

//        protected abstract T InnerVisitMethodDefinition(IInternalMethodDefinition methodDefinition);

//        public T VisitModuleDefinition(IModuleDefinition moduleDefinition)
//        {
//            return InnerVisitModuleDefinition(moduleDefinition);
//        }

//        protected abstract T InnerVisitModuleDefinition(IModuleDefinition moduleDefinition);

//        public T VisitObjectDefiniton(IObjectDefiniton objectDefiniton)
//        {
//            return InnerVisitObjectDefiniton(objectDefiniton);
//        }

//        protected abstract T InnerVisitObjectDefiniton(IObjectDefiniton objectDefiniton);

//        public T VisitTypeDefinition(ITypeDefinition typeDefinition)
//        {
//            return InnerVisitTypeDefinition(typeDefinition);
//        }

//        protected abstract T InnerVisitTypeDefinition(ITypeDefinition typeDefinition);

//        public T VisitAddOperation(IAddOperation addOperation)
//        {
//            return InnerVisitAddOperation(addOperation);
//        }

//        protected abstract T InnerVisitAddOperation(IAddOperation addOperation);

//        public T VisitAssignOperation(IAssignOperation assignOperation)
//        {
//            return InnerVisitAssignOperation(assignOperation);
//        }

//        protected abstract T InnerVisitAssignOperation(IAssignOperation assignOperation);

//        protected abstract T InnerVisitElseOperation(IElseOperation elseOperation);

//        public T VisitIfOperation(IIfOperation ifOperation)
//        {
//            return InnerVisitIfOperation(ifOperation);
//        }

//        protected abstract T InnerVisitIfOperation(IIfOperation ifOperation);

//        public T VisitLastCallOperation(ILastCallOperation lastCallOperation)
//        {
//            return InnerVisitLastCallOperation(lastCallOperation);
//        }

//        protected abstract T InnerVisitLastCallOperation(ILastCallOperation lastCallOperation);

//        public T VisitLessThanOperation(ILessThanOperation lessThanOperation)
//        {
//            return InnerVisitLessThanOperation(lessThanOperation);
//        }

//        protected abstract T InnerVisitLessThanOperation(ILessThanOperation lessThanOperation);

//        public T VisitMultiplyOperation(IMultiplyOperation multiplyOperation)
//        {
//            return InnerVisitMultiplyOperation(multiplyOperation);
//        }

//        protected abstract T InnerVisitMultiplyOperation(IMultiplyOperation multiplyOperation);

//        public T VisitNextCallOperation(INextCallOperation nextCallOperation)
//        {
//            return InnerVisitNextCallOperation(nextCallOperation);
//        }

//        protected abstract T InnerVisitNextCallOperation(INextCallOperation nextCallOperation);

//        public T VisitPathOperation(IPathOperation pathOperation)
//        {
//            return InnerVisitPathOperation(pathOperation);
//        }

//        protected abstract T InnerVisitPathOperation(IPathOperation pathOperation);

//        public T VisitReturnOperation(IReturnOperation returnOperation)
//        {
//            return InnerVisitReturnOperation(returnOperation);
//        }

//        protected abstract T InnerVisitReturnOperation(IReturnOperation returnOperation);

//        public T VisitSubtractOperation(ISubtractOperation subtractOperation)
//        {
//            return InnerVisitSubtractOperation(subtractOperation);
//        }

//        protected abstract T InnerVisitSubtractOperation(ISubtractOperation subtractOperation);

//        public T VisitTryAssignOperation(ITryAssignOperation tryAssignOperation)
//        {
//            return InnerVisitTryAssignOperation(tryAssignOperation);
//        }

//        protected abstract T InnerVisitTryAssignOperation(ITryAssignOperation tryAssignOperation);

//        public T VisitTypeOrOperation(ITypeOrOperation typeOrOperation)
//        {
//            return InnerVisitTypeOrOperation(typeOrOperation);
//        }

//        protected abstract T InnerVisitTypeOrOperation(ITypeOrOperation typeOrOperation);


//    }
//}
