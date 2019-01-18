using System;
using System.Collections.Generic;

namespace Tac.Model.Elements
{

    public interface IGenericType : IVerifiableType
    {
        IReadOnlyList<IKey> TypeParameterKeys { get; }
        //IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }
    }
    // I don't think this is a useful interface...
    //public interface IGenericTypeParameterDefinition {
    //    IKey Key { get; }
    //}
    //public class GenericTypeParameterDefinition: IGenericTypeParameterDefinition
    //{
    //    public GenericTypeParameterDefinition(IKey key)
    //    {
    //        Key = key ?? throw new ArgumentNullException(nameof(key));
    //    }

    //    public IKey Key { get; }


    //}

    

}
