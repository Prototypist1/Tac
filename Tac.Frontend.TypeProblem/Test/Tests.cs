using Prototypist.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public interface IHasScope {
        IBox<TestScope> MemberCollection { get; }
    }

    public class TestScope {
        public readonly List<IBox<TestMember>> members = new List<IBox<TestMember>>();
    }
    public class TestExplictType : ITestType, IHasScope
    {
        public IBox<TestScope> MemberCollection { get; }

        public TestExplictType(IBox<TestScope> memberCollection)
        {
            MemberCollection = memberCollection ?? throw new ArgumentNullException(nameof(memberCollection));
        }
    }
    public class TestObject: IHasScope
    {
        public  IBox<TestScope> MemberCollection { get; }
    }
    public class TestOrType : ITestType {
        public readonly IBox<ITestType> Type1;
        public readonly IBox<ITestType> Type2;
    }
    public class TestMethod: IHasScope
    {
        public IBox<TestScope> MemberCollection { get; }

        public TestMethod(IBox<TestScope> @ref)
        {
            this.MemberCollection = @ref ?? throw new ArgumentNullException(nameof(@ref));
        }
    }
    public class TestMember {
        public readonly IBox<ITestType> Type;
        public readonly IKey key;

        public TestMember(IBox<ITestType> testType, IKey key)
        {
            Type = testType ?? throw new ArgumentNullException(nameof(testType));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
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
        private IKey nameKey;

        public TestMemberConverter(IKey nameKey)
        {
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
        }

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
            return new TestMember(testType, nameKey);
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
        #region Help

        private static void HasCount(int count, IHasScope result)
        {
            Assert.Equal(count, result.MemberCollection.GetValue().members.Count);
        }

        private static TestMember HasMember(IHasScope result, IKey key)
        {
            var single = result.MemberCollection.GetValue().members.Where(x => key.Equals(x.GetValue().key));
            Assert.Single(single);
            return single.First().GetValue();
        }

        private static TestExplictType MemberToType(TestMember member) {
            return (TestExplictType)member.Type.GetValue();
        }

        #endregion


        [Fact]
        public void Simplest() {
            var x = new WTF.TypeProblem2(new TestScopeConverter());
            x.Solve(new TestExplictTypeConverter());
        }

        [Fact]
        public void AddType()
        {
            var x = new WTF.TypeProblem2(new TestScopeConverter());
            var hello = x.CreateType(x.Root, new NameKey("Hello"), new TestExplictTypeConverter());
            var hello_x = x.CreateMember(hello, new NameKey("x"), new TestMemberConverter(new NameKey("x")));
            var hello_y = x.CreateMember(hello, new NameKey("y"), new TestMemberConverter(new NameKey("y")));
            var solution = x.Solve(new TestExplictTypeConverter());

            var resultHello = solution.GetExplicitTypeType(hello).GetValue();

            HasCount(2, resultHello);
            HasMember(resultHello, new NameKey("x"));
            HasMember(resultHello, new NameKey("y"));
        }


        [Fact]
        public void AddMethod()
        {
            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var hello = x.CreateType(x.Root, new NameKey("hello"), new TestExplictTypeConverter());
            var hello_x = x.CreateMember(hello, new NameKey("x"), new TestMemberConverter(new NameKey("x")));
            var hello_y = x.CreateMember(hello, new NameKey("y"), new TestMemberConverter(new NameKey("y")));

            var input = x.CreateValue(x.Root,new NameKey("hello"), new TestValueConverter());
            var method = x.CreateMethod(x.Root, "input", new TestMethodConverter(), new TestMemberConverter(new NameKey("input")),new TestMemberConverter(new NameKey("result")));

            var input_x = x.CreateHopefulMember(method.Input(), new NameKey("x"), new TestMemberConverter(new NameKey("x")));
            var input_y = x.CreateHopefulMember(method.Input(), new NameKey("y"), new TestMemberConverter(new NameKey("y")));

            var method_x = x.CreateMember(method, new NameKey("x"), new TestMemberConverter(new NameKey("x")));
            var method_y = x.CreateMember(method, new NameKey("y"), new TestMemberConverter(new NameKey("y")));

            input_x.AssignTo(method_x);
            input_y.AssignTo(method_y);

            method.Input().AssignTo(method.Returns());

            input.AssignTo(method.Input());

            var result = x.Solve(new TestExplictTypeConverter());

            var methodResult = result.GetMethodScopeType(method).GetValue();

            HasCount(4, methodResult);
            HasMember(methodResult,new NameKey("x"));
            HasMember(methodResult,new NameKey("y"));
            var inputResult = MemberToType(HasMember(methodResult, new NameKey("input")));

            HasCount(2, inputResult);
            HasMember(inputResult, new NameKey("x"));
            HasMember(inputResult, new NameKey("y"));

            var helloResult = result.GetExplicitTypeType(hello).GetValue();
            HasCount(2, helloResult);
            HasMember(helloResult, new NameKey("x"));
            HasMember(helloResult, new NameKey("y"));

            // things don't flow downstream 
            var methodReturns = MemberToType(result.GetMemberType(method.Returns()).GetValue());
            HasCount(0, methodReturns);

        }

        [Fact]
        public void AssignmentXDownStream() {

            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var m1 = x.CreateMember(x.Root, new NameKey("m1"), new TestMemberConverter(new NameKey("m1")));
            x.CreateHopefulMember(m1, new NameKey("x"), new TestMemberConverter(new NameKey("x")));
            var m2 = x.CreateMember(x.Root, new NameKey("m2"), new TestMemberConverter(new NameKey("m2")));
            x.CreateHopefulMember(m2, new NameKey("y"), new TestMemberConverter(new NameKey("y")));
            var m3 = x.CreateMember(x.Root, new NameKey("m3"), new TestMemberConverter(new NameKey("m3")));
            var m4 = x.CreateMember(x.Root, new NameKey("m4"), new TestMemberConverter(new NameKey("m4")));
            var m5 = x.CreateMember(x.Root, new NameKey("m5"), new TestMemberConverter(new NameKey("m5")));

            m1.AssignTo(m3);
            m2.AssignTo(m3);
            m3.AssignTo(m4);
            m3.AssignTo(m5);

            var solution = x.Solve(new TestExplictTypeConverter());

            HasCount(1, MemberToType(solution.GetMemberType(m1).GetValue()));
            HasCount(1, MemberToType(solution.GetMemberType(m2).GetValue()));
            HasCount(0, MemberToType(solution.GetMemberType(m3).GetValue()));
            HasCount(0, MemberToType(solution.GetMemberType(m4).GetValue()));
            HasCount(0, MemberToType(solution.GetMemberType(m5).GetValue()));

        }



        [Fact]
        public void AssignmentXUpStream()
        {

            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var m1 = x.CreateMember(x.Root, new NameKey("m1"), new TestMemberConverter(new NameKey("m1")));
            var m2 = x.CreateMember(x.Root, new NameKey("m2"), new TestMemberConverter(new NameKey("m2")));
            var m3 = x.CreateMember(x.Root, new NameKey("m3"), new TestMemberConverter(new NameKey("m3")));
            var m4 = x.CreateMember(x.Root, new NameKey("m4"), new TestMemberConverter(new NameKey("m4")));
            x.CreateHopefulMember(m4, new NameKey("x"), new TestMemberConverter(new NameKey("x")));
            var m5 = x.CreateMember(x.Root, new NameKey("m5"), new TestMemberConverter(new NameKey("m5")));
            x.CreateHopefulMember(m5, new NameKey("y"), new TestMemberConverter(new NameKey("y")));

            m1.AssignTo(m3);
            m2.AssignTo(m3);
            m3.AssignTo(m4);
            m3.AssignTo(m5);

            var solution = x.Solve(new TestExplictTypeConverter());

            HasCount(2, MemberToType(solution.GetMemberType(m1).GetValue()));
            HasCount(2, MemberToType(solution.GetMemberType(m2).GetValue()));
            HasCount(2, MemberToType(solution.GetMemberType(m3).GetValue()));
            HasCount(1, MemberToType(solution.GetMemberType(m4).GetValue()));
            HasCount(1, MemberToType(solution.GetMemberType(m5).GetValue()));

        }

        [Fact]
        public void AssignmentMutual()
        {

            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var m1 = x.CreateMember(x.Root, new NameKey("m1"), new TestMemberConverter(new NameKey("m1")));
            x.CreateHopefulMember(m1, new NameKey("x"), new TestMemberConverter(new NameKey("x")));
            var m2 = x.CreateMember(x.Root, new NameKey("m2"), new TestMemberConverter(new NameKey("m2")));
            x.CreateHopefulMember(m2, new NameKey("y"), new TestMemberConverter(new NameKey("y")));

            m1.AssignTo(m2);
            m2.AssignTo(m1);

            var solution = x.Solve(new TestExplictTypeConverter());

            HasCount(2, MemberToType(solution.GetMemberType(m1).GetValue()));
            HasCount(2, MemberToType(solution.GetMemberType(m2).GetValue()));
        }


        [Fact]
        public void Generic() {

            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var pairType = x.CreateGenericType(x.Root, new NameKey("pair"), new (IKey,WTF.IConvertTo<WTF.TypeProblem2.Type, TestExplictType>)[] {
                (new NameKey("T"), new TestExplictTypeConverter())
            },new TestExplictTypeConverter());

            x.CreateMember(pairType, new NameKey("x"),
                new NameKey("T"),false, new TestMemberConverter(new NameKey("x")));

            var chickenType = x.CreateType(x.Root, new NameKey("chicken"), new TestExplictTypeConverter());

            x.CreateMember(chickenType, new NameKey("eggs"), new TestMemberConverter(new NameKey("eggs")));

            var chickenPair = x.CreateMember(x.Root, new NameKey("x"), new GenericNameKey(new NameKey("pair"), new IKey[] { new NameKey("chicken") }), false, new TestMemberConverter(new NameKey("x")));

            var solution = x.Solve(new TestExplictTypeConverter());

            var chickenPairResult = solution.GetMemberType(chickenPair).GetValue();

            var chickePairResultType = MemberToType(chickenPairResult);

            HasCount(1, chickePairResultType);
            var xResult = HasMember(chickePairResultType, new NameKey("x"));
            var xResultType = MemberToType(xResult);
            HasCount(1, xResultType);
            HasMember(xResultType, new NameKey("eggs"));

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
            }), false, new TestMemberConverter(new NameKey("next")));

            x.CreateType(x.Root, new NameKey("chicken"), new TestExplictTypeConverter());


            var thing = x.CreateMember(x.Root, new NameKey("thing"), new GenericNameKey(new NameKey("node"), new IKey[] {
                new NameKey("chicken")
            }), false, new TestMemberConverter(new NameKey("thing")));

            var solution = x.Solve(new TestExplictTypeConverter());

            var thingResult = solution.GetMemberType(thing).GetValue();
            var thingResultType = MemberToType(thingResult);

            HasCount(1, thingResultType);
            var nextResult = HasMember(thingResultType, new NameKey("next"));
            var nextResultType = MemberToType(nextResult);
            HasCount(1, nextResultType);

            Assert.Equal(thingResultType, nextResultType);
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
            }), false, new TestMemberConverter(new NameKey("thing")));

            var right = x.CreateGenericType(x.Root, new NameKey("right"), new (IKey, WTF.IConvertTo<WTF.TypeProblem2.Type, TestExplictType>)[] {
                (new NameKey("right-t"), new TestExplictTypeConverter())
            }, new TestExplictTypeConverter());

            x.CreateMember(right, new NameKey("thing"), new GenericNameKey(new NameKey("left"), new IKey[] {
                new NameKey("right-t")
            }), false, new TestMemberConverter(new NameKey("thing")));

            x.CreateType(x.Root, new NameKey("chicken"), new TestExplictTypeConverter());

            var leftMember =  x.CreateMember(x.Root, new NameKey("left-member"), new GenericNameKey(new NameKey("left"), new IKey[] { new NameKey("chicken") }), false, new TestMemberConverter(new NameKey("left-member")));

            var rightMember = x.CreateMember(x.Root, new NameKey("right-member"), new GenericNameKey(new NameKey("right"), new IKey[] { new NameKey("chicken") }), false, new TestMemberConverter(new NameKey("right-member")));

            var solution = x.Solve(new TestExplictTypeConverter());

            var leftResult = solution.GetMemberType(leftMember).GetValue();
            var rightResult = solution.GetMemberType(rightMember).GetValue();

            var leftResultType = MemberToType(leftResult);
            var rightResultType = MemberToType(rightResult);

            HasCount(1, leftResultType);
            HasCount(1, rightResultType);

            var leftThing = HasMember(leftResultType, new NameKey("thing"));
            var rightThing = HasMember(rightResultType, new NameKey("thing"));

            var leftThingType = MemberToType(leftThing);
            var rightThingType = MemberToType(rightThing);

            Assert.Equal(leftResultType, rightThingType);
            Assert.Equal(rightResultType, leftThingType);
        }


        [Fact]
        public void NestedGeneric()
        {

            var x = new WTF.TypeProblem2(new TestScopeConverter());

            var pairType = x.CreateGenericType(x.Root, new NameKey("pair"), new (IKey, WTF.IConvertTo<WTF.TypeProblem2.Type, TestExplictType>)[] {
                (new NameKey("T"), new TestExplictTypeConverter())
            }, new TestExplictTypeConverter());


            x.CreateMember(pairType, new NameKey("x"),
                new NameKey("T"), false, new TestMemberConverter(new NameKey("x")));

            var chickenType = x.CreateType(x.Root, new NameKey("chicken"), new TestExplictTypeConverter());

            x.CreateMember(chickenType, new NameKey("eggs"), new TestMemberConverter(new NameKey("eggs")));

            var xMember = x.CreateMember(x.Root, new NameKey("x"), new GenericNameKey(new NameKey("pair"), new IKey[] { new GenericNameKey(new NameKey("pair"), new IKey[] { new NameKey("chicken") }) }), false, new TestMemberConverter(new NameKey("x")));

            var solution = x.Solve(new TestExplictTypeConverter());

            var xMemberResult = solution.GetMemberType(xMember).GetValue();
            var xMemberResultType = MemberToType(xMemberResult);
            HasCount(1, xMemberResultType);
            var xMemberResultX = HasMember(xMemberResultType, new NameKey("x"));
            var xMemberResultXType = MemberToType(xMemberResultX);
            HasCount(1, xMemberResultXType);
            var xMemberResultXTypeX = HasMember(xMemberResultXType, new NameKey("x"));
            var xMemberResultXTypeXType = MemberToType(xMemberResultXTypeX);
            HasCount(1, xMemberResultXTypeXType);
            HasMember(xMemberResultXTypeXType, new NameKey("eggs"));
        }
    }
}
