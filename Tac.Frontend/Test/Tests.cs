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

        //private static void HasCount(int count, IFrontendType<IVerifiableType> result)
        //{
        //    var members = 0;
        //    if (result.SafeIs(out HasMembersType membersType)) {
        //        members =membersType.weakScope.membersList.Count();
        //    }



        //    string error = 0;
        //}

        private static IFrontendType<IVerifiableType> HasMember(IFrontendType<IVerifiableType> result, IKey key)
        {
            return result.TryGetMember(key, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow().Is1OrThrow().Type.GetValue().Is1OrThrow();
        }

        private static void DoesNotHaveMember(IFrontendType<IVerifiableType> result, IKey key)
        {
            result.TryGetMember(key, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is2OrThrow();
        }

        private static IFrontendType<IVerifiableType> MemberToType(WeakMemberDefinition member)
        {
            return member.Type.GetValue().Is1OrThrow();
        }

        private static void Equal(IFrontendType<IVerifiableType> a, IFrontendType<IVerifiableType> b)
        {
            Assert.True(a.TheyAreUs(b, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow() && b.TheyAreUs(a, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
        }

        internal static RootScopePopulateScope DefaultRootScopePopulateScope() => new RootScopePopulateScope(
                Array.Empty<IOrType<WeakAssignOperationPopulateScope, IError>>(),
                OrType.Make<EntryPointDefinitionPopulateScope, IError>(
                    new EntryPointDefinitionPopulateScope(
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
            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });
            x.Solve();
        }

        [Fact]
        public void AddType()
        {
            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });
            var hello = x.builder.CreateType(x.ModuleRoot, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("Hello")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(hello, hello, new NameKey("x"));
            x.builder.CreatePublicMember(hello, hello, new NameKey("y"));
            var solution = x.Solve();

            var resultHello = hello.Converter.Convert(solution, hello).Is1OrThrow();

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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _=> { });

            var hello = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("hello")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(hello, hello, new NameKey("x"));
            x.builder.CreatePublicMember(hello, hello, new NameKey("y"));

            var input = x.builder.CreateValue(x.ModuleRoot.InitizationScope, new NameKey("hello"));
            var method = x.builder.CreateMethod(x.ModuleRoot, "input", new WeakMethodDefinitionConverter(new Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>>(new List<IOrType<IBox<IFrontendCodeElement>, IError>>())));

            var input_x = x.builder.CreateHopefulMember(method.Input(), new NameKey("x"));
            var input_y = x.builder.CreateHopefulMember(method.Input(), new NameKey("y"));

            var method_x = x.builder.CreatePrivateMember(method, method, new NameKey("x"));
            var method_y = x.builder.CreatePrivateMember(method, method, new NameKey("y"));

            input_x.AssignTo(method_x);
            input_y.AssignTo(method_y);

            method.Input().AssignTo(method.Returns());

            input.AssignTo(method.Input());

            var result = x.Solve();

            var methodResult = method.Converter.Convert(result,method).Is1OrThrow();

            var HackToLookAtScope = new HasMembersType(methodResult.Scope.Is1OrThrow());

            HasMember(HackToLookAtScope, new NameKey("input"));
            HasMember(HackToLookAtScope, new NameKey("x"));
            HasMember(HackToLookAtScope, new NameKey("y"));
            var inputResult = HasMember(HackToLookAtScope, new NameKey("input"));

            HasMember(inputResult, new NameKey("x"));
            HasMember(inputResult, new NameKey("y"));

            
            var helloResult = hello.Converter.Convert(result, hello).Is1OrThrow().FrontendType();
            HasMember(helloResult.Is1OrThrow(), new NameKey("x"));
            HasMember(helloResult.Is1OrThrow(), new NameKey("y"));

            // return should really be an any...
            var methodReturns = methodResult.OutputType.Is1OrThrow();
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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            var m1 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m1"));
            x.builder.CreateHopefulMember(m1, new NameKey("x"));
            var m2 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m2"));
            x.builder.CreateHopefulMember(m2, new NameKey("y"));
            var m3 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m3"));
            var m4 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m4"));
            var m5 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m5"));

            m1.AssignTo(m3);
            m2.AssignTo(m3);
            m3.AssignTo(m4);
            m3.AssignTo(m5);

            var solution = x.Solve();

            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();

            var m1t = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("m1"))));
            HasMember(m1t, new NameKey("x"));
            DoesNotHaveMember(m1t, new NameKey("y"));

            var m2t = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("m2"))));
            DoesNotHaveMember(m2t, new NameKey("x"));
            HasMember(m2t, new NameKey("y"));

            var m3t = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("m3"))));
            DoesNotHaveMember(m3t, new NameKey("x"));
            DoesNotHaveMember(m3t, new NameKey("y"));

            var m4t = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("m4"))));
            DoesNotHaveMember(m4t, new NameKey("x"));
            DoesNotHaveMember(m4t, new NameKey("y"));

            var m5t = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("m5"))));
            DoesNotHaveMember(m5t, new NameKey("x"));
            DoesNotHaveMember(m5t, new NameKey("y"));

        }



        [Fact]
        public void AssignmentXUpStream()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            var m1 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m1"));
            var m2 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m2"));
            var m3 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m3"));
            var m4 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m4"));
            x.builder.CreateHopefulMember(m4, new NameKey("x"));
            var m5 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m5"));
            x.builder.CreateHopefulMember(m5, new NameKey("y"));

            m1.AssignTo(m3);
            m2.AssignTo(m3);
            m3.AssignTo(m4);
            m3.AssignTo(m5);

            var solution = x.Solve();

            //HasCount(2, MemberToType(solution.GetMemberFromType(m1).GetValue()));
            //HasCount(2, MemberToType(solution.GetMemberFromType(m2).GetValue()));
            //HasCount(2, MemberToType(solution.GetMemberFromType(m3).GetValue()));
            //HasCount(1, MemberToType(solution.GetMemberFromType(m4).GetValue()));
            //HasCount(1, MemberToType(solution.GetMemberFromType(m5).GetValue()));

            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();


            var m1t = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("m1"))));
            HasMember(m1t, new NameKey("x"));
            HasMember(m1t, new NameKey("y"));

            var m2t = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("m2"))));
            HasMember(m2t, new NameKey("x"));
            HasMember(m2t, new NameKey("y"));

            var m3t = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("m3"))));
            HasMember(m3t, new NameKey("x"));
            HasMember(m3t, new NameKey("y"));

            var m4t = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("m4"))));
            HasMember(m4t, new NameKey("x"));
            DoesNotHaveMember(m4t, new NameKey("y"));

            var m5t = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("m5"))));
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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            var m1 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m1"));
            x.builder.CreateHopefulMember(m1, new NameKey("x"));
            var m2 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m2"));
            x.builder.CreateHopefulMember(m2, new NameKey("y"));

            m1.AssignTo(m2);
            m2.AssignTo(m1);

            var solution = x.Solve();
            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();



            var m1t = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("m1"))));
            HasMember(m1t, new NameKey("x"));
            HasMember(m1t, new NameKey("y"));

            var m2t = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("m2"))));
            HasMember(m2t, new NameKey("x"));
            HasMember(m2t, new NameKey("y"));

            //HasCount(2, MemberToType(solution.GetMemberFromType(m1).GetValue()));
            //HasCount(2, MemberToType(solution.GetMemberFromType(m2).GetValue()));
        }

        [Fact]
        public void HopefulOnHopeful()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            var m1 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m1"));
            var x1 = x.builder.CreateHopefulMember(m1, new NameKey("x1"));
            var x2 = x.builder.CreateHopefulMember(x1, new NameKey("x2"));
            x.builder.CreateHopefulMember(x2, new NameKey("x3"));

            var m2 = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("m2"));

            m2.AssignTo(m1);

            var solution = x.Solve();
            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();



            var m2t = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("m2"))));
            var x1t = HasMember(m2t, new NameKey("x1"));
            var x2t = HasMember(x1t, new NameKey("x2"));
            HasMember(x2t, new NameKey("x3"));
        }

        // given:
        // type[T] pair {T x}
        // type chicken { eggs }
        // piar[chicken] x;
        //
        // x.x.eggs should exist.
        [Fact]
        public void Generic()
        {


            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            var pairType = x.builder.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("pair")),
                new[]{
                    new Tpn.TypeAndConverter(
                        new NameKey("T"),
                        new WeakTypeDefinitionConverter())},
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.builder.CreatePublicMember(pairType, pairType, new NameKey("x"),
                OrType.Make<IKey, IError>(new NameKey("T")));

            var chickenType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            x.builder.CreatePublicMember(chickenType, chickenType, new NameKey("eggs"));

            var chickenPair = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("x"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })));

            var solution = x.Solve();
            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();


            var chickenPairResult = obj.membersList.Single(x => x.Key.Equals(new NameKey("x")));

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
                DefaultRootScopePopulateScope(), _ => { });

            var type = x.builder.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("node")),
                new[]{
                    new Tpn.TypeAndConverter(new NameKey("node-t"), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
            );

            x.builder.CreatePublicMember(type, type, new NameKey("next"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("node-t"))
            })));

            x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());


            var thing = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("chicken"))
            })));

            var solution = x.Solve();
            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();


            var thingResult = obj.membersList.Single(x => x.Key.Equals(new NameKey("thing")));
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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            var type = x.builder.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("node")),
                new[]{
                    new Tpn.TypeAndConverter(new NameKey("node-t"), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
            );

            x.builder.CreatePublicMember(type, type, new NameKey("next"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("node-t"))
            })));

            x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            var thing = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("chicken"))
            })));

            var xMember = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("x"));

            // this assignment is an important part of the test
            // we want to test that members flow as they should
            x.builder.IsAssignedTo(xMember, thing);

            var solution = x.Solve();
            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();

            var thingResult = obj.membersList.Single(x => x.Key.Equals(new NameKey("thing")));
            var thingResultType = MemberToType(thingResult);

            var nextResult = HasMember(thingResultType, new NameKey("next"));
            HasMember(nextResult, new NameKey("next"));

            Equal(thingResultType, nextResult);

            var xMemberResult = MemberToType(obj.membersList.Single(x => x.Key.Equals(new NameKey("x"))));
            for (int i = 0; i < 100; i++)
            {
                xMemberResult = HasMember(xMemberResult, new NameKey("next"));
            }
        }


        [Fact]
        public void GenericCircular()
        {
            throw new Exception("stack overflow!");
            // type[left-t] left {  right[left-t] thing }
            // type[right-t] right {  left[right-t] thing }
            // type chicken {}
            // left[chicken] left-member;
            // right[chicken] right-member

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            var left = x.builder.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("left")),
                new[]{
                    new Tpn.TypeAndConverter(new NameKey("left-t"), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...? {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.builder.CreatePublicMember(left, left, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("right"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("left-t"))
            })));

            var right = x.builder.CreateGenericType(
                x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("right")),
                new[]{
                    new Tpn.TypeAndConverter(new NameKey("right-t"), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...? {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.builder.CreatePublicMember(right, right, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("left"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("right-t"))
            })));

            x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            var leftMember = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("left-member"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("left"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })));

            var rightMember = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("right-member"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("right"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })));

            // this assignment is an important part of the test
            x.builder.IsAssignedTo(leftMember, rightMember);
            x.builder.IsAssignedTo(rightMember, leftMember);

            var solution = x.Solve();
            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();

            var leftResult = obj.membersList.Single(x => x.Key.Equals(new NameKey("left-member")));
            var rightResult = obj.membersList.Single(x => x.Key.Equals(new NameKey("right-member")));

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

        // given:
        // type pair [T] {T x}
        // type chicken { eggs; }
        // pair [pair[chicken]] x
        // then:
        // x.x.eggs should 
        [Fact]
        public void NestedGeneric()
        {


            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                DefaultRootScopePopulateScope(), _ => { });

            var pairType = x.builder.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("pair")),
                new[]{
                    new Tpn.TypeAndConverter(new NameKey("T"), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.builder.CreatePublicMember(pairType, pairType, new NameKey("x"),
                OrType.Make<IKey, IError>(new NameKey("T")));

            var chickenType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            x.builder.CreatePublicMember(chickenType, chickenType, new NameKey("eggs"));

            var xMember = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("x"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })) })));

            var solution = x.Solve();
            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();

            var xMemberResult = obj.membersList.Single(x => x.Key.Equals(new NameKey("x")));
            var xMemberResultType = MemberToType(xMemberResult);
            //HasCount(1, xMemberResultType);s
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
                DefaultRootScopePopulateScope(), _ => { });


            var aType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(aType, aType, new NameKey("x"));
            x.builder.CreatePublicMember(aType, aType, new NameKey("y"));

            var bType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(bType, bType, new NameKey("x"));
            x.builder.CreatePublicMember(bType, bType, new NameKey("z"));

            var cType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("C")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(cType, cType, new NameKey("x"), OrType.Make<IKey, IError>(new NameKey("number")));

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.builder.CreateOrType(x.ModuleRoot, key,
                 OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("B"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var ab = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("ab"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType,Tpn.TypeProblem2.GenericTypeParameter, IError >(orType));

            var c = x.builder.CreatePublicMember(x.ModuleRoot, new NameKey("c"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(cType));

            x.builder.IsAssignedTo(ab, c);

            var solution = x.Solve();

            var aTypeSolution = aType.Converter.Convert(solution, aType).Is1OrThrow();
            var aMember = HasMember(aTypeSolution.FrontendType().Is1OrThrow(), new NameKey("x"));
            Assert.True(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType().TheyAreUs(aMember, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

            var bTypeSolution = bType.Converter.Convert(solution, bType).Is1OrThrow();
            var bMember = HasMember(bTypeSolution.FrontendType().Is1OrThrow(), new NameKey("x"));
            Assert.True(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType().TheyAreUs(bMember, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
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
                DefaultRootScopePopulateScope(), _ => { });


            var aType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(aType, aType, new NameKey("x"));
            x.builder.CreatePublicMember(aType, aType, new NameKey("y"));

            var bType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(bType, bType, new NameKey("x"));
            x.builder.CreatePublicMember(bType, bType, new NameKey("z"));

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.builder.CreateOrType(x.ModuleRoot, key,
                 OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("B"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var ab = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("ab"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(orType));

            var c = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"));

            x.builder.IsAssignedTo(c, ab);

            var solution = x.Solve();
            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();

            var member = obj.membersList.Single(x => x.Key.Equals(new NameKey("c")));

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
                DefaultRootScopePopulateScope(), _ => { });

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.builder.CreateOrType(x.ModuleRoot, key,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("number"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("string"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var b = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(orType));

            var c = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"));

            var five = x.builder.CreateValue(x.ModuleRoot.InitizationScope, new NameKey("number"));


            x.builder.IsAssignedTo(five, c);
            x.builder.IsAssignedTo(c, b);

            var solution = x.Solve();
            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();

            var cType = obj.membersList.Single(x => x.Key.Equals(new NameKey("c"))).Type.GetValue().Is1OrThrow();

            var cOrType = Assert.IsType<FrontEndOrType>(cType);

            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.NumberType>(cOrType.left.GetValue().Is1OrThrow());
            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.StringType>(cOrType.right.GetValue().Is1OrThrow());
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
                DefaultRootScopePopulateScope(), _ => { });

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.builder.CreateOrType(x.ModuleRoot, key,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("number"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("string"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var b = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(orType));

            var c = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"));

            var d = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("d"), OrType.Make<IKey, IError>(new NameKey("number")));

            var five = x.builder.CreateValue(x.ModuleRoot.InitizationScope, new NameKey("number"));

            x.builder.IsAssignedTo(five, c);
            x.builder.IsAssignedTo(c, b);
            x.builder.IsAssignedTo(c, d);

            var solution = x.Solve();
            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();

            var cType = obj.membersList.Single(x => x.Key.Equals(new NameKey("c"))).Type.GetValue().Is1OrThrow();

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
                DefaultRootScopePopulateScope(), _ => { });

            var aType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(aType, aType, new NameKey("x"));
            x.builder.CreatePublicMember(aType, aType, new NameKey("y"));

            var bType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(bType, bType, new NameKey("x"));
            x.builder.CreatePublicMember(bType, bType, new NameKey("z"));

            var cType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("C")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(cType, cType, new NameKey("z"));
            x.builder.CreatePublicMember(cType, cType, new NameKey("w"));

            var dType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("D")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(dType, dType, new NameKey("y"));
            x.builder.CreatePublicMember(dType, dType, new NameKey("w"));

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
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(orType1));

            var b = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(orType2));

            var c = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"));

            x.builder.IsAssignedTo(c, a);
            x.builder.IsAssignedTo(c, b);

            var solution = x.Solve();
            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();

            var cTypeResult = obj.membersList.Single(x => x.Key.Equals(new NameKey("c"))).Type.GetValue().Is1OrThrow();

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<WeakMemberDefinition> {
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("x"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))),
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("y"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))),
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("z"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))
                })), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<WeakMemberDefinition>{
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("x"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))),
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("y"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))),
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("w"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))
                })),
                new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<WeakMemberDefinition> {
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("z"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))),
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("w"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))),
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("x"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))
                }
                )),
                new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<WeakMemberDefinition> {
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("z"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))),
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("w"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))),
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("y"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))
                }
                )),
                new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

            Assert.False(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<WeakMemberDefinition> { }
                )), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

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
                DefaultRootScopePopulateScope(), _ => { });

            var aType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(aType, aType, new NameKey("x"));
            x.builder.CreatePublicMember(aType, aType, new NameKey("y"));

            var bType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(bType, bType, new NameKey("x"));
            x.builder.CreatePublicMember(bType, bType, new NameKey("z"));

            var cType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("C")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(cType, cType, new NameKey("z"));
            x.builder.CreatePublicMember(cType, cType, new NameKey("w"));

            var key1 = new ImplicitKey(Guid.NewGuid());
            var orType1 = x.builder.CreateOrType(x.ModuleRoot, key1,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("C"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());


            var a = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("a"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(orType1));

            var b = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(bType));

            var c = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("c"));

            x.builder.IsAssignedTo(c, a);
            x.builder.IsAssignedTo(c, b);

            var solution = x.Solve();
            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();

            var cTypeResult = obj.membersList.Single(x => x.Key.Equals(new NameKey("c"))).Type.GetValue().Is1OrThrow();

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<WeakMemberDefinition> {
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("x"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))),
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("y"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))),
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("z"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))
                })),
                new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

            Assert.True(cTypeResult.TheyAreUs(new HasMembersType(new WeakScope(
                new List<WeakMemberDefinition> {
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("w"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))),
                    new WeakMemberDefinition(Access.ReadWrite,new NameKey("x"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))),
                   new WeakMemberDefinition(Access.ReadWrite,new NameKey("z"),new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>( new Tac.SyntaxModel.Elements.AtomicTypes.AnyType())))
                })),
                new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

            Assert.False(cTypeResult.TheyAreUs(
                new HasMembersType(new WeakScope(new List<WeakMemberDefinition> { })),
                new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

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
                DefaultRootScopePopulateScope(), _ => { });

            var aType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(aType, aType, new NameKey("x"));

            var bType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(bType, bType, new NameKey("x"), OrType.Make<IKey, IError>(new NameKey("number")));

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = x.builder.CreateOrType(x.ModuleRoot, key,
                 OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(x.builder.CreateTypeReference(x.ModuleRoot, new NameKey("number"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var a = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("a"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(orType));

            var b = x.builder.CreatePublicMember(x.ModuleRoot, new NameKey("b"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(bType));

            x.builder.IsAssignedTo(a, b);

            var solution = x.Solve();

            var aTypeSolution = aType.Converter.Convert(solution, aType).Is1OrThrow();
            var aMember = HasMember(aTypeSolution.FrontendType().Is1OrThrow(), new NameKey("x"));
            Assert.True(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType().TheyAreUs(aMember, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
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
            throw new Exception("stack overflow!");

            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            var yType = problem.builder.CreateType(
                problem.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("Y")),
                new WeakTypeDefinitionConverter()
            );
            problem.builder.CreatePublicMember(
                yType,
                new NameKey("member"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(yType));

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
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(orType));

            var a = problem.builder.CreatePublicMember(
                    problem.ModuleRoot,
                    problem.ModuleRoot,
                    new NameKey("a"));

            var x = problem.builder.CreatePublicMember(
                    problem.ModuleRoot,
                    problem.ModuleRoot,
                    new NameKey("x"),
                    OrType.Make<IKey, IError>(new NameKey("X")));

            problem.builder.IsAssignedTo(a, x);

            var solution = problem.Solve();

            // mostly we just want this not to throw 
        }

        // () > a =: number d
        // a is a method and it returns a number
        [Fact]
        public void InferredMethod()
        {

            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            var a = problem.builder.CreatePublicMember(
                    problem.ModuleRoot,
                    problem.ModuleRoot,
                    new NameKey("a"));

            var returns = problem.builder.GetReturns(a);

            var d = problem.builder.CreatePublicMember(problem.ModuleRoot, problem.ModuleRoot, new NameKey("d"), OrType.Make<IKey, IError>(new NameKey("number")));

            problem.builder.IsAssignedTo(returns, d);

            var solution = problem.Solve();

            var res = solution.GetType(a);
            var method = Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.MethodType>(res.Is1OrThrow());
            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.NumberType>(method.OutputType.GetValue().Is1OrThrow());
        }


        // type N { number x; }
        // type B { bool x; }
        // a =: N n
        // c =: a
        // a =: B b

        // what is c.x ??
        [Fact]
        public void UnmergableInferredType()
        {

            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                DefaultRootScopePopulateScope(), _ => { });

            var nType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("N")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(nType, nType, new NameKey("x"), OrType.Make<IKey, IError>(new NameKey("number")));

            var bType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(bType, bType, new NameKey("x"), OrType.Make<IKey, IError>(new NameKey("bool")));

            var a = x.builder.CreatePublicMember(
                    x.ModuleRoot,
                    x.ModuleRoot,
                    new NameKey("a"));

            var c = x.builder.CreatePublicMember(
                x.ModuleRoot,
                x.ModuleRoot,
                new NameKey("c"));

            var b = x.builder.CreatePublicMember(
                    x.ModuleRoot,
                    new NameKey("b"),
                    OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(bType));

            var n = x.builder.CreatePublicMember(
                    x.ModuleRoot,
                    new NameKey("n"),
                    OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(nType));

            x.builder.IsAssignedTo(a, n);
            x.builder.IsAssignedTo(c, a);
            x.builder.IsAssignedTo(a, b);

            var solution = x.Solve();

            var cType = solution.GetType(c);

            var hasMembers = Assert.IsType<HasMembersType>(cType.Is1OrThrow());
            var member = hasMembers.TryGetMember(new NameKey("x"), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow().Is1OrThrow();
            member.Type.GetValue().Is2OrThrow();
        }


        // flow in to member
        // type D { y; } 
        // a.x =: D d
        // a = D d
        [Fact]
        public void FlowToMember()
        {
            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                DefaultRootScopePopulateScope(), _ => { });

            var dType = x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("D")), new WeakTypeDefinitionConverter());
            x.builder.CreatePublicMember(dType, dType, new NameKey("y"));

            var a = x.builder.CreatePublicMember(
                x.ModuleRoot,
                x.ModuleRoot,
                new NameKey("a"));

            var a_x = x.builder.CreateHopefulMember(a, new NameKey("x"));

            var d = x.builder.CreatePublicMember(
                x.ModuleRoot,
                new NameKey("d"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(dType));


            x.builder.IsAssignedTo(a_x, d);
            x.builder.IsAssignedTo(a, d);

            var solution = x.Solve();

            var aType = solution.GetType(a).Is1OrThrow();
            var a_xType = aType.TryGetMember(new NameKey("x"), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow().Is1OrThrow().Type.GetValue().Is1OrThrow();
            a_xType.TryGetMember(new NameKey("y"), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow().Is1OrThrow();
        }


        // this is such a mean pair:
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
        // object { number x := 2 } =: a        // this puts a restraint on a... it has to have an x and the x can only be set to a number
        // "test" =: a.x                        // this puts a restraint on x... it has to be allowed to be a string   
        // x end up as { number | string x } or any.... I wonder which?
        // reversing again, I think these clash the object might be relying on x to be a number, we can't just make it a string

        // can yo do this? 
        // {int a} x =: {int|string a} y 
        // I don't think so...
        // because you can't do 
        // "test" =: (a.x)
        // so you should be able to do
        // "test" =: y.a
        // if a is readonly then it is fine

        // so... there are upstream constraints and downstream constraints and it depends on varience??

        // {int a} x =: y
        // y is any

        // {int a} x =: y
        // b =: y.a
        // y is {int a} and b is int


        // method [number,number ] input { input return; } =: res
        // 2 > res
        // res is method [number; any;]
        [Fact]
        public void MethodAndCall()
        {

            var x = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                DefaultRootScopePopulateScope(), _ => { });

            var methodValue = x.builder.CreateValue(x.ModuleRoot.InitizationScope, new GenericNameKey(new NameKey("method"), new IOrType<IKey, IError>[] {
                    OrType.Make<IKey, IError>(new NameKey("number")),
                    OrType.Make<IKey, IError>(new NameKey("number")),
                }));

            var res = x.builder.CreatePublicMember(
                x.ModuleRoot,
                x.ModuleRoot,
                new NameKey("res"));


            var two = x.builder.CreateValue(x.ModuleRoot.InitizationScope, new NameKey("number"));

            x.builder.IsAssignedTo(methodValue, res);
            x.builder.IsAssignedTo(two, res.Input());

            var solution = x.Solve();

            // currently we have: 
            // method<any|number, any> | method<number, number> 
            var resType = solution.GetType(res).Is1OrThrow();

            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.NumberType>(resType.TryGetInput().Is1OrThrow().Is1OrThrow());
            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.NumberType>(resType.TryGetReturn().Is1OrThrow().Is1OrThrow());
        }

        // method [number,number ] input { input return; } =: res
        // 2 > res
        // x =: res
        //
        // x shares res's type
        [Fact]
        public void MethodAndCallAndUp()
        {

            var typeProblem = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                DefaultRootScopePopulateScope(), _ => { });

            var methodValue = typeProblem.builder.CreateValue(typeProblem.ModuleRoot.InitizationScope, new GenericNameKey(new NameKey("method"), new IOrType<IKey, IError>[] {
                    OrType.Make<IKey, IError>(new NameKey("number")),
                    OrType.Make<IKey, IError>(new NameKey("number")),
                }));

            var res = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("res"));

            var x = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("x"));


              
            var two = typeProblem.builder.CreateValue(typeProblem.ModuleRoot.InitizationScope, new NameKey("number"));

            typeProblem.builder.IsAssignedTo(methodValue, res);
            typeProblem.builder.IsAssignedTo(two, res.Input());
            typeProblem.builder.IsAssignedTo(x, res);

            var solution = typeProblem.Solve();

            // currently we have: 
            // method<any|number, any> | method<number, number> 
            var resType = solution.GetType(res).Is1OrThrow();

            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.NumberType>(resType.TryGetInput().Is1OrThrow().Is1OrThrow());
            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.NumberType>(resType.TryGetReturn().Is1OrThrow().Is1OrThrow());


            var xType = solution.GetType(x).Is1OrThrow();

            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.NumberType>(xType.TryGetInput().Is1OrThrow().Is1OrThrow());
            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.NumberType>(xType.TryGetReturn().Is1OrThrow().Is1OrThrow());
        }

        // type {x;} a =: b
        // a =: type {y;} c
        // d =: b
        //
        // b and d should be {x;y;} ... no this should be an error 



        // type { number x } a =: b             // this puts a restraint on b... it has to have an x and the x can only be set to a number
        // "test" =: b.x                        // this puts a restraint on x... it has to be allowed to be a string   
        // c =: b.x
        // b.x and c end up as ... any?
        [Fact]
        public void UpAndDown() {
            var typeProblem = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            // TODO!
        }


        // a =: b
        // number c =: b
        // b =: number|bool d
        // 
        // a and b are both number|bool
        [Fact]
        public void AssignmentY() {
            var typeProblem = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            var keyD = new ImplicitKey(Guid.NewGuid());
            var orTypeD = typeProblem.builder.CreateOrType(
                typeProblem.ModuleRoot, 
                keyD,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(typeProblem.builder.CreateTypeReference(typeProblem.ModuleRoot, new NameKey("number"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(typeProblem.builder.CreateTypeReference(typeProblem.ModuleRoot, new NameKey("bool"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var a = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("a"));
            var b = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("b"));
            var c = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("c"),
                OrType.Make<IKey, IError>(new NameKey("bool")));
            var d = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                new NameKey("d"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(orTypeD));

            typeProblem.builder.IsAssignedTo(a, b);
            typeProblem.builder.IsAssignedTo(c, b);
            typeProblem.builder.IsAssignedTo(b, d);

            var solution = typeProblem.Solve();


            var bType = solution.GetType(b).Is1OrThrow();
            Assert.True(bType.TheyAreUs(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType(), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
            Assert.True(bType.TheyAreUs(new Tac.SyntaxModel.Elements.AtomicTypes.BooleanType(), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

            var aType = solution.GetType(a).Is1OrThrow();
            Assert.True(aType.TheyAreUs(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType(), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
            Assert.True(aType.TheyAreUs(new Tac.SyntaxModel.Elements.AtomicTypes.BooleanType(), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
        }


        // x.a := x
        //
        // x.a is any
        [Fact]
        public void AofXisX()
        {
            var typeProblem = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                DefaultRootScopePopulateScope(), _ => { });

            var x = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("x"));

            var a = typeProblem.builder.CreateHopefulMember(x, new NameKey("a"));

            typeProblem.builder.IsAssignedTo(x, a);

            var solution = typeProblem.Solve();

            var xType = solution.GetType(x).Is1OrThrow();
            var aType = xType.TryGetMember(new NameKey("a"), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow().Is1OrThrow().Type.GetValue().Is1OrThrow();

            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.AnyType>(aType);
        }

        // x.a =: x
        //
        // x.a.a.a.a exists
        [Fact]
        public void XisAofX()
        {
            var typeProblem = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                DefaultRootScopePopulateScope(), _ => { });

            var x = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("x"));

            var a = typeProblem.builder.CreateHopefulMember(x, new NameKey("a"));

            typeProblem.builder.IsAssignedTo(a, x);

            var solution = typeProblem.Solve();


            var xType = solution.GetType(x).Is1OrThrow();
            var aType = xType.TryGetMember(new NameKey("a"), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow().Is1OrThrow().Type.GetValue().Is1OrThrow(); ;
            var aaType = aType.TryGetMember(new NameKey("a"), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow().Is1OrThrow().Type.GetValue().Is1OrThrow(); ;
            var aaaType = aaType.TryGetMember(new NameKey("a"), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow().Is1OrThrow().Type.GetValue().Is1OrThrow(); ;
        }

        // x.a =: x
        // x.a := x
        //
        // x.a.a.a.a exists
        [Fact]
        public void XisAofXandAofXisX()
        {
            var typeProblem = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                DefaultRootScopePopulateScope(), _ => { });

            var x = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("x"));

            var a = typeProblem.builder.CreateHopefulMember(x, new NameKey("a"));

            typeProblem.builder.IsAssignedTo(a, x);
            typeProblem.builder.IsAssignedTo(x, a);

            var solution = typeProblem.Solve();


            var xType = solution.GetType(x).Is1OrThrow();
            var aType = xType.TryGetMember(new NameKey("a"), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow().Is1OrThrow().Type.GetValue().Is1OrThrow(); ;
            var aaType = aType.TryGetMember(new NameKey("a"), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow().Is1OrThrow().Type.GetValue().Is1OrThrow(); ;
            var aaaType = aaType.TryGetMember(new NameKey("a"), new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow().Is1OrThrow().Type.GetValue().Is1OrThrow(); ;
        }

        // flow in to member 2
        // type D { number|string y; } 
        // a =: D d
        // a.x =: D d
        // a.x.y =: number n
        [Fact]
        public void FlowToMember2()
        {
            var typeProblem = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                DefaultRootScopePopulateScope(), _ => { });

            var dType = typeProblem.builder.CreateType(typeProblem.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("D")), new WeakTypeDefinitionConverter());

            var key = new ImplicitKey(Guid.NewGuid());
            var orType = typeProblem.builder.CreateOrType(typeProblem.ModuleRoot, key,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(typeProblem.builder.CreateTypeReference(typeProblem.ModuleRoot, new NameKey("number"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(typeProblem.builder.CreateTypeReference(typeProblem.ModuleRoot, new NameKey("string"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            typeProblem.builder.CreatePublicMember(dType,
                new NameKey("y"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(orType));

            var a = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("a"));

            var x = typeProblem.builder.CreateHopefulMember(a, new NameKey("x"));
            var y = typeProblem.builder.CreateHopefulMember(a, new NameKey("y"));

            var d = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                new NameKey("d"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(dType));

            var n = typeProblem.builder.CreatePublicMember(typeProblem.ModuleRoot, typeProblem.ModuleRoot, new NameKey("n"), OrType.Make<IKey, IError>(new NameKey("number")));

            typeProblem.builder.IsAssignedTo(a, d);
            typeProblem.builder.IsAssignedTo(x, d);
            typeProblem.builder.IsAssignedTo(y, n);

            var solution = typeProblem.Solve();

            var yType = solution.GetType(y).Is1OrThrow();
            Assert.IsType<Tac.SyntaxModel.Elements.AtomicTypes.NumberType>(yType);

        }


        // type A {a;b; }
        // type B {b;c;}
        // x =: A|B ab
        // x better be A|B and not just {b;}
        [Fact]
        public void OrTypesReallyFlowNotThierIntersections()
        {

            var typeProblem = new Tpn.TypeProblem2(
                new WeakScopeConverter(),
                DefaultRootScopePopulateScope(), _ => { });

            var aType = typeProblem.builder.CreateType(typeProblem.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("A")), new WeakTypeDefinitionConverter());
                typeProblem.builder.CreatePublicMember(aType, aType, new NameKey("a"));
                typeProblem.builder.CreatePublicMember(aType, aType, new NameKey("b"));

            var bType = typeProblem.builder.CreateType(typeProblem.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("B")), new WeakTypeDefinitionConverter());
                typeProblem.builder.CreatePublicMember(bType, bType, new NameKey("b"));
                typeProblem.builder.CreatePublicMember(bType, bType, new NameKey("c"));
            
            var key = new ImplicitKey(Guid.NewGuid());
            var orType = typeProblem.builder.CreateOrType(typeProblem.ModuleRoot, key,
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(typeProblem.builder.CreateTypeReference(typeProblem.ModuleRoot, new NameKey("A"), new WeakTypeReferenceConverter())),
                OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(typeProblem.builder.CreateTypeReference(typeProblem.ModuleRoot, new NameKey("B"), new WeakTypeReferenceConverter())),
                new WeakTypeOrOperationConverter());

            var x = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("x"));


            var ab = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                new NameKey("ab"),
                OrType.Make<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, Tpn.TypeProblem2.GenericTypeParameter, IError>(orType));

            typeProblem.builder.IsAssignedTo(x, ab);

            var solution = typeProblem.Solve();

            var xType = solution.GetType(x).Is1OrThrow();
            Assert.IsType<FrontEndOrType>(xType);
        }



        // typc more {a;b;}
        // type less {a;}
        // method [more,less] x;
        // method [less,more] y;
        //
        // z =: x
        // z =: y
        //
        // z is method [more, more]
        [Fact]
        public void MethodVarience()
        {
            var typeProblem = new Tpn.TypeProblem2(
                 new WeakScopeConverter(),
                 DefaultRootScopePopulateScope(), _ => { });

            var moreType = typeProblem.builder.CreateType(typeProblem.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("more")), new WeakTypeDefinitionConverter());
            typeProblem.builder.CreatePublicMember(moreType, moreType, new NameKey("a"));
            typeProblem.builder.CreatePublicMember(moreType, moreType, new NameKey("b"));

            var lessType = typeProblem.builder.CreateType(typeProblem.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("less")), new WeakTypeDefinitionConverter());
            typeProblem.builder.CreatePublicMember(lessType, lessType, new NameKey("a"));

            var x = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("x"),
                OrType.Make<IKey, IError>(
                    new GenericNameKey(
                        new NameKey("method"),
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("more")),
                            OrType.Make<IKey,IError>(new NameKey("less"))})));

            var y = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("y"),
                OrType.Make<IKey, IError>(
                    new GenericNameKey(
                        new NameKey("method"),
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("less")),
                            OrType.Make<IKey,IError>(new NameKey("more"))})));


            var z = typeProblem.builder.CreatePublicMember(typeProblem.ModuleRoot, typeProblem.ModuleRoot, new NameKey("z"));
            typeProblem.builder.IsAssignedTo(z, x);
            typeProblem.builder.IsAssignedTo(z, y);

            var solution = typeProblem.Solve();

            var zType = solution.GetType(z).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.MethodType _);
            var zInput = zType.InputType.GetValue().Is1OrThrow();
            HasMember(zInput, new NameKey("a"));
            HasMember(zInput, new NameKey("b"));
            var zOutput = zType.OutputType.GetValue().Is1OrThrow();
            HasMember(zOutput, new NameKey("a"));
            HasMember(zOutput, new NameKey("b"));
        }

        // typc more {a;b;}
        // type less {a;}
        // type has-more {more item};
        // type has-less {less item};
        // method [has-more,has-less] x;
        // method [has-less,has-more] y;
        //
        // z =: x
        // z =: y
        //
        // z is method [has-more, has-more]
        [Fact]
        public void DeepMethodVarience()
        {
            var typeProblem = new Tpn.TypeProblem2(
                 new WeakScopeConverter(),
                 DefaultRootScopePopulateScope(), _ => { });

            var moreType = typeProblem.builder.CreateType(typeProblem.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("more")), new WeakTypeDefinitionConverter());
            typeProblem.builder.CreatePublicMember(moreType, moreType, new NameKey("a"));
            typeProblem.builder.CreatePublicMember(moreType, moreType, new NameKey("b"));

            var lessType = typeProblem.builder.CreateType(typeProblem.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("less")), new WeakTypeDefinitionConverter());
            typeProblem.builder.CreatePublicMember(lessType, lessType, new NameKey("a"));

            var hasMoreType = typeProblem.builder.CreateType(typeProblem.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("has-more")), new WeakTypeDefinitionConverter());
            typeProblem.builder.CreatePublicMember(hasMoreType, hasMoreType, new NameKey("item"), OrType.Make<IKey, IError>(new NameKey("more")));

            var hasLessType = typeProblem.builder.CreateType(typeProblem.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("has-less")), new WeakTypeDefinitionConverter());
            typeProblem.builder.CreatePublicMember(hasLessType, hasLessType, new NameKey("item"), OrType.Make<IKey, IError>(new NameKey("less")));

            var x = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("x"),
                OrType.Make<IKey, IError>(
                    new GenericNameKey(
                        new NameKey("method"),
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("has-more")),
                            OrType.Make<IKey,IError>(new NameKey("has-less"))})));

            var y = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("y"),
                OrType.Make<IKey, IError>(
                    new GenericNameKey(
                        new NameKey("method"),
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("has-less")),
                            OrType.Make<IKey,IError>(new NameKey("has-more"))})));


            var z = typeProblem.builder.CreatePublicMember(typeProblem.ModuleRoot, typeProblem.ModuleRoot, new NameKey("z"));
            typeProblem.builder.IsAssignedTo(z, x);
            typeProblem.builder.IsAssignedTo(z, y);

            var solution = typeProblem.Solve();

            var zType = solution.GetType(z).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.MethodType _);
            var zInput = zType.InputType.GetValue().Is1OrThrow();
            var inputItem = HasMember(zInput, new NameKey("item"));
            HasMember(inputItem, new NameKey("a"));
            HasMember(inputItem, new NameKey("b"));
            var zOutput = zType.OutputType.GetValue().Is1OrThrow();
            var outputItem = HasMember(zOutput, new NameKey("item"));
            HasMember(outputItem, new NameKey("a"));
            HasMember(outputItem, new NameKey("b"));
        }

        // TODO method varience with or-types

        // method [T] [T,T] x;
        // method [T1] [T1,T1] y;
        //
        // x and y are the same type
        [Fact]
        public void DoubleGenericMethodUnification() {

            var typeProblem = new Tpn.TypeProblem2(
             new WeakScopeConverter(),
             DefaultRootScopePopulateScope(), _ => { });

            var x = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("x"),
                OrType.Make<IKey, IError>(
                    new DoubleGenericNameKey(
                        new NameKey("method"),
                        new[] { new NameKey("T") },
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("T")),
                            OrType.Make<IKey,IError>(new NameKey("T"))})));

            var y = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("y"),
                OrType.Make<IKey, IError>(
                    new DoubleGenericNameKey(
                        new NameKey("method"),
                        new[] { new NameKey("T1") },
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("T1")),
                            OrType.Make<IKey,IError>(new NameKey("T1"))})));

            var solution = typeProblem.Solve();

            var xType = solution.GetType(x).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType _);
            var xInput = Assert.IsType<GenericTypeParameterPlacholder>(xType.inputType.GetValue().Is1OrThrow());
            var xOutput = Assert.IsType<GenericTypeParameterPlacholder>(xType.outputType.GetValue().Is1OrThrow());
            var xParameter = Assert.IsType<GenericTypeParameterPlacholder>(Assert.Single(xType.typeParameterDefinitions).GetValue().Is1OrThrow());

            Assert.Equal(xInput, xOutput);
            Assert.Equal(xInput, xParameter);
            Assert.Equal(xOutput, xParameter);

            var yType = solution.GetType(y).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType _);

            Assert.True(xType.TheyAreUs(yType, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
            Assert.True(yType.TheyAreUs(xType, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
        }

        // method [T] [T, method [TT] [TT,T]] x;
        // method [T1] [T1,method [T2] [T2,T1]] y;
        //
        // x and y are the same type
        [Fact]
        public void DoubleGenericMethodUnification2()
        {

            var typeProblem = new Tpn.TypeProblem2(
             new WeakScopeConverter(),
             DefaultRootScopePopulateScope(), _ => { });

            var x = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("x"),
                OrType.Make<IKey, IError>(
                    new DoubleGenericNameKey(
                        new NameKey("method"),
                        new[] { new NameKey("T") },
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("T")),
                            new DoubleGenericNameKey(
                                new NameKey("method"),
                                new[] { new NameKey("TT") },
                                new[] {
                                    OrType.Make<IKey,IError>(new NameKey("TT")),
                                    OrType.Make<IKey,IError>(new NameKey("T"))})})));

            var y = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("y"),
                OrType.Make<IKey, IError>(
                    new DoubleGenericNameKey(
                        new NameKey("method"),
                        new[] { new NameKey("T1") },
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("T1")),
                            new DoubleGenericNameKey(
                                new NameKey("method"),
                                new[] { new NameKey("T2") },
                                new[] {
                                    OrType.Make<IKey,IError>(new NameKey("T2")),
                                    OrType.Make<IKey,IError>(new NameKey("T1"))})})));

            var solution = typeProblem.Solve();

            var xType = solution.GetType(x).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType _);
            var yType = solution.GetType(y).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType _);

            Assert.True(xType.TheyAreUs(yType, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
        }

        // type [T] pair1 { method [T1] [T1,T] x;}
        // type [T] pair2 { method [T1] [T1,T] y; }
        //
        // x and y are not the same type, one's T is pair1's and the others is pair2's
        // that is to say not equal, they are assignable
        [Fact]
        public void DoubleGenericMethodsInGenericsDontUnify()
        {
            var typeProblem = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            var pairType1 = typeProblem.builder.CreateGenericType(
                typeProblem.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("pair1")),
                new[]{
                    new Tpn.TypeAndConverter(
                        new NameKey("T"),
                        new WeakTypeDefinitionConverter())},
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            var x1 = typeProblem.builder.CreatePublicMember(
                pairType1,
                pairType1,
                new NameKey("x"),
                OrType.Make<IKey, IError>(
                    new DoubleGenericNameKey(
                        new NameKey("method"),
                        new[] { new NameKey("T1") },
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("T1")),
                            OrType.Make<IKey,IError>(new NameKey("T"))})));

            var pairType2 = typeProblem.builder.CreateGenericType(
                typeProblem.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("pair2")),
                new[]{
                    new Tpn.TypeAndConverter(
                        new NameKey("T"),
                        new WeakTypeDefinitionConverter())},
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            var x2 = typeProblem.builder.CreatePublicMember(
                pairType2,
                pairType2,
                new NameKey("x"),
                OrType.Make<IKey, IError>(
                    new DoubleGenericNameKey(
                        new NameKey("method"),
                        new[] { new NameKey("T1") },
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("T1")),
                            OrType.Make<IKey,IError>(new NameKey("T"))})));

            var solution = typeProblem.Solve();

            var x1Type = solution.GetType(x1).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType _);
            var x2Type = solution.GetType(x2).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType _);

            Assert.NotEqual(x1Type, x2Type);
            Assert.True(x1Type.TheyAreUs(x2Type, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
            Assert.True(x2Type.TheyAreUs(x1Type, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

        }

        // type chicken { eggs; } 
        //
        // method [T] [T,T] input {
        //  input =: chicken x;
        // }
        //
        // so T is contrained to be a chicken
        [Fact]
        public void GenericMethod()
        {
            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            // type chicken { eggs; } 
            var chickenType = problem.builder.CreateType(problem.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());
            problem.builder.CreatePublicMember(chickenType, chickenType, new NameKey("eggs"));

            // method [T] [T,T] input {
            var (method,_,_) = problem.builder.CreateGenericMethod(
                problem.ModuleRoot,
                x => OrType.Make<Tpn.TypeProblem2.TypeReference,IError>( problem.builder.CreateTypeReference(problem.ModuleRoot, new NameKey("T"), new WeakTypeReferenceConverter())),
                x => OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(problem.builder.CreateTypeReference(problem.ModuleRoot, new NameKey("T"), new WeakTypeReferenceConverter())),
                "input",
                new WeakMethodDefinitionConverter(new Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>>(new List<IOrType<IBox<IFrontendCodeElement>, IError>>())),
                new[] {
                    new Tpn.TypeAndConverter(new NameKey("T"), new WeakTypeDefinitionConverter())
                });

            // input =: chicken x;
            var x = problem.builder.CreatePrivateMember(method, method, new NameKey("x"), OrType.Make<IKey, Error>(new NameKey("chicken")));
            problem.builder.IsAssignedTo(method.Input.GetOrThrow(), x);

            var result = problem.Solve();

            var methodResult = method.Converter.Convert(result, method).Is4OrThrow();
            var parameter = Assert.Single(methodResult.TypeParameterDefinitions).Is1OrThrow();

            var noEggs = new HasMembersType(new WeakScope(new List<WeakMemberDefinition> { }));
            Assert.False(parameter.TheyAreUs(noEggs, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

            var hasEggs = new HasMembersType(new WeakScope(new List<WeakMemberDefinition> { new WeakMemberDefinition(Access.ReadWrite, new NameKey("eggs"), new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))) }));
            Assert.True(parameter.TheyAreUs(hasEggs, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
        }

        // method [T] [T,T] x;
        // method [T1] [T1,T1] y;
        //
        // z =: x
        // z =: y
        //
        // z is double generic
        [Fact]
        public void DoubleGenericFlow()
        {

            var typeProblem = new Tpn.TypeProblem2(
             new WeakScopeConverter(),
             DefaultRootScopePopulateScope(), _ => { });

            var x = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("x"),
                OrType.Make<IKey, IError>(
                    new DoubleGenericNameKey(
                        new NameKey("method"),
                        new[] { new NameKey("T") },
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("T")),
                            OrType.Make<IKey,IError>(new NameKey("T"))})));

            var y = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("y"),
                OrType.Make<IKey, IError>(
                    new DoubleGenericNameKey(
                        new NameKey("method"),
                        new[] { new NameKey("T1") },
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("T1")),
                            OrType.Make<IKey,IError>(new NameKey("T1"))})));


            var z = typeProblem.builder.CreatePublicMember(typeProblem.ModuleRoot, typeProblem.ModuleRoot, new NameKey("z"));
            typeProblem.builder.IsAssignedTo(z, x); 
            typeProblem.builder.IsAssignedTo(z, y);

            var solution = typeProblem.Solve();

            var zType = solution.GetType(z).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType _);
            var zInput = Assert.IsType<GenericTypeParameterPlacholder>(zType.inputType.GetValue().Is1OrThrow());
            var zOutput = Assert.IsType<GenericTypeParameterPlacholder>(zType.outputType.GetValue().Is1OrThrow());
            var zParameter = Assert.IsType<GenericTypeParameterPlacholder>(Assert.Single(zType.typeParameterDefinitions).GetValue().Is1OrThrow());

            var yType = solution.GetType(y).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType _);


            Assert.True(zType.TheyAreUs(yType, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
        }

        // .. I think I need more tests around this... but I'm failing to come up with one
        // method [T] [method [T1,T2] [T1,T2], method [T3] [T3, T1]]



        // what about...
        //
        // type chicken { eggs; } 
        //
        // method [T] [T,T] input {
        //  input =: chicken x;
        // } =: my-method
        //
        // my-method =: method [T] [T,T] z
        //
        // method [T] [T,T] yolo 
        //
        // my-method and yolo are not the same type
        // my-method is really method [T:chicken] [T,T]
        // but I think probably the types will unify 
        // we don't know the constraint until we flow
        [Fact]
        public void GenericMethodDoesUnifyWithGenericMethodType()
        {
            var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), DefaultRootScopePopulateScope(), _ => { });

            // type chicken { eggs; } 
            var chickenType = problem.builder.CreateType(problem.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());
            problem.builder.CreatePublicMember(chickenType, chickenType, new NameKey("eggs"));

            // method [T] [T,T] input {
            var (method, _, _) = problem.builder.CreateGenericMethod(
                problem.ModuleRoot,
                x => OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(problem.builder.CreateTypeReference(problem.ModuleRoot, new NameKey("T"), new WeakTypeReferenceConverter())),
                x => OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(problem.builder.CreateTypeReference(problem.ModuleRoot, new NameKey("T"), new WeakTypeReferenceConverter())),
                "input",
                new WeakMethodDefinitionConverter(new Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>>(new List<IOrType<IBox<IFrontendCodeElement>, IError>>())),
                new[] {
                    new Tpn.TypeAndConverter(new NameKey("T"), new WeakTypeDefinitionConverter())
                });


            // input =: chicken x;
            var x = problem.builder.CreatePrivateMember(method, method, new NameKey("x"), OrType.Make<IKey, Error>(new NameKey("chicken")));
            problem.builder.IsAssignedTo(method.Input.GetOrThrow(), x);

            //} =: my-method
            var myMethod = problem.builder.CreatePublicMember(problem.ModuleRoot, problem.ModuleRoot, new NameKey("my-method"));
            
            // this is copied from here:
            // {8E138F8D-53AA-4D6A-B337-64CAFED23391}
            var inputMember = problem.builder.GetInput(myMethod);
            problem.builder.IsAssignedTo(inputMember, method.Input.GetOrThrow()/*lazy GetOrThrow*/);

            var returnsMember = problem.builder.GetReturns(myMethod);
            problem.builder.IsAssignedTo(method.Returns.GetOrThrow()/*lazy GetOrThrow*/, returnsMember);

            var genericParameters = new[] { new NameKey("T") };
            var dict = problem.builder.HasGenerics(myMethod, genericParameters);
            foreach (var key in genericParameters)
            {
                problem.builder.AssertIs(dict[key], method.Generics[key]/*lazy GetOrThrow*/);
            }

            // my-method =: method [T] [T,T] z
            var z = problem.builder.CreatePublicMember(
                problem.ModuleRoot,
                problem.ModuleRoot,
                new NameKey("z"),
                OrType.Make<IKey, IError>(
                    new DoubleGenericNameKey(
                        new NameKey("method"),
                        new[] { new NameKey("T") },
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("T")),
                            OrType.Make<IKey,IError>(new NameKey("T"))})));
            problem.builder.IsAssignedTo(myMethod, z);

            var yolo = problem.builder.CreatePublicMember(
                problem.ModuleRoot,
                problem.ModuleRoot,
                new NameKey("yolo"),
                OrType.Make<IKey, IError>(
                    new DoubleGenericNameKey(
                        new NameKey("method"),
                        new[] { new NameKey("T") },
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("T")),
                            OrType.Make<IKey,IError>(new NameKey("T"))})));

            var result = problem.Solve();

            var noEggs = new HasMembersType(new WeakScope(new List<WeakMemberDefinition> { }));
            var hasEggs = new HasMembersType(new WeakScope(new List<WeakMemberDefinition> { new WeakMemberDefinition(Access.ReadWrite, new NameKey("eggs"), new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.AnyType()))) }));


            var methodResult = method.Converter.Convert(result, method).Is4OrThrow();
            var parameter = Assert.Single(methodResult.TypeParameterDefinitions).Is1OrThrow();
            Assert.False(parameter.TheyAreUs(noEggs, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
            Assert.True(parameter.TheyAreUs(hasEggs, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

            var myMethodReturns = result.GetType(myMethod).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType _).typeParameterDefinitions.Single().GetValue().Is1OrThrow();
            Assert.False(myMethodReturns.TheyAreUs(noEggs, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
            Assert.True(myMethodReturns.TheyAreUs(hasEggs, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

            var yoloReturns = result.GetType(yolo).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType _).typeParameterDefinitions.Single().GetValue().Is1OrThrow();
            Assert.True(yoloReturns.TheyAreUs(noEggs, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
            Assert.True(yoloReturns.TheyAreUs(hasEggs, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());
        }

        // type chicken {eggs;}
        // 
        // method [T] [T,T] x;
        // method [T1] [T1,T1] y;
        // method [T2] [T2, chicken] z
        //
        // x =: z
        //
        // x and y are different types
        // they pick up different contraints
        // ...
        // I don't know what happenss here
        // "x" becomes "method [T:chicken] [T,T]" and "z" becomes "method [T2:chicken] [2T:chicken, chicken]" ??
        // ...
        // in general I've been treating method [T] [T,T] x as method [T:infer] [T,T] x
        [Fact]
        public void Damning()
        {

            var typeProblem = new Tpn.TypeProblem2(
             new WeakScopeConverter(),
             DefaultRootScopePopulateScope(), _ => { });

            // type chicken { eggs; } 
            var chickenType = typeProblem.builder.CreateType(typeProblem.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());
            typeProblem.builder.CreatePublicMember(chickenType, chickenType, new NameKey("eggs"));

            var x = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("x"),
                OrType.Make<IKey, IError>(
                    new DoubleGenericNameKey(
                        new NameKey("method"),
                        new[] { new NameKey("T") },
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("T")),
                            OrType.Make<IKey,IError>(new NameKey("T"))})));

            var y = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("y"),
                OrType.Make<IKey, IError>(
                    new DoubleGenericNameKey(
                        new NameKey("method"),
                        new[] { new NameKey("T1") },
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("T1")),
                            OrType.Make<IKey,IError>(new NameKey("T1"))})));


            var z = typeProblem.builder.CreatePublicMember(
                typeProblem.ModuleRoot,
                typeProblem.ModuleRoot,
                new NameKey("z"),
                OrType.Make<IKey, IError>(
                    new DoubleGenericNameKey(
                        new NameKey("method"),
                        new[] { new NameKey("T2") },
                        new[] {
                            OrType.Make<IKey,IError>(new NameKey("T2")),
                            OrType.Make<IKey,IError>(new NameKey("chicken"))})));

            typeProblem.builder.IsAssignedTo(x, z);

            var solution = typeProblem.Solve();

            var xType = solution.GetType(x).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType _);
            var yType = solution.GetType(y).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType _);
            var zType = solution.GetType(z).Is1OrThrow().SafeCastTo(out Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType _);
            Assert.False(xType.TheyAreUs(yType, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).Is1OrThrow());

        }

        [Fact]
        public void GenericContainsSelfContainsDoubleGeneric()
        {

            // type[node-t] node {node[node-t] next; method [T] [T,T] z}
            // type chicken {}
            // node[chicken] thing;

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(),
                DefaultRootScopePopulateScope(), _ => { });

            var type = x.builder.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("node")),
                new[]{
                    new Tpn.TypeAndConverter(new NameKey("node-t"), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
            );

            x.builder.CreatePublicMember(type, type, new NameKey("next"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("node-t"))
            })));
            var z = x.builder.CreatePublicMember(
                        type,
                        type,
                        new NameKey("z"),
                        OrType.Make<IKey, IError>(
                            new DoubleGenericNameKey(
                                new NameKey("method"),
                                new[] { new NameKey("T") },
                                new[] {
                                        OrType.Make<IKey,IError>(new NameKey("T")),
                                        OrType.Make<IKey,IError>(new NameKey("T"))})));

            x.builder.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());


            var thing = x.builder.CreatePublicMember(x.ModuleRoot, x.ModuleRoot, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("chicken"))
            })));

            var solution = x.Solve();
            var obj = x.ModuleRoot.Converter.Convert(solution, x.ModuleRoot).Is2OrThrow().Scope.Is1OrThrow();


            var thingResult = obj.membersList.Single(x => x.Key.Equals(new NameKey("thing")));
            var thingResultType = MemberToType(thingResult);

            //HasCount(1, thingResultType);
            var nextResult = HasMember(thingResultType, new NameKey("next"));
            //var nextResultType = MemberToType(nextResult);
            //HasCount(1, nextResultType);
            HasMember(nextResult, new NameKey("next"));

            Equal(thingResultType, nextResult);
        }
    }
}
