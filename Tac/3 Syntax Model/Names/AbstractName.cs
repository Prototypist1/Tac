using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model.Names
{
    public abstract class AbstractName {

    }

    public class GenericExplicitName : ExplicitName
    {
        public GenericExplicitName(string name,params AbstractName[] types) : base(name)
        {
            Types = types ?? throw new System.ArgumentNullException(nameof(types));
        }

        public AbstractName[] Types { get; }
    }
}
