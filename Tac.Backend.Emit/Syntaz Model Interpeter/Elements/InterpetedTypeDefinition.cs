
using System;
using System.Reflection;
using System.Reflection.Emit;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedTypeDefinition: IAssembledOperation
    {


        public InterpetedTypeDefinition Init(string name, InterpetedMemberDefinition[]? members, ModuleBuilder moduleBuilder)
        {
            this.name = name;
            this.members = members;
            this.type = moduleBuilder.DefineType(Name);
            return this;
        }


        private string? name;
        public string Name => name ?? throw new NullReferenceException(nameof(name));

        public InterpetedMemberDefinition[]? members;
        private TypeBuilder? type;
        public System.Type Type => type ?? throw new NullReferenceException(nameof(type));


        // there has to be a type pass

        public IInterpetedResult<IInterpetedMember> Assemble(AssemblyContext interpetedContext)
        {

            var type = interpetedContext.moduleBuilder.DefineType(Name);


            foreach (var member in members)
            {
                type.DefineField(member.name, member.interpetedTypeDefinition.Type, FieldAttributes.Public);
            }

            // I am going to have to create types in order??
            var result =  type.CreateType();


            //Your last paragraph is roughly right, but TypeBuilder derives from Type, so you don't need to call CreateType. That is, create type builders for each of the recursive types, then define the properties passing the respective builders themselves as the return types.

            // define members


            //TypeBuilder myTypeBld;

            //MethodBuilder myMthdBld = myTypeBld.DefineMethod(
            //                             mthdName,
            //                             MethodAttributes.Public |
            //                             MethodAttributes.Static,
            //                             returnType,
            //                             mthdParams);

            return InterpetedResult.Create(TypeManager.EmptyMember(TypeManager.Empty()));
        }
    }
}