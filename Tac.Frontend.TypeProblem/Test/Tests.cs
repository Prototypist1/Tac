using System;
using System.Collections.Generic;
using System.Text;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Xunit;

namespace Tac.Frontend.TypeProblem.Test
{

    public class TestType { }
    public class TestScope { }
    public class TestExplictType { }
    public class TestObject { }
    public class TestOrType { }
    public class TestMethod { }


    public class TestTypeConverter: IConvertTo<TestType> { }
    public class TestScopeConverter : IConvertTo<TestScope> { }
    public class TestExplictTypeConverter : IConvertTo<TestExplictType> { }
    public class TestObjectConverter : IConvertTo<TestObject> { }
    public class TestOrTypeConverter : IConvertTo<TestOrType> { }
    public class TestMethodConverter : IConvertTo<TestMethod> { }

    public class WTF : Tpn<TestType, TestScope, TestExplictType, TestObject, TestOrType, TestMethod> { 
    }

    public class Tests
    {

        [Fact]
        public void Simplest() {
            var x = new WTF.TypeProblem2(new TestScopeConverter());
            x.Solve(new TestExplictTypeConverter());
        }



        [Fact]
        public void AddType()
        {
            var x = new WTF.TypeProblem2(new TestScopeConverter());
            var hello = x.CreateType(x.Root, new NameKey("Hello"),new TestExplictTypeConverter());
            var hello_x = x.CreateMember(hello, new NameKey("x"), new TestTypeConverter());
            var hello_y = x.CreateMember(hello, new NameKey("y"), new TestTypeConverter());
            var solution = x.Solve(new TestExplictTypeConverter());

            var resultHello = solution.GetExplicitTypeType(hello);

            Assert.True(resultHello.Is2(out var concreteSolution));
            Assert.Equal(2, concreteSolution.Count);
            Assert.True(concreteSolution.ContainsKey(new NameKey("x")));
            Assert.True(concreteSolution.ContainsKey(new NameKey("y")));
        }

        [Fact]
        public void AddMethod()
        {
            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var hello = x.CreateType(x.Root, new NameKey("hello"), new TestExplictTypeConverter());
            var hello_x = x.CreateMember(hello, new NameKey("x"), new TestTypeConverter());
            var hello_y = x.CreateMember(hello, new NameKey("y"), new TestTypeConverter());

            var input = x.CreateValue(x.Root,new NameKey("hello"), new TestTypeConverter());
            var method = x.CreateMethod(x.Root, "input", new TestMethodConverter(), new TestTypeConverter(),new TestTypeConverter());

            var input_x = x.CreateHopefulMember(method.Input(), new NameKey("x"), new TestTypeConverter());
            var input_y = x.CreateHopefulMember(method.Input(), new NameKey("y"), new TestTypeConverter());

            var method_x = x.CreateMember(method, new NameKey("x"), new TestTypeConverter());
            var method_y = x.CreateMember(method, new NameKey("y"), new TestTypeConverter());

            input_x.AssignTo(method_x);
            input_y.AssignTo(method_y);

            method.Input().AssignTo(method.Returns());

            input.AssignTo(method.Input());

            var result = x.Solve(new TestExplictTypeConverter());

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

            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var m1 = x.CreateMember(x.Root, new NameKey("m1"), new TestTypeConverter());
            x.CreateHopefulMember(m1, new NameKey("x"), new TestTypeConverter());
            var m2 = x.CreateMember(x.Root, new NameKey("m2"), new TestTypeConverter());
            x.CreateHopefulMember(m2, new NameKey("y"), new TestTypeConverter());
            var m3 = x.CreateMember(x.Root, new NameKey("m3"), new TestTypeConverter());
            var m4 = x.CreateMember(x.Root, new NameKey("m4"), new TestTypeConverter());
            var m5 = x.CreateMember(x.Root, new NameKey("m5"), new TestTypeConverter());

            m1.AssignTo(m3);
            m2.AssignTo(m3);
            m3.AssignTo(m4);
            m3.AssignTo(m5);

            var solution = x.Solve(new TestExplictTypeConverter());

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

            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var m1 = x.CreateMember(x.Root, new NameKey("m1"), new TestTypeConverter());
            var m2 = x.CreateMember(x.Root, new NameKey("m2"), new TestTypeConverter());
            var m3 = x.CreateMember(x.Root, new NameKey("m3"), new TestTypeConverter());
            var m4 = x.CreateMember(x.Root, new NameKey("m4"), new TestTypeConverter());
            x.CreateHopefulMember(m4, new NameKey("x"), new TestTypeConverter());
            var m5 = x.CreateMember(x.Root, new NameKey("m5"), new TestTypeConverter());
            x.CreateHopefulMember(m5, new NameKey("y"), new TestTypeConverter());

            m1.AssignTo(m3);
            m2.AssignTo(m3);
            m3.AssignTo(m4);
            m3.AssignTo(m5);

            var solution = x.Solve(new TestExplictTypeConverter());

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

            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var m1 = x.CreateMember(x.Root, new NameKey("m1"), new TestTypeConverter());
            x.CreateHopefulMember(m1, new NameKey("x"), new TestTypeConverter());
            var m2 = x.CreateMember(x.Root, new NameKey("m2"), new TestTypeConverter());
            x.CreateHopefulMember(m2, new NameKey("y"), new TestTypeConverter());

            m1.AssignTo(m2);
            m2.AssignTo(m1);

            var solution = x.Solve(new TestExplictTypeConverter());

            var m1Or = solution.GetMemberType(m1);
            Assert.True(m1Or.Is2(out var m1concrete));
            Assert.Equal(2, m1concrete.Count);

            var m2Or = solution.GetMemberType(m2);
            Assert.True(m2Or.Is2(out var m2concrete));
            Assert.Equal(2, m2concrete.Count);
        }


