//using Prototypist.Toolbox;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Tac.Frontend;
//using Tac.Frontend.SyntaxModel.Operations;
//using Tac.Frontend.New;
//using Tac.Frontend.New.CrzayNamespace;
//using Tac.Frontend.Parser;
//using Tac.Model;
//using Tac.Model.Elements;
//using Tac.Model.Instantiated;
//using Tac.Infastructure;
//using Tac.Parser;
//using Tac.SemanticModel;
//using Tac.SyntaxModel.Elements.AtomicTypes;
//using Tac.SemanticModel.Operations;


//namespace Tac.SemanticModel
//{

//    // honestly these being types is wierd
//    // espially since this is probably the same type as an object
//    // I think this returns a WeakTypeDefinition or maybe there should be a class for that
//    // I think there should be a class for that
//    internal class WeakModuleDefinition : IScoped, IConvertableFrontendCodeElement<IModuleDefinition>, IReturn
//    {
//        public WeakModuleDefinition(IOrType<IBox<WeakScope>, IError> scope, IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>> staticInitialization, IKey Key, IBox<WeakEntryPointDefinition> entryPoint)
//        {
//            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
//            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
//            this.Key = Key ?? throw new ArgumentNullException(nameof(Key));
//            EntryPoint = entryPoint ?? throw new ArgumentNullException(nameof(entryPoint));
//        }

//        public IOrType<IBox<WeakScope>, IError> Scope { get; }
//        IBox<WeakEntryPointDefinition> EntryPoint { get; }
//        public IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>> StaticInitialization { get; }

//        public IKey Key
//        {
//            get;
//        }

//        public IBuildIntention<IModuleDefinition> GetBuildIntention(IConversionContext context)
//        {
//            var (toBuild, maker) = ModuleDefinition.Create();
//            return new BuildIntention<IModuleDefinition>(toBuild, () =>
//            {
//                var staticInit = new List<IOrType<ICodeElement, IError>>();

//                foreach (var item in StaticInitialization)
//                {
//                    item.Switch(x1 =>
//                    {
//                        var converted = x1.GetValue().PossiblyConvert(context);
//                        if (converted is IIsDefinately<ICodeElement> isCodeElement)
//                        {
//                            staticInit.Add(OrType.Make<ICodeElement, IError>(isCodeElement.Value));
//                        }
//                    }, x2 =>
//                    {
//                        staticInit.Add(OrType.Make<ICodeElement, IError>(x2));
//                    });
//                }


//                maker.Build(
//                    Scope.Is1OrThrow().GetValue().Convert(context),
//                    staticInit.Select(x => x.Is1OrThrow()).ToArray(),
//                    Key,
//                    EntryPoint.GetValue().Convert(context));
//            });
//        }

//        public IEnumerable<IError> Validate()
//        {

//            if (Scope.Is2(out var e1))
//            {
//                yield return e1;
//            }
//            else
//            {
//                foreach (var error in Scope.Is1OrThrow().GetValue().Validate())
//                {
//                    yield return error;
//                }
//            }
//            foreach (var error in EntryPoint.GetValue().Validate())
//            {
//                yield return error;
//            }
//            foreach (var line in StaticInitialization)
//            {
//                foreach (var error in line.SwitchReturns(x => x.GetValue().Validate(), x => new[] { x }))
//                {
//                    yield return error;
//                }
//            }
//        }

//        //public IFrontendType AssuredReturns()
//        //{
//        //    return Scope.SwitchReturns<IFrontendType>(
//        //        x=>new HasMembersType(x.GetValue()),
//        //        x=> new IndeterminateType(x));
//        //}

//        public IOrType<IFrontendType, IError> Returns()
//        {
//            return Scope.TransformInner(x => new HasMembersType(x.GetValue()));//  OrType.Make< IFrontendType, IError > (AssuredReturns());
//        }
//    }
//}
