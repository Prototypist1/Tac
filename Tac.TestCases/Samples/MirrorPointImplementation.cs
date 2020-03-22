using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Tests.Samples;

namespace Tac.TestCases.Samples
{
    public class MirrorPointImplementation : ITestCase
    {
        // TODO this needs to 'of' operations
        // '( context . x )' is the same as:  'x of context'
        public string Text
        {
            get
            {
                return 
@"
module mirror-module {
    implementation [ type { x ; y ; } ; empty ; empty ] context input { 
        context . x =: temp ;
        context . y =: ( context . x ) ;
        temp =: ( context . y ) ; 
    } =: mirror ;
} ; ";
            }
        }

        public MirrorPointImplementation()
        {
            var keyX = new NameKey("x");
            var localX = MemberDefinition.CreateAndBuild(keyX, new AnyType(), false);

            var keyY = new NameKey("y");
            var localY = MemberDefinition.CreateAndBuild(keyY, new AnyType(), false);

            var contextKey = new NameKey("context");

            var context = MemberDefinition.CreateAndBuild(
                contextKey, 
                    InterfaceType.CreateAndBuild(
                        Scope.CreateAndBuild(
                            new List<IsStatic>() {
                                new IsStatic(localX ,false),
                                new IsStatic(localY ,false),
                            }).Members.Values.Select(x => x.Value).ToList()),
                false); ;

            var inputKey = new NameKey("input");
            var input = MemberDefinition.CreateAndBuild(inputKey, new EmptyType(), false);

            var tempKey = new NameKey("temp");
            var temp = MemberDefinition.CreateAndBuild(tempKey, new AnyType(), false);

            var implementationScope = Scope.CreateAndBuild(
                new List<IsStatic> {
                    new IsStatic( input ,false),
                    new IsStatic(temp ,false) });


            ModuleDefinition = Model.Instantiated.ModuleDefinition.CreateAndBuild(
                 Scope.CreateAndBuild(
                    new List<IsStatic>() {
                        new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("mirror"), new AnyType(), false) ,false) }),
                new[] {
                    new OrType<ICodeElement, IError>(AssignOperation.CreateAndBuild(
                    new OrType<ICodeElement, IError>(ImplementationDefinition.CreateAndBuild(
                        new EmptyType(),
                        context,
                        input,
                        implementationScope,
                        new ICodeElement[] {
                                AssignOperation.CreateAndBuild(
                                    new OrType<ICodeElement, IError>(PathOperation.CreateAndBuild(new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(context)),new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(localX)))),
                                    new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(temp))
                                    ),
                                AssignOperation.CreateAndBuild(
                                    new OrType<ICodeElement, IError>(PathOperation.CreateAndBuild(new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(context)),new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(localY)))),
                                    new OrType<ICodeElement, IError>(PathOperation.CreateAndBuild(new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(context)),new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(localX))))
                                    ),
                                AssignOperation.CreateAndBuild(
                                    new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(temp)),
                                    new OrType<ICodeElement, IError>(PathOperation.CreateAndBuild(new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(context)),new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(localY))))
                                    )
                        },
                        Array.Empty<ICodeElement>())),
                    new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("mirror"), new AnyType(), false)))))
                },
                new NameKey("mirror-module"),
                EntryPointDefinition.CreateAndBuild(Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<OrType<ICodeElement, IError>>(), Array.Empty<ICodeElement>()));
            
        }


        public IModuleDefinition ModuleDefinition {get;}
    }
}
