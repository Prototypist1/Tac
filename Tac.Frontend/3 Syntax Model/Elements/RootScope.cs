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
        public Box<IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>>> Assignments { get; }
        public Box<IOrType<IBox<WeakEntryPointDefinition>, IError>> EntryPoint { get; }

        public WeakRootScope(IOrType<IBox<WeakScope>, IError> scope, Box<IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>>> assigns, Box<IOrType<IBox<WeakEntryPointDefinition>, IError>> EntryPoint)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.EntryPoint = EntryPoint ?? throw new ArgumentNullException(nameof(EntryPoint));
            Assignments = assigns ?? throw new ArgumentNullException(nameof(assigns));


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
                    Assignments.GetValue().Select(x => x.Is1OrThrow().GetValue().Convert(context)).ToArray(),
                    EntryPoint.GetValue().Is1OrThrow().GetValue().Convert(context)
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
            foreach (var assignment in Assignments.GetValue())
            {
                foreach (var error in assignment.SwitchReturns<IEnumerable<IError>>(x => x.GetValue().Validate(), x => new List<IError>() { x }))
                {
                    yield return error;
                }
            }
            foreach (var error in EntryPoint.GetValue().SwitchReturns<IEnumerable<IError>>(x => x.GetValue().Validate(), x => new List<IError>() { x }))
            {
                yield return error;
            }
        }

        private readonly Lazy<IOrType<IFrontendType, IError>> returns;

        public IOrType<IFrontendType, IError> Returns()
        {
            return returns.Value;
        }
    }


    internal class RootScopePopulateScope : ISetUp<IBox<WeakRootScope>, Tpn.TypeProblem2.Object>
    {
        private readonly IReadOnlyList<IOrType<WeakAssignOperationPopulateScope, IError>> elements;
        private readonly IOrType<EntryPointDefinitionPopulateScope, IError> entry;
        private readonly IReadOnlyList<IOrType<TypeDefinitionPopulateScope,  IError>> types;
        private readonly IReadOnlyList<IOrType<GenericTypeDefinitionPopulateScope, IError>> genericTypes;


        public RootScopePopulateScope(
            IReadOnlyList<IOrType<WeakAssignOperationPopulateScope, IError>> elements, 
            IOrType<EntryPointDefinitionPopulateScope, IError> entry, 
            IReadOnlyList<IOrType<TypeDefinitionPopulateScope,  IError>> types,
            IReadOnlyList<IOrType<GenericTypeDefinitionPopulateScope, IError>> genericTypes)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.entry = entry ?? throw new ArgumentNullException(nameof(entry));
            this.types = types ?? throw new ArgumentNullException(nameof(types));
            this.genericTypes = genericTypes ?? throw new ArgumentNullException(nameof(genericTypes));
        }


        private Box<IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>>> assignmentsBox = new Box<IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>>>();
        private Box<IOrType<IBox<WeakEntryPointDefinition>,IError>> entryBox = new Box<IOrType<IBox<WeakEntryPointDefinition>, IError>>();

        private ImplicitKey key;
        private Tpn.TypeProblem2.Object myScope;
        public Tpn.TypeProblem2.Object InitizeForTypeProblem(Tpn.TypeProblem2 typeProblem2) {
            key = new ImplicitKey(Guid.NewGuid());
            myScope = typeProblem2.builder.CreateObjectOrModule(typeProblem2.Dependency, key,  new WeakRootConverter(assignmentsBox, entryBox), new WeakScopeConverter());
            return myScope;
        }

        public ISetUpResult<IBox<WeakRootScope>, Tpn.TypeProblem2.Object> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            //scope = scope.EnterInitizaionScopeIfNessisary();

            //if (!(scope is Tpn.IScope runtimeScope))
            //{
            //    throw new NotImplementedException("this should be an IError");
            //}


            //var assignmentsBox = new Box<IReadOnlyList<IOrType<IResolve<IBox<WeakAssignOperation>>, IError>>>();
            //var entryBox = new Box<IOrType<IResolve<IBox<WeakEntryPointDefinition>>, IError>>();
            //var myScope = context.TypeProblem.CreateObjectOrModule(context.TypeProblem.ModuleRoot, key, new WeakRootConverter(assignmentsBox, entryBox), new WeakScopeConverter());

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

            var nextElements = elements.Select(x =>x.TransformInner(y => y.Run(myScope, context.CreateChildContext(this)).Resolve)).ToArray();
            var nextEntry = entry.TransformInner(y => y.Run(myScope, context.CreateChildContext(this)).Resolve);


            var ranTypes = types.Select(x => x.TransformInner(y => y.Run(myScope, context.CreateChildContext(this)).Resolve)).ToArray();
            var ranGenericTypes = genericTypes.Select(x => x.TransformInner(y => y.Run(myScope, context.CreateChildContext(this)).Resolve)).ToArray();

            //var value = context.TypeProblem.CreateValue(runtimeScope, key, new PlaceholderValueConverter());
            // ugh! an object is a type
            //

            return new SetUpResult<IBox<WeakRootScope>, Tpn.TypeProblem2.Object>(new ResolveReferanceRootScope(myScope, ranTypes, ranGenericTypes, nextElements, nextEntry, assignmentsBox,entryBox), OrType.Make<Tpn.TypeProblem2.Object, IError>(myScope));
        }
    }

    internal class ResolveReferanceRootScope : IResolve<IBox<WeakRootScope>>
    {
        private readonly Tpn.TypeProblem2.Object myScope;
        private readonly IReadOnlyList< IOrType<IResolve<IBox<IFrontendType>>, IError>> ranTypes;
        private readonly IReadOnlyList<IOrType<IResolve<IBox<WeakGenericTypeDefinition>>, IError>> ranGenericTypes;
        private readonly IOrType<IResolve<IBox<WeakAssignOperation>>, IError>[] nextElements;
        private readonly IOrType<IResolve<IBox<WeakEntryPointDefinition>>, IError> nextEntry;
        private readonly Box<IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>>> assignmentsBox;
        private readonly Box<IOrType<IBox<WeakEntryPointDefinition>, IError>> entryBox;

        public ResolveReferanceRootScope(Tpn.TypeProblem2.Object myScope, IReadOnlyList<IOrType<IResolve<IBox<IFrontendType>>, IError>> ranTypes, IReadOnlyList<IOrType<IResolve<IBox<WeakGenericTypeDefinition>>, IError>> ranGenericTypes,
            IOrType<IResolve<IBox<WeakAssignOperation>>,IError>[] nextElements,
            IOrType<IResolve<IBox<WeakEntryPointDefinition>>,IError> nextEntry,
            Box<IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>>> assignmentsBox,
            Box<IOrType<IBox<WeakEntryPointDefinition>, IError>> entryBox
            )
        {
            this.myScope = myScope ?? throw new ArgumentNullException(nameof(myScope));
            this.ranTypes = ranTypes ?? throw new ArgumentNullException(nameof(ranTypes));
            this.ranGenericTypes = ranGenericTypes ?? throw new ArgumentNullException(nameof(ranGenericTypes));
            this.nextElements = nextElements ?? throw new ArgumentNullException(nameof(nextElements));
            this.nextEntry = nextEntry ?? throw new ArgumentNullException(nameof(nextEntry));
            this.assignmentsBox = assignmentsBox ?? throw new ArgumentNullException(nameof(assignmentsBox));
            this.entryBox = entryBox ?? throw new ArgumentNullException(nameof(entryBox));
        }

        // do these really need to be IBox? they seeme to generally be filled...
        // mayble IPossibly...
        public IBox<WeakRootScope> Run(Tpn.TypeSolution context)
        {
            assignmentsBox.Fill(nextElements.Select(x => x.TransformInner(y => y.Run(context))).ToArray());

            entryBox.Fill(nextEntry.TransformInner(x => x.Run(context)));

            ranTypes.Select(x => x.TransformInner(y => y.Run(context))).ToArray();
            ranGenericTypes.Select(x => x.TransformInner(y => y.Run(context))).ToArray();
            var objectOr = context.GetObject(myScope);
            if (objectOr.GetValue().Is2(out var v2))
            {
                return new Box<WeakRootScope>(v2);
            }
            throw new Exception("wrong or");
        }
    }
}
