using Prototypist.TaskChain;
using Prototypist.Toolbox.IEnumerable;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Backend.Emit.Support
{

    public struct TacCastObject : ITacObject
    {
        public TacObject @object;
        public Indexer indexer;
        //public IVerifiableType memberType;
        private static TacCastObject Create(Indexer nextIndexer, TacCastObject tacCastObject)
        {
            return new TacCastObject()
            {
                @object = tacCastObject.@object,
                indexer = Indexer.Overlay(tacCastObject.indexer, nextIndexer)
            };
        }

        // read-write complex members must be the same type as they are in TacObject
        // if they were more restrictive setting would be on the TacCastObject object could break the TacObject
        // if they were less restrictive setting on TacObject could break the TacCastObject

        // there is some danager of the same type defined in two different places
        // I need to be careful to sort members by name when I create objects
        // and when I create interfaces
        public TacCastObject GetComplexMember(int position) {
            return @object.GetComplexMember(indexer.indexOffsets[position]);
        }

        public void SetComplexMember(int position, TacCastObject value)
        {
            @object.SetComplexMember(indexer.indexOffsets[position], value);
        }

        public T GetSimpleMember<T>(int position)
        {
            return @object.GetSimpleMember<T>(indexer.indexOffsets[position]);
        }

        public void SetSimpleMember(int position, object o)
        {
            @object.SetSimpleMember(indexer.indexOffsets[position],o);
        }

        public TacCastObject GetComplexReadonlyMember(int position)
        {
            return Create(indexer.nextIndexers[position], @object.GetComplexMember(indexer.indexOffsets[position]));
        }

        public void SetComplexWriteonlyMember(int position, TacCastObject tacCastObject)
        {
            throw new NotImplementedException();
        }

    }

    public class TacObject : ITacObject
    {

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public object[] members;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        //public IVerifiableType type;

        public TacCastObject GetComplexMember(int position)
        {
            // when this returns a member it is important that it is a copy
            // if the member on TacObject is modified the TacCastObject should not be
            return (TacCastObject)members[position];
        }

        public void SetComplexMember(int position, TacCastObject tacCastObject)
        {
            members[position] = tacCastObject;
        }


        public T GetSimpleMember<T>(int position)
        {
            return (T)members[position];
        }


        public void SetSimpleMember(int position, object value)
        {
            members[position] = value;
        }

        public TacCastObject GetComplexReadonlyMember(int position)
        {
            return GetComplexMember(position);
        }

        public void SetComplexWriteonlyMember(int position, TacCastObject tacCastObject)
        {
            throw new NotImplementedException();
        }
    }

    //class TacCastObject {
    //    TacObject @object;
    //    Indexer indexer;
    //    public IVerifiableType memberType;

    //    public TacTransfromedObject GetComplexMember(int position)
    //    {
    //        return @object.GetComplexMember(indexer.indexOffsets[position]);
    //    }

    //    public T GetSimpleMember<T>(int position)
    //    {
    //        return @object.GetSimpleMember<T>(indexer.indexOffsets[position]);
    //    }

    //}



    // an indexer describes how to convert from one known type to another known type
    // say this converts cat {  number birds-killed; number bugs-killed;  number mice-killed; mouse nemesis}
    // to mouse-trap { number mice-killed; ; mouse nemesis;}
    // indexOffsets would be [2,3]
    // mice-killed is 0 in mouse-trap and 2 in cat
    // nemesis is 1 in mouse-trap and 3 in cat
    // nextIndexers would be [null, indexer to convert a mouse to a mouse]
    public class Indexer
    {
        public Indexer[] nextIndexers;
        public int[] indexOffsets;

        // for assignment to work we need two layers of overlay
        // this index is from one interface to another. say i1 to i2
        // when the members in @object change they may need an indexer to present like i1
        // those indexers are in objectIndexers
        // 
        public (Indexer, object[]) GetComplexMember(object[] @object, Indexer[] objectIndexers, int position) {
            return (
                // this overlay makes my very sad, it makes preformance a lot worse
                // if the member cannot be set this is not a concern
                Overlay(objectIndexers[indexOffsets[position]], nextIndexers[position]), 
                (object[])@object[indexOffsets[position]]);
        }

        public T GetSimpleMember<T>(object[] @object, int position)
        {
            return (T)@object[indexOffsets[position]];
        }

        // indexes have to overlay existing indexers?
        // class -> interface1 -> interface2
        // I think this is easy tho
        // [0,1,2,3,4,5] class
        // [1,3,5] interface1
        // [3,5] interface2

        // [1,3,5] class to interface1 indexer  "indexerc1"
        // [1,2] interface1 to interface2 indexer "indexer12"
        // [3,5] class to interface2 indexer "indexerc2"
        // run the 
        // indexerc2 = [indexerc1[indexer12[0]], indexerc1[indexer12[1]]];
        // the next indederx can be calculated similarly
        public static Indexer Overlay(Indexer first, Indexer second) {
            var resultIndexOffsets = new int[second.indexOffsets.Length];
            var resultNextIndexers = new Indexer[second.indexOffsets.Length];
            for (int i = 0; i < second.indexOffsets.Length;i++) {
                resultIndexOffsets[i] = first.indexOffsets[second.indexOffsets[i]];
                resultNextIndexers[i] = Overlay(first.nextIndexers[second.indexOffsets[i]], second.nextIndexers[i]);
            }
            return new Indexer()
            {
                nextIndexers = resultNextIndexers,
                indexOffsets = resultIndexOffsets,
            };
        }

        private static ConcurrentIndexed<(IInterfaceModuleType, IInterfaceModuleType), Indexer> map = new ConcurrentIndexed<(IInterfaceModuleType, IInterfaceModuleType), Indexer>();

        public static Indexer Create(IInterfaceModuleType from, IInterfaceModuleType to) {
            var toAdd = new Indexer();
            if (map.TryAdd((from, to), toAdd)) {

                var toMembers = to.Members.OrderBy(x => ((NameKey)x.Key).Name).ToList();
                var fromMembers = from.Members.OrderBy(x => ((NameKey)x.Key).Name).ToList();

                var indexOffsets = new int[toMembers.Count];
                var nextIndexers = new Indexer[toMembers.Count];

                for (int toIndex = 0; toIndex < toMembers.Count; toIndex++) {
                    var toMember = toMembers[toIndex];
                    for (int fromIndex = 0; fromIndex < fromMembers.Count; fromIndex++) {
                        var fromMember = fromMembers[fromIndex];
                        if (fromMember.Key.Equals(toMember.Key)) {
                            indexOffsets[toIndex] = fromIndex;
                            if (fromMember.Type.SafeIs(out IInterfaceModuleType fromInterface) && toMember.Type.SafeIs(out IInterfaceType toInterface)) {
                                nextIndexers[toIndex] = Create(fromInterface, toInterface);
                            }
                            goto matched;
                        }
                    }
            matched:;
                }
                toAdd.nextIndexers = nextIndexers;
                toAdd.indexOffsets = indexOffsets;
                return toAdd;
            }
            return map[(from, to)];
        }
    }

    // the thrid thing we pass around is the IVerifableType
    // you need to be able to generate Indexers from pairs of IVerifableType
    // we know the type outside of this tho that is static infomation


  

    // how does setting work?
    // I might need a two layer system
    // what is in the member to the interface the class persents
    // the interface the class persents to the interface subquenet types persent



}
