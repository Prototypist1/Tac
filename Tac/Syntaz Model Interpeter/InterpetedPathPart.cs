using Tac.Semantic_Model;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedPathPart: PathPart
    {
        internal static readonly PathPart.Make MakeNew = (memberDefinition) => new InterpetedPathPart(memberDefinition);

        public InterpetedPathPart(IBox<MemberDefinition> memberDefinition) : base(memberDefinition)
        {
        }
    }
}