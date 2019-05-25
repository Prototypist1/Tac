//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Tac.Model.Elements;

//namespace Tac.Model.Instantiated
//{
//    // I don't think generics exist in the model.. compiled out 
//    public class GenericInterfaceDefinition : IGenericInterfaceDefinition, IGenericInterfaceDefinitionBuilder
//    {
//        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();
//        private readonly Buildable<IVerifiableType[]> buildableTypeParameterDefinitions = new Buildable<IVerifiableType[]>();


//        public IReadOnlyList<IKey> TypeParameterKeys => buildableTypeParameterDefinitions.Get().OfType<GemericTypeParameterPlacholder>().Select(x => x.Key).ToArray();


//        public GenericInterfaceDefinition()
//        {
//        }

//        public IFinalizedScope Scope { get => buildableScope.Get(); }
//        public IVerifiableType[] TypeParameterDefinitions { get => buildableTypeParameterDefinitions.Get(); }
//        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
//            where TBacking : IBacking
//        {
//            return context.GenericTypeDefinition(this);
//        }

//        public void Build(IFinalizedScope scope, IVerifiableType[] typeParameterDefinitions)
//        {
//            buildableScope.Set(scope);
//            buildableTypeParameterDefinitions.Set(typeParameterDefinitions);
//        }

//        public static (IGenericInterfaceDefinition, IGenericInterfaceDefinitionBuilder) Create()
//        {
//            var res = new GenericInterfaceDefinition();
//            return (res, res);
//        }

//        public static IGenericInterfaceDefinition CreateAndBuild(IFinalizedScope scope, IVerifiableType[] typeParameterDefinitions) {
//            var (x, y) = Create();
//            y.Build(scope, typeParameterDefinitions);
//            return x;
//        }
//    }

//    //public class TestGenericTypeParameterDefinition : IGenericTypeParameterDefinition
//    //{
//    //    public TestGenericTypeParameterDefinition(IKey key)
//    //    {
//    //        Key = key ?? throw new ArgumentNullException(nameof(key));
//    //    }

//    //    public IKey Key { get; }
//    //}

//    public interface IGenericInterfaceDefinitionBuilder
//    {
//        void Build(IFinalizedScope scope, IVerifiableType[] typeParameterDefinitions);
//    }
//}
