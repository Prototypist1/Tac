//using System.Collections.Generic;
//using Tac.Semantic_Model.CodeStuff;
//using Tac.Semantic_Model.Names;

//namespace Tac.Semantic_Model
//{
//    // TODO this is not really a thing... 
//    // static members exist, some of them are mothods

//    public sealed class StaticMethodDefinition : MethodDefinition, IReferanced
//    {
//        public StaticMethodDefinition(TypeReferance outputType, ParameterDefinition parameterDefinition, ICodeElement[] body, AbstractName key) : base(outputType, parameterDefinition, body)
//        {
//            Key = key;
//        }

//        public AbstractName Key { get; }

//        public override bool Equals(object obj)
//        {
//            var definition = obj as StaticMethodDefinition;
//            return definition != null &&
//                   base.Equals(obj) &&
//                   EqualityComparer<AbstractName>.Default.Equals(Key, definition.Key);
//        }

//        public override int GetHashCode()
//        {
//            var hashCode = -229860446;
//            hashCode = hashCode * -1521134295 + base.GetHashCode();
//            hashCode = hashCode * -1521134295 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
//            return hashCode;
//        }
//    }
//}
