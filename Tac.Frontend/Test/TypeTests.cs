using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.SemanticModel;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Xunit;

namespace Tac.Frontend.Test
{
    public class TypeTests
    {
        #region Help

        // a2 is a1
        // a1 is not b2
        // b2 is b1
        // b1 is not b2

        private IFrontendType A1()
        {
            var member1Key = new NameKey("am1");
            var member1 = new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, member1Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new NumberType()))));
            var type1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member1 }));

            return type1;
        }

        private IFrontendType A2()
        {
            var member1Key = new NameKey("am1");
            var member1 = new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, member1Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new NumberType()))));

            var member2Key = new NameKey("am2");
            var member2 = new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, member2Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new NumberType()))));

            var type1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member1, member2 }));

            return type1;
        }

        private IFrontendType B1()
        {
            var member1Key = new NameKey("bm1");
            var member1 = new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, member1Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new NumberType()))));
            var type1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member1 }));

            return type1;
        }

        private IFrontendType B2()
        {
            var member1Key = new NameKey("bm1");
            var member1 = new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, member1Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new NumberType()))));

            var member2Key = new NameKey("bm2");
            var member2 = new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, member2Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new NumberType()))));

            var type1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member1, member2 }));

            return type1;
        }

        private IFrontendType B3()
        {
            var member1Key = new NameKey("bm1");
            var member1 = new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, member1Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new NumberType()))));

            var member2Key = new NameKey("bm2");
            var member2 = new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, member2Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new NumberType()))));


            var member3Key = new NameKey("bm3");
            var member3 = new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, member3Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new NumberType()))));

            var type1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member1, member2, member3 }));

            return type1;
        }

        #endregion


        [Fact]
        public void A2IsA1()
        {
            Assert.True(A1().TheyAreUs(A2(), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        [Fact]
        public void A1IsNotA2()
        {
            Assert.False(A2().TheyAreUs(A1(), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }


        [Fact]
        public void B2IsB1()
        {
            Assert.True(B1().TheyAreUs(B2(), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        [Fact]
        public void B1IsNotB2()
        {
            Assert.False(B2().TheyAreUs(B1(), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        [Fact]
        public void PrimitivesAreThemselves()
        {

            var left = new IFrontendType[] { new NumberType(), new StringType(), new BooleanType() };
            var right = new IFrontendType[] { new NumberType(), new StringType(), new BooleanType() };

            for (int i = 0; i < left.Length; i++)
            {
                for (int j = 0; j < right.Length; j++)
                {
                    if (i == j)
                    {
                        Assert.True(left[i].TheyAreUs(left[j], new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
                    }
                    else
                    {
                        Assert.False(left[i].TheyAreUs(left[j], new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
                    }
                }
            }
        }

        [Fact]
        public void OrTypesWork()
        {

            var or = new FrontEndOrType(OrType.Make<IFrontendType, IError>(new NumberType()), OrType.Make<IFrontendType, IError>(new StringType()));

            Assert.True(or.TheyAreUs(new NumberType(), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.True(or.TheyAreUs(new StringType(), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.False(new NumberType().TheyAreUs(or, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.False(new StringType().TheyAreUs(or, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        // more interesting or type test (co/contra varience)
        [Fact]
        public void OrTypesAreVarient()
        {
            var a1 = A1();
            var a2 = A2();
            var b1 = B1();
            var b2 = B2();

            var or1 = new FrontEndOrType(OrType.Make<IFrontendType, IError>(a1), OrType.Make<IFrontendType, IError>(b1));
            var or2 = new FrontEndOrType(OrType.Make<IFrontendType, IError>(a2), OrType.Make<IFrontendType, IError>(b2));

            Assert.True(or1.TheyAreUs(or2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.False(or2.TheyAreUs(or1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        [Fact]
        public void OrTypesJustNeedBothToMatchOne()
        {
            var a1 = A1();
            var b1 = B1();
            var b2 = B2();
            var b3 = B3();

            var or1 = new FrontEndOrType(OrType.Make<IFrontendType, IError>(a1), OrType.Make<IFrontendType, IError>(b1));
            var or2 = new FrontEndOrType(OrType.Make<IFrontendType, IError>(b2), OrType.Make<IFrontendType, IError>(b3));

            // this works since both b2 and b3 are b1
            Assert.True(or1.TheyAreUs(or2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            // this does not work because niether b2 nor b3 are a1
            Assert.False(or2.TheyAreUs(or1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        [Fact]
        public void OrTypesOrderDoesNotMatter()
        {
            var a1 = A1();
            var b1 = B1();

            var or1 = new FrontEndOrType(OrType.Make<IFrontendType, IError>(a1), OrType.Make<IFrontendType, IError>(b1));
            var or2 = new FrontEndOrType(OrType.Make<IFrontendType, IError>(b1), OrType.Make<IFrontendType, IError>(a1));

            Assert.True(or1.TheyAreUs(or2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.True(or2.TheyAreUs(or1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

        }

        [Fact]
        public void BasicTypes()
        {
            var member1Key = new NameKey("x");
            var member1 = new Box<WeakMemberDefinition>();
            var type1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member1 }));
            member1.Fill(new WeakMemberDefinition(Access.ReadWrite, member1Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new NumberType()))));

            var member2Key = new NameKey("x");
            var member2 = new Box<WeakMemberDefinition>();
            var member3Key = new NameKey("y");
            var member3 = new Box<WeakMemberDefinition>();
            var type2 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member2, member3 }));
            member2.Fill(new WeakMemberDefinition(Access.ReadWrite, member2Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new NumberType()))));
            member3.Fill(new WeakMemberDefinition(Access.ReadWrite, member3Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new NumberType()))));

            Assert.True(type1.TheyAreUs(type2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.False(type2.TheyAreUs(type1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        //Type A { A x } is Type B { B x }
        [Fact]
        public void InterestingCase1()
        {
            var member1Key = new NameKey("x");
            var member1 = new Box<WeakMemberDefinition>();
            var type1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member1 }));
            member1.Fill(new WeakMemberDefinition(Access.ReadWrite, member1Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(type1))));

            var member2Key = new NameKey("x");
            var member2 = new Box<WeakMemberDefinition>();
            var type2 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member2 }));
            member2.Fill(new WeakMemberDefinition(Access.ReadWrite, member2Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(type2))));

            Assert.True(type1.TheyAreUs(type2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.True(type2.TheyAreUs(type1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        //Type A { A x } is not Type B { B x int y }
        [Fact]
        public void InterestingCase2()
        {
            var member1Key = new NameKey("x");
            var member1 = new Box<WeakMemberDefinition>();
            var type1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member1 }));
            member1.Fill(new WeakMemberDefinition(Access.ReadWrite, member1Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(type1))));

            var member2Key = new NameKey("x");
            var member2 = new Box<WeakMemberDefinition>();
            var member3Key = new NameKey("y");
            var member3 = new Box<WeakMemberDefinition>();
            var type2 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member2, member3 }));
            member2.Fill(new WeakMemberDefinition(Access.ReadWrite, member2Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(type2))));
            member3.Fill(new WeakMemberDefinition(Access.ReadWrite, member3Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new NumberType()))));

            Assert.False(type1.TheyAreUs(type2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.False(type2.TheyAreUs(type1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }


        //Type A { B x } is Type B { A x }
        [Fact]
        public void InterestingCase3()
        {
            var member1Key = new NameKey("x");
            var member1 = new Box<WeakMemberDefinition>();
            var type1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member1 }));

            var member2Key = new NameKey("x");
            var member2 = new Box<WeakMemberDefinition>();
            var type2 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member2 }));

            member1.Fill(new WeakMemberDefinition(Access.ReadWrite, member1Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(type2))));
            member2.Fill(new WeakMemberDefinition(Access.ReadWrite, member2Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(type1))));

            Assert.True(type1.TheyAreUs(type2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.True(type2.TheyAreUs(type1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        // Type A { B x } is Type B { C x } is Type C { A x }
        [Fact]
        public void InterestingCase4()
        {
            var member1Key = new NameKey("x");
            var member1 = new Box<WeakMemberDefinition>();
            var type1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member1 }));

            var member2Key = new NameKey("x");
            var member2 = new Box<WeakMemberDefinition>();
            var type2 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member2 }));

            var member3Key = new NameKey("x");
            var member3 = new Box<WeakMemberDefinition>();
            var type3 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member3 }));

            member1.Fill(new WeakMemberDefinition(Access.ReadWrite, member1Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(type2))));
            member2.Fill(new WeakMemberDefinition(Access.ReadWrite, member2Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(type3))));
            member3.Fill(new WeakMemberDefinition(Access.ReadWrite, member3Key, new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(type1))));

            Assert.True(type1.TheyAreUs(type2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.True(type2.TheyAreUs(type1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.True(type2.TheyAreUs(type3, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.True(type3.TheyAreUs(type2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.True(type1.TheyAreUs(type3, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.True(type3.TheyAreUs(type1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        // this is not true
        // Type T2 { A2 a, B2 b } is Type T1 { A1 a, B1 b } where A2 is A1 and B2 is B1
        [Fact]
        public void TypeDontHaveVariance()
        {
            var a1 = A1();
            var a2 = A2();
            var b1 = B1();
            var b2 = B2();

            var t1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> {
                new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, new NameKey("a"), new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(a1)))),
                new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, new NameKey("b"), new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(b1))))}));

            var t2 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { 
                new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, new NameKey("a"), new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(a2)))),
                new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, new NameKey("b"), new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(b2))))}));


            Assert.False(t2.TheyAreUs(t1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.False(t1.TheyAreUs(t2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        // method co/contra-variance 
        // m1<a2,b1> is m2<a1,b2>
        [Fact]
        public void MethodVariance()
        {
            var a1 = A1();
            var a2 = A2();
            var b1 = B1();
            var b2 = B2();

            var m1 = new MethodType(OrType.Make<IFrontendType, IError>(a2),OrType.Make<IFrontendType, IError>(b1));

            var m2 = new MethodType(OrType.Make<IFrontendType, IError>(a1), OrType.Make<IFrontendType, IError>(b2));

            Assert.True(m2.TheyAreUs(m1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.False(m1.TheyAreUs(m2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        [Fact]
        public void Any()
        {
            var a1 = A1();
            var a2 = A2();
            var b1 = B1();
            var b2 = B2();
            var m1 = new MethodType(OrType.Make<IFrontendType, IError>(a2), OrType.Make<IFrontendType, IError>(b1));
            var m2 = new MethodType(OrType.Make<IFrontendType, IError>(a1), OrType.Make<IFrontendType, IError>(b2));
            var t1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> {
                new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, new NameKey("a"), new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(a1)))),
                new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, new NameKey("b"), new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(b1))))}));
            var t2 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> {
                new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, new NameKey("a"), new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(a2)))),
                new Box<WeakMemberDefinition>(new WeakMemberDefinition(Access.ReadWrite, new NameKey("b"), new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(b2))))}));
            var or1 = new FrontEndOrType(OrType.Make<IFrontendType, IError>(a1), OrType.Make<IFrontendType, IError>(b1));
            var or2 = new FrontEndOrType(OrType.Make<IFrontendType, IError>(b1), OrType.Make<IFrontendType, IError>(a1));

            var any = new AnyType();

            Assert.All(new[] { a1, a2, b1, b2, m1, m2, t1, t2, or1, or2 }, x => any.TheyAreUs(x, new List<(IFrontendType, IFrontendType)>()));
        }
    }
}

