using Prototypist.LeftToRight;
using System;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMemberDefinition: IInterpeted
    {
        public InterpetedMemberDefinition Init(IKey key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            return this;
        }
        
        public IKey Key { get; private set; }
        
        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(this);
        }
    }
}