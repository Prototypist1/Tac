using System;
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

    // I had an interfact moment and it was too much
    internal interface IWeakMemberReference : IConvertableFrontendCodeElement<IMemberReference>
    {
    //    IBox<WeakMemberDefinition> MemberDefinition { get; }
    }

    // TODO I don't think I want this...
    // just use member definition 
    internal class WeakMemberReference : IWeakMemberReference, IReturn
    {
        public WeakMemberReference(IBox<IOrType<WeakMemberDefinition,IError>> memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public IBox<IOrType<WeakMemberDefinition, IError>> MemberDefinition { get; }

        public IBuildIntention<IMemberReference> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = MemberReference.Create();
            return new BuildIntention<IMemberReference>(toBuild, () =>
            {
                maker.Build(MemberDefinition.GetValue().Is1OrThrow().Convert(context));
            });
        }

        public IOrType<IFrontendType<IVerifiableType>, IError> Returns()
        {
            return MemberDefinition.GetValue().TransformInner(x => new Tac.SyntaxModel.Elements.AtomicTypes.RefType(x.Type.GetValue()));
        }

        public IEnumerable<IError> Validate() {
            return MemberDefinition.GetValue().SwitchReturns(x => x.Validate(),
                error => new[] { error });
                
        }
    }
}