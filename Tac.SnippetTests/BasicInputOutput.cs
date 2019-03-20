using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Public;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Xunit;
using static Tac.Backend.Public.AssemblyBuilder;

namespace Tac.SnippetTests
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

        public static IAssembly<InterpetedAssemblyBacking> Input(Func<double> numberSource, Func<string> stringSource) {
            return new AssemblyBuilder(new NameKey("input"))
                       .AddMethod(
                           new NameKey("read-number"),
                           (IInterpedEmpty x) => new BoxedDouble(numberSource()), 
                           TypeReference.CreateAndBuild( 
                               MethodType.CreateAndBuild(
                                   new EmptyType(), 
                                   new NumberType())))
                       .AddMethod(
                           new NameKey("read-string"),
                           (IInterpedEmpty x) => new BoxedString(stringSource()),
                           TypeReference.CreateAndBuild(
                               MethodType.CreateAndBuild(
                                   new EmptyType(),
                                   new StringType())))
                       .Build();
        }

        public static IAssembly<InterpetedAssemblyBacking> Output(Action<double> numberDestination, Action<string> stringDestination)
        {
            return new AssemblyBuilder(new NameKey("output"))
                       .AddMethod(
                           new NameKey("write-number"),
                           (BoxedDouble x) => { numberDestination(x.Value); return new RunTimeEmpty(); },
                           TypeReference.CreateAndBuild(
                               MethodType.CreateAndBuild(
                                   new NumberType(),
                                   new EmptyType())))
                       .AddMethod(
                           new NameKey("write-string"),
                           (BoxedString x) => { stringDestination(x.Value); return new RunTimeEmpty(); },
                           TypeReference.CreateAndBuild(
                               MethodType.CreateAndBuild(
                                   new StringType(),
                                   new EmptyType())))
                       .Build();
        }
    }
}
