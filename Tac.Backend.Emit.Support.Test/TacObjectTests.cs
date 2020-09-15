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


        // TODO test a long chain of narrowing and not narrowing 
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
