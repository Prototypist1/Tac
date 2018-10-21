using Prototypist.LeftToRight;
using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using System.Linq;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedImplementationDefinition : ImplementationDefinition, IInterpeted, IInterpetedPrimitiveType
    {
        public InterpetedImplementationDefinition(IBox<MemberDefinition> contextDefinition, IBox<MemberDefinition> parameterDefinition, IBox<IReturnable> outputType, IEnumerable<ICodeElement> metohdBody, IResolvableScope scope, IEnumerable<ICodeElement> staticInitializers) : base(contextDefinition, parameterDefinition, outputType, metohdBody, scope, staticInitializers)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedImplementation(
                ParameterDefinition.GetValue(),
                ContextDefinition.GetValue(),
                MethodBody.ToArray(),
                interpetedContext,
                Scope));
        }

        internal static ImplementationDefinition MakeNew(IBox<MemberDefinition> contextDefinition, IBox<MemberDefinition> parameterDefinition, IBox<IReturnable> outputType, IEnumerable<ICodeElement> metohdBody, IResolvableScope scope, IEnumerable<ICodeElement> staticInitializers)
        {
            return new InterpetedImplementationDefinition(contextDefinition, parameterDefinition, outputType, metohdBody, scope, staticInitializers);
        }

        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return new InterpetedImplementation(
                    interpetedContext.elementBuilders.MemberDefinition(false,new NameKey("input"),new Box<IReturnable>(new InterpetedAnyType())),
                    interpetedContext.elementBuilders.MemberDefinition(false, new NameKey("countex"), new Box<IReturnable>(new InterpetedAnyType())),
                    new ICodeElement[] { },
                    interpetedContext,
                    new EmptyScope());
        }
    }

    public class EmptyScope : IResolvableScope
    {
        public IReadOnlyList<IBox<MemberDefinition>> Members
        {
            get
            {
                return new List<IBox<MemberDefinition>>();
            }
        }

        public bool TryGetMember(NameKey name, bool staticOnly, out IBox<MemberDefinition> member)
        {
            member = default;
            return false;
        }

        public bool TryGetType(IKey name, out IBox<IReturnable> type)
        {
            type = default;
            return false;
        }
    }
}