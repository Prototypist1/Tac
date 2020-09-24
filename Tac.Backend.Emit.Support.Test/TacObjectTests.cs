using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Xunit;

namespace Tac.Backend.Emit.Support.Test
{
    public class TacObjectTests
    {
        [Fact]
        public void ReadTests() {
            // create a couple of people
            var friend1 = new TacObject();
            var friend2 = new TacObject();

            friend1.members = new object[] {
                30,
                new TacCastObject{
                    @object = friend2,
                    indexer = Indexer.Create(Types.Value.person, Types.Value.person)
                }
            };

            friend2.members = new object[] {
                29,
                new TacCastObject{
                    @object = friend1,
                    indexer = Indexer.Create(Types.Value.person, Types.Value.person)
                }
            };

            Assert.Equal(30, friend1.GetSimpleMember<int>(0));
            Assert.Equal(29, friend2.GetSimpleMember<int>(0));

            Assert.Equal(29, friend1.GetComplexMember(1).GetSimpleMember<int>(0));
            Assert.Equal(30, friend2.GetComplexMember(1).GetSimpleMember<int>(0));


            var friend1AsHasFriends = new TacCastObject()
            {
                @object = friend1,
                indexer = Indexer.Create(Types.Value.person, Types.Value.hasFriend)
            };

            Assert.Equal(29, friend1AsHasFriends.GetComplexMember(0).GetSimpleMember<int>(0));

            var friend1AsHasAge = new TacCastObject()
            {
                @object = friend1,
                indexer = Indexer.Create(Types.Value.person, Types.Value.hasAge)
            };

            Assert.Equal(30, friend1AsHasAge.GetSimpleMember<int>(0));

            // all these friend1AsPerson are pointless they are an artifact of an older time
            var friend1AsPerson = new TacCastObject()
            {
                @object = friend1,
                indexer = Indexer.Create(Types.Value.person, Types.Value.person)
            };

            Assert.Equal(30, friend1AsPerson.GetSimpleMember<int>(0));
            Assert.Equal(29, friend1AsPerson.GetComplexMember(1).GetSimpleMember<int>(0));
        }

        [Fact]
        public void WriteTest()
        {
            // create a couple of people
            var friend1 = new TacObject();
            var friend2 = new TacObject();

            friend1.members = new object[] {
                1,
                new TacCastObject{
                    @object = friend1,
                    indexer = Indexer.Create(Types.Value.person, Types.Value.person)
                }
            };

            friend2.members = new object[] {
                2,
                new TacCastObject{
                    @object = friend2,
                    indexer = Indexer.Create(Types.Value.person, Types.Value.person)
                }
            };

            var friend1_friend_Before = friend1.GetComplexMember(1);

            var friend2AsPerson = new TacCastObject()
            {
                @object = friend2,
                indexer = Indexer.Create(Types.Value.person, Types.Value.person)
            };

            friend1.SetComplexMember(1, friend2AsPerson);


            var friend1_friend_After = friend1.GetComplexMember(1);

            Assert.Equal(1, friend1_friend_Before.GetSimpleMember<int>(0));
            Assert.Equal(2, friend1_friend_After.GetSimpleMember<int>(0));

            var friend1AsHasFriends = new TacCastObject()
            {
                @object = friend1,
                indexer = Indexer.Create(Types.Value.person, Types.Value.hasFriend)
            };

            var friend1AsPerson = new TacCastObject()
            {
                @object = friend1,
                indexer = Indexer.Create(Types.Value.person, Types.Value.person)
            };

            friend1AsHasFriends.SetComplexMember(0, friend1AsPerson);

            // when we set all the forms should work
            Assert.Equal(1, friend1AsHasFriends.GetComplexMember(0).GetSimpleMember<int>(0));
            Assert.Equal(1, friend1.GetComplexMember(1).GetSimpleMember<int>(0));
            Assert.Equal(1, friend1AsPerson.GetComplexMember(1).GetSimpleMember<int>(0));

            // but the stuff we already read should hold it's value
            Assert.Equal(1, friend1_friend_Before.GetSimpleMember<int>(0));
            Assert.Equal(2, friend1_friend_After.GetSimpleMember<int>(0));
        }

