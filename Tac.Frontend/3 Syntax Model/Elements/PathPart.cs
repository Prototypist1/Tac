﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;

namespace Tac.SemanticModel
{

    // does not seems like this should be an "IFrontendType"
    internal interface IWeakMemberReference : IConvertableFrontendCodeElement<IMemberReferance>
    {
        IBox<IWeakMemberDefinition> MemberDefinition { get; }
    }

    // TODO I don't think I want this...
    // just use member definition 
    internal class WeakMemberReference : IWeakMemberReference
    {
        public WeakMemberReference(IBox<IWeakMemberDefinition> memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public IBox<IWeakMemberDefinition> MemberDefinition { get; }

        public IBuildIntention<IMemberReferance> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = MemberReference.Create();
            return new BuildIntention<IMemberReferance>(toBuild, () =>
            {
                maker.Build(MemberDefinition.GetValue().Convert(context));
            });
        }
    }
}