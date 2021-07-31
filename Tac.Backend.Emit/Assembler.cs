using System;
using Tac.Model.Elements;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using Tac.Model;
using Tac.Backend.Emit.Walkers;
using Tac.Backend.Emit.Lookup;
using Tac.Backend.Emit.Visitors;
using System.Linq;
using Prototypist.TaskChain;
using Tac.Model.Instantiated;

namespace Tac.Backend.Emit
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

        public static TOut BuildAndRun<Tin, TOut>(IRootScope scope, Tin input) 
        {
            var complitation = Build<Tin, TOut>(new Project<Assembly, object>(scope, Array.Empty<Assembly>(), Scope.CreateAndBuild(Array.Empty<IsStatic>())));
            return complitation.main(input);
        }

        public static TOut BuildAndRun<Tin, TOut>(IRootScope scope, Tin input, IReadOnlyList<Assembly> assemblies)
        {
            var complitation = Build<Tin, TOut>(new Project<Assembly, object>(scope, assemblies, Scope.CreateAndBuild(Array.Empty<IsStatic>())));
            return complitation.main(input);
        }

        public static TOut BuildAndRun<Tin,TOut>(IProject<Assembly,object> project, Tin input)
        {
            var complitation = Build<Tin,TOut>(project);
            return complitation.main(input);
        }

        private static TacCompilation<Tin, TOut> Build<Tin, TOut>(IProject<Assembly, object> project)
        {
            var rootScope = project.RootScope;
            // I think we are actually not making an assembly,
            // just a type 
            
            var extensionLookup = new WhoDefinedMemberByMethodlike();
            var closureVisitor = new ClosureVisitor(extensionLookup, project.DependencyScope.Members.Values.Select(x=>x.Value).ToHashSet());
            rootScope.Convert(closureVisitor);

            var (memberKindVisitor, memberKindLookup) = MemberKindVisitor.Make(extensionLookup, project);
            rootScope.Convert(memberKindVisitor);
            memberKindVisitor.HandleDependencies(project.DependencyScope);

            var gens = new List<DebuggableILGenerator>();

            var typeTracker = new TypePassTypeTracker(module.Value, gens);
            var typeVisitor = new TypeVisitor(typeTracker);
            rootScope.Convert(typeVisitor);
            typeVisitor.HandleDependencies(project);

            var typeTracker2 = typeTracker.CreateTypesAndProperties();

            var dependenciesTypeBuider = module.Value.DefineType(AssemblerVisitor.GenerateName(), TypeAttributes.Public);
            foreach (var member in project.References)
            {
                dependenciesTypeBuider.DefineField(TypeTracker.ConvertName(member.Key.Name), typeTracker2.ResolvePossiblyPrimitive(member.Scope), FieldAttributes.Public);
            }
            var dependenciesType = dependenciesTypeBuider.CreateType() ?? throw new NullReferenceException("dependenciesTypeBuider.CreateType should not return null");
            //var dependenciesType = typeTracker2.GetDependencyType();

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
                gens,
                dependenciesType);
            rootScope.Convert(assemblerVisitor);

            //finish up
            // this is a bit sloppy, maybe disposable?
            after();

            // we have to actually create the types
            realizedMethodLookup.CreateTypes();
            assemblerVisitor.rootType.CreateType();

            var yo = String.Join(Environment.NewLine, gens.Select(x => x.GetDeubbingSting()));

            // now I need to reflexively find my type and call main
            var complitation = (TacCompilation)(Assembly.Value.CreateInstance(assemblerVisitor.rootType.Name) ?? throw new NullReferenceException());

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

            var compType =  complitation.GetType();

            var dependenciesField = compType.GetField(nameof(TacCompilation<int, int, object>.dependencies)) ?? throw new NullReferenceException();

            var dependencies = Activator.CreateInstance(dependenciesField.FieldType);

            dependenciesField.SetValue(complitation, dependencies);

            // A502902A-8EC8-4F5F-A55F-32E36F146762 this is a bit weird
            // it would be great if these didn't have to be wraped
            // the MSIL type could flow into the front end 
            // and be picked up again on the backend
            foreach (var reference in project.References)
            {
                var field = dependenciesType.GetField(TypeTracker.ConvertName(reference.Key.Name)) ?? throw new NullReferenceException();
                var setTo = AssemblyWalkerHelp.TryAssignOperationHelper_Cast(reference.Backing, typeTracker2.ResolvePossiblyPrimitive(reference.Scope), complitation.runTimeTypeTracker);
                field.SetValue(dependencies, setTo);
            }

            //complitation.indexerArray = assemblerVisitor.indexerList.indexers.ToArray();
            //complitation.verifyableTypesArray = assemblerVisitor.verifyableTypesList.types.ToArray();
            complitation.Init();
            return (TacCompilation<Tin, TOut>)complitation;
        }
    }
}
