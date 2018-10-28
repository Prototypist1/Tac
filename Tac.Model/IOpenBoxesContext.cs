using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model
{
    // this is a very interesting patter
    // it is a manipulation of the type system
    // so that it enforces a contraint
    // the backend need to convert the model objects to it's own very of the model objects
    // it does so with this interface

    // it competes with the visister pattern
    // but, visiters have some down sides
    // if you add a visitable class
    // you have to remember to update a bunch of stuff
    // becuase the visitable classes do not generally know they are being visited
    // here the visited classes know they are being visited
    // and so it requires an entry here
    // which forces the front end to up
    public interface IOpenBoxesContext<T> 
    {
        T BlockDefinition(IBlockDefinition codeElement);
        T AssignOperation(IAssignOperation co);
        T ConstantNumber(IConstantNumber codeElement);
        T PathOperation(IPathOperation co);
        T GenericTypeDefinition(IGenericTypeDefinition codeElement);
        T ImplementationDefinition(IImplementationDefinition codeElement);
        T MemberDefinition(IMemberDefinition codeElement);
        T MemberReferance(IMemberReferance codeElement);
        T MethodDefinition(IMethodDefinition codeElement);
        T ModuleDefinition(IModuleDefinition codeElement);
        T LastCallOperation(ILastCallOperation co);
        T ObjectDefinition(IObjectDefiniton codeElement);
        T TypeDefinition(ITypeDefinition codeElement);
        T AddOperation(IAddOperation co);
        T NextCallOperation(INextCallOperation co);
        T ElseOperation(IElseOperation co);
        T IfTrueOperation(IIfOperation co);
        T LessThanOperation(ILessThanOperation co);
        T MultiplyOperation(IMultiplyOperation co);
        T SubtractOperation(ISubtractOperation co);
        T ReturnOperation(IReturnOperation co);
    }
}
