using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.TestCases.Help;
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
                return @"
module {
    implementation [ type { x ; y ; } ; empty ; empty ] context input { 
        context . x =: temp ;
        context . y =: ( context . x ) ;
        temp =: context . y ; 
    } =: mirror ;
} ; ";
            }
        }

        public MirrorPointImplementation()
        {
            var keyX = new NameKey("x");
            var localX = MemberDefinition.CreateAndBuild(keyX, TypeReference.CreateAndBuild(new AnyType()), false);

            var keyY = new NameKey("y");
            var localY = MemberDefinition.CreateAndBuild(keyY, TypeReference.CreateAndBuild(new AnyType()), false);

            var contextKey = new NameKey("context");

            var context = MemberDefinition.CreateAndBuild(contextKey, TypeReference.CreateAndBuild(InterfaceType.CreateAndBuild(
                new FinalizedScope(new Dictionary<IKey, IMemberDefinition>() {
                        { keyX, localX },
                        { keyY, localY },
                })
                )), false); ;

            var inputKey = new NameKey("input");
            var input = MemberDefinition.CreateAndBuild(inputKey, TypeReference.CreateAndBuild(new EmptyType()), false);

            var tempKey = new NameKey("temp");
            var temp = MemberDefinition.CreateAndBuild(tempKey, TypeReference.CreateAndBuild(new NumberType()), false);

            var implementationScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> {
                    { inputKey, input },{ contextKey, context },{ tempKey, temp } });


            Module = ModuleDefinition.CreateAndBuild(
                new FinalizedScope(new Dictionary<IKey, IMemberDefinition>() { { new NameKey("mirror"), MemberDefinition.CreateAndBuild(new NameKey("mirror"), TypeReference.CreateAndBuild(new AnyType()), false) } }),
                new[] {
                    AssignOperation.CreateAndBuild(
                    ImplementationDefinition.CreateAndBuild(
                        TypeReference.CreateAndBuild(new EmptyType()),
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
                        new ICodeElement[0]),
                    MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("mirror"), TypeReference.CreateAndBuild(new AnyType()), false)))
                });
            
        }


        public IModuleDefinition Module {get;}
    }
}
