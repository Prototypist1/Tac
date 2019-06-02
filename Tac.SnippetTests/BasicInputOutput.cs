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
using Prototypist.LeftToRight;

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

        public static IAssembly<InterpetedAssemblyBacking> Input(Func<double> numberSource, Func<string> stringSource, Func<bool> boolSource) {
            return new AssemblyBuilder(new NameKey("in"))
                       .AddMethod(
                           new NameKey("read-number"),
                           (IInterpedEmpty x) => new BoxedDouble(numberSource()), 
                           MethodType.CreateAndBuild(
                                   new EmptyType(), 
                                   new NumberType()))
                       .AddMethod(
                           new NameKey("read-string"),
                           (IInterpedEmpty x) => new BoxedString(stringSource()),
                           MethodType.CreateAndBuild(
                                   new EmptyType(),
                                   new StringType()))
                       .AddMethod(
                           new NameKey("read-bool"),
                           (IInterpedEmpty x) => new BoxedBool(boolSource()),
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
                           (BoxedDouble x) => { numberDestination(x.Value); return new RunTimeEmpty().Cast<IInterpedEmpty>(); },
                           MethodType.CreateAndBuild(
                                   new NumberType(),
                                   new EmptyType()))
                       .AddMethod(
                           new NameKey("write-string"),
                           (BoxedString x) => { stringDestination(x.Value); return new RunTimeEmpty().Cast<IInterpedEmpty>(); },
                           MethodType.CreateAndBuild(
                                   new StringType(),
                                   new EmptyType()))
                       .AddMethod(
                           new NameKey("write-bool"),
                           (BoxedBool x) => { boolDestination(x.Value); return new RunTimeEmpty().Cast<IInterpedEmpty>(); },
                           MethodType.CreateAndBuild(
                                   new BooleanType(),
                                   new EmptyType()))
                       .Build();
        }
    }
}
