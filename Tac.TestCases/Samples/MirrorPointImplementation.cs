﻿using Prototypist.Toolbox;
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
implementation [ type { x ; y ; } ; empty ; empty ] context input { 
    context . x =: temp ;
    context . y =: ( context . x ) ;
    temp =: ( context . y ) ; 
} =: mirror ;
";
            }
        }

        public MirrorPointImplementation()
        {
            var keyX = new NameKey("x");
            var localX = MemberDefinition.CreateAndBuild(keyX, new AnyType(), Access.ReadWrite);

            var keyY = new NameKey("y");
            var localY = MemberDefinition.CreateAndBuild(keyY, new AnyType(), Access.ReadWrite);

            var contextKey = new NameKey("context");

            var context = MemberDefinition.CreateAndBuild(
                contextKey,
                    InterfaceType.CreateAndBuild(
                        Scope.CreateAndBuild(
                            new List<IsStatic>() {
                                new IsStatic(localX ,false),
                                new IsStatic(localY ,false),
                            }).Members.Values.Select(x => x.Value).ToList()),
                Access.ReadWrite); ;

            var inputKey = new NameKey("input");
            var input = MemberDefinition.CreateAndBuild(inputKey, new EmptyType(), Access.ReadWrite);

            var tempKey = new NameKey("temp");
            var temp = MemberDefinition.CreateAndBuild(tempKey, new AnyType(), Access.ReadWrite);

            var implementationScope = Scope.CreateAndBuild(
                new List<IsStatic> {
                    new IsStatic( input ,false),
                    new IsStatic(temp ,false) });

            var intermediateScope = Scope.CreateAndBuild(
                new List<IsStatic> {
                    new IsStatic( context ,false)});


            RootScope = Model.Instantiated.RootScope.CreateAndBuild(
                 Scope.CreateAndBuild(
                    new List<IsStatic>() {
                        new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("mirror"), new AnyType(), Access.ReadWrite) ,false) }),
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
                            Array.Empty<ICodeElement>(),
                            intermediateScope),
                    MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("mirror"), new AnyType(), Access.ReadWrite)))
                },
                EntryPointDefinition.CreateAndBuild(new EmptyType(), MemberDefinition.CreateAndBuild(new NameKey("input"), new NumberType(), Access.ReadWrite), Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<ICodeElement>(), Array.Empty<ICodeElement>()));
            
        }

        public IRootScope RootScope {get;}
    }
}
