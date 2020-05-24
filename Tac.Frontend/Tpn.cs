using Prototypist.Toolbox;
using System;
using System.Collections;
using System.Collections.Generic;
using Tac.Model;
using Tac.SemanticModel;
using Tac.SyntaxModel.Elements.AtomicTypes;

namespace Tac.Frontend.New.CrzayNamespace
{

    // todo there are some pretty stupid helpers here
    // I think I like isPossibly more than IKey? atm it is stronger
    // if there is anywhere I need comments it is here

    internal class Unset { }

    // this static class is here just to make us all think in terms of these bros
    internal partial class Tpn
    {

        internal class TypeAndConverter
        {
            public readonly IOrType<NameKey, ImplicitKey> key;
            public readonly IConvertTo<TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter;

            public TypeAndConverter(IOrType<NameKey, ImplicitKey> key, IConvertTo<TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter)
            {
                this.key = key ?? throw new ArgumentNullException(nameof(key));
                this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
            }

            public override string? ToString()
            {
                return key.ToString();
            }
        }

        internal interface IConvertTo<in TConvertFrom, out TConvertsTo>
        {
            TConvertsTo Convert(ITypeSolution typeSolution, TConvertFrom from);
        }

        // the simple model of or-types:
        // they don't have any members
        // they don't have any types

        // they might be able to flow there or-ness up stream 
        // but that is more complex than I am interested in right now

        // maybe they are a primitive generic - no 
        // they are a concept created by the type system

        // to the type system they almost just look like an empty user defined type
        // 


        internal interface ITypeProblemNode
        {
            ISetUpTypeProblem Problem { get; }
        }

        internal interface IHavePublicMembers : ITypeProblemNode
        {
            public Dictionary<IKey, TypeProblem2.Member> PublicMembers { get; }
        }

        internal interface IHavePossibleMembers : ITypeProblemNode
        {
            public Dictionary<IKey, TypeProblem2.Member> PossibleMembers { get; }
        }

        internal interface IHavePrivateMembers : ITypeProblemNode
        {
            public Dictionary<IKey, TypeProblem2.Member> PrivateMembers { get; }
        }

        internal interface ILookUpType : ITypeProblemNode
        {
            public IOrType<IKey, IError, Unset> TypeKey { get; set; }
            public IIsPossibly<IStaticScope> Context { get; set; }
            public IIsPossibly<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> LooksUp { get; set; }
        }

        internal interface ICanAssignFromMe : ITypeProblemNode, ILookUpType { }
        internal interface ICanBeAssignedTo : ITypeProblemNode, ILookUpType { }
        internal interface IValue : ITypeProblemNode, ILookUpType, ICanAssignFromMe
        {
            public Dictionary<IKey, TypeProblem2.Member> HopefulMembers { get; }
            public IIsPossibly<TypeProblem2.InferredType> HopefulMethod { get; set; }
        }
        //public interface Member :  IValue, ILookUpType, ICanBeAssignedTo {bool IsReadonly { get; }}
        internal interface IExplicitType : IStaticScope, IHavePublicMembers { 
        
        }

        internal interface IStaticScope : ITypeProblemNode{
            IIsPossibly<IStaticScope> Parent { get; set; }
            public List<TypeProblem2.TypeReference> Refs { get; }
            public Dictionary<IKey, TypeProblem2.OrType> OrTypes { get; }
            public Dictionary<IKey, TypeProblem2.Type> Types { get; }
            public Dictionary<IKey, TypeProblem2.MethodType> MethodTypes { get; }
            public Dictionary<IKey, TypeProblem2.Method> Methods { get; }
            public Dictionary<IKey, TypeProblem2.Object> Objects { get; }
            public List<TypeProblem2.Scope> EntryPoints { get; }
            public List<TypeProblem2.Value> Values { get; }
        }

        internal interface IScope: IStaticScope,IHavePrivateMembers, IHavePossibleMembers
        {

            public List<TypeProblem2.TransientMember> TransientMembers { get; }
        }
        //internal interface IMethod : IHaveMembers, IScope { }
        internal interface IHaveInputAndOutput : ITypeProblemNode { }
        //internal interface IHavePlaceholders: ITypeProblemNode { }

        // TODO is transient member really not a member?
        // can't it's transientness be captured but waht dict it is in??
        internal interface IMember : IValue, ILookUpType, ICanBeAssignedTo { }
    }

    internal static class TpnExtensions
    {
        //extensions
        public static IKey Key(this Tpn.TypeProblem2.TypeReference type)
        {
            // I THINK typeReference always have a Key
            return type.Problem.GetKey(type).GetOrThrow();
        }

        public static Tpn.TypeProblem2.TransientMember Returns(this Tpn.IValue method)
        {
            return method.Problem.GetReturns(method);
        }

        public static Tpn.TypeProblem2.TransientMember Returns(this Tpn.IScope method)
        {
            return method.Problem.GetReturns(method);
        }


        public static Tpn.TypeProblem2.Member Input(this Tpn.TypeProblem2.Method method)
        {
            return method.Problem.GetInput(method);
        }

        public static Tpn.TypeProblem2.Member Input(this Tpn.IValue method)
        {
            return method.Problem.GetInput(method);
        }

        public static void AssignTo(this Tpn.ICanAssignFromMe from, Tpn.ICanBeAssignedTo to)
        {
            from.Problem.IsAssignedTo(from, to);
        }
    }
}
