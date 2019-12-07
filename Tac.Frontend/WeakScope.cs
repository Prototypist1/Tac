using System.Collections.Generic;
using Tac.Model;
using Tac.Semantic_Model;

namespace Tac.Frontend
{
    internal class WeakScope : IConvertable<IFinalizedScope>
    {
        private List<IBox<WeakMemberDefinition>> membersList;

        public WeakScope(List<IBox<WeakMemberDefinition>> membersList)
        {
            this.membersList = membersList;
        }

        public IBuildIntention<IFinalizedScope> GetBuildIntention(IConversionContext context)
        {

        }
    }
}