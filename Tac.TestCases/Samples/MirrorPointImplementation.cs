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


            Module = ModuleDefinition.CreateAndBuild(
                 Scope.CreateAndBuild(
                    new List<IsStatic>() {
                        new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("mirror"), new AnyType(), false) ,false) }),
                new[] {
                    AssignOperation.CreateAndBuild(
                    ImplementationDefinition.CreateAndBuild(
                        new EmptyType(),
                        context,
                        input,
                        implementationScope,
                        new ICodeElement[] {
                                AssignOperation.CreateAndBuild(
                                    PathOperation.CreateAndBuild(MemberReference.CreateAndBuild(context),MemberReference.CreateAndBuild(localX)),
                                    MemberReference.CreateAndBuild(temp)
                                    ),
                                AssignOperation.CreateAndBuild(
                                    PathOperation.CreateAndBuild(MemberReference.CreateAndBuild(context),MemberReference.CreateAndBuild(localY)),
                                    PathOperation.CreateAndBuild(MemberReference.CreateAndBuild(context),MemberReference.CreateAndBuild(localX))
                                    ),
                                AssignOperation.CreateAndBuild(
                                    MemberReference.CreateAndBuild(temp),
                                    PathOperation.CreateAndBuild(MemberReference.CreateAndBuild(context),MemberReference.CreateAndBuild(localY))
                                    )
                        },
                        Array.Empty<ICodeElement>()),
                    MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("mirror"), new AnyType(), false)))
                },
                new NameKey("mirror-module"),
                EntryPointDefinition.CreateAndBuild(Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<ICodeElement>(), Array.Empty<ICodeElement>()));
            
        }


        public IModuleDefinition Module {get;}
    }
}
