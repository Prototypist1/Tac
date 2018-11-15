using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.TestCases.Help
{
    public class FinalizedScope: IFinalizedScope
    {
        private readonly IFinalizedScope parent;
        private readonly IReadOnlyDictionary<IKey, IMemberDefinition> members = new Dictionary<IKey,IMemberDefinition>();
        private readonly IReadOnlyDictionary<IKey, IVarifiableType> types = new Dictionary<IKey,IVarifiableType>();

        public FinalizedScope(IReadOnlyDictionary<IKey, IMemberDefinition> members) {
            this.members = members ?? throw new ArgumentNullException(nameof(members));
        }

        public FinalizedScope(IReadOnlyDictionary<IKey, IMemberDefinition> members, IFinalizedScope parent)
        {
            this.members = members ?? throw new ArgumentNullException(nameof(members));
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public IEnumerable<IKey> MemberKeys => members.Keys;

        public bool TryGetMember(IKey name, bool staticOnly, out IMemberDefinition box)
        {
            if (members.ContainsKey(name)){
                box = members[name];
                return true;
            }
            box = default;
            return false;
        }

        public bool TryGetParent(out IFinalizedScope res)
        {
            res = parent;
            return parent != null;
        }

        public bool TryGetType(IKey name, out IVarifiableType type)
        {
            if (types.ContainsKey(name))
            {
                type = types[name];
                return true;
            }
            type = default;
            return false;
        }
    }
}
