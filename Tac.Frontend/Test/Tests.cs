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

        private static void Equal(IFrontendType a, IFrontendType b) {
            Assert.True(a.TheyAreUs(b, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow() && b.TheyAreUs(a, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
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
            var hello = x.CreateType(x.ModuleRoot, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("Hello")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(hello, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            x.CreatePublicMember(hello, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));
            var solution = x.Solve();

            var resultHello = solution.GetExplicitType(hello).GetValue().Is1OrThrow();

            HasMember(resultHello.FrontendType(), new NameKey("x"));
            HasMember(resultHello.FrontendType(), new NameKey("y"));
        }


        [Fact]
        public void AddMethod()
        {
            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var hello = x.CreateType(x.ModuleRoot, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("hello")), new WeakTypeDefinitionConverter());
            x.CreatePublicMember(hello, new NameKey("x"), new WeakMemberDefinitionConverter(false,new NameKey("x")));
            x.CreatePublicMember(hello, new NameKey("y"), new WeakMemberDefinitionConverter(false,new NameKey("y")));

            var input = x.CreateValue(x.ModuleRoot, new NameKey("hello"), new PlaceholderValueConverter());
            var method = x.CreateMethod(x.ModuleRoot, "input", new WeakMethodDefinitionConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()),false), new WeakMemberDefinitionConverter(false, new NameKey("input")));

            var input_x = x.CreateHopefulMember(method.Input(), new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var input_y = x.CreateHopefulMember(method.Input(), new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

            var method_x = x.CreatePrivateMember(method, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var method_y = x.CreatePrivateMember(method, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));

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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var m1 = x.CreatePublicMember(x.ModuleRoot, new NameKey("m1"), new WeakMemberDefinitionConverter(false, new NameKey("m1")));
            x.CreateHopefulMember(m1, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var m2 = x.CreatePublicMember(x.ModuleRoot, new NameKey("m2"), new WeakMemberDefinitionConverter(false, new NameKey("m2")));
            x.CreateHopefulMember(m2, new NameKey("y"), new WeakMemberDefinitionConverter(false, new NameKey("y")));
            var m3 = x.CreatePublicMember(x.ModuleRoot, new NameKey("m3"), new WeakMemberDefinitionConverter(false, new NameKey("m3")));
            var m4 = x.CreatePublicMember(x.ModuleRoot, new NameKey("m4"), new WeakMemberDefinitionConverter(false, new NameKey("m4")));
            var m5 = x.CreatePublicMember(x.ModuleRoot, new NameKey("m5"), new WeakMemberDefinitionConverter(false, new NameKey("m5")));

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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var m1 = x.CreatePublicMember(x.ModuleRoot, new NameKey("m1"), new WeakMemberDefinitionConverter(false, new NameKey("m1")));
            var m2 = x.CreatePublicMember(x.ModuleRoot, new NameKey("m2"), new WeakMemberDefinitionConverter(false, new NameKey("m2")));
            var m3 = x.CreatePublicMember(x.ModuleRoot, new NameKey("m3"), new WeakMemberDefinitionConverter(false, new NameKey("m3")));
            var m4 = x.CreatePublicMember(x.ModuleRoot, new NameKey("m4"), new WeakMemberDefinitionConverter(false, new NameKey("m4")));
            x.CreateHopefulMember(m4, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var m5 = x.CreatePublicMember(x.ModuleRoot, new NameKey("m5"), new WeakMemberDefinitionConverter(false, new NameKey("m5")));
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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var m1 = x.CreatePublicMember(x.ModuleRoot, new NameKey("m1"), new WeakMemberDefinitionConverter(false, new NameKey("m1")));
            x.CreateHopefulMember(m1, new NameKey("x"), new WeakMemberDefinitionConverter(false, new NameKey("x")));
            var m2 = x.CreatePublicMember(x.ModuleRoot, new NameKey("m2"), new WeakMemberDefinitionConverter(false, new NameKey("m2")));
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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var pairType = x.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("pair")), 
                new []{ 
                    new Tpn.TypeAndConverter(
                        OrType.Make<NameKey, ImplicitKey>(new NameKey("T")), 
                        new WeakTypeDefinitionConverter())}, 
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.CreatePublicMember(pairType, new NameKey("x"),
                OrType.Make<IKey, IError>(new NameKey("T")), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var chickenType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            x.CreatePublicMember(chickenType, new NameKey("eggs"), new WeakMemberDefinitionConverter(false, new NameKey("eggs")));

            var chickenPair = x.CreatePublicMember(x.ModuleRoot, new NameKey("x"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })), new WeakMemberDefinitionConverter(false, new NameKey("x")));

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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var type = x.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("node")),
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("node-t")), new WeakTypeDefinitionConverter())
                }, 
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
            );

            x.CreatePublicMember(type, new NameKey("next"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("node-t"))
            })), new WeakMemberDefinitionConverter(false, new NameKey("next")));

            x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());


            var thing = x.CreatePublicMember(x.ModuleRoot, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("node"), new IOrType<IKey, IError>[] {
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
        public void GenericCircular()
        {

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var left = x.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("left")), 
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("left-t")), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...? {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.CreatePublicMember(left, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("right"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("left-t"))
            })), new WeakMemberDefinitionConverter(false, new NameKey("thing")));

            var right = x.CreateGenericType(
                x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("right")), 
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("right-t")), new WeakTypeDefinitionConverter())
                }, 
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...? {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.CreatePublicMember(right, new NameKey("thing"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("left"), new IOrType<IKey, IError>[] {
                OrType.Make<IKey, IError>(new NameKey("right-t"))
            })), new WeakMemberDefinitionConverter(false, new NameKey("thing")));

            x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            var leftMember = x.CreatePublicMember(x.ModuleRoot, new NameKey("left-member"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("left"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })), new WeakMemberDefinitionConverter(false, new NameKey("left-member")));

            var rightMember = x.CreatePublicMember(x.ModuleRoot, new NameKey("right-member"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("right"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })), new WeakMemberDefinitionConverter(false, new NameKey("right-member")));

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

            var x = new Tpn.TypeProblem2(new WeakScopeConverter(), new WeakModuleConverter(new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>(new List<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>()), new NameKey("test module")));

            var pairType = x.CreateGenericType(
                x.ModuleRoot,
                OrType.Make<NameKey, ImplicitKey>(new NameKey("pair")), 
                new[]{
                    new Tpn.TypeAndConverter(OrType.Make<NameKey, ImplicitKey>(new NameKey("T")), new WeakTypeDefinitionConverter())
                },
                new WeakTypeDefinitionConverter() // this is so werid shouldn' these use a convert that converts to a generic type...?  {0A2986D9-59AA-460C-B946-FF20B15FCEE6}
                );

            x.CreatePublicMember(pairType, new NameKey("x"),
                OrType.Make<IKey, IError>(new NameKey("T")), new WeakMemberDefinitionConverter(false, new NameKey("x")));

            var chickenType = x.CreateType(x.ModuleRoot, OrType.Make<NameKey, ImplicitKey>(new NameKey("chicken")), new WeakTypeDefinitionConverter());

            x.CreatePublicMember(chickenType, new NameKey("eggs"), new WeakMemberDefinitionConverter(false, new NameKey("eggs")));

            var xMember = x.CreatePublicMember(x.ModuleRoot, new NameKey("x"), OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new GenericNameKey(new NameKey("pair"), new IOrType<IKey, IError>[] { OrType.Make<IKey, IError>(new NameKey("chicken")) })) })), new WeakMemberDefinitionConverter(false, new NameKey("x")));

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
    }
}
