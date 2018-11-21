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
        // TODO this needs to 'of' operations
        // '( context . x )' is the same as:  'x of context'
        public string Text =>
@"
implementation [ type { x ; y ; } ; empty ; empty ] context input { 
    context . x =: temp ;
    context . y =: ( context . x ) ;
    temp =: context . y ; 
} 
";

        public ICodeElement[] CodeElements
        {
            get
            {
                var keyX = new NameKey("x");
                var localX = new TestMemberDefinition(keyX, new TestTypeReferance(new TestAnyType()), false);

                var keyY = new NameKey("y");
                var localY = new TestMemberDefinition(keyY, new TestTypeReferance(new TestAnyType()), false);
                
                var contextKey = new NameKey("context");
                
                var context = new TestMemberDefinition(contextKey, new TestTypeReferance(new TestInterfaceType(
                    new FinalizedScope(new Dictionary<IKey, IMemberDefinition>() {
                        { keyX, localX },
                        { keyY, localY },
                    })
                    )), false); ;

                var inputKey = new NameKey("input");
                var input  = new TestMemberDefinition(inputKey, new TestTypeReferance(new TestEmptyType()), false);

                var tempKey= new NameKey("temp");
                var temp = new TestMemberDefinition(tempKey, new TestTypeReferance(new TestNumberType()), false);

                var implementationScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> {
                    { inputKey, input },{ contextKey, context },{ tempKey, temp } });


                var implementation = new TestImplementationDefinition(
                    new TestTypeReferance(new TestEmptyType()),
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

                return new ICodeElement[] {
                    implementation
                };

            }
        }

        public IFinalizedScope Scope => new FinalizedScope(new Dictionary<IKey, IMemberDefinition>());
    }
}
