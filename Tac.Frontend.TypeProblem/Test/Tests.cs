using System;
using System.Collections.Generic;
using System.Text;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Xunit;

namespace Tac.Frontend.TypeProblem.Test
{
    public class Tests
    {

        [Fact]
        public void Simplest() {
            var x = new TypeProblem2();
            x.Solve();
        }



        [Fact]
        public void AddType()
        {
            var x = new TypeProblem2();
            var hello = x.CreateType(x.Root, new NameKey("Hello"));
            var hello_x = x.CreateMember(hello, new NameKey("x"));
            var hello_y = x.CreateMember(hello, new NameKey("y"));
            x.Solve();
        }

        [Fact]
        public void AddMethod()
        {
            var x = new TypeProblem2();

            var hello = x.CreateType(x.Root, new NameKey("hello"));
            var hello_x = x.CreateMember(hello, new NameKey("x"));
            var hello_y = x.CreateMember(hello, new NameKey("y"));

            var input = x.CreateValue(x.Root,new NameKey("hello"));
            var method = x.CreateMethod(x.Root, "input");

            var input_x = x.CreateHopefulMember(method.Input(), new NameKey("x"));
            var input_y = x.CreateHopefulMember(method.Input(), new NameKey("y"));

            var method_x = x.CreateMember(method, new NameKey("x"));
            var method_y = x.CreateMember(method, new NameKey("y"));

            input_x.AssignTo(method_x);
            input_y.AssignTo(method_y);

            method.Input().AssignTo(method.Returns());

            input.AssignTo(method.Input());

            x.Solve();
        }
    }
}
