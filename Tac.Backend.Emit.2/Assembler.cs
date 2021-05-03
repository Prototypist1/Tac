﻿using System;
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

            var typeCache = new ConcurrentIndexed<IVerifiableType, TypeBuilder>();
            var objectCache = new ConcurrentIndexed<IObjectDefiniton, TypeBuilder>();

            // TODO 
            // TODO 
            // TODO
            // YOU ARE HERE 
            // primitive types arn't in the type cache
            // method types arn't in the type cahe
            // the type cache needs to be an object
            // it owns type look up
            // keeps track of the list of type we need to finalize
            // handles look up for methods greatfully 

            var typeVisitor = new TypeVisitor(typeCache, objectCache, module.Value);
            rootScope.Convert(typeVisitor);

            var realizedMethodLookup = new RealizedMethodLookup();
            var methodMakerVisitor = new MethodMakerVisitor(module.Value, extensionLookup, realizedMethodLookup, typeCache);

                rootScope.Convert(methodMakerVisitor);


            var conversionTypes = new ConcurrentIndexed<(System.Type, System.Type), TypeBuilder>();
            var (assemblerVisitor, after) = AssemblerVisitor.Create(
                memberKindLookup,
                extensionLookup,
                typeCache,
                objectCache,
                conversionTypes,
                module.Value,
                realizedMethodLookup,
                typeCache[rootScope.EntryPoint.InputType],
                typeCache[rootScope.EntryPoint.OutputType]);
                rootScope.Convert(assemblerVisitor);

            //finish up
            // this is a bit sloppy, maybe disposable?
            after();

            // we have to actually create the types
            typeVisitor.CreateTypes();
            realizedMethodLookup.CreateTypes();
            assemblerVisitor.rootType.CreateType();

            var yo = String.Join(Environment.NewLine, assemblerVisitor.gens.Select(x => x.GetDeubbingSting()));

            // now I need to reflexively find my type and call main
            var complitation = (TacCompilation)Assembly.Value.CreateInstance(assemblerVisitor.rootType.Name);

            var runtimeTypeCache = new ConcurrentIndexed<System.Type,IVerifiableType>();

            foreach (var type in typeCache)
            {
                runtimeTypeCache.AddOrThrow(type.Value, type.Key);
            }

            foreach (var pair in conversionTypes)
            {
                // the runtimeTypeCache shouldn't cause any trouble
                // pair.Key.Item2 should all be root 
                runtimeTypeCache.AddOrThrow(pair.Value, runtimeTypeCache[pair.Key.Item2]);
            }

            complitation.typeCache = runtimeTypeCache;
            //complitation.indexerArray = assemblerVisitor.indexerList.indexers.ToArray();
            //complitation.verifyableTypesArray = assemblerVisitor.verifyableTypesList.types.ToArray();
            complitation.Init();
            return (TacCompilation<Tin, TOut>)complitation;
        }
    }
}
