using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;
using Tac.TestCases;

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
                var localX = new WeakMemberDefinition(false, keyX, new Box<IWeakReturnable>(new InterpetedAnyType()));
                var keyY = new NameKey("y");
                var localY = new WeakMemberDefinition(false, keyY, new Box<IWeakReturnable>(new InterpetedAnyType()));
                                
                return new IWeakCodeElement[] {
                    new InterpetedObjectDefinition(
                        new FinalizedScope(
                        new Dictionary<IKey, IBox<WeakMemberDefinition>> {
                            { keyX, new Box<WeakMemberDefinition>(localX) },
                            { keyY, new Box<WeakMemberDefinition>(localY) }
                        }),
                        new InterpetedAssignOperation[]{
                            new InterpetedAssignOperation(
                                new InterpetedConstantNumber(5),
                                new InterpetedMemberReferance(new Box<WeakMemberDefinition>(localX))),
                            new InterpetedAssignOperation(
                                new InterpetedConstantNumber(2),
                                new InterpetedMemberReferance(new Box<WeakMemberDefinition>(localY)))
                        },
                        new ImplicitKey())
                };
            }
        }
    }
}