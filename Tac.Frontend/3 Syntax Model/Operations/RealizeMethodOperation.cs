using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tac.Frontend;
using Tac.Frontend._3_Syntax_Model.Operations;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Infastructure;
using Tac.Model;
using Tac.Model.Operations;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SemanticModel.CodeStuff;

namespace Tac.Frontend._3_Syntax_Model.Operations
{
    internal class WeakRealizeMethodOperation : IConvertableFrontendCodeElement<IRealizeMethodOperation>
    {
        public IBuildIntention<IRealizeMethodOperation> GetBuildIntention(IConversionContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IError> Validate()
        {
            throw new NotImplementedException();
        }
    }

    // matches something of the form:
    // [a,b,c]
    // also
    // [a[a1,a2],b[b1,b2], c[c1,c2]]
    // but 
    internal class ContextDependentSquareListMaker : IMaker<ISetUp<IBox<WeakTypeCollection>, Tpn.IValue>>
    {
        public ITokenMatching<ISetUp<IBox<WeakTypeCollection>, Tpn.IValue>> TryMake(IMatchedTokenMatching elementToken)
        {
            if (elementToken.Has(new GenericNMaker(), out var collection).SafeIs(out IMatchedTokenMatching<IKey[]> matched))
            {

                return TokenMatching<ISetUp<IBox<WeakTypeCollection>, Tpn.IValue>>.MakeMatch(
                    elementToken,
                    new ContextDependentSquareListPopulateScope(collection),
                    matched.EndIndex);
            }

            return TokenMatching<ISetUp<IBox<WeakTypeCollection>, Tpn.IValue>>.MakeNotMatch(elementToken.Context);
        }

    }


    internal class ContextDependentSquareListPopulateScope : ISetUp<IBox<WeakTypeCollection>, Tpn.IValue>
    {
        private IKey[] collection;

        public ContextDependentSquareListPopulateScope(IKey[] collection)
        {
            this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public ISetUpResult<IBox<WeakTypeCollection>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            return new SetUpResult<IBox<WeakTypeCollection>, Tpn.IValue>(
                new ContextDependentSquareListResolveReferance(),
                getReturnedValue(scope, context, nextLeft));
        }
    }

    internal class ContextDependentSquareListResolveReferance : IResolve<IBox<WeakTypeCollection>>
    {
        public IBox<WeakTypeCollection> Run(Tpn.TypeSolution context)
        {
            throw new NotImplementedException();
        }
    }
}


internal class WeakTypeCollection
{

}

internal class RealizeMethodOperationMaker : IMaker<ISetUp<IBox<WeakRealizeMethodOperation>, Tpn.IValue>>
{
    public ITokenMatching<ISetUp<IBox<WeakRealizeMethodOperation>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
    {
        if (tokenMatching.Has(new BinaryOperationMatcher(SymbolsRegistry.RealizeSymbol), out (ISetUp lhs, ISetUp rhs) _).SafeIs(out IMatchedTokenMatching<(ISetUp lhs, ISetUp rhs)> matched))
        {

            return TokenMatching<ISetUp<IBox<WeakRealizeMethodOperation>, Tpn.IValue>>.MakeMatch(
                tokenMatching,
                new RealizeMethodOperationPopulateScope(),
                matched.EndIndex);
        }

        return TokenMatching<ISetUp<IBox<WeakRealizeMethodOperation>, Tpn.IValue>>.MakeNotMatch(tokenMatching.Context);
    }
}

internal class RealizeMethodOperationPopulateScope : ISetUp<IBox<WeakRealizeMethodOperation>, Tpn.IValue>
{
    public ISetUpResult<IBox<WeakRealizeMethodOperation>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
    {
        return new SetUpResult<IBox<WeakRealizeMethodOperation>, Tpn.IValue>(
            new RealizeMethodOperationResolveReferance(),
            getReturnedValue(scope, context, nextLeft));
    }
}

internal class RealizeMethodOperationResolveReferance : IResolve<IBox<WeakRealizeMethodOperation>>
{
    public IBox<WeakRealizeMethodOperation> Run(Tpn.TypeSolution context)
    {
        throw new NotImplementedException();
    }
}
}


namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticRealizeMethodOperation = AddOperationMatcher(() => new RealizeMethodOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> RealizeMethodOperation = StaticRealizeMethodOperation;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.SemanticModel.CodeStuff
{
    public partial class SymbolsRegistry
    {
        public static readonly string RealizeSymbol = StaticSymbolsRegistry.AddOrThrow("realize");
    }
}

