using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tac.Backend.Emit;
using Tac.Backend.Emit.Walkers;
using Tac.Backend.Interpreted.Public;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Xunit;

namespace Tac.Emit.SnippetTests
{
    internal static class BasicInputOutput
    {
        internal static (Func<T>, Action) ToInput<T>(IReadOnlyList<T> list)
        {
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
                }
            );
        }

        public class InputBacking {
            public Func<Empty, double> read_number { init; get; }
            public Func<Empty, string> read_string { init; get; }
            public Func<Empty, bool> read_bool { init; get; }
        }

        public static Assembly Input(Func<double> numberSource, Func<string> stringSource, Func<bool> boolSource)
        {
            var input = new InputBacking
            {
                read_number = (Empty _) => numberSource(),
                read_string = (Empty _) => stringSource(),
                read_bool = (Empty _) => boolSource(),
            };

            List<IsStatic> members = new List<IsStatic>();
            members.Add(new IsStatic(
                MemberDefinition.CreateAndBuild(
                    new NameKey("read-number"), 
                    MethodType.CreateAndBuild(
                        new EmptyType(),
                        new NumberType()), 
                    Access.ReadOnly), 
                    false));
            members.Add(new IsStatic(
                MemberDefinition.CreateAndBuild(
                    new NameKey("read-string"),
                    MethodType.CreateAndBuild(
                        new EmptyType(),
                        new StringType()),
                    Access.ReadOnly),
                    false));
            members.Add(new IsStatic(
                MemberDefinition.CreateAndBuild(
                    new NameKey("read-bool"),
                    MethodType.CreateAndBuild(
                        new EmptyType(),
                        new BooleanType()),
                    Access.ReadOnly),
                    false));

            //var scope = new Scope();

            //scope.Build(members);

            ;

            return new Assembly(new NameKey("in"), InterfaceType.CreateAndBuild(members.Select(x => x.Value).ToArray()), input);
        }

        public class OutputBacking
        {
            public Func<double, Empty> write_number { init; get; }
            public Func<string, Empty> write_string { init; get; }
            public Func<bool, Empty> write_bool { init; get; }
        }

        public static Assembly Output(Action<double> numberDestination, Action<string> stringDestination, Action<bool> boolDestination)
        {
            var output = new OutputBacking
            {
                write_number = (double x) => { numberDestination(x); return new Empty(); },
                write_string = (string x) => { stringDestination(x); return new Empty(); } ,
                write_bool = (bool x) => { boolDestination(x); return new Empty(); },
            };

            List<IsStatic> members = new List<IsStatic>();
            members.Add(new IsStatic(
                MemberDefinition.CreateAndBuild(
                    new NameKey("write-number"),
                    MethodType.CreateAndBuild(
                        new NumberType(),
                        new EmptyType()),
                    Access.ReadOnly),
                    false));
            members.Add(new IsStatic(
                MemberDefinition.CreateAndBuild(
                    new NameKey("write-string"),
                    MethodType.CreateAndBuild(
                        new StringType(),
                        new EmptyType()),
                    Access.ReadOnly),
                    false));
            members.Add(new IsStatic(
                MemberDefinition.CreateAndBuild(
                    new NameKey("write-bool"),
                    MethodType.CreateAndBuild(
                        new BooleanType(),
                        new EmptyType()),
                    Access.ReadOnly),
                    false));

            //var scope = new Scope();

            //scope.Build(members);

            return new Assembly(new NameKey("out"), InterfaceType.CreateAndBuild(members.Select(x => x.Value).ToArray()), output);
        }
    }
}
