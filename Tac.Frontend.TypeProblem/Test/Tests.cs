using System;
using System.Collections.Generic;
using System.Text;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Semantic_Model;
using Xunit;

namespace Tac.Frontend.TypeProblem.Test
{

    // Colin! you shit! why don't you just use real stuff!?
    public interface ITestType { 
    
    }

    public class TestScope {
        public readonly List<IBox<TestMember>> members = new List<IBox<TestMember>>();
    }
    public class TestExplictType : ITestType {
        public readonly IBox<TestScope> MemberCollection;

        public TestExplictType(IBox<TestScope> memberCollection)
        {
            MemberCollection = memberCollection ?? throw new ArgumentNullException(nameof(memberCollection));
        }
    }
    public class TestObject {
        public readonly IBox<TestScope> MemberCollection;
    }
    public class TestOrType : ITestType {
        public readonly IBox<ITestType> Type1;
        public readonly IBox<ITestType> Type2;
    }
    public class TestMethod {
        public readonly IBox<TestScope> Scope;

        public TestMethod(IBox<TestScope> @ref)
        {
            this.Scope = @ref ?? throw new ArgumentNullException(nameof(@ref));
        }
    }
    public class TestMember {
        public readonly IBox<ITestType> Type;

        public TestMember(IBox<ITestType> testType)
        {
            Type = testType ?? throw new ArgumentNullException(nameof(testType));
        }
    }
    public class TestTypeReference {
        public readonly IBox<ITestType> Type;

        public TestTypeReference(IBox<ITestType> testType)
        {
            Type = testType ?? throw new ArgumentNullException(nameof(testType));
        }
    }
    public class TestValue {
        public readonly IBox<ITestType> Type;

        public TestValue(IBox<ITestType> testType)
        {
            Type = testType ?? throw new ArgumentNullException(nameof(testType));
        }
    }


    public class TestScopeConverter : WTF.IConvertTo<WTF.TypeProblem2.Scope, TestScope>
    {
        public TestScope Convert(Tpn<TestScope, TestExplictType, TestObject, TestOrType, TestMethod, TestValue, TestMember, TestTypeReference>.ITypeSolution typeSolution, Tpn<TestScope, TestExplictType, TestObject, TestOrType, TestMethod, TestValue, TestMember, TestTypeReference>.TypeProblem2.Scope from)
        {
            var members = typeSolution.GetMembers(from);
            var res = new TestScope();
            foreach (var member in members)
            {
                res.members.Add(typeSolution.GetMemberType(member));
            }
            return res;
        }
    }
    public class TestExplictTypeConverter : WTF.IConvertTo<WTF.TypeProblem2.Type, TestExplictType>
    {
        public TestExplictType Convert(Tpn<TestScope, TestExplictType, TestObject, TestOrType, TestMethod, TestValue, TestMember, TestTypeReference>.ITypeSolution typeSolution, Tpn<TestScope, TestExplictType, TestObject, TestOrType, TestMethod, TestValue, TestMember, TestTypeReference>.TypeProblem2.Type from)
        {
            var members = typeSolution.GetMembers(from);
            var scope = new TestScope();
            foreach (var member in members)
            {
                scope.members.Add(typeSolution.GetMemberType(member));
            }
            var @ref = new Box<TestScope>();
            @ref.Fill(scope);
            return new TestExplictType(@ref);
        }
    }
    public class TestMethodConverter : WTF.IConvertTo<WTF.TypeProblem2.Method, TestMethod>
    {
        public TestMethod Convert(Tpn<TestScope, TestExplictType, TestObject, TestOrType, TestMethod, TestValue, TestMember, TestTypeReference>.ITypeSolution typeSolution, Tpn<TestScope, TestExplictType, TestObject, TestOrType, TestMethod, TestValue, TestMember, TestTypeReference>.TypeProblem2.Method from)
        {
            var members = typeSolution.GetMembers(from);
            var scope = new TestScope();
            foreach (var member in members)
            {
                scope.members.Add(typeSolution.GetMemberType(member));
            }
            var @ref = new Box<TestScope>();
            @ref.Fill(scope);
            return new TestMethod(@ref);
        }
    }
    public class TestMemberConverter : WTF.IConvertTo<WTF.TypeProblem2.Member, TestMember>
    {
        public TestMember Convert(Tpn<TestScope, TestExplictType, TestObject, TestOrType, TestMethod, TestValue, TestMember, TestTypeReference>.ITypeSolution typeSolution, Tpn<TestScope, TestExplictType, TestObject, TestOrType, TestMethod, TestValue, TestMember, TestTypeReference>.TypeProblem2.Member from)
        {
            var orType = typeSolution.GetType(from);

            IBox<ITestType> testType;
            if (orType.Is1(out var v1))
            {
                testType = typeSolution.GetExplicitTypeType(v1);
            }
            else if (orType.Is2(out var v2))
            {

                testType = typeSolution.GetOrType(v2);
            }
            else {
                throw new Exception("well, should have been one of those");
            }
            return new TestMember(testType);
        }
    }
    public class TestTypeReferenceConverter : WTF.IConvertTo<WTF.TypeProblem2.Member, TestTypeReference>
    {
        public TestTypeReference Convert(Tpn<TestScope, TestExplictType, TestObject, TestOrType, TestMethod, TestValue, TestMember, TestTypeReference>.ITypeSolution typeSolution, Tpn<TestScope, TestExplictType, TestObject, TestOrType, TestMethod, TestValue, TestMember, TestTypeReference>.TypeProblem2.Member from)
        {
            var orType = typeSolution.GetType(from);

            IBox<ITestType> testType;
            if (orType.Is1(out var v1))
            {
                testType = typeSolution.GetExplicitTypeType(v1);
            }
            else if (orType.Is2(out var v2))
            {

                testType = typeSolution.GetOrType(v2);
            }
            else
            {
                throw new Exception("well, should have been one of those");
            }
            return new TestTypeReference(testType);
        }
    }
    public class TestValueConverter : WTF.IConvertTo<WTF.TypeProblem2.Value, TestValue>
    {
        public TestValue Convert(Tpn<TestScope, TestExplictType, TestObject, TestOrType, TestMethod, TestValue, TestMember, TestTypeReference>.ITypeSolution typeSolution, Tpn<TestScope, TestExplictType, TestObject, TestOrType, TestMethod, TestValue, TestMember, TestTypeReference>.TypeProblem2.Value from)
        {
            var orType = typeSolution.GetType(from);

            IBox<ITestType> testType;
            if (orType.Is1(out var v1))
            {
                testType = typeSolution.GetExplicitTypeType(v1);
            }
            else if (orType.Is2(out var v2))
            {

                testType = typeSolution.GetOrType(v2);
            }
            else
            {
                throw new Exception("well, should have been one of those");
            }
            return new TestValue(testType);
        }
    }

