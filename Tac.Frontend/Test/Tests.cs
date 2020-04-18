using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Infastructure;
using Tac.SemanticModel;
using Xunit;

namespace Tac.Frontend.TypeProblem.Test
{

    // TODO test or types
    public class TestTpn
    {
        #region Help

        private static void HasCount(int count, IScoped result)
        {
            Assert.Equal(count, result.Scope.GetValue().membersList.Count);
        }

        private static WeakMemberDefinition HasMember(IScoped result, IKey key)
        {
            var thing = Assert.Single(result.Scope.GetValue().membersList.Where(x => key.Equals(x.GetValue().Key)));
            return thing.GetValue();
        }

        private static WeakTypeDefinition MemberToType(WeakMemberDefinition member)
        {
            return (WeakTypeDefinition)member.Type.Is1OrThrow().GetValue();
        }

        #endregion


        [Fact]
        public void Simplest()
        {
            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));
            x.Solve();
        }

        [Fact]
        public void AddType()
        {
            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));
            var hello = x.CreateType(x.ModuleRoot, new NameKey("Hello"), new WeakTypeDefinitionConverter());
            x.CreateMember(hello, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            x.CreateMember(hello, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));
            var solution = x.Solve();

            var resultHello = solution.GetExplicitType(hello).GetValue().Is1OrThrow();

            HasCount(2, resultHello);
            HasMember(resultHello, new NameKey("x"));
            HasMember(resultHello, new NameKey("y"));
        }


        [Fact]
        public void AddMethod()
        {
            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var hello = x.CreateType(x.ModuleRoot, new NameKey("hello"), new WeakTypeDefinitionConverter());
            x.CreateMember(hello, new NameKey("x"), new WeakMemberDefinitionConverter(false,new NameKey("x")));
            x.CreateMember(hello, new NameKey("y"), new WeakMemberDefinitionConverter(false,new NameKey("y")));

            var input = x.CreateValue(x.ModuleRoot, new NameKey("hello"), new PlaceholderValueConverter());
            var method = x.CreateMethod(x.ModuleRoot, "input", new WeakMethodDefinitionConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()),false), new WeakMemberDefinitionConverter(false, new NameKey("input")));

            var input_x = x.CreateHopefulMember(method.Input(), new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var input_y = x.CreateHopefulMember(method.Input(), new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            var method_x = x.CreateMember(method, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var method_y = x.CreateMember(method, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            input_x.AssignTo(method_x);
            input_y.AssignTo(method_y);

            method.Input().AssignTo(method.Returns());

            input.AssignTo(method.Input());

            var result = x.Solve();

            var methodResult = result.GetMethod(method).GetValue().Is1OrThrow();

            HasCount(3, methodResult);
            HasMember(methodResult, new NameKey("input"));
            HasMember(methodResult, new NameKey("x"));
            HasMember(methodResult, new NameKey("y"));
            var inputResult = MemberToType(HasMember(methodResult, new NameKey("input")));

            HasCount(2, inputResult);
            HasMember(inputResult, new NameKey("x"));
            HasMember(inputResult, new NameKey("y"));

            var helloResult = result.GetExplicitType(hello).GetValue().Is1OrThrow();
            HasCount(2, helloResult);
            HasMember(helloResult, new NameKey("x"));
            HasMember(helloResult, new NameKey("y"));

            // things don't flow downstream 
            var methodReturns = methodResult.OutputType.Is1OrThrow().GetValue().CastTo<WeakTypeDefinition>();
            HasCount(0, methodReturns);
        }

        [Fact]
        public void AssignmentXDownStream()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var m1 = x.CreateMember(x.ModuleRoot, new NameKey("m1"), new WeakMemberDefinitionConverter(false, new NameKey("m1")));
            x.CreateHopefulMember(m1, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var m2 = x.CreateMember(x.ModuleRoot, new NameKey("m2"), new WeakMemberDefinitionConverter(false, new NameKey("m2")));
            x.CreateHopefulMember(m2, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));
            var m3 = x.CreateMember(x.ModuleRoot, new NameKey("m3"), new WeakMemberDefinitionConverter(false, new NameKey("m3")));
            var m4 = x.CreateMember(x.ModuleRoot, new NameKey("m4"), new WeakMemberDefinitionConverter(false, new NameKey("m4")));
            var m5 = x.CreateMember(x.ModuleRoot, new NameKey("m5"), new WeakMemberDefinitionConverter(false, new NameKey("m5")));

            m1.AssignTo(m3);
            m2.AssignTo(m3);
            m3.AssignTo(m4);
            m3.AssignTo(m5);

            var solution = x.Solve();

            HasCount(1, MemberToType(solution.GetMember(m1).GetValue()));
            HasCount(1, MemberToType(solution.GetMember(m2).GetValue()));
            HasCount(0, MemberToType(solution.GetMember(m3).GetValue()));
            HasCount(0, MemberToType(solution.GetMember(m4).GetValue()));
            HasCount(0, MemberToType(solution.GetMember(m5).GetValue()));

        }



        [Fact]
        public void AssignmentXUpStream()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var m1 = x.CreateMember(x.ModuleRoot, new NameKey("m1"), new WeakMemberDefinitionConverter(false, new NameKey("m1")));
            var m2 = x.CreateMember(x.ModuleRoot, new NameKey("m2"), new WeakMemberDefinitionConverter(false, new NameKey("m2")));
            var m3 = x.CreateMember(x.ModuleRoot, new NameKey("m3"), new WeakMemberDefinitionConverter(false, new NameKey("m3")));
            var m4 = x.CreateMember(x.ModuleRoot, new NameKey("m4"), new WeakMemberDefinitionConverter(false, new NameKey("m4")));
            x.CreateHopefulMember(m4, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var m5 = x.CreateMember(x.ModuleRoot, new NameKey("m5"), new WeakMemberDefinitionConverter(false, new NameKey("m5")));
            x.CreateHopefulMember(m5, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            m1.AssignTo(m3);
            m2.AssignTo(m3);
            m3.AssignTo(m4);
            m3.AssignTo(m5);

            var solution = x.Solve();

            HasCount(2, MemberToType(solution.GetMember(m1).GetValue()));
            HasCount(2, MemberToType(solution.GetMember(m2).GetValue()));
            HasCount(2, MemberToType(solution.GetMember(m3).GetValue()));
            HasCount(1, MemberToType(solution.GetMember(m4).GetValue()));
            HasCount(1, MemberToType(solution.GetMember(m5).GetValue()));

        }

        [Fact]
        public void AssignmentMutual()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var m1 = x.CreateMember(x.ModuleRoot, new NameKey("m1"), new WeakMemberDefinitionConverter(false, new NameKey("m1")));
            x.CreateHopefulMember(m1, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var m2 = x.CreateMember(x.ModuleRoot, new NameKey("m2"), new WeakMemberDefinitionConverter(false, new NameKey("m2")));
            x.CreateHopefulMember(m2, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            m1.AssignTo(m2);
            m2.AssignTo(m1);

            var solution = x.Solve();

            HasCount(2, MemberToType(solution.GetMember(m1).GetValue()));
            HasCount(2, MemberToType(solution.GetMember(m2).GetValue()));
        }


        [Fact]
        public void Generic()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var pairType = x.CreateGenericType(
                x.ModuleRoot, 
                new NameKey("pair"), 
                new []{ 
                    new Tpn.TypeAndConverter(
                        new NameKey("T"), 
                        new WeakTypeDefinitionConverter())}, 
                new WeakTypeDefinitionConverter());

            x.CreateMember(pairType, new NameKey("x"),
                OrType.Make<IKey, IError>(new NameKey("T")), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var chickenType = x.CreateType(x.ModuleRoot, new NameKey("chicken"), new WeakTypeDefinitionConverter());

            x.CreateMember(chickenType, new NameKey("eggs"), new WeakMemberDefinitionConverter(false, new NameKey("eggs")));

            var chickenPair = x.CreateMember(x.ModuleRoot, new NameKey("x"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var solution = x.Solve();

            var chickenPairResult = solution.GetMember(chickenPair).GetValue();

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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var type = x.CreateGenericType(x.ModuleRoot, new NameKey("node"), new[]{
                    new Tpn.TypeAndConverter(new NameKey("node-t"), new WeakTypeDefinitionConverter())
            }, new WeakTypeDefinitionConverter());

            x.CreateMember(type, new NameKey("next"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("node-t"))
            })), new WeakMemberDefinitionConverter(false, new NameKey("next")));

            x.CreateType(x.ModuleRoot, new NameKey("chicken"), new WeakTypeDefinitionConverter());


            var thing = x.CreateMember(x.ModuleRoot, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("chicken"))
            })), new WeakMemberDefinitionConverter(false, new NameKey("thing")));

            var solution = x.Solve();

            var thingResult = solution.GetMember(thing).GetValue();
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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var left = x.CreateGenericType(x.ModuleRoot, new NameKey("left"), new[]{
                    new Tpn.TypeAndConverter(new NameKey("left-t"), new WeakTypeDefinitionConverter())
            }, new WeakTypeDefinitionConverter());

            x.CreateMember(left, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("right"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("left-t"))
            })), new WeakMemberDefinitionConverter(false, new NameKey("thing")));

            var right = x.CreateGenericType(x.ModuleRoot, new NameKey("right"), new[]{
                    new Tpn.TypeAndConverter(new NameKey("right-t"), new WeakTypeDefinitionConverter())
            }, new WeakTypeDefinitionConverter());

            x.CreateMember(right, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("left"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("right-t"))
            })), new WeakMemberDefinitionConverter(false, new NameKey("thing")));

            x.CreateType(x.ModuleRoot, new NameKey("chicken"), new WeakTypeDefinitionConverter());

            var leftMember = x.CreateMember(x.ModuleRoot, new NameKey("left-member"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("left"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })), new WeakMemberDefinitionConverter(false, new NameKey("left-member")));

            var rightMember = x.CreateMember(x.ModuleRoot, new NameKey("right-member"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("right"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })), new WeakMemberDefinitionConverter(false, new NameKey("right-member")));

            var solution = x.Solve();

            var leftResult = solution.GetMember(leftMember).GetValue();
            var rightResult = solution.GetMember(rightMember).GetValue();

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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var pairType = x.CreateGenericType(x.ModuleRoot, new NameKey("pair"), new[]{
                    new Tpn.TypeAndConverter(new NameKey("T"), new WeakTypeDefinitionConverter())
            }, new WeakTypeDefinitionConverter());

            x.CreateMember(pairType, new NameKey("x"),
                OrType.Make<IKey, IError>(new NameKey("T")), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var chickenType = x.CreateType(x.ModuleRoot, new NameKey("chicken"), new WeakTypeDefinitionConverter());

            x.CreateMember(chickenType, new NameKey("eggs"), new WeakMemberDefinitionConverter(false, new NameKey("eggs")));

            var xMember = x.CreateMember(x.ModuleRoot, new NameKey("x"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })) })), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var solution = x.Solve();

            var xMemberResult = solution.GetMember(xMember).GetValue();
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
