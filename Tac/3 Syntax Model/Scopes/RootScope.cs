using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class RootScope : IScope
    {
        public override bool Equals(object obj) => obj is RootScope;
        public override int GetHashCode() => 1522345295;

        private RootScope(){}

        public static RootScope Root = new RootScope(); 

        public bool TryGet<TReferanced>(NamePath key, out TReferanced item) where TReferanced : IReferanced
        {

            item = default;
            return false;
        }


        private class FundimentalType : ITypeDefinition
        {
            public ITypeDefinition ReturnType(IScope scope) => this;
        }

        private class GenericFundimentalType : ITypeDefinition
        {
            public ITypeDefinition ReturnType(IScope scope) => this;
        }

        public static ITypeDefinition StringType { get; } = new FundimentalType();
        public static ITypeDefinition NumberType { get; } = new FundimentalType();
        public static ITypeDefinition EmptyType { get; } = new FundimentalType();
        public static ITypeDefinition BooleanType { get; } = new FundimentalType();
        public static ITypeDefinition MethodType()
        {
            new GenericFundimentalType();
        }


    }

}