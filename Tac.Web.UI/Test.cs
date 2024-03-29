﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tac.Backend.Emit.Walkers;
using Tac.Emit.Runner;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;

namespace Tac.Web.UI
{
    public class Test
    {

        public string InputValue { get; set; } =
@"entry-point [empty; empty;] input {
    ""hello world!"" > (out.write-string);
    new-empty return;
}";

        private OutputBacking outputBacking = new OutputBacking();

        private Task? task = null;
        public IEnumerable<string> Output => outputBacking.consoleLines;
        public bool Running => !task?.IsCompleted ?? false;
        public string Text => !task?.IsCompleted ?? false ? "..." : "run";

        public class InputBacking
        {
            public InputBacking()
            {
                read_number = (Empty _) => 0;
                read_string = (Empty _) => "";
                read_bool = (Empty _) => false;
            }

            public Func<Empty, double> read_number { get; set; }
            public Func<Empty, string> read_string { get; set; }
            public Func<Empty, bool> read_bool { get; set; }
        }


        public class OutputBacking
        {
            internal List<string> consoleLines = new List<string>();

            public OutputBacking()
            {
                write_number = num => { consoleLines.Add(num.ToString()); return null; };
                write_string = str => { consoleLines.Add(str.ToString()); return null; };
                write_bool = boo => { consoleLines.Add(boo.ToString()); return null; };
            }

            public Func<double, Empty> write_number { get; set; }
            public Func<string, Empty> write_string { get; set; }
            public Func<bool, Empty> write_bool { get; set; }
        }

        public async Task Execute()
        {
            if (Running)
            {
                return;
            }

            outputBacking.consoleLines.Clear();
            task = Task.WhenAll(
                Task.Delay(1000),
                Task.Run(() =>
            {
                try
                {
                    var result = Run.CompileAndRun<Empty, Empty>(
                        "test",
                        InputValue,
                        null,
                        new[] {
                 new Tac.Backend.Emit.Assembly(
                    new NameKey("in"),
                    InterfaceType.CreateAndBuild(
                        new List<IMemberDefinition>() {
                            MemberDefinition.CreateAndBuild(
                                new NameKey("read-number"),
                                MethodType.CreateAndBuild(
                                    new EmptyType(),
                                    new NumberType()),
                                Access.ReadOnly),
                            MemberDefinition.CreateAndBuild(
                                new NameKey("read-string"),
                                MethodType.CreateAndBuild(
                                    new EmptyType(),
                                    new StringType()),
                                Access.ReadOnly),
                            MemberDefinition.CreateAndBuild(
                                new NameKey("read-bool"),
                                MethodType.CreateAndBuild(
                                    new EmptyType(),
                                    new BooleanType()),
                                Access.ReadOnly)
                        }),
                    new InputBacking()),
                new Tac.Backend.Emit.Assembly(
                    new NameKey("out"),
                    InterfaceType.CreateAndBuild(
                        new List<IMemberDefinition>{
                            MemberDefinition.CreateAndBuild(
                                new NameKey("write-number"),
                                MethodType.CreateAndBuild(
                                    new NumberType(),
                                    new EmptyType()),
                                Access.ReadOnly),
                            MemberDefinition.CreateAndBuild(
                                new NameKey("write-string"),
                                MethodType.CreateAndBuild(
                                    new StringType(),
                                    new EmptyType()),
                                Access.ReadOnly),
                            MemberDefinition.CreateAndBuild(
                                new NameKey("write-bool"),
                                MethodType.CreateAndBuild(
                                    new BooleanType(),
                                    new EmptyType()),
                                Access.ReadOnly) }),
                outputBacking)
                    });

                    result.Switch(
                        val =>
                        {
                            // it has to return empty otherwise we crash
                        },
                        errors =>
                        {
                            foreach (var error in errors)
                            {
                                outputBacking.consoleLines.Add("error: " + error);
                            }
                        });
                }
                catch (Exception e)
                {
                    outputBacking.consoleLines.Add(e.Message);
                }

            }));
            await task;
        }
    }
}
