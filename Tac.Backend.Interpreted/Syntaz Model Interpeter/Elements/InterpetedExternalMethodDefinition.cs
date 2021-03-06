﻿using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;

namespace Tac.Backend.Interpreted.SyntazModelInterpeter.Elements
{
    internal class InterpetedExternalMethodDefinition : IInterpetedOperation
    {
        private IMethodType? methodType;
        public IMethodType MethodType { get => methodType ?? throw new NullReferenceException(nameof(methodType)); private set => methodType = value ?? throw new NullReferenceException(nameof(value)); }

        public void Init(Func<IInterpetedAnyType, IInterpetedAnyType> backing, IMethodType methodType)
        {
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
            MethodType = methodType ?? throw new ArgumentNullException(nameof(methodType));
        }

        public IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext)
        {
            var thing = TypeManager.ExternalMethod(Backing, MethodType);

            return InterpetedResult.Create(TypeManager.Member(thing.Convert(TransformerExtensions.NewConversionContext()), thing));
        }

        private Func<IInterpetedAnyType, IInterpetedAnyType>? backing;
        public Func<IInterpetedAnyType, IInterpetedAnyType> Backing { get => backing ?? throw new NullReferenceException(nameof(backing)); private set => backing = value ?? throw new NullReferenceException(nameof(value)); }


    }
}