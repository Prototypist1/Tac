using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Backend.Emit.Support
{
    // an indexer describes how to convert from one known type to another known type
    // say this converts cat {  number birds-killed; number bugs-killed;  number mice-killed; mouse nemesis}
    // to mouse-trap { number mice-killed; ; mouse nemesis;}
    // indexOffsets would be [2,3]
    // mice-killed is 0 in mouse-trap and 2 in cat
    // nemesis is 1 in mouse-trap and 3 in cat
    // nextIndexers would be [null, indexer to convert a mouse to a mouse]
    class Indexer
    {
        private Indexer[] nextIndexers;
        private int[] indexOffsets;

        public (Indexer, object[]) GetComplexMember(object[] @object, int position) {
            return (
                nextIndexers[position], 
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
    }

    // the thrid thing we pass around is the IVerifableType
    // you need to be able to generate Indexers from pairs of IVerifableType
    // we know the type outside of this tho that is static infomation


  

    // how does setting work?

}
