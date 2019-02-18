using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Backend.Syntaz_Model_Interpeter
{

    internal class Definitions: IOpenBoxesContext<IInterpetedOperation>
    {
        private readonly Dictionary<object, IInterpeted> backing = new Dictionary<object, IInterpeted>();

        public Definitions()
        {
        }

        public IInterpetedMemberDefinition MemberDefinition(IMemberDefinition member)
        {
            if (backing.TryGetValue(member, out var res))
            {
                return res.Cast<IInterpetedMemberDefinition>();
            }
            else
            {
                var interpetedMemberDefinition = new InterpetedMemberDefinition<object>();
                backing.Add(member, interpetedMemberDefinition);
                return interpetedMemberDefinition.Init(member.Key);
            }
        }

        public InterpetedAddOperation AddOperation(IAddOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res.Cast<InterpetedAddOperation>();
            }
            else {
                var op = new InterpetedAddOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).Cast<IInterpetedOperation<double>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<double>>());
                return op;
            }
        }

        public IInterpetedAssignOperation AssignOperation(IAssignOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res.Cast<IInterpetedAssignOperation>();
            }
            else
            {
                var op = new InterpetedAssignOperation<object>();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public InterpetedBlockDefinition BlockDefinition(IBlockDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res.Cast<InterpetedBlockDefinition>();
            }
            else
            {
                var op = new InterpetedBlockDefinition();
                backing.Add(codeElement, op);
                op.Init(
                    codeElement.Body.Select(x=>x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope));
                return op;
            }
        }

        public InterpetedConstantNumber ConstantNumber(IConstantNumber codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res.Cast<InterpetedConstantNumber>();
            }
            else
            {
                var op = new InterpetedConstantNumber();
                backing.Add(codeElement, op);
                op.Init(codeElement.Value);
                return op;
            }
        }

        public InterpetedElseOperation ElseOperation(IElseOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res.Cast<InterpetedElseOperation>();
            }
            else
            {
                var op = new InterpetedElseOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).Cast<IInterpetedOperation<bool>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<IInterpedEmpty>>());
                return op;
            }
        }

        public InterpetedGenericTypeDefinition GenericTypeDefinition(IGenericInterfaceDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res.Cast<InterpetedGenericTypeDefinition>();
            }
            else
            {
                var op = new InterpetedGenericTypeDefinition();
                backing.Add(codeElement, op);
                op.Init();
                return op;
            }
        }

        public InterpetedIfTrueOperation IfTrueOperation(IIfOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res.Cast<InterpetedIfTrueOperation>();
            }
            else
            {
                var op = new InterpetedIfTrueOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).Cast<IInterpetedOperation<bool>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<IInterpedEmpty>>());
                return op;
            }
        }

        public IInterpetedImplementationDefinition ImplementationDefinition(IImplementationDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res.Cast<IInterpetedImplementationDefinition>();
            }
            else
            {
                var op = new InterpetedImplementationDefinition<object, object, object>();
                backing.Add(codeElement, op);
                op.Init(
                    MemberDefinition(codeElement.ParameterDefinition),
                    MemberDefinition(codeElement.ContextDefinition),
                    codeElement.MethodBody.Select(x=>x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope));
                return op;
            }
        }

        public IInterpetedLastCallOperation LastCallOperation(ILastCallOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res.Cast<IInterpetedLastCallOperation>();
            }
            else
            {
                var op = new InterpetedLastCallOperation<object,object>();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public InterpetedLessThanOperation LessThanOperation(ILessThanOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res.Cast<InterpetedLessThanOperation>();
            }
            else
            {
                var op = new InterpetedLessThanOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).Cast<IInterpetedOperation<double>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<double>>());
                return op;
            }
        }
        
        public IInterpetedMemberReferance MemberReferance(IMemberReferance codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res.Cast<IInterpetedMemberReferance>();
            }
            else
            {
                var op = new InterpetedMemberReferance<object>();
                backing.Add(codeElement, op);
                op.Init(
                    MemberDefinition(codeElement.MemberDefinition));
                return op;
            }
        }

        public IInterpetedMethodDefinition MethodDefinition(IInternalMethodDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res.Cast<IInterpetedMethodDefinition>();
            }
            else
            {
                var op = new InterpetedMethodDefinition<object,object>();
                backing.Add(codeElement, op);
                op.Init(
                    MemberDefinition(codeElement.ParameterDefinition),
                    codeElement.Body.Select(x => x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope));
                return op;
            }
        }

        public InterpetedModuleDefinition ModuleDefinition(IModuleDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res.Cast<InterpetedModuleDefinition>();
            }
            else
            {
                var op = new InterpetedModuleDefinition();
                backing.Add(codeElement, op);
                op.Init(new InterpetedScopeTemplate(codeElement.Scope),
                    codeElement.StaticInitialization.Select(x => x.Convert(this)).ToArray()
                    );
                return op;
            }
        }

        public InterpetedMultiplyOperation MultiplyOperation(IMultiplyOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res.Cast<InterpetedMultiplyOperation>();
            }
            else
            {
                var op = new InterpetedMultiplyOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).Cast<IInterpetedOperation<double>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<double>>());
                return op;
            }
        }

        public IInterpetedNextCallOperation NextCallOperation(INextCallOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res.Cast<IInterpetedNextCallOperation>();
            }
            else
            {
                var op = new InterpetedNextCallOperation<object,object>();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public InterpetedObjectDefinition ObjectDefinition(IObjectDefiniton codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res.Cast<InterpetedObjectDefinition>();
            }
            else
            {
                var op = new InterpetedObjectDefinition();
                backing.Add(codeElement, op);
                op.Init(new InterpetedScopeTemplate(codeElement.Scope),
                    codeElement.Assignments.Select(x => AssignOperation(x)).ToArray()
                    );
                return op;
            }
        }

        public InterpetedPathOperation PathOperation(IPathOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res.Cast<InterpetedPathOperation>();
            }
            else
            {
                var op = new InterpetedPathOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).Cast<IInterpetedOperation<IInterpetedScope>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<IInterpetedData>>());
                return op;
            }
        }

        public IInterpetedReturnOperation ReturnOperation(IReturnOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                // TODO I don't much like all these casts,
                // maybe I should split up my backing??
                return res.Cast<IInterpetedReturnOperation>();
            }
            else
            {
                var op = new InterpetedReturnOperation<object>();
                backing.Add(co, op);
                op.Init(
                    co.Result.Convert(this));
                return op;
            }
        }

        public InterpetedSubtractOperation SubtractOperation(ISubtractOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res.Cast<InterpetedSubtractOperation>();
            }
            else
            {
                var op = new InterpetedSubtractOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).Cast<IInterpetedOperation<double>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<double>>());
                return op;
            }
        }

        public InterpetedTypeDefinition TypeDefinition(IInterfaceType codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res.Cast<InterpetedTypeDefinition>();
            }
            else
            {
                var op = new InterpetedTypeDefinition();
                backing.Add(codeElement, op);
                op.Init();
                return op;
            }
        }


        public InterpetedTypeReferance TypeReferance(ITypeReferance codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res.Cast<InterpetedTypeReferance>();
            }
            else
            {
                var op = new InterpetedTypeReferance();
                backing.Add(codeElement, op);
                op.Init(codeElement.TypeDefinition);
                return op;
            }
        }

        #region IOpenBoxesContext<IInterpeted>

        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.BlockDefinition(IBlockDefinition codeElement) => BlockDefinition(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.AssignOperation(IAssignOperation codeElement) => AssignOperation(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.ConstantNumber(IConstantNumber codeElement) => ConstantNumber(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.PathOperation(IPathOperation codeElement) => PathOperation(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.GenericTypeDefinition(IGenericInterfaceDefinition codeElement) => GenericTypeDefinition(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.ImplementationDefinition(IImplementationDefinition codeElement) => ImplementationDefinition(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.MemberDefinition(IMemberDefinition codeElement) => MemberDefinition(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.MemberReferance(IMemberReferance codeElement) => MemberReferance(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.InternalMethodDefinition(IInternalMethodDefinition codeElement) => MethodDefinition(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.ModuleDefinition(IModuleDefinition codeElement) => ModuleDefinition(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.LastCallOperation(ILastCallOperation codeElement) => LastCallOperation(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.ObjectDefinition(IObjectDefiniton codeElement) => ObjectDefinition(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.TypeDefinition(IInterfaceType codeElement) => TypeDefinition(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.AddOperation(IAddOperation codeElement) => AddOperation(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.NextCallOperation(INextCallOperation codeElement) => NextCallOperation(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.ElseOperation(IElseOperation codeElement) => ElseOperation(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.IfTrueOperation(IIfOperation codeElement) => IfTrueOperation(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.LessThanOperation(ILessThanOperation codeElement) => LessThanOperation(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.MultiplyOperation(IMultiplyOperation codeElement) => MultiplyOperation(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.SubtractOperation(ISubtractOperation codeElement) => SubtractOperation(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.ReturnOperation(IReturnOperation codeElement) => ReturnOperation(codeElement);
        IInterpetedOperation IOpenBoxesContext<IInterpetedOperation>.TypeReferance(ITypeReferance codeElement) => TypeReferance(codeElement);
        
        #endregion

    }
}
