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
        private readonly IReadOnlyDictionary<IKey, IVerifiableType> types = new Dictionary<IKey,IVerifiableType>();

        public FinalizedScope(IReadOnlyDictionary<IKey, IMemberDefinition> members, IReadOnlyDictionary<IKey,IVerifiableType> types) {
            this.members = members ?? throw new ArgumentNullException(nameof(members));
            this.types = types ?? throw new ArgumentNullException(nameof(types));
        }

        public FinalizedScope(IReadOnlyDictionary<IKey, IMemberDefinition> members, IReadOnlyDictionary<IKey, IVerifiableType> types, IFinalizedScope parent): this(members,types)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public IEnumerable<IKey> MemberKeys => members.Keys;

        public IEnumerable<IMemberDefinition> Members => members.Values;

        public IEnumerable<IVerifiableType> Types => types.Values;

        public IEnumerable<IKey> TypeKeys => types.Keys;

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

        public bool TryGetType(IKey name, out IVerifiableType type)
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
