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

        // TODO test readonly members with more narrow interfaces 
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

        public static Lazy<Types> Types = new Lazy<Types>(() => new Types());

    }

    public class Types {
        public readonly IInterfaceType hasFriend;
        public readonly IInterfaceType hasAge;
        public readonly IInterfaceType hasHasAge;
        public readonly IInterfaceType person;

        public Types() {
            var (hasFriendType, hasFriendBuilder) = InterfaceType.Create();
            var (hasAgeType, hasAgeBuilder) = InterfaceType.Create();
            var (hasHasAgeType, hasHasAgeBuilder) = InterfaceType.Create();
            var (personType, personBuilder) = InterfaceType.Create();

            var ageMember = MemberDefinition.CreateAndBuild(new NameKey("age"), new NumberType(), false);
            var friendMember = MemberDefinition.CreateAndBuild(new NameKey("friend"), personType, false);
            var friendHasAgeMember = MemberDefinition.CreateAndBuild(new NameKey("friend"), hasAgeType, true);

            hasFriendBuilder.Build(new List<IMemberDefinition> { friendMember });
            hasAgeBuilder.Build(new List<IMemberDefinition> { ageMember });
            hasHasAgeBuilder.Build(new List<IMemberDefinition> { friendHasAgeMember });
            personBuilder.Build(new List<IMemberDefinition> { friendMember , ageMember });

            hasFriend = hasFriendType;
            hasAge = hasAgeType;
            person = personType;
            hasHasAge = hasHasAgeType;
        }
    }
}