    public class WTF : Tpn<TestScope, TestExplictType, TestObject, TestOrType, TestMethod, TestValue, TestMember, TestTypeReference> { 
    }

    // TODO test or types
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
            var hello_x = x.CreateMember(hello, new NameKey("x"), new TestMemberConverter());
            var hello_y = x.CreateMember(hello, new NameKey("y"), new TestMemberConverter());
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
            var hello_x = x.CreateMember(hello, new NameKey("x"), new TestMemberConverter());
            var hello_y = x.CreateMember(hello, new NameKey("y"), new TestMemberConverter());

            var input = x.CreateValue(x.Root,new NameKey("hello"), new TestValueConverter());
            var method = x.CreateMethod(x.Root, "input", new TestMethodConverter(), new TestMemberConverter(),new TestMemberConverter());

            var input_x = x.CreateHopefulMember(method.Input(), new NameKey("x"), new TestMemberConverter());
            var input_y = x.CreateHopefulMember(method.Input(), new NameKey("y"), new TestMemberConverter());

            var method_x = x.CreateMember(method, new NameKey("x"), new TestMemberConverter());
            var method_y = x.CreateMember(method, new NameKey("y"), new TestMemberConverter());

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

            var m1 = x.CreateMember(x.Root, new NameKey("m1"), new TestMemberConverter());
            x.CreateHopefulMember(m1, new NameKey("x"), new TestMemberConverter());
            var m2 = x.CreateMember(x.Root, new NameKey("m2"), new TestMemberConverter());
            x.CreateHopefulMember(m2, new NameKey("y"), new TestMemberConverter());
            var m3 = x.CreateMember(x.Root, new NameKey("m3"), new TestMemberConverter());
            var m4 = x.CreateMember(x.Root, new NameKey("m4"), new TestMemberConverter());
            var m5 = x.CreateMember(x.Root, new NameKey("m5"), new TestMemberConverter());

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

            var m1 = x.CreateMember(x.Root, new NameKey("m1"), new TestMemberConverter());
            var m2 = x.CreateMember(x.Root, new NameKey("m2"), new TestMemberConverter());
            var m3 = x.CreateMember(x.Root, new NameKey("m3"), new TestMemberConverter());
            var m4 = x.CreateMember(x.Root, new NameKey("m4"), new TestMemberConverter());
            x.CreateHopefulMember(m4, new NameKey("x"), new TestMemberConverter());
            var m5 = x.CreateMember(x.Root, new NameKey("m5"), new TestMemberConverter());
            x.CreateHopefulMember(m5, new NameKey("y"), new TestMemberConverter());

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