        [Fact]
        public void ReadNarrowedReadonlyMember() {
            var friend1 = new TacObject();
            var friend2 = new TacObject();

            friend1.members = new object[] {
                30,
                new TacCastObject{
                    @object = friend2,
                    indexer = Indexer.Create(Types.Value.person, Types.Value.person)
                }
            };

            friend2.members = new object[] {
                29,
                new TacCastObject{
                    @object = friend1,
                    indexer = Indexer.Create(Types.Value.person, Types.Value.person)
                }
            };

            var friend1AsHasHasAge = new TacCastObject()
            {
                @object = friend1,
                indexer = Indexer.Create(Types.Value.person, Types.Value.hasHasAge)
            };

            Assert.Equal(29, friend1AsHasHasAge.GetComplexReadonlyMember(0).GetSimpleMember<int>(0));

            var friend1AsPerson = new TacCastObject()
            {
                @object = friend1,
                indexer = Indexer.Create(Types.Value.person, Types.Value.person)
            };

            friend1.SetComplexMember(1, friend1AsPerson);

            Assert.Equal(30, friend1AsHasHasAge.GetComplexReadonlyMember(0).GetSimpleMember<int>(0));
        }

        [Fact]
        public void WriteWiderWriteonlyMember() {

            var colin = new TacObject();
            var emily = new TacObject();

            colin.members = new object[] {
                30,
                new TacCastObject{
                    @object = colin,
                    indexer = Indexer.Create(Types.Value.namedPerson, Types.Value.person)
                },
                "Colin"
            };

            var colinAsPerson = new TacCastObject
            {
                @object = colin,
                indexer = Indexer.Create(Types.Value.namedPerson, Types.Value.person)
            };

            var colinAsHasNamedPersonFriend = new TacCastObject
            {
                @object = colin,
                indexer = Indexer.Create(Types.Value.namedPerson, Types.Value.hasWriteOnlyNamedPersonFriend)
            };


            emily.members = new object[] {
                29,
                new TacCastObject{
                    @object = emily,
                    indexer = Indexer.Create(Types.Value.namedPerson, Types.Value.person)
                },
                "Emily"
            };

            colinAsHasNamedPersonFriend.SetComplexWriteonlyMember(
                0,
                new TacCastObject
                {
                    @object = emily,
                    indexer = Indexer.Create(Types.Value.hasWriteOnlyNamedPersonFriend, Types.Value.hasWriteOnlyNamedPersonFriend)
                });

            Assert.Equal(29, colinAsPerson.GetComplexMember(1).GetSimpleMember<int>(0));
        }

        [Fact]
        public void StackedNarrowingTest() {
            var colin = new TacObject();

            var emily = new TacObject();

            colin.members = new object[] {
                30,
                new TacCastObject{
                    @object = emily,
                    indexer = Indexer.Create(Types.Value.namedPerson, Types.Value.person)
                },
                "Colin"
            };

            emily.members = new object[] {
                29,
                new TacCastObject{
                    @object = emily,
                    indexer = Indexer.Create(Types.Value.namedPerson, Types.Value.person)
                },
                "Emily"
            };

            var colinAsHasNamedPersonFriendAndAge = new TacCastObject
            {
                @object = colin,
                indexer = Indexer.Create(Types.Value.hasNamedPersonFriendAndAge, Types.Value.hasNamedPersonFriendAndAge)
            };

            var colinAsHasNamedPersonFriend = new TacCastObject
            {
                @object = colinAsHasNamedPersonFriendAndAge,
                indexer = Indexer.Create(Types.Value.hasNamedPersonFriendAndAge, Types.Value.hasNamedPersonFriend)
            };

            var colinAsHasReadOnlyFriend = new TacCastObject
            {
                @object = colinAsHasNamedPersonFriend,
                indexer = Indexer.Create(Types.Value.hasNamedPersonFriend, Types.Value.hasReadOnlyFriend)
            };

            var colinAsHasHasAge = new TacCastObject
            {
                @object = colinAsHasReadOnlyFriend,
                indexer = Indexer.Create(Types.Value.hasReadOnlyFriend, Types.Value.hasHasAge)
            };

            Assert.Equal(29, colinAsHasHasAge.GetComplexMember(0).GetSimpleMember<int>(0));

        }

