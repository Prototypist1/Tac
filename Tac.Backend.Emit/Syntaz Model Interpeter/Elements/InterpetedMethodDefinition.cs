using System;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{

    internal class InterpetedMethodDefinition : IAssembledOperation
    {
        public void Init(
            InterpetedMemberDefinition parameterDefinition, 
            IAssembledOperation[] methodBody,
            IInterpetedScopeTemplate scope,
            IMethodType methodType )
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            Body = methodBody ?? throw new ArgumentNullException(nameof(methodBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            MethodType = methodType ?? throw new ArgumentNullException(nameof(methodType));
        }


        private InterpetedMemberDefinition? parameterDefinition;
        public InterpetedMemberDefinition ParameterDefinition { get => parameterDefinition ?? throw new NullReferenceException(nameof(parameterDefinition)); private set => parameterDefinition = value ?? throw new NullReferenceException(nameof(value)); }

        private IAssembledOperation[]? body;
        public IAssembledOperation[] Body { get => body ?? throw new NullReferenceException(nameof(body)); private set => body = value ?? throw new NullReferenceException(nameof(value)); }

        private IInterpetedScopeTemplate? scope;
        public IInterpetedScopeTemplate Scope { get => scope ?? throw new NullReferenceException(nameof(scope)); private set => scope = value ?? throw new NullReferenceException(nameof(value)); }


        private IMethodType? methodType;
        public IMethodType MethodType { get => methodType ?? throw new NullReferenceException(nameof(methodType)); private set => methodType = value ?? throw new NullReferenceException(nameof(value)); }


        public IInterpetedResult<IInterpetedMember> Assemble(AssemblyContext interpetedContext)
        {
            // ok so I make a class
            // I figure out what needs to be in the closure
            // I put that stuff in the type

            // C# actually make the closure early and
            // defines anything that is going to live in there
            // in the closure
            // that sounds more optimised than I want atm


            // it is possible we are in a type
            // so we don't need to make a type
            // like:
            // object { x := method [number;number] { 5 return;}}


            // this is a little more interesting:
            // y := 5;
            // object { x := method [number;number] { y return;}}
            // we need to rehome y in to object

            // this:
            // object { x := method [number;number] { 5 return;}}
            // is not that special tho
            // it still needst to be a func so we can assign to it

            // in that case....
            // it should probably be defined on it own type
            // and have it own closure
            // that makes my life simpler anyway


            var thing = TypeManager.InternalMethod(
                        ParameterDefinition,
                        Body,
                        interpetedContext,
                        Scope,
                        MethodType);
            return InterpetedResult.Create(
                TypeManager.Member(
                    thing.Convert(TransformerExtensions.NewConversionContext()), 
                    thing));
        }
        
        //public IInterpeted GetDefault(InterpetedContext interpetedContext)
        //{
        //    // here I need to map TIn, TOut to real types
        //    // not sure 
        //    return TypeManager.InternalMethod<TIn, TOut>(
        //        new InterpetedMemberDefinition<TIn> ().Init(new NameKey("input")),
        //        new IInterpetedOperation<IInterpetedAnyType>[] { },
        //        interpetedContext,
        //        Scope);
        //}

    }
}