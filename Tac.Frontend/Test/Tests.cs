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
using Tac.Model.Elements;
using Tac.Frontend._3_Syntax_Model.Elements;
using Tac.SemanticModel.Operations;

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
            return result.TryGetMember(key, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow().Is1OrThrow().Item1;
        }

        private static void DoesNotHaveMember(IFrontendType result, IKey key)
        {
            result.TryGetMember(key, new List<(IFrontendType, IFrontendType)>()).Is2OrThrow();
        }

        private static IFrontendType MemberToType(WeakMemberDefinition member)
        {
            return member.Type.GetValue().Is1OrThrow();
        }

        private static void Equal(IFrontendType a, IFrontendType b)
        {
            Assert.True(a.TheyAreUs(b, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow() && b.TheyAreUs(a, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        private static RootScopePopulateScope DefaultRootScopePopulateScope() => new RootScopePopulateScope(
                Array.Empty<IOrType<WeakAssignOperationPopulateScope, IError>>(),
                OrType.Make<EntryPointDefinitionPopulateScope, IError>(
                    new EntryPointDefinitionPopulateScope (
                        new TypeReferancePopulateScope(new NameKey("empty")),
                        Array.Empty<IOrType<EntryPointDefinitionPopulateScope, IError>>(),
                        new TypeReferancePopulateScope(new NameKey("empty")), 
                        "unused")),
                Array.Empty<IOrType<TypeDefinitionPopulateScope, IError>>(),
                Array.Empty<IOrType<GenericTypeDefinitionPopulateScope, IError>>()
                );
        

        #endregion


        [Fact]
        public void Simplest()
        {
            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope());
            x.Solve();
        }

        [Fact]
        public void AddType()
        {
            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope());
            var hello = x.builder.CreateType(x.ModuleRoot, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("Hello")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(hello, hello, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            x.builder.CreatePublicMember(hello, hello, new NameKey("y"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("y")));
            var solution = x.Solve();

            var resultHello = solution.GetExplicitType(hello).GetValue().Is1OrThrow();

            HasMember(resultHello.FrontendType().Is1OrThrow(), new NameKey("x"));
            HasMember(resultHello.FrontendType().Is1OrThrow(), new NameKey("y"));
        }


        [Fact]
        public void AddMethod()
        {
            // code is something like this
            // type hello {x;y;}
            //
            // hello z > method {
            //      input.x =: x;
            //      input.y =: y;
            //      input return;
            // }



            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope());

            var hello = x.builder.CreateType(x.ModuleRoot, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("hello")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(hello, hello, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            x.builder.CreatePublicMember(hello, hello, new NameKey("y"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("y")));

            var input = x.builder.CreateValue(x.ModuleRoot.InitizationScope, new NameKey("hello"), new PlaceholderValueConverter());
            var method = x.builder.CreateMethod(x.ModuleRoot, "input", new WeakMethodDefinitionConverter(new Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>>(new List<IOrType<IBox<IFrontendCodeElement>, IError>>())), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("input")));

            var input_x = x.builder.CreateHopefulMember(method.Input(), new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            var input_y = x.builder.CreateHopefulMember(method.Input(), new NameKey("y"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("y")));

            var method_x = x.builder.CreatePrivateMember(method, method, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            var method_y = x.builder.CreatePrivateMember(method, method, new NameKey("y"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("y")));

            input_x.AssignTo(method_x);
            input_y.AssignTo(method_y);

            method.Input().AssignTo(method.Returns());

            input.AssignTo(method.Input());

            var result = x.Solve();

            var methodResult = result.GetMethod(method).GetValue().Is1OrThrow();

            var HackToLookAtScope = new HasMembersType(methodResult.Scope.Is1OrThrow().GetValue());

            HasMember(HackToLookAtScope, new NameKey("input"));
            HasMember(HackToLookAtScope, new NameKey("x"));
            HasMember(HackToLookAtScope, new NameKey("y"));
            var inputResult = HasMember(HackToLookAtScope, new NameKey("input"));

            HasMember(inputResult, new NameKey("x"));
            HasMember(inputResult, new NameKey("y"));

            var helloResult = result.GetExplicitType(hello).GetValue().Is1OrThrow().FrontendType();
            HasMember(helloResult.Is1OrThrow(), new NameKey("x"));
            HasMember(helloResult.Is1OrThrow(), new NameKey("y"));

            // things don't flow downstream 
            var methodReturns = methodResult.OutputType.GetValue().Is1OrThrow();
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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope());

            var m1 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m1"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m1")));
            x.builder.CreateHopefulMember(m1, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            var m2 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m2"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m2")));
            x.builder.CreateHopefulMember(m2, new NameKey("y"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("y")));
            var m3 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m3"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m3")));
            var m4 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m4"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m4")));
            var m5 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m5"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m5")));

            m1.AssignTo(m3);
            m2.AssignTo(m3);
            m3.AssignTo(m4);
            m3.AssignTo(m5);

            var solution = x.Solve();

            var m1t = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("m1")).Is1OrThrow());
            HasMember(m1t, new NameKey("x"));
            DoesNotHaveMember(m1t, new NameKey("y"));

            var m2t = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("m2")).Is1OrThrow());
            DoesNotHaveMember(m2t, new NameKey("x"));
            HasMember(m2t, new NameKey("y"));

            var m3t = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("m3")).Is1OrThrow());
            DoesNotHaveMember(m3t, new NameKey("x"));
            DoesNotHaveMember(m3t, new NameKey("y"));

            var m4t = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("m4")).Is1OrThrow());
            DoesNotHaveMember(m4t, new NameKey("x"));
            DoesNotHaveMember(m4t, new NameKey("y"));

            var m5t = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("m5")).Is1OrThrow());
            DoesNotHaveMember(m5t, new NameKey("x"));
            DoesNotHaveMember(m5t, new NameKey("y"));

        }



        [Fact]
        public void AssignmentXUpStream()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope());

            var m1 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m1"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m1")));
            var m2 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m2"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m2")));
            var m3 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m3"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m3")));
            var m4 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m4"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m4")));
            x.builder.CreateHopefulMember(m4, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            var m5 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m5"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m5")));
            x.builder.CreateHopefulMember(m5, new NameKey("y"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("y")));

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

            var m1t = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("m1")).Is1OrThrow());
            HasMember(m1t, new NameKey("x"));
            HasMember(m1t, new NameKey("y"));

            var m2t = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("m2")).Is1OrThrow());
            HasMember(m2t, new NameKey("x"));
            HasMember(m2t, new NameKey("y"));

            var m3t = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("m3")).Is1OrThrow());
            HasMember(m3t, new NameKey("x"));
            HasMember(m3t, new NameKey("y"));

            var m4t = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("m4")).Is1OrThrow());
            HasMember(m4t, new NameKey("x"));
            DoesNotHaveMember(m4t, new NameKey("y"));

            var m5t = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("m5")).Is1OrThrow());
            DoesNotHaveMember(m5t, new NameKey("x"));
            HasMember(m5t, new NameKey("y"));

        }

        //
        // m1.x
        // m2.y
        // m1 =: m2
        // m2 =: m1
        [Fact]
        public void AssignmentMutual()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope());

            var m1 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m1"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m1")));
            x.builder.CreateHopefulMember(m1, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            var m2 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m2"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m2")));
            x.builder.CreateHopefulMember(m2, new NameKey("y"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("y")));

            m1.AssignTo(m2);
            m2.AssignTo(m1);

            var solution = x.Solve();

            var m1t = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("m1")).Is1OrThrow());
            HasMember(m1t, new NameKey("x"));
            HasMember(m1t, new NameKey("y"));

            var m2t = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("m2")).Is1OrThrow());
            HasMember(m2t, new NameKey("x"));
            HasMember(m2t, new NameKey("y"));

            //HasCount(2, MemberToType(solution.GetMember(m1).GetValue()));
            //HasCount(2, MemberToType(solution.GetMember(m2).GetValue()));
        }

        [Fact]
        public void HopefulOnHopeful()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope());

            var m1 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m1"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m1")));
            var x1 = x.builder.CreateHopefulMember(m1, new NameKey("x1"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x1")));
            var x2 = x.builder.CreateHopefulMember(x1, new NameKey("x2"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x2")));
            x.builder.CreateHopefulMember(x2, new NameKey("x3"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x3")));

            var m2 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m2"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m2")));

            m2.AssignTo(m1);

            var solution = x.Solve();


            var m2t = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("m2")).Is1OrThrow());
            var x1t = HasMember(m2t, new NameKey("x1"));
            var x2t = HasMember(x1t, new NameKey("x2"));
            HasMember(x2t, new NameKey("x3"));
        }

        [Fact]
        public void Generic()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope());

            var pairType = x.builder.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("pair")),
                new[]{
                    new Tpn.TypeAndConverter(
                        OrType.Make<NameKey, ImplicitKey>(new NameKey("T")),
                        new WeakTypeDefinitionConverter())},
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.builder.CreatePublicMember(pairType, pairType, new NameKey("x"),
                OrType.Make<IKey, IError>(new NameKey("T")), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));

            var chickenType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            x.builder.CreatePublicMember(chickenType, chickenType, new NameKey("eggs"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("eggs")));

            var chickenPair = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("x"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));

            var solution = x.Solve();

            var chickenPairResult = solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("x")).Is1OrThrow();

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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(),
                DefaultRootScopePopulateScope());

            var type = x.builder.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("node")),
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("node-t")), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
            );

            x.builder.CreatePublicMember(type, type, new NameKey("next"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("node-t"))
            })), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("next")));

            x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());


            var thing = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("chicken"))
            })), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("thing")));

            var solution = x.Solve();

            var thingResult = solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("thing")).Is1OrThrow();
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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope());

            var type = x.builder.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("node")),
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("node-t")), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
            );

            x.builder.CreatePublicMember(type, type, new NameKey("next"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("node-t"))
            })), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("next")));

            x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            var thing = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("chicken"))
            })), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("thing")));

            var xMember = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("m4")));

            // this assignment is an important part of the test
            // we want to test that members flow as they should
            x.builder.IsAssignedTo(xMember, thing);

            var solution = x.Solve();

            var thingResult = solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("thing")).Is1OrThrow();
            var thingResultType = MemberToType(thingResult);

            var nextResult = HasMember(thingResultType, new NameKey("next"));
            HasMember(nextResult, new NameKey("next"));

            Equal(thingResultType, nextResult);

            var xMemberResult = MemberToType(solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("x")).Is1OrThrow());
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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope());

            var left = x.builder.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("left")),
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("left-t")), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...? {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.builder.CreatePublicMember(left, left, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("right"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("left-t"))
            })), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("thing")));

            var right = x.builder.CreateGenericType(
                x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("right")),
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("right-t")), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...? {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.builder.CreatePublicMember(right, right, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("left"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("right-t"))
            })), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("thing")));

            x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            var leftMember = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("left-member"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("left"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("left-member")));

            var rightMember = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("right-member"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("right"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("right-member")));

            // this assignment is an important part of the test
            x.builder.IsAssignedTo(leftMember, rightMember);
            x.builder.IsAssignedTo(rightMember, leftMember);

            var solution = x.Solve();

            var leftResult = solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("left-member")).Is1OrThrow();
            var rightResult = solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("right-member")).Is1OrThrow();

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
                DefaultRootScopePopulateScope());

            var pairType = x.builder.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("pair")),
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("T")), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.builder.CreatePublicMember(pairType, pairType, new NameKey("x"),
                OrType.Make<IKey, IError>(new NameKey("T")), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));

            var chickenType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            x.builder.CreatePublicMember(chickenType, chickenType, new NameKey("eggs"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("eggs")));

            var xMember = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("x"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })) })), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));

            var solution = x.Solve();

            var xMemberResult = solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("x")).Is1OrThrow();
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
                DefaultRootScopePopulateScope());


            var aType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(aType, aType, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            x.builder.CreatePublicMember(aType, aType, new NameKey("y"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("y")));

            var bType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(bType, bType, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            x.builder.CreatePublicMember(bType, bType, new NameKey("z"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("z")));

            var cType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("C")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(cType, cType, new NameKey("x"), OrType.Make<IKey, IError>(new NameKey("number")), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.builder.CreateOrType(x.ModuleRoot, key,
                 OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("B"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var ab = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("ab"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("ab")));

            var c = x.builder.CreatePublicMember(x.ModuleRoot, new NameKey("c"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(cType),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("c")));

            x.builder.IsAssignedTo(ab, c);

            var solution = x.Solve();

            var aTypeSolution = solution.GetExplicitType(aType).GetValue().Is1OrThrow();
            var aMember = HasMember(aTypeSolution.FrontendType().Is1OrThrow(), new NameKey("x"));
            Assert.True(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType().TheyAreUs(aMember, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            var bTypeSolution = solution.GetExplicitType(aType).GetValue().Is1OrThrow();
            var bMember = HasMember(bTypeSolution.FrontendType().Is1OrThrow(), new NameKey("x"));
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
                DefaultRootScopePopulateScope());


            var aType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(aType, aType, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            x.builder.CreatePublicMember(aType, aType, new NameKey("y"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("y")));

            var bType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(bType, bType, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            x.builder.CreatePublicMember(bType, bType, new NameKey("z"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("z")));

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.builder.CreateOrType(x.ModuleRoot, key,
                 OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("B"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var ab = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("ab"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("ab")));

            var c = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("c")));

            x.builder.IsAssignedTo(c, ab);

            var solution = x.Solve();

            var member = solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("c")).Is1OrThrow();

            HasMember(member.Type.GetValue().Is1OrThrow(), new NameKey("x"));
        }

        // 5 =: c
        // c =: number | string b
        // c should be number | string
        [Fact]
        public void ImplictTypeIntheMiddle()
        {

            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                DefaultRootScopePopulateScope());

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.builder.CreateOrType(x.ModuleRoot, key,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("number"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("string"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var b = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("b")));

            var c = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("c")));

            var five = x.builder.CreateValue(x.ModuleRoot.InitizationScope, new NameKey("number"), new PlaceholderValueConverter());


            x.builder.IsAssignedTo(five, c);
            x.builder.IsAssignedTo(c, b);

            var solution = x.Solve();

            var cType = solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("c")).Is1OrThrow().Type.GetValue().Is1OrThrow();

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
                DefaultRootScopePopulateScope());

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.builder.CreateOrType(x.ModuleRoot, key,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("number"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("string"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var b = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("b")));

            var c = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("c")));

            var d = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("d"), OrType.Make<IKey, IError>(new NameKey("number")), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("d")));

            var five = x.builder.CreateValue(x.ModuleRoot.InitizationScope, new NameKey("number"), new PlaceholderValueConverter());

            x.builder.IsAssignedTo(five, c);
            x.builder.IsAssignedTo(c, b);
            x.builder.IsAssignedTo(c, d);

            var solution = x.Solve();

            var cType = solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("c")).Is1OrThrow().Type.GetValue().Is1OrThrow();

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
                DefaultRootScopePopulateScope());

            var aType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(aType, aType, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            x.builder.CreatePublicMember(aType, aType, new NameKey("y"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("y")));

            var bType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(bType, bType, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            x.builder.CreatePublicMember(bType, bType, new NameKey("z"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("z")));

            var cType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("C")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(cType, cType, new NameKey("z"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("z")));
            x.builder.CreatePublicMember(cType, cType, new NameKey("w"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("w")));

            var dType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("D")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(dType, dType, new NameKey("y"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("y")));
            x.builder.CreatePublicMember(dType, dType, new NameKey("w"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("w")));

            var key1 = new ImplicitKey(Guid.NewGuid());
            var orType1 = x.builder.CreateOrType(x.ModuleRoot, key1,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("C"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var key2 = new ImplicitKey(Guid.NewGuid());
            var orType2 = x.builder.CreateOrType(x.ModuleRoot, key2,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("B"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("D"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var a = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("a"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType1),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("a")));

            var b = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType2),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("b")));

            var c = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("c")));

            x.builder.IsAssignedTo(c, a);
            x.builder.IsAssignedTo(c, b);

            var solution = x.Solve();

            var cTypeResult = solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("c")).Is1OrThrow().Type.GetValue().Is1OrThrow();

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<IBox<WeakMemberDefinition>> {
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("x"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("y"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("z"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))))
                })), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<IBox<WeakMemberDefinition>> {
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("x"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("y"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("w"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))))
                })),
                new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<IBox<WeakMemberDefinition>> {
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("z"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("w"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("x"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))))
                }
                )),
                new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<IBox<WeakMemberDefinition>> {
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("z"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("w"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("y"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))))
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
                DefaultRootScopePopulateScope());

            var aType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(aType, aType, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            x.builder.CreatePublicMember(aType, aType, new NameKey("y"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("y")));

            var bType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(bType, bType, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));
            x.builder.CreatePublicMember(bType, bType, new NameKey("z"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("z")));

            var cType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("C")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(cType, cType, new NameKey("z"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("z")));
            x.builder.CreatePublicMember(cType, cType, new NameKey("w"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("w")));

            var key1 = new ImplicitKey(Guid.NewGuid());
            var orType1 = x.builder.CreateOrType(x.ModuleRoot, key1,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("C"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());


            var a = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("a"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType1),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("a")));

            var b = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(bType),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("b")));

            var c = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("c")));

            x.builder.IsAssignedTo(c, a);
            x.builder.IsAssignedTo(c, b);

            var solution = x.Solve();

            var cTypeResult = solution.GetMember(solution.GetFlowNode(x.ModuleRoot), new NameKey("c")).Is1OrThrow().Type.GetValue().Is1OrThrow();

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<IBox<WeakMemberDefinition>> {
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("x"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("y"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("z"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))))
                })),
                new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<IBox<WeakMemberDefinition>> {
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("w"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("x"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))),
                    new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite,new NameKey("z"),new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))))
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
        // x should be a number  
        // for this to be a legal assignement x has to be a number
        [Fact]
        public void FlowInToOneSideOfOrType()
        {
            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                DefaultRootScopePopulateScope());

            var aType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(aType, aType, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));

            var bType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(bType, bType, new NameKey("x"), OrType.Make<IKey, IError>(new NameKey("number")), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.builder.CreateOrType(x.ModuleRoot, key,
                 OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("number"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var a = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("a"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("a")));

            var b = x.builder.CreatePublicMember(x.ModuleRoot, new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(bType),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("b")));

            x.builder.IsAssignedTo(a, b);

            var solution = x.Solve();

            var aTypeSolution = solution.GetExplicitType(aType).GetValue().Is1OrThrow();
            var aMember = HasMember(aTypeSolution.FrontendType().Is1OrThrow(), new NameKey("x"));
            Assert.True(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType().TheyAreUs(aMember, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
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

            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope());

            var yType = problem.builder.CreateType(
                problem.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("Y")),
                new WeakTypeDefinitionConverter()
            );
            problem.builder.CreatePublicMember(
                yType,
                new NameKey("member"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(yType),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("member")));

            var xType = problem.builder.CreateType(
                problem.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("X")),
                new WeakTypeDefinitionConverter()
            );

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = problem.builder.CreateOrType(problem.ModuleRoot, key,
                 OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(problem.builder.CreateTypeReference(problem.ModuleRoot, new NameKey("X"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(problem.builder.CreateTypeReference(problem.ModuleRoot, new NameKey("Y"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            problem.builder.CreatePublicMember(
                xType,
                new NameKey("member"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(orType),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("member")));

            var a = problem.builder.CreatePublicMember(
                    problem.ModuleRoot,
                    problem.ModuleRoot,
                    new NameKey("a"),
                    new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("a")));

            var x = problem.builder.CreatePublicMember(
                    problem.ModuleRoot,
                    problem.ModuleRoot,
                    new NameKey("x"),
                    OrType.Make<IKey, IError>(new NameKey("X")),
                    new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));

            problem.builder.IsAssignedTo(a, x);

            var solution = problem.Solve();

            // mostly we just want this not to throw 
        }

        // () > a =: number d
        // a is a method and it returns a number
        [Fact]
        public void InferredMethod() {

            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope());

            var a = problem.builder.CreatePublicMember(
                    problem.ModuleRoot,
                    problem.ModuleRoot,
                    new NameKey("a"),
                    new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("a")));

            var returns = problem.builder.GetReturns(a);

            var d = problem.builder.CreatePublicMember(problem.ModuleRoot, problem.ModuleRoot, new NameKey("d"), OrType.Make<IKey, IError>(new NameKey("number")), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("d")));

            problem.builder.IsAssignedTo(returns, d);

            var solution = problem.Solve();

            var res = solution.GetType(a);
            var method = Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.MethodType>(res.GetValue().Is1OrThrow());
            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.NumberType>(method.OutputType.GetValue().Is1OrThrow());
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
                DefaultRootScopePopulateScope());

            var nType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("N")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(nType, nType, new NameKey("x"), OrType.Make<IKey, IError>(new NameKey("number")), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));

            var bType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(bType, bType, new NameKey("x"), OrType.Make<IKey, IError>(new NameKey("bool")), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));

            var a = x.builder.CreatePublicMember(
                    x.ModuleRoot,
                    x.ModuleRoot,
                    new NameKey("a"),
                    new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("a")));

            var c = x.builder.CreatePublicMember(
                x.ModuleRoot,
                x.ModuleRoot,
                new NameKey("c"),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("c")));

            var b = x.builder.CreatePublicMember(
                    x.ModuleRoot,
                    new NameKey("b"),
                    OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(bType),
                    new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("b")));

            var n = x.builder.CreatePublicMember(
                    x.ModuleRoot,
                    new NameKey("n"),
                    OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(nType),
                    new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("n")));

            x.builder.IsAssignedTo(a, n);
            x.builder.IsAssignedTo(c, a);
            x.builder.IsAssignedTo(a, b);

            var solution = x.Solve();

            var cType = solution.GetType(c);

            var hasMembers = Assert.IsType<HasMembersType>(cType.GetValue().Is1OrThrow());
            hasMembers.TryGetMember(new NameKey("x"), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow().Is2OrThrow();
        }

        // TODO!
        // A1A44050-9185-4C49-9C82-B9E9293BE3DF
        // type A {a;b; }
        // type B {b;c;}
        // x =: A|B ab
        // x better be A|B and not just {b;}


        // flow in to member
        // type D { y; } 
        // a.x =: D d
        // a = D d
        [Fact]
        public void FlowToMember() {
            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                DefaultRootScopePopulateScope());

            var dType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("D")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(dType, dType, new NameKey("y"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("y")));

            var a = x.builder.CreatePublicMember(
                x.ModuleRoot,
                x.ModuleRoot,
                new NameKey("a"),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("a")));

            var a_x = x.builder.CreateHopefulMember(a, new NameKey("x"), new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("x")));

            var d = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("d"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>(dType),
                new WeakMemberDefinitionConverter(Access.ReadWrite, new NameKey("d")));


            x.builder.IsAssignedTo(a_x, d);
            x.builder.IsAssignedTo(a, d);

            var solution = x.Solve();

            var aType = solution.GetType(a).GetValue().Is1OrThrow();
            var a_xType = aType.TryGetMember(new NameKey("x"), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow().Is1OrThrow().Item1;
            a_xType.TryGetMember(new NameKey("y"), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow().Is1OrThrow();
        }

        // flow in to member 2
        // type D { number|string y; } 
        // a.x =: D d
        // a =: D d
        // a.x.y =: number n


        // this is such a mea pair:
        // method [number,number ] input { input return; } =: res
        // 2 > res

        // with just this
        // method [number,number ] input { input return; } =: res
        // res is an any

        // with just this
        // 2 > res
        // res is a method[any,any]

        // together, it will only compile if res is method[number,any]
        // method [number,number ] input { input return; } =: res
        // 2 > res

        // somehow they are both placing a requirement
        // 2 > res      makes it a method, 
        // method [number,number ] input { input return; } =: res   when this is a method it's input has to be a number

        // in other words
        // 2 > res      res is: method [any, any], 
        // method [number,number ] input { input return; } =: res   res is: method[number, any]|any

        // when both applied we get:  method [any, any] && (method[number, any]|any), which is method [number, any]

        // that's a reasonable way to think about it
        // but it's still a unpressident the assigned (int this case  method [number,number ] input { input return; }) to put a constraint on the assignee (res)
        // I think it is unique to method  (and possible other "in" Contavariant ??) 
        // by assigning

        // another exmpale is ... 
        // object { number x := 2 } =: a        // this puts a restring on a... it has to have an x and the x can only be set to a number
        // "test" =: a.x                        // this puts a restraint on x... it has to be allowed to be a string   
        // 
    }
}
