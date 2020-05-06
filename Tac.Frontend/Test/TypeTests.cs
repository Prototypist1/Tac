using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
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
    }
}
