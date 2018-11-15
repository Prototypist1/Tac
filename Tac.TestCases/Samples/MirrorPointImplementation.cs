using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Semantic_Model.Names;
using Tac.TestCases.Help;
using Tac.Tests.Samples;

namespace Tac.TestCases.Samples
{
    public class MirrorPointImplementation : ITestCase
    {
        public string Text
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICodeElement[] CodeElements
        {
            get
            {
                var keyX = new NameKey("x"); var localX = new TestMemberDefinition(keyX, new TestAnyType(), false);

                var keyY = new NameKey("y"); var localY = new TestMemberDefinition(keyY, new TestAnyType(), false);


                var contextKey = new NameKey("context");
                var context = new TestMemberDefinition(contextKey, new TestInterfaceType(), false); ;

                var inputKey = new NameKey("input");
                var input  = new TestMemberDefinition(inputKey, new TestEmptyType(), false);

                var tempKey= new NameKey("temp");
                var temp = new TestMemberDefinition(tempKey, new TestNumberType(), false);

                var implementationScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> {
                    { inputKey, input },{ contextKey, context },{ tempKey, temp } });


                var implementation = new TestImplementationDefinition(
                    new TestEmptyType(),
                    context,
                    input,
                    implementationScope,
                    new ICodeElement[] {
                        new TestAssignOperation(
                            new TestPathOperation(new TestMemberReferance(context),new TestMemberReferance(localX)),
                            new TestMemberReferance(temp)
                            ),
                        new TestAssignOperation(
                            new TestPathOperation(new TestMemberReferance(context),new TestMemberReferance(localY)),
                            new TestPathOperation(new TestMemberReferance(context),new TestMemberReferance(localX))
                            ),
                        new TestAssignOperation(
                            new TestMemberReferance(temp),
                            new TestPathOperation(new TestMemberReferance(context),new TestMemberReferance(localY))
                            )
                    },
                    new ICodeElement[0]
                    );

            }
        }

        public IFinalizedScope Scope
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