            var m1 = x.CreateMember(x.Root, new NameKey("m1"), new TestMemberConverter());
            x.CreateHopefulMember(m1, new NameKey("x"), new TestMemberConverter());
            var m2 = x.CreateMember(x.Root, new NameKey("m2"), new TestMemberConverter());
            x.CreateHopefulMember(m2, new NameKey("y"), new TestMemberConverter());

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

            var pairType = x.CreateGenericType(x.Root, new NameKey("pair"), new (IKey,WTF.IConvertTo<WTF.TypeProblem2.Type, TestExplictType>)[] {
                (new NameKey("T"), new TestExplictTypeConverter())
            },new TestExplictTypeConverter());

            x.CreateMember(pairType, new NameKey("x"),
                new NameKey("T"),false, new TestMemberConverter());

            var chickenType = x.CreateType(x.Root, new NameKey("chicken"), new TestExplictTypeConverter());

            x.CreateMember(chickenType, new NameKey("eggs"), new TestMemberConverter());

            var chickenPair = x.CreateMember(x.Root, new NameKey("x"), new GenericNameKey(new NameKey("pair"), new IKey[] { new NameKey("chicken") }), false, new TestMemberConverter());

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

            var type = x.CreateGenericType(x.Root, new NameKey("node"), new (IKey, WTF.IConvertTo<WTF.TypeProblem2.Type, TestExplictType>)[] {
                (new NameKey("node-t"), new TestExplictTypeConverter())
            }, new TestExplictTypeConverter());

            x.CreateMember(type, new NameKey("next"), new GenericNameKey(new NameKey("node"), new IKey[] {
                new NameKey("node-t")
            }), false, new TestMemberConverter());

            x.CreateType(x.Root, new NameKey("chicken"), new TestExplictTypeConverter());


            var thing = x.CreateMember(x.Root, new NameKey("thing"), new GenericNameKey(new NameKey("node"), new IKey[] {
                new NameKey("chicken")
            }), false, new TestMemberConverter());

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

            var left = x.CreateGenericType(x.Root, new NameKey("left"), new (IKey, WTF.IConvertTo<WTF.TypeProblem2.Type, TestExplictType>)[] {
                (new NameKey("left-t"), new TestExplictTypeConverter())
            }, new TestExplictTypeConverter());

            x.CreateMember(left,new NameKey("thing"), new GenericNameKey(new NameKey("right"), new IKey[] {
                new NameKey("left-t")
            }), false, new TestMemberConverter());

            var right = x.CreateGenericType(x.Root, new NameKey("right"), new (IKey, WTF.IConvertTo<WTF.TypeProblem2.Type, TestExplictType>)[] {
                (new NameKey("right-t"), new TestExplictTypeConverter())
            }, new TestExplictTypeConverter());

            x.CreateMember(right, new NameKey("thing"), new GenericNameKey(new NameKey("left"), new IKey[] {
                new NameKey("right-t")
            }), false, new TestMemberConverter());

            x.CreateType(x.Root, new NameKey("chicken"), new TestExplictTypeConverter());

            var leftMember =  x.CreateMember(x.Root, new NameKey("left-member"), new GenericNameKey(new NameKey("left"), new IKey[] { new NameKey("chicken") }), false, new TestMemberConverter());

            var rightMember = x.CreateMember(x.Root, new NameKey("right-member"), new GenericNameKey(new NameKey("right"), new IKey[] { new NameKey("chicken") }), false, new TestMemberConverter());

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

            var pairType = x.CreateGenericType(x.Root, new NameKey("pair"), new (IKey, WTF.IConvertTo<WTF.TypeProblem2.Type, TestExplictType>)[] {
                (new NameKey("T"), new TestExplictTypeConverter())
            }, new TestExplictTypeConverter());


            x.CreateMember(pairType, new NameKey("x"),
                new NameKey("T"), false, new TestMemberConverter());

            var chickenType = x.CreateType(x.Root, new NameKey("chicken"), new TestExplictTypeConverter());

            x.CreateMember(chickenType, new NameKey("eggs"), new TestMemberConverter());

            var xMember = x.CreateMember(x.Root, new NameKey("x"), new GenericNameKey(new NameKey("pair"), new IKey[] { new GenericNameKey(new NameKey("pair"), new IKey[] { new NameKey("chicken") }) }), false, new TestMemberConverter());


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
