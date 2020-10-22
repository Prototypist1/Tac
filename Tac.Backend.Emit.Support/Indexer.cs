using Prototypist.TaskChain;
using Prototypist.Toolbox;
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
    // TODO YOU ARE HERE
    // I need to make a typeCastMethod
    // or something like that
    // it is like an indexer for methods
    // it transforms the input and output
    // simple vs complex vs method

    // methods might just use a indexer with a trivial mapping and 2 next indexers
    // the frist used for input
    // the second used for output

    // calling a method on a TacCastObject is:
    // 1 - cast object transforms the input
    // 2 - call the method
    // 3  - cast object transforms the output


    // NOTE I spent once redid this to stack TacCastObject on TacCastObject for GetComplexReadonlyMember
    // you don't need to call Indexer.Overlay 
    // @object becomes a ITacObject


    //public class TacMethod<TIn,TOut>: ITacMethod<TIn, TOut>
    //{
    //    Func<TIn, TOut> backing;

    //    public TOut Invoke(TIn @in) {
    //        return backing.Invoke(@in);
    //    }
    //}

    public class TacMethod_Complex_Complex: ITacObject
    {
        public Func<ITacObject, ITacObject> backing;
        private readonly IVerifiableType type;

        public TacMethod_Complex_Complex(Func<ITacObject, ITacObject> backing, IVerifiableType type)
        {
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public Tout Call_Complex_Simple<Tout>(ITacObject input)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject Call_Simple_Complex<Tin>(Tin input)
        {
            throw new NotImplementedException("not supported");
        }

        public Tout Call_Simple_Simple<Tin, Tout>(Tin input)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject GetComplexMember(int position)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject GetComplexReadonlyMember(int position)
        {
            throw new NotImplementedException("not supported");
        }

        public T GetSimpleMember<T>(int position)
        {
            throw new NotImplementedException("not supported");
        }

        public void SetComplexMember(ITacObject tacCastObject,int position)
        {
            throw new NotImplementedException("not supported");
        }

        public void SetComplexWriteonlyMember(ITacObject tacCastObject,int position)
        {
            throw new NotImplementedException("not supported");
        }

        public void SetSimpleMember<Tin>(Tin value,int position)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject Call_Complex_Complex(ITacObject input) => backing(input);

        public ITacObject SetComplexMemberReturn(ITacObject tacCastObject, int position)
        {
            SetComplexMemberReturn(tacCastObject, position);
            return tacCastObject;
        }

        public Tin SetSimpleMemberReturn<Tin>(Tin value, int position)
        {
            SetSimpleMemberReturn(value, position);
            return value;
        }

        public ITacObject SetComplexWriteonlyMemberReturn(ITacObject tacCastObject, int position)
        {
            SetComplexWriteonlyMemberReturn(tacCastObject, position);
            return tacCastObject;
        }

        public IVerifiableType TacType() => type;
    }
    public class TacMethod_Simple_Complex<T1>: ITacObject
    {
        public Func<T1, ITacObject> backing;
        private readonly IVerifiableType type;

        public TacMethod_Simple_Complex(Func<T1, ITacObject> backing, IVerifiableType type)
        {
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public ITacObject Call_Complex_Complex(ITacObject input)
        {
            throw new NotImplementedException("not supported");
        }

        public Tout Call_Complex_Simple<Tout>(ITacObject input)
        {
            throw new NotImplementedException("not supported");
        }


        public Tout Call_Simple_Simple<Tin, Tout>(Tin input)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject GetComplexMember(int position)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject GetComplexReadonlyMember(int position)
        {
            throw new NotImplementedException("not supported");
        }

        public T GetSimpleMember<T>(int position)
        {
            throw new NotImplementedException("not supported");
        }

        public void SetComplexMember(ITacObject tacCastObject,int position)
        {
            throw new NotImplementedException("not supported");
        }

        public void SetComplexWriteonlyMember(ITacObject tacCastObject,int position)
        {
            throw new NotImplementedException("not supported");
        }

        public void SetSimpleMember<Tin>(Tin value,int position)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject Call_Simple_Complex<Tin>(Tin input)
        {
            if (input is T1 t1)
            {
                return backing(t1);
            }
            throw new Exception("types are not right");
        }

        public ITacObject SetComplexMemberReturn(ITacObject tacCastObject, int position)
        {
            SetComplexMemberReturn(tacCastObject, position);
            return tacCastObject;
        }

        public Tin SetSimpleMemberReturn<Tin>(Tin value, int position)
        {
            SetSimpleMemberReturn(value, position);
            return value;
        }

        public ITacObject SetComplexWriteonlyMemberReturn(ITacObject tacCastObject, int position)
        {
            SetComplexWriteonlyMemberReturn(tacCastObject, position);
            return tacCastObject;
        }
        public IVerifiableType TacType() => type;
    }
    public class TacMethod_Complex_Simple<T2>: ITacObject
    {

        public Func<ITacObject, T2> backing;
        private readonly IVerifiableType type;

        public TacMethod_Complex_Simple(Func<ITacObject,T2> backing, IVerifiableType type)
        {
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public ITacObject Call_Complex_Complex(ITacObject input)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject Call_Simple_Complex<Tin>(Tin input)
        {
            throw new NotImplementedException("not supported");
        }

        public Tout Call_Simple_Simple<Tin, Tout>(Tin input)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject GetComplexMember(int position)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject GetComplexReadonlyMember(int position)
        {
            throw new NotImplementedException("not supported");
        }

        public T GetSimpleMember<T>(int position)
        {
            throw new NotImplementedException("not supported");
        }

        public void SetComplexMember(ITacObject tacCastObject,int position)
        {
            throw new NotImplementedException("not supported");
        }

        public void SetComplexWriteonlyMember(ITacObject tacCastObject,int position)
        {
            throw new NotImplementedException("not supported");
        }

        public void SetSimpleMember<Tin>(Tin value,int position)
        {
            throw new NotImplementedException("not supported");
        }

        public Tout Call_Complex_Simple<Tout>(ITacObject input)
        {
            var res = backing(input);
            if (res is Tout oout)
            {
                return oout;
            }
            throw new Exception("types are not right");
        }
        public ITacObject SetComplexMemberReturn(ITacObject tacCastObject, int position)
        {
            SetComplexMemberReturn(tacCastObject, position);
            return tacCastObject;
        }

        public Tin SetSimpleMemberReturn<Tin>(Tin value, int position)
        {
            SetSimpleMemberReturn(value, position);
            return value;
        }

        public ITacObject SetComplexWriteonlyMemberReturn(ITacObject tacCastObject, int position)
        {
            SetComplexWriteonlyMemberReturn(tacCastObject, position);
            return tacCastObject;
        }
        public IVerifiableType TacType() => type;
    }
    public class TacMethod_Simple_Simple<T1,T2>: ITacObject
    {
        public Func<T1,T2> backing;
        private readonly IVerifiableType type;

        public TacMethod_Simple_Simple(Func<T1, T2> backing, IVerifiableType type)
        {
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public ITacObject Call_Complex_Complex(ITacObject input)
        {
            throw new NotImplementedException("not supported");
        }

        public Tout Call_Complex_Simple<Tout>(ITacObject input)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject Call_Simple_Complex<Tin>(Tin input)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject GetComplexMember(int position)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject GetComplexReadonlyMember(int position)
        {
            throw new NotImplementedException("not supported");
        }

        public T GetSimpleMember<T>(int position)
        {
            throw new NotImplementedException("not supported");
        }

        public void SetComplexMember(ITacObject tacCastObject,int position)
        {
            throw new NotImplementedException("not supported");
        }

        public void SetComplexWriteonlyMember(ITacObject tacCastObject,int position)
        {
            throw new NotImplementedException("not supported");
        }

        public void SetSimpleMember<Tin>(Tin value,int position)
        {
            throw new NotImplementedException("not supported");
        }

        public Tout Call_Simple_Simple<Tin, Tout>(Tin input)
        {
            if (input is T1 t1) {
                var res = backing(t1);
                if (res is Tout oout) {
                    return oout;
                }
            }
            throw new Exception("types are not right");
        }
        public ITacObject SetComplexMemberReturn(ITacObject tacCastObject, int position)
        {
            SetComplexMemberReturn(tacCastObject, position);
            return tacCastObject;
        }

        public Tin SetSimpleMemberReturn<Tin>(Tin value, int position)
        {
            SetSimpleMemberReturn(value, position);
            return value;
        }

        public ITacObject SetComplexWriteonlyMemberReturn(ITacObject tacCastObject, int position)
        {
            SetComplexWriteonlyMemberReturn(tacCastObject, position);
            return tacCastObject;
        }
        public IVerifiableType TacType() => type;
    }

    public class TacCastObject : ITacObject
    {
        public ITacObject @object;
        public Indexer indexer;

        // TODO I don't need type, the indexer knows it
        public TacCastObject(ITacObject @object, Indexer indexer)
        {
            this.@object = @object ?? throw new ArgumentNullException(nameof(@object));
            this.indexer = indexer ?? throw new ArgumentNullException(nameof(indexer));
        }

        //public IVerifiableType memberType;

        // read-write complex members must be the same type as they are in TacObject
        // if they were more restrictive setting would be on the TacCastObject object could break the TacObject
        // if they were less restrictive setting on TacObject could break the TacCastObject

        // there is some danager of the same type defined in two different places
        // I need to be careful to sort members by name when I create objects
        // and when I create interfaces
        public  ITacObject GetComplexMember(int position) {
            return @object.GetComplexMember(indexer.indexOffsets[position]);
        }

        public void SetComplexMember(ITacObject value,int position)
        {
            @object.SetComplexMember(value, indexer.indexOffsets[position]);
        }

        public T GetSimpleMember<T>(int position)
        {
            return @object.GetSimpleMember<T>(indexer.indexOffsets[position]);
        }

        public void SetSimpleMember<Tin>(Tin o,int position)
        {
            @object.SetSimpleMember(o,indexer.indexOffsets[position]);
        }

        public ITacObject GetComplexReadonlyMember(int position)
        {
            var returnedIndexer = indexer.nextIndexers[position];
            return new TacCastObject(@object.GetComplexReadonlyMember(indexer.indexOffsets[position]), returnedIndexer);
        }

        public void SetComplexWriteonlyMember(ITacObject tacCastObject,int position)
        {
            // tacCastObject has to be converted to the type our TacObject wants 
            // we trust our index to convert that way
            var setIndexer = indexer.nextIndexers[position];
            @object.SetComplexWriteonlyMember(
                new TacCastObject(tacCastObject, setIndexer),
                indexer.indexOffsets[position]);
        }

        // we actully could be a method
        // methods are understood to have an indexer with nothing in its indexOffsets 
        // and 2 nextIndexers with 0 being the input and 1 being the output
        // nextIndexers can be null if the input or output is primitive     

        public ITacObject Call_Complex_Complex(ITacObject input) {
            var inputIndexer = indexer.nextIndexers[0];
            var outputIndexer = indexer.nextIndexers[1];


            return new TacCastObject(@object.Call_Complex_Complex(new TacCastObject(input, inputIndexer)), outputIndexer);
        }
        public ITacObject Call_Simple_Complex<Tin>(Tin input) {

            var outputIndexer = indexer.nextIndexers[1];
            return new TacCastObject(@object.Call_Simple_Complex(input), outputIndexer);
        }
        public Tout Call_Complex_Simple<Tout>(ITacObject input) {

            var inputIndexer = indexer.nextIndexers[0];
            return @object.Call_Complex_Simple<Tout>(new TacCastObject(input, inputIndexer));
        }
        public Tout Call_Simple_Simple<Tin, Tout>(Tin input) {
            return @object.Call_Simple_Simple<Tin, Tout>(input);
        }
        public ITacObject SetComplexMemberReturn(ITacObject tacCastObject, int position)
        {
            SetComplexMemberReturn(tacCastObject, position);
            return tacCastObject;
        }

        public Tin SetSimpleMemberReturn<Tin>(Tin value, int position)
        {
            SetSimpleMemberReturn(value, position);
            return value;
        }

        public ITacObject SetComplexWriteonlyMemberReturn(ITacObject tacCastObject, int position)
        {
            SetComplexWriteonlyMemberReturn(tacCastObject, position);
            return tacCastObject;
        }
        public IVerifiableType TacType() => @object.TacType();
    }


    //why does everything else not have a consturctor
    public class TacObject : ITacObject
    {
        // this is: null, double, string, bool or TacCastObject
        public object[] members;
        public IVerifiableType type;

        //public IVerifiableType type;

        public ITacObject GetComplexMember(int position)
        {
            // when this returns a member it is important that it is a copy
            // if the member on TacObject is modified the TacCastObject should not be
            return (ITacObject)members[position];
        }

        public void SetComplexMember(ITacObject tacCastObject,int position)
        {
            members[position] = tacCastObject;
        }

        public T GetSimpleMember<T>(int position)
        {
            return (T)members[position];
        }

        public void SetSimpleMember<Tin>(Tin value,int position)
        {
            members[position] = value;
        }

        public ITacObject GetComplexReadonlyMember(int position) {
            return GetComplexMember(position);
        }
        public void SetComplexWriteonlyMember(ITacObject tacCastObject,int position) {
            SetComplexMember(tacCastObject,position);
        }

        public ITacObject Call_Complex_Complex(ITacObject input)
        {
            throw new NotImplementedException("not supported");
        }

        public ITacObject Call_Simple_Complex<Tin>(Tin input)
        {
            throw new NotImplementedException("not supported");
        }

        public Tout Call_Complex_Simple<Tout>(ITacObject input)
        {
            throw new NotImplementedException("not supported");
        }

        public Tout Call_Simple_Simple<Tin, Tout>(Tin input)
        {
            throw new NotImplementedException("not supported");
        }
        public ITacObject SetComplexMemberReturn(ITacObject tacCastObject, int position)
        {
            SetComplexMemberReturn(tacCastObject, position);
            return tacCastObject;
        }

        public Tin SetSimpleMemberReturn<Tin>(Tin value, int position)
        {
            SetSimpleMemberReturn(value, position);
            return value;
        }

        public ITacObject SetComplexWriteonlyMemberReturn(ITacObject tacCastObject, int position)
        {
            SetComplexWriteonlyMemberReturn(tacCastObject, position);
            return tacCastObject;
        }
        public IVerifiableType TacType() => type;
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
        //public (Indexer, object[]) GetComplexMember(object[] @object, Indexer[] objectIndexers, int position) {
        //    return (
        //        // this overlay makes my very sad, it makes preformance a lot worse
        //        // if the member cannot be set this is not a concern
        //        Overlay(objectIndexers[indexOffsets[position]], nextIndexers[position]), 
        //        (object[])@object[indexOffsets[position]]);
        //}

        //public T GetSimpleMember<T>(object[] @object, int position)
        //{
        //    return (T)@object[indexOffsets[position]];
        //}

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

        // for write only member the overlays can be backwords
        // I think this is ok, because writeonly will only ever overlay writeonly
        // readonly will likewise only ever overlay readonly
        // do I even use overlay??
        //public static Indexer Overlay(Indexer first, Indexer second) {
        //    if (first == null) {
        //        return second;
        //    }
        //    if (second == null)
        //    {
        //        return first;
        //    }

        //    var resultIndexOffsets = new int[second.indexOffsets.Length];
        //    var resultNextIndexers = new Indexer[second.indexOffsets.Length];
        //    for (int i = 0; i < second.indexOffsets.Length;i++) {
        //        resultIndexOffsets[i] = first.indexOffsets[second.indexOffsets[i]];
        //        resultNextIndexers[i] = Overlay(first.nextIndexers[second.indexOffsets[i]], second.nextIndexers[i]);
        //    }
        //    return new Indexer()
        //    {
        //        nextIndexers = resultNextIndexers,
        //        indexOffsets = resultIndexOffsets,
        //    };
        //}

        private static ConcurrentIndexed<(IVerifiableType, IVerifiableType), Indexer> map = new ConcurrentIndexed<(IVerifiableType, IVerifiableType), Indexer>();

        public static Indexer Create(IVerifiableType from, IVerifiableType to) {
            var toAdd = new Indexer();
            if (map.TryAdd((from, to), toAdd))
            {

                List<IMemberDefinition> toMembers = null, fromMembers = null;
            
                if (from.SafeIs(out IInterfaceType fromInterface)){
                    fromMembers = fromInterface.Members.OrderBy(x => ((NameKey)x.Key).Name).ToList();
                }
                if (from.SafeIs(out ITypeOr fromOr))
                {
                    fromMembers = fromOr.Members.OrderBy(x => ((NameKey)x.Key).Name).ToList();
                }
                if (to.SafeIs(out IInterfaceType toInterface))
                {
                    toMembers = toInterface.Members.OrderBy(x => ((NameKey)x.Key).Name).ToList();
                }
                if (to.SafeIs(out ITypeOr toOr))
                {
                    toMembers = toOr.Members.OrderBy(x => ((NameKey)x.Key).Name).ToList();
                }

                if (toMembers != null && fromMembers != null)
                {
                    return HasMembers(to, toAdd, toMembers, fromMembers);
                }

                from.TryGetInput().IfIs(fromInput =>
                    from.TryGetReturn().IfIs(fromOutput =>
                        to.TryGetInput().IfIs(toInput =>
                             to.TryGetReturn().IfIs(toOutput =>
                             {
                                 toAdd.nextIndexers = new[] {
                                        Create(toInput,fromInput),
                                        Create(fromOutput,toOutput)
                                     };
                             }))));

                if (toAdd.nextIndexers != null) {
                    return toAdd;
                }
                
                // this has to be from an Or
                // but in that case I shouldn't be using CastTacType
                // I should be unrapping in some way 
                if (to.SafeIs(out IPrimitiveType _))
                {
                    toAdd.nextIndexers = new Indexer[] { };
                    return toAdd;
                }

                if (to.SafeIs(out IAnyType toAny))
                {
                    toAdd.nextIndexers = new Indexer[] { };
                    return toAdd;
                }

                if (to.SafeIs(out ITypeOr _))
                {
                    toAdd.nextIndexers = new Indexer[] { };
                    return toAdd;
                }

                throw new NotImplementedException();
            }
            else
            {
                return map[(from, to)];
            }

        }

        private static Indexer HasMembers(IVerifiableType to, Indexer toAdd, List<IMemberDefinition> toMembers, List<IMemberDefinition> fromMembers)
        {
            var indexOffsets = new int[toMembers.Count];
            var nextIndexers = new Indexer[toMembers.Count];

            for (int toIndex = 0; toIndex < toMembers.Count; toIndex++)
            {
                var toMember = toMembers[toIndex];
                for (int fromIndex = 0; fromIndex < fromMembers.Count; fromIndex++)
                {
                    var fromMember = fromMembers[fromIndex];
                    if (fromMember.Key.Equals(toMember.Key))
                    {
                        indexOffsets[toIndex] = fromIndex;
                        if (fromMember.Type.SafeIs(out IMethodType innerFromMethod) && toMember.Type.SafeIs(out IMethodType innerToMethod))
                        {
                            nextIndexers[toIndex] = new Indexer()
                            {
                                nextIndexers = new[] {
                                                Create(innerToMethod.InputType,innerFromMethod.InputType),
                                                Create(innerFromMethod.OutputType,innerToMethod.OutputType)
                                            },
                            };
                        }
                        if (fromMember.Type.SafeIs(out IInterfaceModuleType innerFromInterface) && toMember.Type.SafeIs(out IInterfaceType innerToInterface))
                        {
                            nextIndexers[toIndex] = GetIndexer(toMember.Access, innerFromInterface, innerToInterface);
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

        public static Indexer GetIndexer(Access access, IInterfaceModuleType from, IInterfaceModuleType to) {
            switch (access)
            {
                case Access.ReadOnly:
                    return Create(from, to);
                case Access.ReadWrite:
                    return null;
                case Access.WriteOnly:
                    return Create(to, from);
                default:
                    throw new NotImplementedException("");
            }
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
