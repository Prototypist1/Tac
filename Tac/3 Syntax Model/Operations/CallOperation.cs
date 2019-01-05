using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{


    internal class NextCallSymbols : ISymbols
    {
        public string Symbols => ">";
    }

    internal class WeakNextCallOperation : BinaryOperation<IFrontendCodeElement<ICodeElement>, IFrontendCodeElement<ICodeElement>, INextCallOperation>
    {
        public WeakNextCallOperation(IIsPossibly<IFrontendCodeElement<ICodeElement>> left, IIsPossibly<IFrontendCodeElement<ICodeElement>> right) : base(left, right)
        {
        }
        
        public override IIsPossibly<IFrontendType<IVarifiableType>> Returns()
        {
            return Right.GetOrThrow().Unwrap<WeakMethodDefinition>().OutputType
                .IfIs(x => x.TypeDefinition)
                .IfIs(x => x.GetValue());
        }

        public override IBuildIntention<INextCallOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = NextCallOperation.Create();
            return new BuildIntention<INextCallOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().Convert(context), Right.GetOrThrow().Convert(context));
            });
        }
    }

    internal class NextCallOperationMaker : BinaryOperationMaker<WeakNextCallOperation,INextCallOperation>
    {
        public NextCallOperationMaker() : base(new NextCallSymbols(), (l,r)=> Possibly.Is( new WeakNextCallOperation(l,r)))
        {
        }
    }


    internal class LastCallSymbols : ISymbols
    {
        public string Symbols => "<";
    }

    internal class WeakLastCallOperation : BinaryOperation<IFrontendCodeElement<ICodeElement>, IFrontendCodeElement<ICodeElement>, ILastCallOperation>
    {
        public const string Identifier = "<";

        public WeakLastCallOperation(IIsPossibly<IFrontendCodeElement<ICodeElement>> left, IIsPossibly<IFrontendCodeElement<ICodeElement>> right) : base(left, right)
        {
        }
        
        public override IIsPossibly<IFrontendType<IVarifiableType>> Returns()
        {
            return Left.GetOrThrow().Unwrap<WeakMethodDefinition>().OutputType
                .IfIs(x=>x.TypeDefinition)
                .IfIs(x=>x.GetValue());
        }

        public override IBuildIntention<ILastCallOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = LastCallOperation.Create();
            return new BuildIntention<ILastCallOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().Convert(context), Right.GetOrThrow().Convert(context));
            });
        }
    }

    internal class LastCallOperationMaker : BinaryOperationMaker<WeakLastCallOperation, ILastCallOperation>
    {
        public LastCallOperationMaker() : base(new LastCallSymbols(), (l,r)=>Possibly.Is( new WeakLastCallOperation(l,r)))
        {
        }
    }

    internal static class MemberUnwrapper{
        public static T Unwrap<T>(this IFrontendCodeElement<ICodeElement> codeElement) where T: IFrontendType<IVarifiableType>
        {
            if (codeElement.Returns().Is< WeakMemberDefinition>(out var member) && 
                member.Type.IsDefinately(out var yes, out var _) &&  
                yes.Value.TypeDefinition.IsDefinately(out var yes2, out var _) &&
                yes2.Value.GetValue().IsDefinately(out var yes3, out var _) &&
                yes3.Is<T>(out var t)) {
                return t;
            }
            return codeElement.Returns().Cast<T>();
        }
    }


}
