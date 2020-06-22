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
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Model.Instantiated;
using Tac.Frontend.SyntaxModel.Operations;

namespace Tac.Frontend.TypeProblem.Test
{

    // TODO test or types
    public class TestTpn
    {
        #region Help

        //private static void HasCount(int count, IFrontendType result)
        //{
        //    var members = 0;
        //    if (result.SafeIs(out HasMembersType membersType)) {
        //        members =membersType.weakScope.membersList.Count();
        //    }



        //    string error = 0;
        //}

        private static IFrontendType HasMember(IFrontendType result, IKey key)
        {
            return result.TryGetMember(key).Is1OrThrow().Is1OrThrow();
        }

        private static void DoesNotHaveMember(IFrontendType result, IKey key)
        {
            result.TryGetMember(key).Is2OrThrow();
        }

        private static IFrontendType MemberToType(WeakMemberDefinition member)
        {
            return member.Type.Is1OrThrow().GetValue();
        }

        private static void Equal(IFrontendType a, IFrontendType b)
        {
            Assert.True(a.TheyAreUs(b, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow() && b.TheyAreUs(a, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        #endregion


        [Fact]
        public void Simplest()
        {
            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());
            x.Solve();
        }

        [Fact]
        public void AddType()
        {
            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());
            var hello = x.CreateType(x.ModuleRoot, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("Hello")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(hello, hello, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            x.CreatePublicMember(hello, hello, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));
            var solution = x.Solve();

            var resultHello = solution.GetExplicitType(hello).GetValue().Is1OrThrow();

            HasMember(resultHello.FrontendType(), new NameKey("x"));
            HasMember(resultHello.FrontendType(), new NameKey("y"));
        }


        [Fact]
        public void AddMethod()
        {
            // code is something like this
            // type hello {x;y;}
            //
            // hello z;
            //
            // z -> method {
            //      input.x =: x;
            //      input.y =: y;
            //      input return;
            // }



            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var hello = x.CreateType(x.ModuleRoot, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("hello")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(hello, hello, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            x.CreatePublicMember(hello, hello, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            var input = x.CreateValue(x.ModuleRoot.InitizationScope, new NameKey("hello"), new PlaceholderValueConverter());
            var method = x.CreateMethod(x.ModuleRoot, "input", new WeakMethodDefinitionConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), false), new WeakMemberDefinitionConverter(false, new NameKey("input")));

            var input_x = x.CreateHopefulMember(method.Input(), new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var input_y = x.CreateHopefulMember(method.Input(), new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            var method_x = x.CreatePrivateMember(method, method, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var method_y = x.CreatePrivateMember(method, method, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            input_x.AssignTo(method_x);
            input_y.AssignTo(method_y);

            method.Input().AssignTo(method.Returns());

            input.AssignTo(method.Input());

            var result = x.Solve();

            var methodResult = result.GetMethod(method).GetValue().Is1OrThrow();

            var HackToLookAtScope = new HasMembersType(methodResult.Scope.GetValue());

            HasMember(HackToLookAtScope, new NameKey("input"));
            HasMember(HackToLookAtScope, new NameKey("x"));
            HasMember(HackToLookAtScope, new NameKey("y"));
            var inputResult = HasMember(HackToLookAtScope, new NameKey("input"));

            HasMember(inputResult, new NameKey("x"));
            HasMember(inputResult, new NameKey("y"));

            var helloResult = result.GetExplicitType(hello).GetValue().Is1OrThrow().FrontendType();
            HasMember(helloResult, new NameKey("x"));
            HasMember(helloResult, new NameKey("y"));

            // things don't flow downstream 
            var methodReturns = methodResult.OutputType.Is1OrThrow().GetValue();
            DoesNotHaveMember(methodReturns, new NameKey("x"));
            DoesNotHaveMember(methodReturns, new NameKey("y"));
        }

        [Fact]
        public void AssignmentXDownStream()
        {

            // type {x} m1 =: m3
            // type {y} m2 =: m3
            // m3 =: m4
            // m3 =: m5

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var m1 = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m1"), new WeakMemberDefinitionConverter(false, new NameKey("m1")));
            x.CreateHopefulMember(m1, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var m2 = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m2"), new WeakMemberDefinitionConverter(false, new NameKey("m2")));
            x.CreateHopefulMember(m2, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));
            var m3 = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m3"), new WeakMemberDefinitionConverter(false, new NameKey("m3")));
            var m4 = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m4"), new WeakMemberDefinitionConverter(false, new NameKey("m4")));
            var m5 = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m5"), new WeakMemberDefinitionConverter(false, new NameKey("m5")));

            m1.AssignTo(m3);
            m2.AssignTo(m3);
            m3.AssignTo(m4);
            m3.AssignTo(m5);

            var solution = x.Solve();

            var m1t = MemberToType(solution.GetMember(m1).GetValue());
            HasMember(m1t, new NameKey("x"));
            DoesNotHaveMember(m1t, new NameKey("y"));

            var m2t = MemberToType(solution.GetMember(m2).GetValue());
            DoesNotHaveMember(m2t, new NameKey("x"));
            HasMember(m2t, new NameKey("y"));

            var m3t = MemberToType(solution.GetMember(m3).GetValue());
            DoesNotHaveMember(m3t, new NameKey("x"));
            DoesNotHaveMember(m3t, new NameKey("y"));

            var m4t = MemberToType(solution.GetMember(m4).GetValue());
            DoesNotHaveMember(m4t, new NameKey("x"));
            DoesNotHaveMember(m4t, new NameKey("y"));

            var m5t = MemberToType(solution.GetMember(m5).GetValue());
            DoesNotHaveMember(m5t, new NameKey("x"));
            DoesNotHaveMember(m5t, new NameKey("y"));

        }



        [Fact]
        public void AssignmentXUpStream()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var m1 = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m1"), new WeakMemberDefinitionConverter(false, new NameKey("m1")));
            var m2 = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m2"), new WeakMemberDefinitionConverter(false, new NameKey("m2")));
            var m3 = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m3"), new WeakMemberDefinitionConverter(false, new NameKey("m3")));
            var m4 = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m4"), new WeakMemberDefinitionConverter(false, new NameKey("m4")));
            x.CreateHopefulMember(m4, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var m5 = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m5"), new WeakMemberDefinitionConverter(false, new NameKey("m5")));
            x.CreateHopefulMember(m5, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            m1.AssignTo(m3);
            m2.AssignTo(m3);
            m3.AssignTo(m4);
            m3.AssignTo(m5);

            var solution = x.Solve();

            //HasCount(2, MemberToType(solution.GetMember(m1).GetValue()));
            //HasCount(2, MemberToType(solution.GetMember(m2).GetValue()));
            //HasCount(2, MemberToType(solution.GetMember(m3).GetValue()));
            //HasCount(1, MemberToType(solution.GetMember(m4).GetValue()));
            //HasCount(1, MemberToType(solution.GetMember(m5).GetValue()));

            var m1t = MemberToType(solution.GetMember(m1).GetValue());
            HasMember(m1t, new NameKey("x"));
            HasMember(m1t, new NameKey("y"));

            var m2t = MemberToType(solution.GetMember(m2).GetValue());
            HasMember(m2t, new NameKey("x"));
            HasMember(m2t, new NameKey("y"));

            var m3t = MemberToType(solution.GetMember(m3).GetValue());
            HasMember(m3t, new NameKey("x"));
            HasMember(m3t, new NameKey("y"));

            var m4t = MemberToType(solution.GetMember(m4).GetValue());
            HasMember(m4t, new NameKey("x"));
            DoesNotHaveMember(m4t, new NameKey("y"));

            var m5t = MemberToType(solution.GetMember(m5).GetValue());
            DoesNotHaveMember(m5t, new NameKey("x"));
            HasMember(m5t, new NameKey("y"));

        }

        [Fact]
        public void AssignmentMutual()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var m1 = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m1"), new WeakMemberDefinitionConverter(false, new NameKey("m1")));
            x.CreateHopefulMember(m1, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var m2 = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m2"), new WeakMemberDefinitionConverter(false, new NameKey("m2")));
            x.CreateHopefulMember(m2, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            m1.AssignTo(m2);
            m2.AssignTo(m1);

            var solution = x.Solve();

            var m1t = MemberToType(solution.GetMember(m1).GetValue());
            HasMember(m1t, new NameKey("x"));
            HasMember(m1t, new NameKey("y"));

            var m2t = MemberToType(solution.GetMember(m2).GetValue());
            HasMember(m2t, new NameKey("x"));
            HasMember(m2t, new NameKey("y"));

            //HasCount(2, MemberToType(solution.GetMember(m1).GetValue()));
            //HasCount(2, MemberToType(solution.GetMember(m2).GetValue()));
        }


        [Fact]
        public void Generic()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var pairType = x.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("pair")),
                new[]{
                    new Tpn.TypeAndConverter(
                        OrType.Make<NameKey, ImplicitKey>(new NameKey("T")),
                        new WeakTypeDefinitionConverter())},
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.CreatePublicMember(pairType, pairType, new NameKey("x"),
                OrType.Make<IKey, IError>(new NameKey("T")), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var chickenType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            x.CreatePublicMember(chickenType, chickenType, new NameKey("eggs"), new WeakMemberDefinitionConverter(false, new NameKey("eggs")));

            var chickenPair = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("x"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var solution = x.Solve();

            var chickenPairResult = solution.GetMember(chickenPair).GetValue();

            var chickePairResultType = MemberToType(chickenPairResult);

            //HasCount(1, chickePairResultType);
            var xResultType = HasMember(chickePairResultType, new NameKey("x"));
            //var xResultType = MemberToType(xResult);
            //HasCount(1, xResultType);
            HasMember(xResultType, new NameKey("eggs"));

        }

        [Fact]
        public void GenericContainsSelf()
        {

            // type[node-t] node {node[node-t] next}
            // type chicken {}
            // node[chicken] thing;

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var type = x.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("node")),
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("node-t")), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
            );

            x.CreatePublicMember(type, type, new NameKey("next"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("node-t"))
            })), new WeakMemberDefinitionConverter(false, new NameKey("next")));

            x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());


            var thing = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("chicken"))
            })), new WeakMemberDefinitionConverter(false, new NameKey("thing")));

