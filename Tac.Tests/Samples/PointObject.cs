using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Syntaz_Model_Interpeter;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    public class PointObject : ISample
    {
        public string Text
        {
            get
            {
                return @"object {
                            5 =: x ;
                            2 =: y ;
                        }";
            }
        }

        public IToken Token
        {
            get
            {
                return TokenHelp.File(
                           TokenHelp.Line(
                               TokenHelp.Ele(
                                   TokenHelp.Atom("object"),
                                   TokenHelp.Curl(
                                       TokenHelp.Line(
                                           TokenHelp.Ele(
                                               TokenHelp.Ele(TokenHelp.Atom("5")),
                                               TokenHelp.Atom("=:"),
                                               TokenHelp.Ele(TokenHelp.Atom("x")))),
                                       TokenHelp.Line(
                                           TokenHelp.Ele(
                                               TokenHelp.Ele(TokenHelp.Atom("2")),
                                               TokenHelp.Atom("=:"),
                                               TokenHelp.Ele(TokenHelp.Atom("y"))))))));
            }
        }

        public IEnumerable<ICodeElement> CodeElements
        {
            get
            {

                var keyX = new NameKey("x");
                var localX = new MemberDefinition(false, keyX, new Box<IReturnable>(new AnyType()));
                var keyY = new NameKey("y");
                var localY = new MemberDefinition(false, keyY, new Box<IReturnable>(new AnyType()));

                var objectScope = InterpetedInstanceScope.Make(InterpetedStaticScope.Make(), new TestScope(
                    new Dictionary<IKey, (bool, MemberDefinition)> {
                        {keyX, (false,localX) },
                        { keyY, (false,localY)}
                    }));
                
                
                return new ICodeElement[] {
                    new ObjectDefinition(
                        objectScope,
                        new InterpetedAssignOperation[]{
                            new InterpetedAssignOperation(new InterpetedConstantNumber(5),localX),
                            new InterpetedAssignOperation(new InterpetedConstantNumber(2),localY)
                        },
                        new ImplicitKey())
                };
            }
        }
    }

    internal class TestScope : IResolvableScope
    {
        private readonly Dictionary<IKey, (bool,MemberDefinition)> backingMembers;
        private readonly Dictionary<IKey, IReturnable> backingTypes;

        public TestScope(Dictionary<IKey, (bool,MemberDefinition)> backingMembers, Dictionary<IKey,IReturnable> backingTypes)
        {
            this.backingMembers = backingMembers;
            this.backingTypes = backingTypes;
        }

        public TestScope(Dictionary<IKey, (bool, MemberDefinition)> backingMembers): this(backingMembers, new Dictionary<IKey, IReturnable>()){}

        public IEnumerable<IKey> MembersKeys
        {
            get
            {
                return backingMembers.Keys;
            }
        }

        public bool TryGetMember(NameKey name, bool staticOnly, out IBox<MemberDefinition> member)
        {
            if (!backingMembers.TryGetValue(name, out var entry)) {
                member = default;
                return false;
            }

            if (staticOnly && !entry.Item1) {
                member = default;
                return false;
            }

            member = new Box<MemberDefinition>(entry.Item2);
            return true;
        }

        public bool TryGetType(IKey name, out IBox<IReturnable> type)
        {
            if (!backingTypes.TryGetValue(name, out var entry))
            {
                type = default;
                return false;
            }
            
            type = new Box<IReturnable>(entry);
            return true;
        }
    }
}
