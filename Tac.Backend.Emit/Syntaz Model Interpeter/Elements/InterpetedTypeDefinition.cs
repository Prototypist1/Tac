
using System;
using System.Reflection.Emit;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedTypeDefinition: IAssembledOperation
    {


        public void Init(string name, InterpetedMemberDefinition[]? members)
        {
            this.name = name;
            this.members = members;
        }


        private string? name;
        public string Name => name ?? throw new NullReferenceException(nameof(name));

        public InterpetedMemberDefinition[]? members;


        // there has to be a type pass

        public IInterpetedResult<IInterpetedMember> Assemble(AssemblyContext interpetedContext)
        {

            var type = interpetedContext.moduleBuilder.DefineType(Name);


            foreach (var member in members)
            {
                // in a type everything is a Ref<T>
                // 
                type.DefineField();
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