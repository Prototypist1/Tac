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

    internal class Definitions: IOpenBoxesContext<IInterpetedOperation<object>>
    {
        private readonly Dictionary<object, IInterpetedOperation<object>> backing = new Dictionary<object, IInterpetedOperation<object>>();

        public Definitions()
        {
        }

        public IInterpetedOperation<object> MemberDefinition(IMemberDefinition member)
        {
            if (backing.TryGetValue(member, out var res))
            {
                return res;
            }
            else
            {
                var interpetedMemberDefinition = new InterpetedMemberDefinition<object>();
                backing.Add(member, interpetedMemberDefinition);
                return interpetedMemberDefinition.Init(member.Key);
            }
        }

        public IInterpetedOperation<object> AddOperation(IAddOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
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

        public IInterpetedOperation<object> AssignOperation(IAssignOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
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

        public IInterpetedOperation<object> BlockDefinition(IBlockDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
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

        public IInterpetedOperation<object> ConstantNumber(IConstantNumber codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedConstantNumber();
                backing.Add(codeElement, op);
                op.Init(codeElement.Value);
                return op;
            }
        }

        public IInterpetedOperation<object> ElseOperation(IElseOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
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

        public IInterpetedOperation<object> GenericTypeDefinition(IGenericInterfaceDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedGenericTypeDefinition();
                backing.Add(codeElement, op);
                op.Init();
                return op;
            }
        }

        public IInterpetedOperation<object> IfTrueOperation(IIfOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
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

        public IInterpetedOperation<object> ImplementationDefinition(IImplementationDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedImplementationDefinition<object, object, object>();
                backing.Add(codeElement, op);
                op.Init(
                    MemberDefinition(codeElement.ParameterDefinition).Cast<InterpetedMemberDefinition<object>>(),
                    MemberDefinition(codeElement.ContextDefinition).Cast<InterpetedMemberDefinition<object>>(),
                    codeElement.MethodBody.Select(x=>x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope));
                return op;
            }
        }

        public IInterpetedOperation<object> LastCallOperation(ILastCallOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedLastCallOperation<object,object>();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).Cast<IInterpetedOperation<IInterpetedCallable<object,object>>>(),
                    co.Right.Convert(this));
                return op;
            }
        }

        public IInterpetedOperation<object> LessThanOperation(ILessThanOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
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
        
        public IInterpetedOperation<object> MemberReferance(IMemberReferance codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedMemberReferance<object>();
                backing.Add(codeElement, op);
                op.Init(
                    MemberDefinition(codeElement.MemberDefinition).Cast<InterpetedMemberDefinition<object>>());
                return op;
            }
        }

        public IInterpetedOperation<object> MethodDefinition(IInternalMethodDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedMethodDefinition<object,object>();
                backing.Add(codeElement, op);
                op.Init(
                    MemberDefinition(codeElement.ParameterDefinition).Cast<InterpetedMemberDefinition<object>>(),
                    codeElement.Body.Select(x => x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope));
                return op;
            }
        }

        public IInterpetedOperation<object> ModuleDefinition(IModuleDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
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

        public IInterpetedOperation<object> MultiplyOperation(IMultiplyOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
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

        public IInterpetedOperation<object> NextCallOperation(INextCallOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedNextCallOperation<object,object>();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this).Cast<IInterpetedOperation<IInterpetedCallable<object,object>>>());
                return op;
            }
        }

        public IInterpetedOperation<object> ObjectDefinition(IObjectDefiniton codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res);
            }
            else
            {
                var op = new InterpetedObjectDefinition();
                backing.Add(codeElement, op);
                op.Init(new InterpetedScopeTemplate(codeElement.Scope),
                    codeElement.Assignments.Select(x => AssignOperation(x).Cast<IInterpetedAssignOperation<object>>()).ToArray()
                    );
                return op;
            }
        }

        public IInterpetedOperation<object> PathOperation(IPathOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
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

        public IInterpetedOperation<object> ReturnOperation(IReturnOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
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

        public IInterpetedOperation<object> SubtractOperation(ISubtractOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
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

        public IInterpetedOperation<object> TypeDefinition(IInterfaceType codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedTypeDefinition();
                backing.Add(codeElement, op);
                op.Init();
                return op;
            }
        }


        public IInterpetedOperation<object> TypeReferance(ITypeReferance codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedTypeReferance();
                backing.Add(codeElement, op);
                op.Init(codeElement.TypeDefinition);
                return op;
            }
        }

        public IInterpetedOperation<object> InternalMethodDefinition(IInternalMethodDefinition codeElement) => MethodDefinition(codeElement);
    }
}
