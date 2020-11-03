using System;
using Tac.Model.Elements;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using Tac.Model;
using Tac.Backend.Emit.Walkers;
using Tac.Backend.Emit.Lookup;
using Tac.Backend.Emit.Visitors;
using Prototypist.Toolbox;
using System.Linq;

namespace Tac.Backend.Emit
{
    public static class Compiler
    {

        // {4E963BB1-1C86-4F75-BD4C-3F9BE16386A9}
        private static readonly Random random = new Random();
        private static string GenerateName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, 20)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        private static Lazy<AssemblyBuilder> Assembly = new Lazy<AssemblyBuilder>(() => {

            var assemblyName = new AssemblyName();
            assemblyName.Name = GenerateName();
            return System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
        });

        private static Lazy<ModuleBuilder> module = new Lazy<ModuleBuilder>(() =>
        {

            return Assembly.Value.DefineDynamicModule(GenerateName());
        });

        public static object BuildAndRun(IRootScope rootScope, object input)
        {
            var complitation = Build(rootScope);
            return complitation.main(input);
        }

        private static TacCompilation Build(IRootScope rootScope)
        {
            // I think we are actually not making an assembly,
            // just a type 

            var extensionLookup = new ExtensionLookup();
            var closureVisitor = new ClosureVisitor(extensionLookup);
            rootScope.Convert(closureVisitor);

            var memberKindLookup = new MemberKindLookup();
            var memberKindVisitor = MemberKindVisitor.Make(memberKindLookup, extensionLookup);
            rootScope.Convert(memberKindVisitor);

            var typeCache = new Dictionary<IVerifiableType, System.Type>();
            var typeVisitor = new TypeVisitor(typeCache);
            rootScope.Convert(typeVisitor);

            var realizedMethodLookup = new RealizedMethodLookup();
            var methodMakerVisitor = new MethodMakerVisitor(module.Value, extensionLookup, realizedMethodLookup, typeCache);

                rootScope.Convert(methodMakerVisitor);
            

            var (assemblerVisitor, after) = AssemblerVisitor.Create(
                memberKindLookup,
                extensionLookup,
                typeCache,
                module.Value,
                realizedMethodLookup
                );
                rootScope.Convert(assemblerVisitor);

            //finish up
            // this is a bit sloppy, maybe disposable?
            after();

            // we have to actually create the types
            realizedMethodLookup.CreateTypes();
            assemblerVisitor.rootType.CreateType();

            var yo = String.Join(Environment.NewLine, assemblerVisitor.gens.Select(x => x.GetDeubbingSting()));

            // now I need to reflexively find my type and call main
            var complitation = (TacCompilation)Assembly.Value.CreateInstance(assemblerVisitor.rootType.Name);

            complitation.indexerArray = assemblerVisitor.indexerList.indexers.ToArray();
            complitation.verifyableTypesArray = assemblerVisitor.verifyableTypesList.types.ToArray();
            complitation.Init();
            return complitation;
        }
    }
}
