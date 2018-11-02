using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;
using Tac.TestCases;
using Tac.TestCases.Help;

namespace Tac.Tests.Samples
{
    public class PointObject : ITestCase
    {
        public string Text => @"object {
                            5 =: x ;
                            2 =: y ;
                        }";

        public ICodeElement[] CodeElements
        {
            get
            {
                var keyX = new NameKey("x");
                var localX = new TestMemberDefinition(keyX,new TestAnyType(), false);
                var keyY = new NameKey("y");
                var localY = new TestMemberDefinition(keyY, new TestAnyType(), false);
                                
                return new ICodeElement[] {
                    new TestObjectDefiniton(
                        new FinalizedScope(
                        new Dictionary<IKey, IMemberDefinition> {
                            { keyX, localX },
                            { keyY, localY }
                        }),
                        new TestAssignOperation[]{
                            new TestAssignOperation(
                                new TestConstantNumber(5),
                                new TestMemberReferance(localX)),
                            new TestAssignOperation(
                                new TestConstantNumber(2),
                                new TestMemberReferance(localY))
                        })
                };
            }
        }
    }
}