        [Fact]
        public void ComplexComplexMethodTest() {
            // method[type{a;b;c;},type{a;b;}] method-1 := method[type{a;b;c;},type{a;b;}] input {  input return; }
            // method[type{a;b;c;d;},type{b;}] method-2 := method-1 ;
            // object { a := 1; b := 2; c := 3; d := 4; e := 5; } > method-2 =: result

            var aMember = MemberDefinition.CreateAndBuild(new NameKey("a"), new NumberType(), Access.ReadWrite);
            var bMember = MemberDefinition.CreateAndBuild(new NameKey("b"), new NumberType(), Access.ReadWrite);
            var cMember = MemberDefinition.CreateAndBuild(new NameKey("c"), new NumberType(), Access.ReadWrite);
            var dMember = MemberDefinition.CreateAndBuild(new NameKey("d"), new NumberType(), Access.ReadWrite);
            var eMember = MemberDefinition.CreateAndBuild(new NameKey("e"), new NumberType(), Access.ReadWrite);

            var bType= InterfaceType.CreateAndBuild(new List<IMemberDefinition> { bMember });
            var abType = InterfaceType.CreateAndBuild(new List<IMemberDefinition> { aMember, bMember });
            var abcType = InterfaceType.CreateAndBuild(new List<IMemberDefinition> { aMember, bMember, cMember });
            var abcdType = InterfaceType.CreateAndBuild(new List<IMemberDefinition> { aMember, bMember, cMember, dMember });
            var abcdeType = InterfaceType.CreateAndBuild(new List<IMemberDefinition> { aMember, bMember, cMember, dMember, eMember });

            var method1Type = MethodType.CreateAndBuild(abcType, abType);

            var method2Type = MethodType.CreateAndBuild(abcdType, bType);

            Func<ITacObject, ITacObject> func = x => new TacCastObject
            {
                indexer = Indexer.Create(abcType, abType),
                @object = x
            };

            var method = new TacMethod_Complex_Complex()
            {
                backing = func
            };

            var castMethod = new TacCastObject()
            {
                indexer = Indexer.Create(method1Type, method2Type),
                @object = method
            };

            var @object = new TacObject
            {
                members = new object[] { 3, 2, 5, 1, 4 }
            };

            var objectAsABCD = new TacCastObject
            {
                @object = @object,
                indexer = Indexer.Create(abcdeType, abcdType),
            };

            var objectAsABC = new TacCastObject
            {
                @object = @object,
                indexer = Indexer.Create(abcdeType, abcType),
            };

            var objectAsAB = method.Call_Complex_Complex(objectAsABC);


            Assert.Equal(3, objectAsAB.GetSimpleMember<int>(0));
            Assert.Equal(2, objectAsAB.GetSimpleMember<int>(1));

            var objectAsB = castMethod.Call_Complex_Complex(objectAsABCD);

            Assert.Equal(2, objectAsB.GetSimpleMember<int>(0));

        }


        [Fact]
        public void ComplexSimpleMethodTest()
        {
            // type-1 { method[type{b;c;d;},int] method-1} instance-1 := object{  method-1:= method[type{b;c;d;},int] input {  input.c return; }}
            // type-2 { readonly method[type{a;b;c;d;},int] method-1 } instance-2 := instance-1 ;
            // object { a := 1; b := 2; c := 3; d := 4; e := 5; } > (instance-2.method-1) =: result

            var aMember = MemberDefinition.CreateAndBuild(new NameKey("a"), new NumberType(), Access.ReadWrite);
            var bMember = MemberDefinition.CreateAndBuild(new NameKey("b"), new NumberType(), Access.ReadWrite);
            var cMember = MemberDefinition.CreateAndBuild(new NameKey("c"), new NumberType(), Access.ReadWrite);
            var dMember = MemberDefinition.CreateAndBuild(new NameKey("d"), new NumberType(), Access.ReadWrite);
            var eMember = MemberDefinition.CreateAndBuild(new NameKey("e"), new NumberType(), Access.ReadWrite);

            var bcdType = InterfaceType.CreateAndBuild(new List<IMemberDefinition> { bMember, cMember, dMember });
            var abcdType = InterfaceType.CreateAndBuild(new List<IMemberDefinition> { aMember, bMember, cMember, dMember });
            var abcdeType = InterfaceType.CreateAndBuild(new List<IMemberDefinition> { aMember, bMember, cMember, dMember, eMember });

            var method1Type = MethodType.CreateAndBuild(bcdType, new NumberType());

            var method2Type = MethodType.CreateAndBuild(abcdType, new NumberType());

            var methodMemberForType1 = MemberDefinition.CreateAndBuild(new NameKey("method-1"), method1Type, Access.ReadWrite);
            var methodMemberForType2 = MemberDefinition.CreateAndBuild(new NameKey("method-1"), method2Type, Access.ReadOnly);


            var type1 = InterfaceType.CreateAndBuild(new List<IMemberDefinition> { methodMemberForType1 });
            var type2 = InterfaceType.CreateAndBuild(new List<IMemberDefinition> { methodMemberForType2 });

            Func<ITacObject, object> func = x =>
            {
                return x.GetSimpleMember<int>(1);
            };

            var @object = new TacObject
            {
                members = new object[] { new TacMethod_Complex_Simple { backing = func } }
            };

            var castObject = new TacCastObject
            {
                @object = @object,
                indexer = Indexer.Create(type1, type2)
            };


            var inputObject = new TacObject
            {
                members = new object[] { 3, 2, 5, 1, 4 }
            };

            var inputObjectAsABCD = new TacCastObject
            {
                @object = inputObject,
                indexer = Indexer.Create(abcdeType, abcdType),
            };

            Assert.Equal(5, castObject.GetComplexReadonlyMember(0).Call_Complex_Simple<int>(inputObjectAsABCD));

        }

