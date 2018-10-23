using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;

namespace Tac.Tests.Samples
{
    //internal class TestScope : IFinalizedScope
    //{
    //    private readonly Dictionary<IKey, (bool,MemberDefinition)> backingMembers;
    //    private readonly Dictionary<IKey, IReturnable> backingTypes;

    //    public TestScope(Dictionary<IKey, (bool,MemberDefinition)> backingMembers, Dictionary<IKey,IReturnable> backingTypes)
    //    {
    //        this.backingMembers = backingMembers;
    //        this.backingTypes = backingTypes;
    //    }

    //    public TestScope(Dictionary<IKey, (bool, MemberDefinition)> backingMembers): this(backingMembers, new Dictionary<IKey, IReturnable>()){}

    //    public IReadOnlyList<IBox<MemberDefinition>> Members
    //    {
    //        get
    //        {
    //            return backingMembers.Select(x=>new Box<MemberDefinition>(x.Value.Item2)).ToArray();
    //        }
    //    }

    //    //public bool TryGetMember(IKey name, bool staticOnly, out IBox<MemberDefinition> member)
    //    //{
    //    //    if (!backingMembers.TryGetValue(name, out var entry)) {
    //    //        member = default;
    //    //        return false;
    //    //    }

    //    //    if (staticOnly && !entry.Item1) {
    //    //        member = default;
    //    //        return false;
    //    //    }

    //    //    member = new Box<MemberDefinition>(entry.Item2);
    //    //    return true;
    //    //}

    //    //public bool TryGetType(IKey name, out IBox<IReturnable> type)
    //    //{
    //    //    if (!backingTypes.TryGetValue(name, out var entry))
    //    //    {
    //    //        type = default;
    //    //        return false;
    //    //    }
            
    //    //    type = new Box<IReturnable>(entry);
    //    //    return true;
    //    //}
    //}
}
