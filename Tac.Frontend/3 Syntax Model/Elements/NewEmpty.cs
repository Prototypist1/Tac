using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Model.Elements;
using Tac.Model.Instantiated.Elements;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.Frontend;
using Tac.Frontend.SyntaxModel.Elements;
using Tac.Model;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Prototypist.Toolbox;
using Tac.SemanticModel.CodeStuff;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticEmptyInstanceMaker = AddElementMakers(
            () => new EmptyInstanceMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> EmptyInstanceMaker = StaticEmptyInstanceMaker;
#pragma warning restore CA1823
    }
}



namespace Tac.Frontend.SyntaxModel.Elements
{
    // what is the empty instance...
    // I though empty was a type
    // more analgous to something like a bool with a single value
    // than the null reference 
    // yeah but you still need to be able to create one...
    internal class WeakEmptyInstance : IConvertableFrontendCodeElement<IEmptyInstance>, IReturn
    {
        public WeakEmptyInstance()
        {
        }

        public IBuildIntention<IEmptyInstance> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = EmptyInstance.Create();
            return new BuildIntention<IEmptyInstance>(toBuild, () =>
            {
                maker.Build();
            });
        }

        public IOrType<IFrontendType<IVerifiableType>, IError> Returns() => OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType());
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }

    // just call this null!
    internal class EmptyInstanceMaker : IMaker<ISetUp<IBox<WeakEmptyInstance>, Tpn.IValue>>
    {
        public EmptyInstanceMaker() { }

        public ITokenMatching<ISetUp<IBox<WeakEmptyInstance>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            // change key word to nothing?
            return tokenMatching
                .Has(new KeyWordMaker("new-empty"), out var _)
                .ConvertIfMatched(()=> new EmptyInstancePopulateScope(), tokenMatching);
        }

        public static ISetUp<IBox<WeakEmptyInstance>, Tpn.IValue> PopulateScope()
        {
            return new EmptyInstancePopulateScope();
        }
        public static IResolve<IBox<WeakEmptyInstance>> PopulateBoxes()
        {
            return new EmptyInstanceResolveReferance();
        }

    }


    internal class EmptyInstancePopulateScope : ISetUp<IBox<WeakEmptyInstance>, Tpn.IValue>
    {

        public EmptyInstancePopulateScope() { }

        public ISetUpResult<IBox<WeakEmptyInstance>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {

            scope = scope.EnterInitizaionScopeIfNessisary();
            if (!(scope is Tpn.IScope runtimeScope))
            {
                throw new NotImplementedException("this should be an IError");
            }

            var value = context.TypeProblem.CreateValue(runtimeScope, new NameKey("empty"), new PlaceholderValueConverter());
            return new SetUpResult<IBox<WeakEmptyInstance>, Tpn.IValue>(new EmptyInstanceResolveReferance(), OrType.Make<Tpn.IValue, IError>(value));
        }
    }

    internal class EmptyInstanceResolveReferance : IResolve<IBox<WeakEmptyInstance>>
    {
        public EmptyInstanceResolveReferance()
        {
        }

        public IBox<WeakEmptyInstance> Run(Tpn.TypeSolution context)
        {
            return new Box<WeakEmptyInstance>(new WeakEmptyInstance());
        }
    }
}