        public static Lazy<Types> Types = new Lazy<Types>(() => new Types());

    }


    public class Types {
        public readonly IInterfaceType hasFriend;
        public readonly IInterfaceType hasReadOnlyFriend;
        public readonly IInterfaceType hasAge;
        public readonly IInterfaceType hasHasAge;
        public readonly IInterfaceType person;
        public readonly IInterfaceType namedPerson;
        public readonly IInterfaceType hasWriteOnlyNamedPersonFriend;
        public readonly IInterfaceType hasNamedPersonFriend;
        public readonly IInterfaceType hasNamedPersonFriendAndAge;

        public Types() {
            var (hasFriendType, hasFriendBuilder) = InterfaceType.Create();
            var (hasReadOnlyFriendType, hasReadOnlyFriendBuilder) = InterfaceType.Create();
            var (hasAgeType, hasAgeBuilder) = InterfaceType.Create();
            var (hasHasAgeType, hasHasAgeBuilder) = InterfaceType.Create();
            var (personType, personBuilder) = InterfaceType.Create();
            var (namedPersonType, namedPersonBuilder) = InterfaceType.Create();
            var (hasWriteOnlyNamedPersonFriendType, hasWriteOnlyNamedPersonFriendBuilder) = InterfaceType.Create();
            var (hasNamedPersonFriendType, hasNamedPersonFriendBuilder) = InterfaceType.Create();
            var (hasNamedPersonFriendAndAgeType, hasNamedPersonFriendAndAgeBuilder) = InterfaceType.Create();

            var ageMember = MemberDefinition.CreateAndBuild(new NameKey("age"), new NumberType(), Access.ReadWrite);
            var friendMember = MemberDefinition.CreateAndBuild(new NameKey("friend"), personType, Access.ReadWrite);
            var readonlyFriendMember = MemberDefinition.CreateAndBuild(new NameKey("friend"), personType, Access.ReadOnly);
            var friendHasAgeMember = MemberDefinition.CreateAndBuild(new NameKey("friend"), hasAgeType, Access.ReadOnly);
            var writeonlyNamedFriendMember = MemberDefinition.CreateAndBuild(new NameKey("friend"), namedPersonType, Access.WriteOnly);
            var namedFriendMember = MemberDefinition.CreateAndBuild(new NameKey("friend"), namedPersonType, Access.ReadWrite);
            var nameMember = MemberDefinition.CreateAndBuild(new NameKey("name"), new StringType(), Access.ReadWrite);

            hasFriendBuilder.Build(new List<IMemberDefinition> { friendMember });
            hasAgeBuilder.Build(new List<IMemberDefinition> { ageMember });
            hasHasAgeBuilder.Build(new List<IMemberDefinition> { friendHasAgeMember });
            personBuilder.Build(new List<IMemberDefinition> { friendMember , ageMember  });
            namedPersonBuilder.Build(new List<IMemberDefinition> { friendMember, ageMember , nameMember });
            hasWriteOnlyNamedPersonFriendBuilder.Build(new List<IMemberDefinition> { writeonlyNamedFriendMember });
            hasNamedPersonFriendBuilder.Build(new List<IMemberDefinition> { namedFriendMember });
            hasNamedPersonFriendAndAgeBuilder.Build(new List<IMemberDefinition> { namedFriendMember, ageMember });
            hasReadOnlyFriendBuilder.Build(new List<IMemberDefinition> { readonlyFriendMember });

            hasFriend = hasFriendType;
            hasAge = hasAgeType;
            person = personType;
            hasHasAge = hasHasAgeType;
            namedPerson = namedPersonType;
            hasWriteOnlyNamedPersonFriend = hasWriteOnlyNamedPersonFriendType;
            hasNamedPersonFriend = hasNamedPersonFriendType;
            hasNamedPersonFriendAndAge = hasNamedPersonFriendAndAgeType;
            hasReadOnlyFriend = hasReadOnlyFriendType;
        }
    }
}
