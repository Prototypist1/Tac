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

        [Fact]
        public void AssignmentX() {

            var x = new TypeProblem2();

            var m1 = x.CreateMember(x.Root, new NameKey("m1"));
            x.CreateHopefulMember(m1, new NameKey("x"));
            var m2 = x.CreateMember(x.Root, new NameKey("m2"));
            x.CreateHopefulMember(m2, new NameKey("y"));
            var m3 = x.CreateMember(x.Root, new NameKey("m3"));
            var m4 = x.CreateMember(x.Root, new NameKey("m4"));
            var m5 = x.CreateMember(x.Root, new NameKey("m5"));

            m1.AssignTo(m3);
            m2.AssignTo(m3);
            m3.AssignTo(m4);
            m3.AssignTo(m5);

            x.Solve();
        }


        [Fact]
        public void AssignmentMutual()
        {

            var x = new TypeProblem2();

            var m1 = x.CreateMember(x.Root, new NameKey("m1"));
            x.CreateHopefulMember(m1, new NameKey("x"));
            var m2 = x.CreateMember(x.Root, new NameKey("m2"));
            x.CreateHopefulMember(m2, new NameKey("y"));

            m1.AssignTo(m2);
            m2.AssignTo(m1);

            x.Solve();
        }


        [Fact]
        public void Generic() {

            var x = new TypeProblem2();

            x.CreateGenericType(x.Root, new NameKey("pair"), new IKey[] {
                new NameKey("T")
            });

            x.CreateType(x.Root, new NameKey("chicken"));

            x.CreateMember(x.Root, new NameKey("x"), new GenericNameKey(new NameKey("pair"), new IKey[] { new NameKey("chicken") }));

            x.Solve();
        }



        [Fact]
        public void GenericCircular()
        {

            var x = new TypeProblem2();

            var left = x.CreateGenericType(x.Root, new NameKey("left"), new IKey[] {
                new NameKey("left-t")
            });

            x.CreateMember(left,new NameKey("thing"), new GenericNameKey(new NameKey("right"), new IKey[] {
                new NameKey("left-t")
            }));

            var right = x.CreateGenericType(x.Root, new NameKey("right"), new IKey[] {
                new NameKey("right-t")
            });

            x.CreateMember(right, new NameKey("thing"), new GenericNameKey(new NameKey("left"), new IKey[] {
                new NameKey("right-t")
            }));

            x.CreateType(x.Root, new NameKey("chicken"));

            x.CreateMember(x.Root, new NameKey("left-member"), new GenericNameKey(new NameKey("left"), new IKey[] { new NameKey("chicken") }));

            x.CreateMember(x.Root, new NameKey("right-member"), new GenericNameKey(new NameKey("right"), new IKey[] { new NameKey("chicken") }));

            x.Solve();
        }


        [Fact]
        public void NestedGeneric()
        {

            var x = new TypeProblem2();

            x.CreateGenericType(x.Root, new NameKey("pair"), new IKey[] {
                new NameKey("T")
            });

            x.CreateType(x.Root, new NameKey("chicken"));

            x.CreateMember(x.Root, new NameKey("x"), new GenericNameKey(new NameKey("pair"), new IKey[] { new GenericNameKey(new NameKey("pair"), new IKey[] { new NameKey("chicken") }) }));

            x.Solve();
        }
    }
}
