using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Backend.Syntaz_Model_Interpeter
{

    internal class Definitions: IOpenBoxesContext<IInterpeted>
    {
        private readonly Dictionary<object, IInterpeted> backing = new Dictionary<object, IInterpeted>();

        public InterpetedMemberDefinition Convert(IMemberDefinition member)
        {
            if (backing.TryGetValue(member, out var res))
            {
                return res.Cast<InterpetedMemberDefinition>();
            }
            else
            {
                var interpetedMemberDefinition = new InterpetedMemberDefinition();
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
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public InterpetedAssignOperation AssignOperation(IAssignOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res.Cast<InterpetedAssignOperation>();
            }
            else
            {
                var op = new InterpetedAssignOperation();
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
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public InterpetedGenericTypeDefinition GenericTypeDefinition(IGenericTypeDefinition codeElement)
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
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public InterpetedImplementationDefinition ImplementationDefinition(IImplementationDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res.Cast<InterpetedImplementationDefinition>();
            }
            else
            {
                var op = new InterpetedImplementationDefinition();
                backing.Add(codeElement, op);
                op.Init(
                    Convert(codeElement.ParameterDefinition),
                    Convert(codeElement.ContextDefinition),
                    codeElement.MethodBody.Select(x=>x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope));
                return op;
            }
        }

        public InterpetedLastCallOperation LastCallOperation(ILastCallOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res.Cast<InterpetedLastCallOperation>();
            }
            else
            {
                var op = new InterpetedLastCallOperation();
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
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public InterpetedMemberDefinition MemberDefinition(IMemberDefinition codeElement)
        {
            return Convert(codeElement);
        }

        public InterpetedMemberReferance MemberReferance(IMemberReferance codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res.Cast<InterpetedMemberReferance>();
            }
            else
            {
                var op = new InterpetedMemberReferance();
                backing.Add(codeElement, op);
                op.Init(
                    Convert(codeElement.MemberDefinition));
                return op;
            }
        }

        public InterpetedMethodDefinition MethodDefinition(IMethodDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res.Cast<InterpetedMethodDefinition>();
            }
            else
            {
                var op = new InterpetedMethodDefinition();
                backing.Add(codeElement, op);
                op.Init(
                    Convert(codeElement.ParameterDefinition),
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
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public InterpetedNextCallOperation NextCallOperation(INextCallOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res.Cast<InterpetedNextCallOperation>();
            }
            else
            {
                var op = new InterpetedNextCallOperation();
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
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public InterpetedReturnOperation ReturnOperation(IReturnOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                // TODO I don't much like all these casts,
                // maybe I should split up my backing??
                return res.Cast<InterpetedReturnOperation>();
            }
            else
            {
                var op = new InterpetedReturnOperation();
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
                    co.Left.Convert(this),
                    co.Right.Convert(this));
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
        

        #region IOpenBoxesContext<IInterpeted>


        IInterpeted IOpenBoxesContext<IInterpeted>.BlockDefinition(IBlockDefinition codeElement) => BlockDefinition(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.AssignOperation(IAssignOperation codeElement) => AssignOperation(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.ConstantNumber(IConstantNumber codeElement) => ConstantNumber(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.PathOperation(IPathOperation codeElement) => PathOperation(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.GenericTypeDefinition(IGenericTypeDefinition codeElement) => GenericTypeDefinition(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.ImplementationDefinition(IImplementationDefinition codeElement) => ImplementationDefinition(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.MemberDefinition(IMemberDefinition codeElement) => MemberDefinition(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.MemberReferance(IMemberReferance codeElement) => MemberReferance(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.MethodDefinition(IMethodDefinition codeElement) => MethodDefinition(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.ModuleDefinition(IModuleDefinition codeElement) => ModuleDefinition(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.LastCallOperation(ILastCallOperation codeElement) => LastCallOperation(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.ObjectDefinition(IObjectDefiniton codeElement) => ObjectDefinition(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.TypeDefinition(IInterfaceType codeElement) => TypeDefinition(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.AddOperation(IAddOperation codeElement) => AddOperation(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.NextCallOperation(INextCallOperation codeElement) => NextCallOperation(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.ElseOperation(IElseOperation codeElement) => ElseOperation(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.IfTrueOperation(IIfOperation codeElement) => IfTrueOperation(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.LessThanOperation(ILessThanOperation codeElement) => LessThanOperation(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.MultiplyOperation(IMultiplyOperation codeElement) => MultiplyOperation(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.SubtractOperation(ISubtractOperation codeElement) => SubtractOperation(codeElement);
        IInterpeted IOpenBoxesContext<IInterpeted>.ReturnOperation(IReturnOperation codeElement) => ReturnOperation(codeElement);
        
        #endregion
        
    }
}
