using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.SemanticModel;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Xunit;

namespace Tac.Frontend.Test
{
    public class TypeTests
    {

        [Fact]
        public void PrimitivesAreThemselves() {

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
        public void OrTypesWork() {

            var or = new FrontEndOrType(OrType.Make<IFrontendType, IError>(new NumberType()), OrType.Make<IFrontendType, IError>(new StringType()));

            Assert.True(or.TheyAreUs(new NumberType(), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.True(or.TheyAreUs(new StringType(), new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.False(new NumberType().TheyAreUs(or, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.False(new StringType().TheyAreUs(or, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }


        // A { A x } is B { B x }
        [Fact]
        public void InterestingCase1() {
            var member1Key = new NameKey("x");
            var member1 =new Box<WeakMemberDefinition>();
            var type1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member1 }));
            member1.Fill(new WeakMemberDefinition(false, member1Key, OrType.Make<IBox<IFrontendType>,IError>(new Box<IFrontendType>(type1))));

            var member2Key = new NameKey("x");
            var member2 = new Box<WeakMemberDefinition>();
            var type2 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member2 }));
            member2.Fill(new WeakMemberDefinition(false, member2Key, OrType.Make<IBox<IFrontendType>, IError>(new Box<IFrontendType>(type2))));

            Assert.True(type1.TheyAreUs(type2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.True(type2.TheyAreUs(type1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        // A { A x } is not B { B x int y }
        [Fact]
        public void InterestingCase2()
        {
            var member1Key = new NameKey("x");
            var member1 = new Box<WeakMemberDefinition>();
            var type1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member1 }));
            member1.Fill(new WeakMemberDefinition(false, member1Key, OrType.Make<IBox<IFrontendType>, IError>(new Box<IFrontendType>(type1))));

            var member2Key = new NameKey("x");
            var member2 = new Box<WeakMemberDefinition>(); 
            var member3Key = new NameKey("x");
            var member3 = new Box<WeakMemberDefinition>();
            var type2 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member2, member3 }));
            member2.Fill(new WeakMemberDefinition(false, member2Key, OrType.Make<IBox<IFrontendType>, IError>(new Box<IFrontendType>(type2))));
            member3.Fill(new WeakMemberDefinition(false, member3Key, OrType.Make<IBox<IFrontendType>, IError>(new Box<IFrontendType>(new NumberType()))));

            Assert.False(type1.TheyAreUs(type2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.False(type2.TheyAreUs(type1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }


        // A { B x } is B { A x }
        [Fact]
        public void InterestingCase3() {
            var member1Key = new NameKey("x");
            var member1 = new Box<WeakMemberDefinition>();
            var type1 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member1 }));

            var member2Key = new NameKey("x");
            var member2 = new Box<WeakMemberDefinition>();
            var type2 = new HasMembersType(new WeakScope(new List<IBox<WeakMemberDefinition>> { member2 }));

            member1.Fill(new WeakMemberDefinition(false, member1Key, OrType.Make<IBox<IFrontendType>, IError>(new Box<IFrontendType>(type2))));
            member2.Fill(new WeakMemberDefinition(false, member2Key, OrType.Make<IBox<IFrontendType>, IError>(new Box<IFrontendType>(type1))));

            Assert.True(type1.TheyAreUs(type2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.True(type2.TheyAreUs(type1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }

        // A { B x } is B { C x } is C { A x }
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

            member1.Fill(new WeakMemberDefinition(false, member1Key, OrType.Make<IBox<IFrontendType>, IError>(new Box<IFrontendType>(type2))));
            member2.Fill(new WeakMemberDefinition(false, member2Key, OrType.Make<IBox<IFrontendType>, IError>(new Box<IFrontendType>(type3))));
            member3.Fill(new WeakMemberDefinition(false, member3Key, OrType.Make<IBox<IFrontendType>, IError>(new Box<IFrontendType>(type1))));

            Assert.True(type1.TheyAreUs(type2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.True(type2.TheyAreUs(type1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.True(type2.TheyAreUs(type3, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.True(type3.TheyAreUs(type2, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());

            Assert.True(type1.TheyAreUs(type3, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
            Assert.True(type3.TheyAreUs(type1, new List<(IFrontendType, IFrontendType)>()).Is1OrThrow());
        }


        // method co/contra-variance 
        // any
    }
}
