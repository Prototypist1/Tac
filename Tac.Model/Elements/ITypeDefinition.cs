using Prototypist.Toolbox;
using System.Collections.Generic;
using Tac.Model;

namespace Tac.Model.Elements
{

    // maybe I don't want to write equals for all these types
    // I hate value type equality
    // so maybe there is a single type that wraps the type
    // and owns equality 

    //public interface ITypeComparer {
    //    bool IsAssignableTo(IVerifiableType from, IVerifiableType to);
    //}

    public interface IVerifiableType {

        //bool TheyAreUs(IVerifiableType they, bool noTagBacks);
        //bool WeAreThem(IVerifiableType them, bool noTagBacks);

        bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue);
        IIsPossibly<(IVerifiableType type , Access access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue);


        IIsPossibly<IVerifiableType> TryGetReturn();
        IIsPossibly<IVerifiableType> TryGetInput();
    }

    public static class IVerifiableTypeExtensions {

        // TODO
        // might be worth pushing this into IVerifiableType and replacing TryGetReturn and TryGetInput
        // so IVerifiableType can't do different things with TryGetReturn and TryGetInput
        public static IIsPossibly<(IVerifiableType input, IVerifiableType output)> TryGetIO(this IVerifiableType self) {
            var hasReturns = self.TryGetReturn().Is(out var returnedType);
            var hasInputs = self.TryGetInput().Is(out var inputType);

            if (hasReturns != hasInputs) {
                throw new System.Exception("these should be the same...");
            }

            if (!hasReturns) {
                return Possibly.IsNot<(IVerifiableType input, IVerifiableType output)>();
            }

            return Possibly.Is((input: inputType, output: returnedType));
        }

        public static bool TryGetIO(this IVerifiableType self, out IVerifiableType input, out IVerifiableType output)
        {
            var hasReturns = self.TryGetReturn().Is(out var returnedType);
            var hasInputs = self.TryGetInput().Is(out var inputType);

            if (hasReturns != hasInputs)
            {
                throw new System.Exception("these should be the same...");
            }

            if (!hasReturns)
            {
                input = default;
                output = default;
                return false;
            }
            input = inputType;
            output = returnedType;
            return true;
        }
    }

    public interface ILogicalOperationType {
    }

    public interface ITypeOr : IVerifiableType {
        IVerifiableType Left { get; }
        IVerifiableType Right { get; }
        IReadOnlyList<IMemberDefinition> Members { get; }
    }

    public interface ITypeAnd : IVerifiableType {
        IVerifiableType Left { get; }
        IVerifiableType Right { get; }
    }


    public interface IPrimitiveType : IVerifiableType { }

    public interface IBlockType : IVerifiableType { }
    public interface IEntryPointType : IVerifiableType { }
    public interface IReferanceType : IVerifiableType { }

    public interface INumberType: IPrimitiveType { }
    public interface IBooleanType: IPrimitiveType { }
    public interface IStringType: IPrimitiveType { }

    public interface IAnyType : IVerifiableType { }
    public interface IEmptyType : IPrimitiveType { }

    public interface IInterfaceModuleType : IVerifiableType, ICodeElement
    {
        // am I sure these need to be ordered?
        // is staticness important here?
        // staticness just controlls access 
        IReadOnlyList<IMemberDefinition> Members { get; }
    }

    public interface IInterfaceType : IInterfaceModuleType
    {
    }

    public interface IModuleType : IInterfaceModuleType
    {
    }

    public interface IMethodType : IVerifiableType {
        IVerifiableType InputType { get; }
        IVerifiableType OutputType { get; }
    }

    public interface IGenericMethodType : IVerifiableType
    {
        IGenericTypeParameter[] Parameters { get; }
        IVerifiableType InputType { get; }
        IVerifiableType OutputType { get; }
    }

    public interface IGenericTypeParameter : IVerifiableType
    { 
    
    }


    // what is this??
    // 
    // so Method is system generic type
    // i have a hard time thinking about this
    // 
    // but really it is not so complex 
    // this creates methods with types
    // 
    // in that case this should have an input type and a output type
    // they are either placeholders or real types.. we can expect atleat one is a placeholder
    //
    // 
    //public interface IGenericMethodType : IGenericType
    //{
    //}

    // TODO an implementation type is a method type!
    //public interface IImplementationType : IVerifiableType {
    //    IVerifiableType InputType { get; }
    //    IVerifiableType OutputType { get; }
    //    IVerifiableType ContextType { get; }
    //}

    //public interface IGenericImplementationType : IGenericType
    //{
    //}
}
