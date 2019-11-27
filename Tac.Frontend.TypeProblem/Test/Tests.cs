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
            var solution = x.Solve();

            var resultHello = solution.GetExplicitTypeType(hello);

            Assert.True(resultHello.Is2(out var concreteSolution));
            Assert.Equal(2, concreteSolution.Count);
            Assert.True(concreteSolution.ContainsKey(new NameKey("x")));
            Assert.True(concreteSolution.ContainsKey(new NameKey("y")));
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

            var result = x.Solve();

            var methodResultOr = result.GetMethodScopeType(method);

            Assert.True(methodResultOr.Is2(out var concertMethod));
            Assert.True(concertMethod.TryGetValue(new NameKey("input"), out var inputOr));
            Assert.True(concertMethod.ContainsKey(new NameKey("x")));
            Assert.True(concertMethod.ContainsKey(new NameKey("y")));
            Assert.Equal(4, concertMethod.Count);
            Assert.True(inputOr.Is2(out var concertInput));
            Assert.True(concertInput.ContainsKey(new NameKey("x")));
            Assert.True(concertInput.ContainsKey(new NameKey("y")));
            Assert.Equal(2, concertInput.Count);

            var resultHello = result.GetExplicitTypeType(hello);
            Assert.True(resultHello.Is2(out var concreteSolution));
            Assert.Equal(2, concreteSolution.Count);
            Assert.True(concreteSolution.ContainsKey(new NameKey("x")));
            Assert.True(concreteSolution.ContainsKey(new NameKey("y")));

            // things don't flow downstream 
            var methodReturns = result.GetMemberType(method.Returns());
            Assert.True(methodReturns.Is2(out var concertReturns));
            Assert.Empty(concertReturns);

        }

        [Fact]
        public void AssignmentXDownStream() {

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

            var solution = x.Solve();

            var m1Or = solution.GetMemberType(m1);
            Assert.True(m1Or.Is2(out var m1concrete));
            Assert.Single(m1concrete);

            var m2Or = solution.GetMemberType(m2);
            Assert.True(m2Or.Is2(out var m2concrete));
            Assert.Single(m2concrete);

            var m3Or = solution.GetMemberType(m3);
            Assert.True(m3Or.Is2(out var m3concrete));
            Assert.Empty(m3concrete);

            var m4Or = solution.GetMemberType(m4);
            Assert.True(m4Or.Is2(out var m4concrete));
            Assert.Empty(m4concrete);

            var m5Or = solution.GetMemberType(m5);
            Assert.True(m5Or.Is2(out var m5concrete));
            Assert.Empty(m5concrete);

        }



        [Fact]
        public void AssignmentXUpStream()
        {

            var x = new TypeProblem2();

            var m1 = x.CreateMember(x.Root, new NameKey("m1"));
            var m2 = x.CreateMember(x.Root, new NameKey("m2"));
            var m3 = x.CreateMember(x.Root, new NameKey("m3"));
            var m4 = x.CreateMember(x.Root, new NameKey("m4"));
            x.CreateHopefulMember(m4, new NameKey("x"));
            var m5 = x.CreateMember(x.Root, new NameKey("m5"));
            x.CreateHopefulMember(m5, new NameKey("y"));

            m1.AssignTo(m3);
            m2.AssignTo(m3);
            m3.AssignTo(m4);
            m3.AssignTo(m5);

            var solution = x.Solve();

            var m1Or = solution.GetMemberType(m1);
            Assert.True(m1Or.Is2(out var m1concrete));
            Assert.Equal(2,m1concrete.Count);

            var m2Or = solution.GetMemberType(m2);
            Assert.True(m2Or.Is2(out var m2concrete));
            Assert.Equal(2, m2concrete.Count);

            var m3Or = solution.GetMemberType(m3);
            Assert.True(m3Or.Is2(out var m3concrete));
            Assert.Equal(2, m3concrete.Count);

            var m4Or = solution.GetMemberType(m4);
            Assert.True(m4Or.Is2(out var m4concrete));
            Assert.Single(m4concrete);

            var m5Or = solution.GetMemberType(m5);
            Assert.True(m5Or.Is2(out var m5concrete));
            Assert.Single(m5concrete);

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

            var solution = x.Solve();

            var m1Or = solution.GetMemberType(m1);
            Assert.True(m1Or.Is2(out var m1concrete));
            Assert.Equal(2, m1concrete.Count);

            var m2Or = solution.GetMemberType(m2);
            Assert.True(m2Or.Is2(out var m2concrete));
            Assert.Equal(2, m2concrete.Count);
        }


        [Fact]
        public void Generic() {

            var x = new TypeProblem2();

            var pairType = x.CreateGenericType(x.Root, new NameKey("pair"), new IKey[] {
                new NameKey("T")
            });

            x.CreateMember(pairType, new NameKey("x"),
                new NameKey("T"));

            var chickenType = x.CreateType(x.Root, new NameKey("chicken"));

            x.CreateMember(chickenType, new NameKey("eggs"));

            var chickenPair = x.CreateMember(x.Root, new NameKey("x"), new GenericNameKey(new NameKey("pair"), new IKey[] { new NameKey("chicken") }));

            var solution = x.Solve();

            var chickenPairOr = solution.GetMemberType(chickenPair);

            Assert.True(chickenPairOr.Is2(out var chickPairConcrete));
            var pair = Assert.Single(chickPairConcrete);
            Assert.True(pair.Value.Is2(out var chickenMemberConcrete));
            Assert.True(chickenMemberConcrete.ContainsKey(new NameKey("eggs")));

        }

        [Fact]
        public void GenericContainsSelf()
        {

            var x = new TypeProblem2();

            var type = x.CreateGenericType(x.Root, new NameKey("node"), new IKey[] {
                new NameKey("node-t")
            });

            x.CreateMember(type, new NameKey("next"), new GenericNameKey(new NameKey("node"), new IKey[] {
                new NameKey("node-t")
            }));

            x.CreateType(x.Root, new NameKey("chicken"));


            var thing = x.CreateMember(x.Root, new NameKey("thing"), new GenericNameKey(new NameKey("node"), new IKey[] {
                new NameKey("chicken")
            }));

            var solution = x.Solve();

            var thingOr = solution.GetMemberType(thing);

            Assert.True(thingOr.Is2(out var thingConcrete));
            var pair = Assert.Single(thingConcrete);
            Assert.True(pair.Value.Is2(out var pairConcrete));
            Assert.Equal(pairConcrete, thingConcrete);
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

            var leftMember =  x.CreateMember(x.Root, new NameKey("left-member"), new GenericNameKey(new NameKey("left"), new IKey[] { new NameKey("chicken") }));

            var rightMember = x.CreateMember(x.Root, new NameKey("right-member"), new GenericNameKey(new NameKey("right"), new IKey[] { new NameKey("chicken") }));

            var solution = x.Solve();

            var leftMemberOr = solution.GetMemberType(leftMember);
            var rightMemberOr = solution.GetMemberType(rightMember);

            Assert.True(leftMemberOr.Is2(out var leftMemberConcrete));
            Assert.True(rightMemberOr.Is2(out var rightMemberConcrete));

            var leftMemberConcreteMemberPair = Assert.Single(leftMemberConcrete);
            Assert.True(leftMemberConcreteMemberPair.Value.Is2(out var leftMemberConcreteMemberConcrete));
            var rightMemberConcreteMemberPair = Assert.Single(rightMemberConcrete);
            Assert.True(rightMemberConcreteMemberPair.Value.Is2(out var rightMemberConcreteMemberConcrete));

            Assert.Equal(leftMemberConcreteMemberConcrete, rightMemberConcrete);
            Assert.Equal(rightMemberConcreteMemberConcrete, leftMemberConcrete);
        }


        [Fact]
        public void NestedGeneric()
        {

            var x = new TypeProblem2();

            var pairType = x.CreateGenericType(x.Root, new NameKey("pair"), new IKey[] {
                new NameKey("T")
            });


            x.CreateMember(pairType, new NameKey("x"),
                new NameKey("T"));

            var chickenType = x.CreateType(x.Root, new NameKey("chicken"));

            x.CreateMember(chickenType, new NameKey("eggs"));

            var xMember = x.CreateMember(x.Root, new NameKey("x"), new GenericNameKey(new NameKey("pair"), new IKey[] { new GenericNameKey(new NameKey("pair"), new IKey[] { new NameKey("chicken") }) }));


            var solution = x.Solve();

            var chickenPairOr = solution.GetMemberType(xMember);

            Assert.True(chickenPairOr.Is2(out var chickenPairPairConcrete));
            var chickenPairPairConcreteMember = Assert.Single(chickenPairPairConcrete);
            Assert.True(chickenPairPairConcreteMember.Value.Is2(out var chickenPairConcrete));
            var chickenPairConcreteMember = Assert.Single(chickenPairConcrete);
            Assert.True(chickenPairConcreteMember.Value.Is2(out var chickenConcrete));
            Assert.True(chickenConcrete.ContainsKey(new NameKey("eggs")));
        }
    }
}
