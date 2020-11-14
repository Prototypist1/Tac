using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Interpreted.Public;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;
using Xunit;
using static Tac.Backend.Interpreted.Public.AssemblyBuilder;
using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;

namespace Tac.Interpreted.SnippetTests
{


    

    internal static class BasicInputOutput
    {
        internal static (Func<T>, Action) ToInput<T>(IReadOnlyList<T> list) {
            var at = 0;
            return (() => list[at++], () => Assert.Equal(list.Count, at));
        }

        internal static (Action<T>, Action) ToOutput<T>(IReadOnlyList<T> list)
        {
            var outputs = new List<T>();
            return (
                (x) => outputs.Add(x), 
                () => {
                    Assert.Equal(list.Count, outputs.Count);
                    list.Zip(outputs, (x, y) => { Assert.Equal(x, y); return 0; }).ToArray();
                });
        }

        public static IAssembly<InterpetedAssemblyBacking> Input(Func<double> numberSource, Func<string> stringSource, Func<bool> boolSource) {
            return new AssemblyBuilder(new NameKey("in"))
                       .AddMethod(
                           new NameKey("read-number"),
                           (IInterpetedAnyType x) => TypeManager.Double(numberSource()), 
                           MethodType.CreateAndBuild(
                                   new EmptyType(), 
                                   new NumberType()))
                       .AddMethod(
                           new NameKey("read-string"),
                           (IInterpetedAnyType x) => TypeManager.String(stringSource()),
                           MethodType.CreateAndBuild(
                                   new EmptyType(),
                                   new StringType()))
                       .AddMethod(
                           new NameKey("read-bool"),
                           (IInterpetedAnyType x) => TypeManager.Bool(boolSource()),
                           MethodType.CreateAndBuild(
                                   new EmptyType(),
                                   new BooleanType()))
                       .Build();
        }

        public static IAssembly<InterpetedAssemblyBacking> Output(Action<double> numberDestination, Action<string> stringDestination, Action<bool> boolDestination)
        {
            return new AssemblyBuilder(new NameKey("out"))
                       .AddMethod(
                           new NameKey("write-number"),
                           (IInterpetedAnyType x) => { numberDestination(x.Has<IBoxedDouble>().Value); return TypeManager.Empty(); },
                           MethodType.CreateAndBuild(
                                   new NumberType(),
                                   new EmptyType()))
                       .AddMethod(
                           new NameKey("write-string"),
                           (IInterpetedAnyType x) => { stringDestination(x.Has<IBoxedString>().Value); return TypeManager.Empty(); },
                           MethodType.CreateAndBuild(
                                   new StringType(),
                                   new EmptyType()))
                       .AddMethod(
                           new NameKey("write-bool"),
                           (IInterpetedAnyType x) => { boolDestination(x.Has<IBoxedBool>().Value); return TypeManager.Empty(); },
                           MethodType.CreateAndBuild(
                                   new BooleanType(),
                                   new EmptyType()))
                       .Build();
        }
    }
}
