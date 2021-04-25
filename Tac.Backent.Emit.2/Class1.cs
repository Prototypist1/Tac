using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Tac.Backent.Emit._2
{

    public partial class CanI { 
    
    }

    public class InteraceEmmiter
    {

        public static To EmitOrCast<From, To>(From from)
            where To : class
        {

            if (from is To to)
            {
                return to;
            }

            Emit<From, To>();

            return from as To;
        }

        public static void Emit<From, To>()
        {
            var assemblyName = new Guid().ToString();
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule(typeof(From).Module.FullyQualifiedName);
            var type = module.DefineType(typeof(From).Name, TypeAttributes.Public);
            type.AddInterfaceImplementation(typeof(To));
        }
    }
}
