using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
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
            var x = new MemberDefinition(xKey, new TypeReferance(new NumberType()), false);

            var yKey = new NameKey("y");
            var y = new MemberDefinition(yKey, new TypeReferance(new NumberType()), false);
            
            var methodScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> { { xKey, x } });
            var innerMethodScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> { { yKey, y } }, methodScope);
            
            var method = new MethodDefinition(
                        new TypeReferance(new NumberType()),
                        new TypeReferance(new MethodType(
                            new EmptyType(),
                            new NumberType())),
                        x,
                        methodScope,
                        new ICodeElement[]{
                            new ReturnOperation(
                                new MethodDefinition(
                                    new TypeReferance(new EmptyType()),
                                    new TypeReferance(new NumberType()),
                                    y,
                                    innerMethodScope,
                                    new ICodeElement[]{
                                        new AssignOperation(
                                            new AddOperation(
                                                new MemberReferance(x),
                                                new MemberReferance(y)),
                                            new MemberReferance(x)),
                                        new ReturnOperation(
                                            new MemberReferance(x))
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
