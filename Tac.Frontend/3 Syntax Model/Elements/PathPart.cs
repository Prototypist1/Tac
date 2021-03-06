﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;

namespace Tac.SemanticModel
{

    // does not seems like this should be an "IFrontendType<IVerifiableType>"
    internal interface IWeakMemberReference : IConvertableFrontendCodeElement<IMemberReference>
    {
        IBox<WeakMemberDefinition> MemberDefinition { get; }
    }

    // TODO I don't think I want this...
    // just use member definition 
    internal class WeakMemberReference : IWeakMemberReference, IReturn
    {
        public WeakMemberReference(IBox<WeakMemberDefinition> memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public IBox<WeakMemberDefinition> MemberDefinition { get; }

        public IBuildIntention<IMemberReference> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = MemberReference.Create();
            return new BuildIntention<IMemberReference>(toBuild, () =>
            {
                maker.Build(MemberDefinition.GetValue().Convert(context));
            });
        }

        public IOrType<IFrontendType<IVerifiableType>, IError> Returns()
        {
            return OrType.Make<IFrontendType<IVerifiableType>, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.RefType(MemberDefinition.GetValue().Type.GetValue()));
        }

        public IEnumerable<IError> Validate() => MemberDefinition.GetValue().Validate();
    }
}