        [Fact]
        public void Generic() {

            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var pairType = x.CreateGenericType(x.Root, new NameKey("pair"), new (IKey,IConvertTo<TestExplictType>)[] {
                (new NameKey("T"), new TestExplictTypeConverter())
            },new TestExplictTypeConverter());

            x.CreateMember(pairType, new NameKey("x"),
                new NameKey("T"),false, new TestTypeConverter());

            var chickenType = x.CreateType(x.Root, new NameKey("chicken"), new TestExplictTypeConverter());

            x.CreateMember(chickenType, new NameKey("eggs"), new TestTypeConverter());

            var chickenPair = x.CreateMember(x.Root, new NameKey("x"), new GenericNameKey(new NameKey("pair"), new IKey[] { new NameKey("chicken") }), false, new TestTypeConverter());

            var solution = x.Solve(new TestExplictTypeConverter());

            var chickenPairOr = solution.GetMemberType(chickenPair);

            Assert.True(chickenPairOr.Is2(out var chickPairConcrete));
            var pair = Assert.Single(chickPairConcrete);
            Assert.True(pair.Value.Is2(out var chickenMemberConcrete));
            Assert.True(chickenMemberConcrete.ContainsKey(new NameKey("eggs")));

        }

        [Fact]
        public void GenericContainsSelf()
        {

            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var type = x.CreateGenericType(x.Root, new NameKey("node"), new (IKey, IConvertTo<TestExplictType>)[] {
                (new NameKey("node-t"), new TestExplictTypeConverter())
            }, new TestExplictTypeConverter());

            x.CreateMember(type, new NameKey("next"), new GenericNameKey(new NameKey("node"), new IKey[] {
                new NameKey("node-t")
            }), false, new TestTypeConverter());

            x.CreateType(x.Root, new NameKey("chicken"), new TestExplictTypeConverter());


            var thing = x.CreateMember(x.Root, new NameKey("thing"), new GenericNameKey(new NameKey("node"), new IKey[] {
                new NameKey("chicken")
            }), false, new TestTypeConverter());

            var solution = x.Solve(new TestExplictTypeConverter());

            var thingOr = solution.GetMemberType(thing);

            Assert.True(thingOr.Is2(out var thingConcrete));
            var pair = Assert.Single(thingConcrete);
            Assert.True(pair.Value.Is2(out var pairConcrete));
            Assert.Equal(pairConcrete, thingConcrete);
        }


        [Fact]
        public void GenericCircular()
        {

            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var left = x.CreateGenericType(x.Root, new NameKey("left"), new (IKey, IConvertTo<TestExplictType>)[] {
                (new NameKey("left-t"), new TestExplictTypeConverter())
            }, new TestExplictTypeConverter());

            x.CreateMember(left,new NameKey("thing"), new GenericNameKey(new NameKey("right"), new IKey[] {
                new NameKey("left-t")
            }), false, new TestTypeConverter());

            var right = x.CreateGenericType(x.Root, new NameKey("right"), new (IKey, IConvertTo<TestExplictType>)[] {
                (new NameKey("right-t"), new TestExplictTypeConverter())
            }, new TestExplictTypeConverter());

            x.CreateMember(right, new NameKey("thing"), new GenericNameKey(new NameKey("left"), new IKey[] {
                new NameKey("right-t")
            }), false, new TestTypeConverter());

            x.CreateType(x.Root, new NameKey("chicken"), new TestExplictTypeConverter());

            var leftMember =  x.CreateMember(x.Root, new NameKey("left-member"), new GenericNameKey(new NameKey("left"), new IKey[] { new NameKey("chicken") }), false, new TestTypeConverter());

            var rightMember = x.CreateMember(x.Root, new NameKey("right-member"), new GenericNameKey(new NameKey("right"), new IKey[] { new NameKey("chicken") }), false, new TestTypeConverter());

            var solution = x.Solve(new TestExplictTypeConverter());

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

            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var pairType = x.CreateGenericType(x.Root, new NameKey("pair"), new (IKey, IConvertTo<TestExplictType>)[] {
                (new NameKey("T"), new TestExplictTypeConverter())
            }, new TestExplictTypeConverter());


            x.CreateMember(pairType, new NameKey("x"),
                new NameKey("T"), false, new TestTypeConverter());

            var chickenType = x.CreateType(x.Root, new NameKey("chicken"), new TestExplictTypeConverter());

            x.CreateMember(chickenType, new NameKey("eggs"), new TestTypeConverter());

            var xMember = x.CreateMember(x.Root, new NameKey("x"), new GenericNameKey(new NameKey("pair"), new IKey[] { new GenericNameKey(new NameKey("pair"), new IKey[] { new NameKey("chicken") }) }), false, new TestTypeConverter());


            var solution = x.Solve(new TestExplictTypeConverter());

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
