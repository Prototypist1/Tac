using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model.Names
{
    public abstract class AbstractName {

    }

    public class GenericExplicitName : ExplicitName
    {
        public GenericExplicitName(string name) : base(name)
        {
        }
    }
}
