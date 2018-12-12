using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.instantiated;
using Tac.Semantic_Model.Names;
using Tac.TestCases;
using Tac.TestCases.Help;

namespace Tac.Tests.Samples
{
    public class PairType : ITestCase
    {
        public string Text => @"type [ T ; ] pair {
                            T x ;
                            T y ;
                        }";

        public ICodeElement[] CodeElements
        {
            get
            {
                var key = new NameKey("T");
                var type = new TestGemericTypeParameterPlacholder(key);

                var keyX = new NameKey("x");
                var localX = new TestMemberDefinition(keyX, new TestTypeReferance(type), false);
                var keyY = new NameKey("y");
                var localY = new TestMemberDefinition(keyY, new TestTypeReferance(type), false);

                return new ICodeElement[] {
                    new TestGenericInterfaceDefinition(
                        new FinalizedScope(
                            new Dictionary<IKey, IMemberDefinition> {
                                { keyX, localX },
                                { keyY, localY }
                            }),
                        new TestGenericTypeParameterDefinition[]{
                            new TestGenericTypeParameterDefinition(key)
                        })
                };
            }
        }

        public IFinalizedScope Scope => new FinalizedScope(new Dictionary<IKey, IMemberDefinition>());
    }
}