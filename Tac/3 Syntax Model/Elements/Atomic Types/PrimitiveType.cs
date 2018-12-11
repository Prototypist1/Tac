using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Semantic_Model;

namespace Tac._3_Syntax_Model.Elements.Atomic_Types
{


    internal class StringType : IFrontendType
    {
    }
    internal class EmptyType : IFrontendType
    {
    }
    internal class NumberType : IFrontendType
    {
    }
    internal class GemericTypeParameterPlacholder : IFrontendType
    {
        public GemericTypeParameterPlacholder(IKey key)
        {
            this.Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IKey Key { get; }

        public override bool Equals(object obj)
        {
            var placholder = obj as GemericTypeParameterPlacholder;
            return placholder != null &&
                   EqualityComparer<IKey>.Default.Equals(Key, placholder.Key);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }
    }
    internal class AnyType : IFrontendType
    {
    }
    internal class BooleanType : IFrontendType
    {
    }
    internal class ImplementationType : IFrontendType
    {
        public ImplementationType(IVarifiableType inputType, IVarifiableType outputType, IVarifiableType contextType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
        }

        public IVarifiableType InputType { get; }
        public IVarifiableType OutputType {get;}
        public IVarifiableType ContextType{get;}
    }
    internal class MethodType : IFrontendType
    {
        public MethodType(IVarifiableType inputType, IVarifiableType outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }

        public IVarifiableType InputType{get;}
        public IVarifiableType OutputType{get;}
    }

    internal class GenericMethodType : IFrontendType
    {

        private readonly IGenericTypeParameterDefinition input = new GenericTypeParameterDefinition("input");
        private readonly IGenericTypeParameterDefinition output = new GenericTypeParameterDefinition("output");

        public GenericMethodType()
        {
            TypeParameterDefinitions = new[] {input,output,};
        }

        public IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }

        public IFrontendType GetConcreteType(Model.Elements.GenericTypeParameter[] parameters)
        {
            if (parameters.Length == 2) {
                return new MethodType(
                    parameters.Single(x => x.Parameter.Key.Equals(input)).Type, 
                    parameters.Single(x => x.Parameter.Key.Equals(output)).Type);
            }
            throw new Exception("Exceptions important, why do you always half ass them?");
        }
    }

    internal class GenericImplementationType : IFrontendType
    {

        private readonly IGenericTypeParameterDefinition input = new GenericTypeParameterDefinition("input");
        private readonly IGenericTypeParameterDefinition output = new GenericTypeParameterDefinition("output");
        private readonly IGenericTypeParameterDefinition context = new GenericTypeParameterDefinition("context");

        public GenericImplementationType()
        {
            TypeParameterDefinitions = new[] { input, output, };
        }

        public IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }

        public IFrontendType GetConcreteType(Model.Elements.GenericTypeParameter[] parameters)
        {
            if (parameters.Length == 3)
            {
                return new ImplementationType(
                    parameters.Single(x => x.Parameter.Key.Equals(input)).Type,
                    parameters.Single(x => x.Parameter.Key.Equals(output)).Type,
                    parameters.Single(x => x.Parameter.Key.Equals(context)).Type);
            }

            throw new Exception("Exceptions important, why do you always half ass them?");
        }
    }
}
