using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedGenericExplicitTypeName : GenericExplicitTypeName, IInterpeted
    {
        public InterpetedGenericExplicitTypeName(string name, params ITypeSource[] types) : base(name, types)
        {
        }
    }
}