            var solution = x.Solve();

            var thingResult = solution.GetMember(thing).GetValue();
            var thingResultType = MemberToType(thingResult);

            //HasCount(1, thingResultType);
            var nextResult = HasMember(thingResultType, new NameKey("next"));
            //var nextResultType = MemberToType(nextResult);
            //HasCount(1, nextResultType);
            HasMember(nextResult, new NameKey("next"));

            Equal(thingResultType, nextResult);
        }

        [Fact]
        public void GenericContainsSelfWithInferred()
        {

            // type[node-t] node {node[node-t] next}
            // type chicken {}
            // node[chicken] thing;
            // x =: thing

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var type = x.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("node")),
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("node-t")), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
            );

            x.CreatePublicMember(type, type, new NameKey("next"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("node-t"))
            })), new WeakMemberDefinitionConverter(false, new NameKey("next")));

            x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            var thing = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("chicken"))
            })), new WeakMemberDefinitionConverter(false, new NameKey("thing")));

            var xMember = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("m4")));

            // this assignment is an important part of the test
            // we want to test that members flow as they should
            x.IsAssignedTo(xMember, thing);

            var solution = x.Solve();

            var thingResult = solution.GetMember(thing).GetValue();
            var thingResultType = MemberToType(thingResult);

            var nextResult = HasMember(thingResultType, new NameKey("next"));
            HasMember(nextResult, new NameKey("next"));

            Equal(thingResultType, nextResult);

            var xMemberResult = MemberToType(solution.GetMember(xMember).GetValue());
            for (int i = 0; i < 100; i++)
            {
                xMemberResult = HasMember(xMemberResult, new NameKey("next"));
            }
        }


        [Fact]
        public void GenericCircular()
        {
            // type[left-t] left {  right[left-t] thing }
            // type[right-t] right {  left[right-t] thing }
            // type chicken {}
            // left[chicken] left-member;
            // right[chicken] right-member

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var left = x.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("left")),
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("left-t")), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...? {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.CreatePublicMember(left, left, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("right"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("left-t"))
            })), new WeakMemberDefinitionConverter(false, new NameKey("thing")));

            var right = x.CreateGenericType(
                x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("right")),
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("right-t")), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...? {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.CreatePublicMember(right, right, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("left"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("right-t"))
            })), new WeakMemberDefinitionConverter(false, new NameKey("thing")));

            x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            var leftMember = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("left-member"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("left"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })), new WeakMemberDefinitionConverter(false, new NameKey("left-member")));

            var rightMember = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("right-member"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("right"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })), new WeakMemberDefinitionConverter(false, new NameKey("right-member")));

            // this assignment is an important part of the test
            x.IsAssignedTo(leftMember, rightMember);
            x.IsAssignedTo(rightMember, leftMember);

            var solution = x.Solve();

            var leftResult = solution.GetMember(leftMember).GetValue();
            var rightResult = solution.GetMember(rightMember).GetValue();

            var leftResultType = MemberToType(leftResult);
            var rightResultType = MemberToType(rightResult);

            //HasCount(1, leftResultType);
            //HasCount(1, rightResultType);

            var rightThingType = HasMember(leftResultType, new NameKey("thing"));
            var leftThingType = HasMember(rightResultType, new NameKey("thing"));

            //var leftThingType = MemberToType(leftThing);
            //var rightThingType = MemberToType(rightThing);

            Equal(leftResultType, rightThingType);
            Equal(rightResultType, leftThingType);
        }


        [Fact]
        public void NestedGeneric()
        {

            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var pairType = x.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("pair")),
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("T")), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.CreatePublicMember(pairType, pairType, new NameKey("x"),
                OrType.Make<IKey, IError>(new NameKey("T")), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var chickenType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            x.CreatePublicMember(chickenType, chickenType, new NameKey("eggs"), new WeakMemberDefinitionConverter(false, new NameKey("eggs")));

            var xMember = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("x"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })) })), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var solution = x.Solve();

            var xMemberResult = solution.GetMember(xMember).GetValue();
            var xMemberResultType = MemberToType(xMemberResult);
            //HasCount(1, xMemberResultType);
            var xMemberResultX = HasMember(xMemberResultType, new NameKey("x"));
            //var xMemberResultXType = MemberToType(xMemberResultX);
            //HasCount(1, xMemberResultXType);
            var xMemberResultXTypeX = HasMember(xMemberResultX, new NameKey("x"));
            //var xMemberResultXTypeXType = MemberToType(xMemberResultXTypeX);
            //HasCount(1, xMemberResultXTypeXType);
            HasMember(xMemberResultXTypeX, new NameKey("eggs"));
        }

        // type A {x;y}
        // type B {x;z}
        // type C {number x}
        // A | B ab;
        // ab =: C c;
        // C flows in to A and B

        [Fact]
        public void FlowInToOrType()
        {
            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());


            var aType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(aType, aType, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            x.CreatePublicMember(aType, aType, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            var bType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(bType, bType, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            x.CreatePublicMember(bType, bType, new NameKey("z"), new WeakMemberDefinitionConverter(false, new NameKey("z")));

            var cType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("C")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(cType, cType, new NameKey("x"), OrType.Make<IKey, IError>(new NameKey("number")), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.CreateOrType(x.ModuleRoot, key,
                 OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("B"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var ab = x.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("ab"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType),
                new WeakMemberDefinitionConverter(false, new NameKey("ab")));

            var c = x.CreatePublicMember(x.ModuleRoot, new NameKey("c"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(cType),
                new WeakMemberDefinitionConverter(false, new NameKey("c")));

            x.IsAssignedTo(ab, c);

            var solution = x.Solve();

            var aTypeSolution = solution.GetExplicitType(aType).GetValue().Is1OrThrow();
            var aMember = HasMember(aTypeSolution.FrontendType(), new NameKey("x"));
            Assert.True(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType().TheyAreUs(aMember, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            var bTypeSolution = solution.GetExplicitType(aType).GetValue().Is1OrThrow();
            var bMember = HasMember(bTypeSolution.FrontendType(), new NameKey("x"));
            Assert.True(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType().TheyAreUs(bMember, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

        }

        // type A {x;y}
        // type B {x;z}
        // c =: A | B ab;
        // intersect A and B flows in to c
        // c should have x

        [Fact]
        public void FlowFromOrType()
        {
            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());


            var aType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(aType, aType, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            x.CreatePublicMember(aType, aType, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            var bType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(bType, bType, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            x.CreatePublicMember(bType, bType, new NameKey("z"), new WeakMemberDefinitionConverter(false, new NameKey("z")));

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.CreateOrType(x.ModuleRoot, key,
                 OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("B"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var ab = x.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("ab"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType),
                new WeakMemberDefinitionConverter(false, new NameKey("ab")));

            var c = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"),
                new WeakMemberDefinitionConverter(false, new NameKey("c")));

            x.IsAssignedTo(c, ab);

            var solution = x.Solve();

            var member = solution.GetMember(c).GetValue();

            HasMember(member.Type.Is1OrThrow().GetValue(), new NameKey("x"));
        }

        // 5 =: c
        // c =: number | string b
        // c should be number | string
        [Fact]
        public void ImplictTypeIntheMiddle()
        {

            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.CreateOrType(x.ModuleRoot, key,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("number"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("string"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var b = x.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType),
                new WeakMemberDefinitionConverter(false, new NameKey("b")));

            var c = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"), new WeakMemberDefinitionConverter(false, new NameKey("c")));

            var five = x.CreateValue(x.ModuleRoot.InitizationScope, new NameKey("number"), new PlaceholderValueConverter());


            x.IsAssignedTo(five, c);
            x.IsAssignedTo(c, b);

            var solution = x.Solve();

            var cType = solution.GetMember(c).GetValue().Type.Is1OrThrow().GetValue();

            var cOrType = Assert.IsType<FrontEndOrType>(cType);

            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.NumberType>(cOrType.left.Is1OrThrow());
            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.StringType>(cOrType.right.Is1OrThrow());
        }

        // 5 =: c
        // c =: number | string b
        // c =: number d
        // c should be number
        [Fact]
        public void ImplictTypeIntheMiddleTwoInFlows()
        {
            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.CreateOrType(x.ModuleRoot, key,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("number"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("string"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var b = x.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType),
                new WeakMemberDefinitionConverter(false, new NameKey("b")));

            var c = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"), new WeakMemberDefinitionConverter(false, new NameKey("c")));

            var d = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("d"), OrType.Make<IKey, IError>(new NameKey("number")), new WeakMemberDefinitionConverter(false, new NameKey("d")));

            var five = x.CreateValue(x.ModuleRoot.InitizationScope, new NameKey("number"), new PlaceholderValueConverter());

            x.IsAssignedTo(five, c);
            x.IsAssignedTo(c, b);
            x.IsAssignedTo(c, d);

            var solution = x.Solve();

            var cType = solution.GetMember(c).GetValue().Type.Is1OrThrow().GetValue();

            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.NumberType>(cType);

        }


        // Type A { x; y; }
        // Type B { x; z; }
        // Type C { z; w; }
        // Type D { y; w; }
        // c =: A | C a
        // c =: B | D b
        // so c is...
        // A&B | A&D | C&B | C&D
        // {x;y;z;} | {x;y;w;} | {z;w;x;} | {z;w;y;}

        [Fact]
        public void TwoOrTypesFlowIn()
        {

            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var aType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(aType, aType, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            x.CreatePublicMember(aType, aType, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            var bType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(bType, bType, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            x.CreatePublicMember(bType, bType, new NameKey("z"), new WeakMemberDefinitionConverter(false, new NameKey("z")));

            var cType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("C")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(cType, cType, new NameKey("z"), new WeakMemberDefinitionConverter(false, new NameKey("z")));
            x.CreatePublicMember(cType, cType, new NameKey("w"), new WeakMemberDefinitionConverter(false, new NameKey("w")));

            var dType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("D")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(dType, dType, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));
            x.CreatePublicMember(dType, dType, new NameKey("w"), new WeakMemberDefinitionConverter(false, new NameKey("w")));

            var key1 = new ImplicitKey(Guid.NewGuid());
            var orType1 = x.CreateOrType(x.ModuleRoot, key1,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("C"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var key2 = new ImplicitKey(Guid.NewGuid());
            var orType2 = x.CreateOrType(x.ModuleRoot, key2,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("B"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("D"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var a = x.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("a"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType1),
                new WeakMemberDefinitionConverter(false, new NameKey("a")));

            var b = x.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType2),
                new WeakMemberDefinitionConverter(false, new NameKey("b")));

            var c = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"), new WeakMemberDefinitionConverter(false, new NameKey("c")));

            x.IsAssignedTo(c, a);
            x.IsAssignedTo(c, b);

            var solution = x.Solve();

            var cTypeResult = solution.GetMember(c).GetValue().Type.Is1OrThrow().GetValue();

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<IBox<WeakMemberDefinition>> {
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("x"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("y"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("z"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))))
                })), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<IBox<WeakMemberDefinition>> {
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("x"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("y"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("w"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))))
                })),
                new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<IBox<WeakMemberDefinition>> {
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("z"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("w"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("x"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))))
                }
                )),
                new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<IBox<WeakMemberDefinition>> {
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("z"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("w"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("y"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))))
                }
                )),
                new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.False(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<IBox<WeakMemberDefinition>> { }
                )), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

        }

        // Type A { x; y; }
        // Type B { x; z; }
        // Type C { z; w; }
        // c =: A | C a
        // c =: B b
        // so c is...
        // A&B | C&B
        // {x;y;z;} | {w;x;z; } 
        [Fact]
        public void OrTypeFlowsIn()
        {

            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var aType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(aType, aType, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            x.CreatePublicMember(aType, aType, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            var bType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(bType, bType, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            x.CreatePublicMember(bType, bType, new NameKey("z"), new WeakMemberDefinitionConverter(false, new NameKey("z")));

            var cType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("C")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(cType, cType, new NameKey("z"), new WeakMemberDefinitionConverter(false, new NameKey("z")));
            x.CreatePublicMember(cType, cType, new NameKey("w"), new WeakMemberDefinitionConverter(false, new NameKey("w")));

            var key1 = new ImplicitKey(Guid.NewGuid());
            var orType1 = x.CreateOrType(x.ModuleRoot, key1,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("C"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());


            var a = x.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("a"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType1),
                new WeakMemberDefinitionConverter(false, new NameKey("a")));

            var b = x.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(bType),
                new WeakMemberDefinitionConverter(false, new NameKey("b")));

            var c = x.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"), new WeakMemberDefinitionConverter(false, new NameKey("c")));

            x.IsAssignedTo(c, a);
            x.IsAssignedTo(c, b);

            var solution = x.Solve();

            var cTypeResult = solution.GetMember(c).GetValue().Type.Is1OrThrow().GetValue();

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<IBox<WeakMemberDefinition>> {
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("x"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("y"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("z"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))))
                })),
                new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<IBox<WeakMemberDefinition>> {
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("w"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("x"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,new NameKey("z"),OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))))
                })),
                new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.False(cTypeResult.TheyAreUs(
                new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { })), 
                new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

        }

        // type A {x}
        // type B { number x}
        // A | number a
        // a =: B b
        // x should not be a number  
        // even if x was a number that would not make this a legal assignment 
        [Fact]
        public void FlowInToOneSideOfOrType()
        {
            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var aType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(aType, aType, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var bType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(bType, bType, new NameKey("x"), OrType.Make<IKey, IError>(new NameKey("number")), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.CreateOrType(x.ModuleRoot, key,
                 OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.CreateTypeReference(x.ModuleRoot, new NameKey("number"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var a = x.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("a"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType),
                new WeakMemberDefinitionConverter(false, new NameKey("a")));

            var b = x.CreatePublicMember(x.ModuleRoot, new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(bType),
                new WeakMemberDefinitionConverter(false, new NameKey("b")));

            x.IsAssignedTo(a, b);

            var solution = x.Solve();

            var aTypeSolution = solution.GetExplicitType(aType).GetValue().Is1OrThrow();
            var aMember = HasMember(aTypeSolution.FrontendType(), new NameKey("x"));
            Assert.False(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType().TheyAreUs(aMember, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }




        // Type X {
        //     Y | X member;
        // }
        // Type Y {
        //    Y member;
        // }
        // a =: X x
        // do these colapse?

        [Fact]
        public void Complex()
        {

            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var yType = problem.CreateType(
                problem.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("Y")),
                new WeakTypeDefinitionConverter()
            );
            problem.CreatePublicMember(
                yType,
                new NameKey("member"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(yType),
                new WeakMemberDefinitionConverter(false, new NameKey("member")));

            var xType = problem.CreateType(
                problem.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("X")),
                new WeakTypeDefinitionConverter()
            );

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = problem.CreateOrType(problem.ModuleRoot, key,
                 OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(problem.CreateTypeReference(problem.ModuleRoot, new NameKey("X"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(problem.CreateTypeReference(problem.ModuleRoot, new NameKey("Y"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            problem.CreatePublicMember(
                xType,
                new NameKey("member"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType),
                new WeakMemberDefinitionConverter(false, new NameKey("member")));

            var a = problem.CreatePublicMember(
                    problem.ModuleRoot,
                    problem.ModuleRoot,
                    new NameKey("a"),
                    new WeakMemberDefinitionConverter(false, new NameKey("a")));

            var x = problem.CreatePublicMember(
                    problem.ModuleRoot,
                    problem.ModuleRoot,
                    new NameKey("x"),
                    OrType.Make<IKey, IError>(new NameKey("X")),
                    new WeakMemberDefinitionConverter(false, new NameKey("x")));

            problem.IsAssignedTo(a, x);

            var solution = problem.Solve();

            // mostly we just want this not to throw 
        }

        // () > a =: number d
        // a is a method and it returns a number
        [Fact]
        public void InferredMethod() {

            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var a = problem.CreatePublicMember(
                    problem.ModuleRoot,
                    problem.ModuleRoot,
                    new NameKey("a"),
                    new WeakMemberDefinitionConverter(false, new NameKey("a")));

            var returns = problem.GetReturns(a);

            var d = problem.CreatePublicMember(problem.ModuleRoot, problem.ModuleRoot, new NameKey("d"), OrType.Make<IKey, IError>(new NameKey("number")), new WeakMemberDefinitionConverter(false, new NameKey("d")));

            problem.IsAssignedTo(returns, d);

            var solution = problem.Solve();

            var flowNode = solution.GetFlowNode2(a);
            var res = solution.GetType(flowNode);
            var method = Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.MethodType>(res.Is1OrThrow().GetValue());
            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.NumberType>(method.OutputType.Is1OrThrow());
        }


        // type N { number x; }
        // type B { bool x; }
        // a =: N n
        // c =: a
        // a =: B b
        
        // what is c.x ??
        [Fact]
        public void UnmergableInferredType (){

            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")), new WeakScopeConverter());

            var nType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("N")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(nType, nType, new NameKey("x"), OrType.Make<IKey, IError>(new NameKey("number")), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var bType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(bType, bType, new NameKey("x"), OrType.Make<IKey, IError>(new NameKey("bool")), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var a = x.CreatePublicMember(
                    x.ModuleRoot,
                    x.ModuleRoot,
                    new NameKey("a"),
                    new WeakMemberDefinitionConverter(false, new NameKey("a")));

            var c = x.CreatePublicMember(
                x.ModuleRoot,
                x.ModuleRoot,
                new NameKey("c"),
                new WeakMemberDefinitionConverter(false, new NameKey("c")));

            var b = x.CreatePublicMember(
                    x.ModuleRoot,
                    new NameKey("b"),
                    OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(bType),
                    new WeakMemberDefinitionConverter(false, new NameKey("b")));

            var n = x.CreatePublicMember(
                    x.ModuleRoot,
                    new NameKey("n"),
                    OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(nType),
                    new WeakMemberDefinitionConverter(false, new NameKey("n")));

            x.IsAssignedTo(a, n);
            x.IsAssignedTo(c, a);
            x.IsAssignedTo(a, b);

            var solution = x.Solve();

            var cFlowNode = solution.GetFlowNode2(c);
            var cType = solution.GetType(cFlowNode);
        }
    }
}
