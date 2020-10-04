using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.Model.Instantiated.Elements;
using Xunit;

namespace Tac.Backend.Emit.Test
{
    public class SimpleTests
    {
        [Fact]
        public void Simplist() {
            Compiler.BuildAndRun(
                new List<ICodeElement>{ 
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        new List<ICodeElement> {
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
                });
        }

        [Fact]
        public void Add()
        {
            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        new List<ICodeElement> {
                            AddOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(1),ConstantNumber.CreateAndBuild(1)),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
                });
        }

        [Fact]
        public void Multiply()
        {
            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        new List<ICodeElement> {
                            MultiplyOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2),ConstantNumber.CreateAndBuild(2)),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
                });
        }
    }
}
