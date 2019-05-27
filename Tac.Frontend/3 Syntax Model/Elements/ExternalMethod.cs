using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Semantic_Model;

namespace Tac.Frontend._3_Syntax_Model.Elements
{

    internal interface IMethodDefinition: IFrontendType
    {
        IIsPossibly<IWeakTypeReference> InputType { get; }
        IIsPossibly<IWeakTypeReference> OutputType { get; }
        IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> ParameterDefinition { get; }
    }
    
    //internal class ExternalMethod : 
    //    IMethodDefinition,
    //    IFrontendCodeElement<IExternalMethodDefinition>, IFrontendType<IVerifiableType>
    //{
    //    private readonly Guid Id;

    //    public ExternalMethod(Guid id, IIsPossibly<IWeakTypeReferance> outputType, IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> parameterDefinition)
    //    {
    //        Id = id;
    //        OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
    //        ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
    //    }

    //    public IIsPossibly<IWeakTypeReferance> InputType => ParameterDefinition.IfIs(x => x.GetValue()).IfIs(x => x.Type);
    //    public IIsPossibly<IWeakTypeReferance> OutputType { get; }
    //    public IIsPossibly<IBox<IIsPossibly<Semantic_Model.IWeakMemberDefinition>>> ParameterDefinition { get; }

    //    public IBuildIntention<IExternalMethodDefinition> GetBuildIntention(TransformerExtensions.ConversionContext context)
    //    {
    //        var (toBuild, maker) = ExternalMethodDefinition.Create();
    //        return new BuildIntention<IExternalMethodDefinition>(toBuild, () =>
    //        {
    //            maker.Build(
    //                TransformerExtensions.Convert<ITypeReferance>(InputType.GetOrThrow(), context),
    //                TransformerExtensions.Convert<ITypeReferance>(OutputType.GetOrThrow(), context),
    //                ParameterDefinition.GetOrThrow().GetValue().GetOrThrow().Convert(context),
    //                Id);
    //        });
    //    }

    //    public IIsPossibly<IFrontendType<IVerifiableType>> Returns() => Possibly.Is(this);

    //    IBuildIntention<IVerifiableType> IConvertable<IVerifiableType>.GetBuildIntention(TransformerExtensions.ConversionContext context) => GetBuildIntention(context);
    //}
}
