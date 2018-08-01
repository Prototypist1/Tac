using System.Collections.Generic;

namespace Tac.Semantic_Model
{
    public abstract class StaticScope : AbstractScope {
        public bool TryAddStaticMethod(MethodDefinition methodDefinition) {

        }
        public void TryAddStaticMember(MemberDefinition memberDefinition)
        {

        }
        public void TryAddStaticImplementation(ImplementationDefinition implementationDefinition)
        {

        }
    }

    public abstract class LocalScope : StaticScope
    {
        public void TryAddLocalMember(MemberDefinition memberDefinition)
        {

        }
    }

    public abstract class ClosureScope : StaticScope
    {
        public void TryAddClouseMember(MemberDefinition memberDefinition)
        {

        }
    }
    
}