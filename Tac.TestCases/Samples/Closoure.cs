using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.instantiated;
using Tac.Model.Operations;
using Tac.Semantic_Model.Names;
using Tac.TestCases;
using Tac.TestCases.Help;

namespace Tac.Tests.Samples
{
    public class Closoure : ITestCase
    {
        public Closoure()
        {
            var xKey = new NameKey("x");
            var x = new TestMemberDefinition(xKey, new TestTypeReferance(new TestNumberType()), false);

            var yKey = new NameKey("y");
            var y = new TestMemberDefinition(yKey, new TestTypeReferance(new TestNumberType()), false);
            
            var methodScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> { { xKey, x } });
            var innerMethodScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> { { yKey, y } }, methodScope);
            
            var method = new TestMethodDefinition(
                        new TestTypeReferance(new TestNumberType()),
                        new TestTypeReferance(new TestMethodType(
                            new TestEmptyType(),
                            new TestNumberType())),
                        x,
                        methodScope,
                        new ICodeElement[]{
                            new TestReturnOperation(
                                new TestMethodDefinition(
                                    new TestTypeReferance(new TestEmptyType()),
                                    new TestTypeReferance(new TestNumberType()),
                                    y,
                                    innerMethodScope,
                                    new ICodeElement[]{
                                        new TestAssignOperation(
                                            new TestAddOperation(
                                                new TestMemberReferance(x),
                                                new TestMemberReferance(y)),
                                            new TestMemberReferance(x)),
                                        new TestReturnOperation(
                                            new TestMemberReferance(x))
                                    },
                                    new ICodeElement[0]
                                    )
                                )},
                        new ICodeElement[0]);

            CodeElements = new ICodeElement[] { method, };
            Scope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> {});
        }

        public string Text
        {
            get
            {
                return
@"
method [ int ; method [ empty ; int ; ] ; ] x {
    method [ int ; int ; ] y {
        x + y =: x ;
        x return ;
    } return ;
} ;
";
            }
        }

        public ICodeElement[] CodeElements
        {
            get;
        }

        public IFinalizedScope Scope { get; }
    }

}
