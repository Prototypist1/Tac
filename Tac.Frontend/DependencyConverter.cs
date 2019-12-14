using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Semantic_Model;
using System.Linq;
using Prototypist.Toolbox.Object;

namespace Tac.Frontend
{
    // oh shit, forgot about this 😬
    internal class DependencyConverter
    {


        public WeakTypeDefinition ConvertToType<TBaking>(IAssembly<TBaking> assembly)
            where TBaking:IBacking
        {
            // is it ok to create a scope here?
            // yeah i think so
            // it is not like you are going to be mocking scope
            // i mean it is not a pure data objet
            // what is the cost to passing it in?

            //var scope = new PopulatableScope();
            //foreach (var member in assembly.Scope.Members)
            //{
            //    if (!scope.TryAddMember(DefintionLifetime.Instance,member.Key,new Box<IIsPossibly<WeakMemberDefinition>>(Possibly.Is( MemberDefinition(member))))) {
            //        throw new Exception("😨 member should not already exist");
            //    }
            //}
            ////foreach (var type in assembly.Scope.Types)
            ////{
            ////    if (type.Type is IInterfaceType interfaceType)
            ////    {
            ////        if (!scope.TryAddType(type.Key, new Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>>(Possibly.Is(TypeDefinition(interfaceType)))))
            ////        {
            ////            throw new Exception("type should not already exist");
            ////        }
            ////    }
            ////}
            ////foreach (var genericType in assembly.Scope.GenericTypes)
            ////{
            ////    if (genericType.Type is IGenericInterfaceDefinition genericInterface)
            ////    {
            ////        if (!scope.TryAddGeneric(genericType.Key.Name, new Box<IIsPossibly<IFrontendGenericType>>(Possibly.Is(GenericTypeDefinition(genericInterface)))))
            ////        {
            ////            throw new Exception("type should not already exist");
            ////        }
            ////    }
            ////}
            //var resolvelizableScope = scope.GetResolvelizableScope();
            //var resolvableScope = resolvelizableScope.FinalizeScope();
            //return new WeakTypeDefinition(resolvableScope, Possibly.Is(new ImplicitKey()));


            var scope = new WeakScope(
                assembly.Scope.Members.Select(x=> new Box<WeakMemberDefinition>(MemberDefinition(x)).CastTo<IBox<WeakMemberDefinition>>()).ToList());

            return new WeakTypeDefinition(new Box<WeakScope>(scope));

        }


        private readonly Dictionary<IMemberDefinition, WeakMemberDefinition> backing = new Dictionary<IMemberDefinition, WeakMemberDefinition>();

        public DependencyConverter()
        {
        }

        public WeakMemberDefinition MemberDefinition(IMemberDefinition member)
        {
            if (backing.TryGetValue(member, out var res))
            {
                return res;
            }
            else
            {
                var interpetedMemberDefinition = new WeakMemberDefinition(
                    member.ReadOnly,
                    member.Key,
                    new Box<IFrontendType>(
                                        TypeMap.MapType(member.Type)));
                backing.Add(member, interpetedMemberDefinition);
                return interpetedMemberDefinition;
            }
        }

        //public IFrontendGenericType GenericTypeDefinition(IGenericInterfaceDefinition _)
        //{
        //    throw new NotImplementedException();
        //    //if (backing.TryGetValue(codeElement, out var res))
        //    //{
        //    //    return res;
        //    //}
        //    //else
        //    //{
        //    //    var op = new WeakGenericTypeDefinition(,,);
        //    //    backing.Add(codeElement, op);
        //    //    return op;
        //    //}
        //}

        public IConvertableFrontendType<IVerifiableType> TypeDefinition(IInterfaceType _)
        {
            throw new NotImplementedException();
            //if (backing.TryGetValue(codeElement, out var res))
            //{
            //    return res;
            //}
            //else
            //{
            //    var op = new WeakTypeDefinition(,);
            //    backing.Add(codeElement, op);
            //    return op;
            //}
        }
    }

    internal static class TypeMap
    {

        public static IConvertableFrontendType<IVerifiableType> MapType(IVerifiableType verifiableType)
        {
            if (verifiableType is INumberType)
            {
                return PrimitiveTypes.CreateNumberType();
            }
            if (verifiableType is IBooleanType)
            {
                return PrimitiveTypes.CreateBooleanType();
            }
            if (verifiableType is IStringType)
            {
                return PrimitiveTypes.CreateStringType();
            }
            if (verifiableType is IBlockType)
            {
                return PrimitiveTypes.CreateBlockType();
            }
            if (verifiableType is IEmptyType)
            {
                return PrimitiveTypes.CreateEmptyType();
            }
            if (verifiableType is IAnyType)
            {
                return PrimitiveTypes.CreateAnyType();
            }
            if (verifiableType is IMethodType method)
            {
                return PrimitiveTypes.CreateMethodType(
                    MapType(method.InputType),
                    MapType(method.OutputType)
                    );
            }
            if (verifiableType is IImplementationType implementation)
            {
                return PrimitiveTypes.CreateImplementationType(
                    MapType(implementation.ContextType),
                    MapType(implementation.InputType),
                    MapType(implementation.OutputType)
                    );
            }

            throw new NotImplementedException();
        }

    }

}

