using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    // really really not sure how these work atm
    // for now they just hold everything you need to ake a method

    public class ImplementationDefinition: ITypeSource, ITypeDefinition
    {
        public ImplementationDefinition(MemberDefinition contextDefinition, ITypeDefinition outputType, MemberDefinition parameterDefinition, IEnumerable<ICodeElement> metohdBody, IScope scope, IEnumerable<ICodeElement> staticInitializers)
        {
            ContextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            MethodBody = metohdBody ?? throw new ArgumentNullException(nameof(metohdBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialzers = staticInitializers ?? throw new ArgumentNullException(nameof(staticInitializers));
        }

        // dang! these could also be inline definitions 
        public ITypeDefinition ContextType
        {
            get
            {
                return ContextDefinition.Type;
            }
        }
        public ITypeDefinition InputType
        {
            get
            {
                return ParameterDefinition.Type;
            }
        }
        public ITypeDefinition OutputType { get; }
        public MemberDefinition ContextDefinition { get; }
        public MemberDefinition ParameterDefinition { get; }
        public IScope Scope { get; }
        public IEnumerable<ICodeElement> MethodBody { get; }
        public IEnumerable<ICodeElement> StaticInitialzers { get; }
        
        public ITypeDefinition ReturnType(ScopeStack scope) {
                return scope.GetGenericType(new GenericExplicitTypeName(RootScope.ImplementationType.Name,new ITypeDefinition[] { ContextType, InputType, OutputType }));
        }

        public ITypeDefinition GetTypeDefinition(ScopeStack scopeStack)
        {
            return scopeStack.GetGenericType(new GenericExplicitTypeName(RootScope.ImplementationType.Name, new ITypeDefinition[] { ContextType, InputType, OutputType }));
        }
    }
}
