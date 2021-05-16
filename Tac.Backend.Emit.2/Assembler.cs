using System;
using Tac.Model.Elements;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using Tac.Model;
using Tac.Backend.Emit._2.Walkers;
using Tac.Backend.Emit._2.Lookup;
using Tac.Backend.Emit._2.Visitors;
using Prototypist.Toolbox;
using System.Linq;
using Prototypist.TaskChain;

namespace Tac.Backend.Emit._2
{
    public static class Compiler
    {

        // {4E963BB1-1C86-4F75-BD4C-3F9BE16386A9}
        private static readonly Random random = new Random();
        internal static string GenerateName()
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

        public static TOut BuildAndRun<Tin,TOut>(IRootScope rootScope, Tin input)
        {
            var complitation = Build<Tin,TOut>(rootScope);
            return complitation.main(input);
        }

        private static TacCompilation<Tin, TOut> Build<Tin, TOut>(IRootScope rootScope)
        {
            // I think we are actually not making an assembly,
            // just a type 

            var extensionLookup = new ExtensionLookup();
            var closureVisitor = new ClosureVisitor(extensionLookup);
            rootScope.Convert(closureVisitor);

            var memberKindLookup = new MemberKindLookup();
            var memberKindVisitor = MemberKindVisitor.Make(memberKindLookup, extensionLookup);
            rootScope.Convert(memberKindVisitor);

            var gens = new List<DebuggableILGenerator>();

            var typeTracker = new TypePassTypeTracker(module.Value, gens);
            var typeVisitor = new TypeVisitor(typeTracker);
            rootScope.Convert(typeVisitor);
            var typeTracker2 = typeTracker.CreateTypesAndProperties();

            var realizedMethodLookup = new RealizedMethodLookup();
            var methodMakerVisitor = new MethodMakerVisitor(module.Value, extensionLookup, realizedMethodLookup, typeTracker2);

            rootScope.Convert(methodMakerVisitor);


            var conversionTypes = new ConcurrentIndexed<(System.Type, System.Type), TypeBuilder>();
            var (assemblerVisitor, after) = AssemblerVisitor.Create(
                memberKindLookup,
                extensionLookup,
                typeTracker2,
                conversionTypes,
                module.Value,
                realizedMethodLookup,
                typeTracker2.ResolvePossiblyPrimitive(rootScope.EntryPoint.InputType),
                typeTracker2.ResolvePossiblyPrimitive(rootScope.EntryPoint.OutputType),
                gens);
            rootScope.Convert(assemblerVisitor);

            //finish up
            // this is a bit sloppy, maybe disposable?
            after();

            // we have to actually create the types
            realizedMethodLookup.CreateTypes();
            assemblerVisitor.rootType.CreateType();

            var yo = String.Join(Environment.NewLine, gens.Select(x => x.GetDeubbingSting()));

            // now I need to reflexively find my type and call main
            var complitation = (TacCompilation)Assembly.Value.CreateInstance(assemblerVisitor.rootType.Name);

            //var runtimeTypeCache = new ConcurrentIndexed<System.Type,IVerifiableType>();

            //foreach (var type in typeTracker.GetTypes())
            //{
            //    runtimeTypeCache.AddOrThrow(type.Value, type.Key);
            //}

            //foreach (var pair in conversionTypes)
            //{
            //    // the runtimeTypeCache shouldn't cause any trouble
            //    // pair.Key.Item2 should all be root 
            //    runtimeTypeCache.AddOrThrow(pair.Value, runtimeTypeCache[pair.Key.Item2]);
            //}

            //complitation.addType = (type, tacType) => { typeTracker.typeCache.AddOrThrow(type, tacType); };
            //complitation.

            //var wrapsAndImplementsCache = new ConcurrentIndexed<(System.Type, System.Type), System.Type>();

            //foreach (var conversion in conversionTypes)
            //{
            //    wrapsAndImplementsCache[conversion.Key] = conversion.Value;
            //}

            complitation.runTimeTypeTracker = typeTracker2.RunTimeTypeTracker();
            //complitation.indexerArray = assemblerVisitor.indexerList.indexers.ToArray();
            //complitation.verifyableTypesArray = assemblerVisitor.verifyableTypesList.types.ToArray();
            complitation.Init();
            return (TacCompilation<Tin, TOut>)complitation;
        }
    }
}
