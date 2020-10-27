using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Infastructure;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.SemanticModel;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;
using Tac.SyntaxModel.Elements.AtomicTypes;

namespace Tac.Frontend._3_Syntax_Model.Elements
{
    internal class WeakRootScope : IConvertableFrontendCodeElement<IRootScope>, IScoped, IReturn
    {
        public IOrType<IBox<WeakScope>, IError> Scope { get; }
        public IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>> Assignments { get; }
        public IOrType<IBox<WeakEntryPointDefinition>, IError> EntryPoint { get; }

        public WeakRootScope(IOrType<IBox<WeakScope>, IError> scope, IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>> assigns, IOrType<IBox<WeakEntryPointDefinition>, IError> EntryPoint)
        {
            if (assigns == null)
            {
                throw new ArgumentNullException(nameof(assigns));
            }

            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.EntryPoint = EntryPoint ?? throw new ArgumentNullException(nameof(EntryPoint));
            Assignments = assigns.ToArray();


            returns = new Lazy<IOrType<IFrontendType, IError>>(() =>
            {
                return Scope.SwitchReturns(
                      x => OrType.Make<IFrontendType, IError>(new HasMembersType(x.GetValue())),
                      x => OrType.Make<IFrontendType, IError>(x));
            });
        }

        public IBuildIntention<IRootScope> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = RootScope.Create();
            return new BuildIntention<IRootScope>(toBuild, () =>
            {
                maker.Build(
                    Scope.Is1OrThrow().GetValue().Convert(context),
                    Assignments.Select(x => x.Is1OrThrow().GetValue().Convert(context)).ToArray(),
                    EntryPoint.Is1OrThrow().GetValue().Convert(context)
                    );
            });
        }

        public IEnumerable<IError> Validate()
        {
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
                foreach (var error in assignment.SwitchReturns<IEnumerable<IError>>(x => x.GetValue().Validate(), x => new List<IError>() { x }))
                {
                    yield return error;
                }
            }
        }

        private readonly Lazy<IOrType<IFrontendType, IError>> returns;

        public IOrType<IFrontendType, IError> Returns()
        {
            return returns.Value;
        }
    }




    internal class RootScopePopulateScope : ISetUp<IBox<WeakRootScope>, Tpn.IValue>
    {
        private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements;

        public RootScopePopulateScope(IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
        }

        public ISetUpResult<IBox<WeakRootScope>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            scope = scope.EnterInitizaionScopeIfNessisary();

            if (!(scope is Tpn.IScope runtimeScope))
            {
                throw new NotImplementedException("this should be an IError");
            }

            var key = new ImplicitKey(Guid.NewGuid());

            var box = new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>();
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
            x.TransformInner(y => y.Run(myScope, context.CreateChildContext(this)).Resolve)).ToArray());

            var value = context.TypeProblem.CreateValue(runtimeScope, key, new PlaceholderValueConverter());
            // ugh! an object is a type
            //

            return new SetUpResult<IBox<WeakRootScope>, Tpn.IValue>(new ResolveReferanceRootScope(myScope), OrType.Make<Tpn.IValue, IError>(value));
        }
    }

    internal class ResolveReferanceRootScope : IResolve<IBox<WeakRootScope>>
    {
        private readonly Tpn.TypeProblem2.Object myScope;

        public ResolveReferanceRootScope(Tpn.TypeProblem2.Object myScope)
        {
            this.myScope = myScope ?? throw new ArgumentNullException(nameof(myScope));
        }

        // do these really need to be IBox? they seeme to generally be filled...
        // mayble IPossibly...
        public IBox<WeakRootScope> Run(Tpn.TypeSolution context)
        {
            var objectOr = context.GetObject(myScope);
            if (objectOr.GetValue().Is3(out var v3))
            {
                return new Box<WeakRootScope>(v3);
            }
            throw new Exception("wrong or");
        }
    }
}
