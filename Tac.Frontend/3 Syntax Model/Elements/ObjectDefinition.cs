using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;
using Tac.SemanticModel;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Prototypist.Toolbox;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Frontend.Parser;
using Tac.SyntaxModel.Elements.AtomicTypes;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticObjectDefinitionMaker = AddElementMakers(
            () => new ObjectDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> ObjectDefinitionMaker = StaticObjectDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}


namespace Tac.SemanticModel
{
    // honestly these being types is wierd
    // espially since this is probably the same type as an module
    // I think this returns a WeakTypeDefinition or maybe there should be a class for that
    // I think there should be a class for that
    internal class WeakObjectDefinition: IConvertableFrontendCodeElement<IObjectDefiniton>, IScoped, IReturn
    {
        public WeakObjectDefinition(IOrType<IBox<WeakScope>, IError> scope, IReadOnlyList<IOrType<IBox<WeakAssignOperation>,IError>> assigns) {
            if (assigns == null)
            {
                throw new ArgumentNullException(nameof(assigns));
            }

            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Assignments = assigns.ToArray();
        }

        public IOrType<IBox<WeakScope>, IError> Scope { get; }
        public IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>> Assignments { get; }

        public IBuildIntention<IObjectDefiniton> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ObjectDefiniton.Create();
            return new BuildIntention<IObjectDefiniton>(toBuild, () =>
            {
                maker.Build(
                    Scope.Is1OrThrow().GetValue().Convert(context), 
                    Assignments.Select(x => x.Is1OrThrow().GetValue().Convert(context)).ToArray());
            });
        }


        public IEnumerable<IError> Validate() {
            if (Scope.Is2(out var e1))
            {
                yield return e1;
            }
            else
            {
                foreach (var item in Scope.Is1OrThrow().GetValue().Validate())
                {
                    yield return item;
                }
            }
            foreach (var assignment in Assignments)
            {
                foreach (var error in assignment.SwitchReturns<IEnumerable<IError>>(x => x.GetValue().Validate(), x => new List<IError>() { x}))
                {
                    yield return error;
                }
            }
        }

        public IOrType<IFrontendType, IError> Returns()
        {
            return Scope.SwitchReturns(
                x => OrType.Make<IFrontendType, IError > (new HasMembersType(x.GetValue())),
                x => OrType.Make<IFrontendType, IError>(x));
        }
    }

    internal class ObjectDefinitionMaker : IMaker<ISetUp<IBox<WeakObjectDefinition>, Tpn.IValue>>
    {
        public ObjectDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<IBox<WeakObjectDefinition>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            return tokenMatching
                .Has(new KeyWordMaker("object"))
                .Has(new BodyMaker())
                .ConvertIfMatched((_,block) => new ObjectDefinitionPopulateScope(tokenMatching.Context.ParseBlock(block)));
        }

        private class ObjectDefinitionPopulateScope : ISetUp<IBox<WeakObjectDefinition>, Tpn.IValue>
        {
            private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements;

            public ObjectDefinitionPopulateScope(IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements)
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public ISetUpResult<IBox<WeakObjectDefinition>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
            {
                if (!(scope is Tpn.IScope runtimeScope))
                {
                    throw new NotImplementedException("this should be an IError");
                }

                var key = new ImplicitKey(Guid.NewGuid());

                var box = new Box<IReadOnlyList< IOrType<IResolve<IBox<IFrontendCodeElement>>,IError>>>();
                var myScope = context.TypeProblem.CreateObjectOrModule(scope, key, new WeakObjectConverter(box), new WeakScopeConverter());

                // {6B83A7F1-0E28-4D07-91C8-57E6878E97D9}
                // module has similar code
                //foreach (var element in elements)
                //{
                //    element.Switch(
                //        y =>
                //        {
                //            list.Add(OrType.Make<IResolve<IBox<IFrontendCodeElement>>, IError>(y.Run(myScope, context).Resolve));
                //        },
                //        y =>
                //        {
                //            // it is a bit weird that types are not used at all
                //            y.Run(myScope, context);
                //        },
                //        y =>
                //        {
                //            // this is also a bit wierd, these errors are anything that was not parsed
                //            // they are not really related to the assignments they are bing placed next to
                //            list.Add(OrType.Make<IResolve<IBox<IFrontendCodeElement>>, IError>(y));
                //        });
                //}

                box.Fill(elements.Select(x =>
                x.TransformInner(y => y.Run(myScope, context).Resolve)).ToArray());

                var value = context.TypeProblem.CreateValue(runtimeScope, key, new PlaceholderValueConverter());
                // ugh! an object is a type
                //

                return new SetUpResult<IBox<WeakObjectDefinition>, Tpn.IValue>(new ResolveReferanceObjectDefinition(myScope), OrType.Make<Tpn.IValue, IError>(value));
            }
        }

        private class ResolveReferanceObjectDefinition : IResolve<IBox<WeakObjectDefinition>>
        {
            private readonly Tpn.TypeProblem2.Object myScope;

            public ResolveReferanceObjectDefinition(Tpn.TypeProblem2.Object myScope)
            {
                this.myScope = myScope ?? throw new ArgumentNullException(nameof(myScope));
            }

            // do these really need to be IBox? they seeme to generally be filled...
            // mayble IPossibly...
            public IBox<WeakObjectDefinition> Run(Tpn.TypeSolution context)
            {
                var objectOr = context.GetObject(myScope);
                if (objectOr.GetValue().Is1(out var v1))
                {
                    return new Box<WeakObjectDefinition>(v1);
                }
                throw new Exception("wrong or");
            }
        }
    }
}
