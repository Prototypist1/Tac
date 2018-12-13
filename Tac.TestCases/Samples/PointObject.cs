using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
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
                var localX = new MemberDefinition(keyX, new TypeReferance(new AnyType()), false);
                var keyY = new NameKey("y");
                var localY = new MemberDefinition(keyY, new TypeReferance(new AnyType()), false);
                                
                return new ICodeElement[] {
                    new ObjectDefiniton(
                        new FinalizedScope(
                        new Dictionary<IKey, IMemberDefinition> {
                            { keyX, localX },
                            { keyY, localY }
                        }),
                        new AssignOperation[]{
                            new AssignOperation(
                                new ConstantNumber(5),
                                new MemberReferance(localX)),
                            new AssignOperation(
                                new ConstantNumber(2),
                                new MemberReferance(localY))
                        })
                };
            }
        }

        public IFinalizedScope Scope => new FinalizedScope(new Dictionary<IKey, IMemberDefinition>());
    }
}