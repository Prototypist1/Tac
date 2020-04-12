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
            var localX = MemberDefinition.CreateAndBuild(keyX, OrType.Make<IVerifiableType, IError>(new AnyType()), false);

            var keyY = new NameKey("y");
            var localY = MemberDefinition.CreateAndBuild(keyY, OrType.Make<IVerifiableType, IError>(new AnyType()), false);

            var contextKey = new NameKey("context");

            var context = MemberDefinition.CreateAndBuild(
                contextKey,
                    OrType.Make<IVerifiableType, IError>(InterfaceType.CreateAndBuild(
                        Scope.CreateAndBuild(
                            new List<IsStatic>() {
                                new IsStatic(localX ,false),
                                new IsStatic(localY ,false),
                            }).Members.Values.Select(x => x.Value).ToList())),
                false); ;

            var inputKey = new NameKey("input");
            var input = MemberDefinition.CreateAndBuild(inputKey, OrType.Make<IVerifiableType, IError>(new EmptyType()), false);

            var tempKey = new NameKey("temp");
            var temp = MemberDefinition.CreateAndBuild(tempKey, OrType.Make<IVerifiableType, IError>(new AnyType()), false);

            var implementationScope = Scope.CreateAndBuild(
                new List<IsStatic> {
                    new IsStatic( input ,false),
                    new IsStatic(temp ,false) });


            ModuleDefinition = Model.Instantiated.ModuleDefinition.CreateAndBuild(
                 Scope.CreateAndBuild(
                    new List<IsStatic>() {
                        new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("mirror"), OrType.Make< IVerifiableType ,IError>(new AnyType()), false) ,false) }),
                new[] {
                    OrType.Make<ICodeElement, IError>(AssignOperation.CreateAndBuild(
                    OrType.Make<ICodeElement, IError>(ImplementationDefinition.CreateAndBuild(
                        OrType.Make< IVerifiableType ,IError>(new EmptyType()),
                        context,
                        input,
                        implementationScope,
                        new ICodeElement[] {
                                AssignOperation.CreateAndBuild(
                                    OrType.Make<ICodeElement, IError>(PathOperation.CreateAndBuild(OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(context)),OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(localX)))),
                                    OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(temp))
                                    ),
                                AssignOperation.CreateAndBuild(
                                    OrType.Make<ICodeElement, IError>(PathOperation.CreateAndBuild(OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(context)),OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(localY)))),
                                    OrType.Make<ICodeElement, IError>(PathOperation.CreateAndBuild(OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(context)),OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(localX))))
                                    ),
                                AssignOperation.CreateAndBuild(
                                    OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(temp)),
                                    OrType.Make<ICodeElement, IError>(PathOperation.CreateAndBuild(OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(context)),OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(localY))))
                                    )
                        },
                        Array.Empty<ICodeElement>())),
                    OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("mirror"), OrType.Make< IVerifiableType ,IError>(new AnyType()), false)))))
                },
                new NameKey("mirror-module"),
                EntryPointDefinition.CreateAndBuild(Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<IOrType<ICodeElement, IError>>(), Array.Empty<ICodeElement>()));
            
        }


        public IModuleDefinition ModuleDefinition {get;}
    }
}
