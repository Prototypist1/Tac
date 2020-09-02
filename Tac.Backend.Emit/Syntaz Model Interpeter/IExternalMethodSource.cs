//using System;
//using System.Collections.Generic;
//using Tac.Backend.Emit.SyntazModel.Elements;
//using Tac.Model.Elements;

//namespace Tac.Backend.Emit.SyntazModel
//{
//    internal interface IExternalMethodSource
//    {
//        InterpetedExternalMethodDefinition GetExternalMethod(IExternalMethodDefinition codeElement);
//    }

//    internal class ExternalMethodSource : IExternalMethodSource
//    {
//        private readonly Dictionary<Guid, InterpetedExternalMethodDefinition> map;

//        public ExternalMethodSource(Dictionary<Guid, InterpetedExternalMethodDefinition> map)
//        {
//            this.map = map ?? throw new ArgumentNullException(nameof(map));
//        }

//        public InterpetedExternalMethodDefinition GetExternalMethod(IExternalMethodDefinition codeElement) => map[codeElement.Id];
//    }
//}