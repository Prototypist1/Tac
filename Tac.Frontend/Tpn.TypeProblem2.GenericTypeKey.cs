//using Prototypist.Toolbox;
//using Prototypist.Toolbox.Object;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using Tac.Frontend.TypeProblem.Test;
//using Tac.Model;
//using Xunit;

//namespace Tac.Frontend.New.CrzayNamespace
//{

//    internal partial class Tpn
//    {


//        internal partial class TypeProblem2
//        {
//            // "key" in the sense this as used as a key for a dictionary
//            // describes an overlayed generic type
//            private class GenericTypeKey
//            {
//                // eqaulity for these get a big complex
//                // we have a circle equality situtation
//                // say we have GenericTypeKey's G1 and G2
//                // with TypeParameter G1T and G2T respectively
//                // G1<G1T> is G2<G2T> 
//                // to there we check G1 and G2 are the same if their type parameters are equal
//                // and G1T and G2T are the same if thier GenericTYpeKey's are equal
//                // 
//                // we solve this by first comparing G1 and G2 if they are equal 
//                // we complere G1T and G2T and tell them to do the comparision assume G1 equals G2
//                //
//                // things can of cource get pretty complex with generic in generic in generic
//                // maybe unit test this?
//                public class GenericTypeKeyEqualityComparer : EqualityComparer<GenericTypeKey>
//                {

//                    public bool Equals(GenericTypeKey? x, GenericTypeKey? y, List<(GenericTypeKey, GenericTypeKey)> assumeTrue)
//                    {
//                        if (x == y)
//                        {
//                            return true;
//                        }

//                        if ((x == null) || (y == null))
//                        {
//                            return false;
//                        }

//                        if (x.from == y.from) {
//                            return true;
//                        }

//                        // this is just "contains"
//                        // we use any and reference equals to avoid calls to "Equals" which was causing a stack overflow
//                        if (assumeTrue.Any(pair => (ReferenceEquals(pair.Item1, x) && ReferenceEquals(pair.Item2, y)) || (ReferenceEquals(pair.Item1, y) && ReferenceEquals(pair.Item2, x))))
//                        {
//                            return true;
//                        }
//                        assumeTrue.Add((x, y));

//                        return
//                            !x.sourceTypes.Any() &&
//                            !y.sourceTypes.Any() &&
//                            x.primary.Equals(y.primary) &&
//                            x.parameters.Length == x.parameters.Length &&
//                            x.parameters.Zip(y.parameters, (innerX, innerY) =>
//                            {
//                                if (innerX.Is7(out var x7) && innerY.Is7(out var y7))
//                                {
//                                    return x7.Equals(y7, (nextX, nextY) => this.Equals(nextX, nextY, assumeTrue));
//                                }

//                                if (innerX.Is8(out var x8) && innerY.Is8(out var y8))
//                                {
//                                    return Equals(x8, y8, assumeTrue);
//                                }

//                                return innerX.Equals(innerY);
//                            }).All(x => x);
//                    }

//                    public override bool Equals(GenericTypeKey? x, GenericTypeKey? y) => Equals(x, y, new List<(GenericTypeKey, GenericTypeKey)>());

//                    public override int GetHashCode([DisallowNull] GenericTypeKey obj)
//                    {
//                        return HashCode.Combine(obj.primary, obj.parameters.Length);
//                    }
//                }

//                private IKey from;
//                public IOrType<MethodType, Type> primary;
//                // I kind of feel like parameters
//                // should not be all of this stuff
//                // just Type, IError, TypeParameter and GenericTypeKey
//                public IOrType<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey>[] parameters;
//                //public Dictionary<NameKey, GenericTypeKey.TypeParameter> sourceTypes;

//                public GenericTypeKey(NameKey[] types, IKey from)
//                {
//                    if (types is null)
//                    {
//                        throw new ArgumentNullException(nameof(types));
//                    }

//                    int i = 0;
//                    this.sourceTypes = types?.ToDictionary(x => x, x => new GenericTypeKey.TypeParameter(i++, this)) ?? throw new ArgumentNullException(nameof(types));
//                    this.from = from ?? throw new ArgumentNullException(nameof(from));
//                }

//                public override bool Equals(object? obj)
//                {
//                    return Equals(obj as GenericTypeKey);
//                }

//                public bool Equals(GenericTypeKey? other)
//                {
//                    return new GenericTypeKeyEqualityComparer().Equals(this, other);
//                }

//                public override int GetHashCode()
//                {
//                    return HashCode.Combine(primary, parameters.Length);
//                }

//                public override string ToString()
//                {
//                    return primary.ToString() + " [" + String.Join(", ", sourceTypes.Select(x => x.Value.ToString())) + "] " + " [" + String.Join(", ", parameters.Select(x => x.ToString())) + "] ";
//                }
//                //public class TypeParameter
//                //{
//                //    public readonly int index;
//                //    public GenericTypeKey owner;

//                //    public TypeParameter(int index, GenericTypeKey owner)
//                //    {
//                //        this.index = index;
//                //        this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
//                //    }

//                //    public bool Equals(TypeParameter typeParameter, Func<GenericTypeKey, GenericTypeKey, bool> equal)
//                //    {
//                //        return index == typeParameter.index && equal(this.owner, typeParameter.owner);
//                //    }

//                //    public override bool Equals(object? obj)
//                //    {
//                //        return obj is TypeParameter other &&
//                //            index == other.index &&
//                //            new GenericTypeKeyEqualityComparer().Equals(this.owner, other.owner);
//                //    }

//                //    public override int GetHashCode()
//                //    {
//                //        return HashCode.Combine(index, owner);
//                //    }

//                //    public override string ToString()
//                //    {
//                //        return "T" + index;
//                //    }
//                //}
//            }



//            //private class DoubleGenericTypeKey
//            //{
//            //    private readonly IOrType<MethodType, Type, Method> primary;
//            //    private readonly IOrType<MethodType, Type, Object, OrType, InferredType, IError, TypeParameter, DoubleGenericTypeKey, GenericTypeKey>[] parameters;

//            //    public DoubleGenericTypeKey(IOrType<MethodType, Type, Method> primary, IOrType<MethodType, Type, Object, OrType, InferredType, IError, TypeParameter, DoubleGenericTypeKey, GenericTypeKey>[] parameters)
//            //    {
//            //        this.primary = primary ?? throw new ArgumentNullException(nameof(primary));
//            //        this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
//            //    }



//            //}

//            // we only handle control back for simple look ups, for generics we build the GenericTypeKey
//            // if I do that GenericTypeKey is going to need to be able to hold  DoubleGenericTypeKey.TypeParameter and DoubleGenericTypeKey
//            // maybe I can combine GenericTypeKey and DoubleGenericTypeKey

//            // why does this exist?
//            // ultimate we want to determine equality of complex generics
//            // is generic-method [t1,ta] [t1, generic-method [t2,tb] [Pair[t1,t2], Pair[ta,tb]]] the same as generic-method [t11,ta1] [t11, generic-method [t21,tb1] [Pair[t11,t21], Pair[ta1,tb1]]]? (yes, names don't matter, provided Pair resolves to the same type in the contexts where both are used)
//            // so the goal is to better representation of these things with the simple types resolved and the names replaced by indexes
//            // this creates those representations
//            // we couldn't ask the keys to do that themselves
//            // because sometimes your rep depends on the rep of the key you were defined in
//            // for example: generic-method [T1,T2] [T1, Pair[T2]], Pair need to look up T2 from the generic-method that contains it
//            private class KeyVisitor : IKeyVisitor<IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>>
//            {
//                private readonly Func<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> lookup;
//                //private readonly Dictionary<NameKey, GenericTypeKey.TypeParameter> generics;
//                //private readonly MethodType rootMethod;

//                private KeyVisitor(
//                    Func<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> lookup)
//                {
//                    this.lookup = lookup ?? throw new ArgumentNullException(nameof(lookup));
//                    //this.rootMethod = rootMethod ?? throw new ArgumentNullException(nameof(rootMethod));
//                }

//                public static KeyVisitor Base(Func<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> lookup)
//                {
//                    return new KeyVisitor(lookup);
//                }


//                public IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError> DoubleGenericNameKey(DoubleGenericNameKey doubleGenericNameKey)
//                {
//                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(DoubleGenericNameKey_BetterType(doubleGenericNameKey));
//                }


//                public GenericTypeKey DoubleGenericNameKey_BetterType(DoubleGenericNameKey doubleGenericNameKey)
//                {
//                    //if (doubleGenericNameKey.Name.Name != "generic-method")
//                    //{
//                    //    throw new Exception("we really only suppory methods");
//                    //}

//                    var primary = lookup(doubleGenericNameKey.Name).SwitchReturns(
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type>(x),
//                        x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
//                        x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
//                        x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
//                        x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
//                        x => throw new Exception("only methodType, Type, and Method can have generic parameters"));

//                    return DuobleGenericNameKey_BetterType_PassInPrimary(doubleGenericNameKey, primary);
//                }

//                public GenericTypeKey DuobleGenericNameKey_BetterType_PassInPrimary(DoubleGenericNameKey doubleGenericNameKey, IOrType<MethodType, Type> primary)
//                {
//                    var res = new GenericTypeKey(doubleGenericNameKey.Types, doubleGenericNameKey);

//                    var context = Next(res);

//                    res.parameters = doubleGenericNameKey.DependentTypes.Select(dependentType => dependentType.SwitchReturns(
//                                x => x.Visit(context),
//                                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey>(x))).ToArray();
//                    res.primary = primary;

//                    return res;
//                }

//                public IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey> GenericNameKey(GenericNameKey genericNameKey)
//                {
//                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey > (GenericNameKey_BetterTyped(genericNameKey));
//                }

//                public GenericTypeKey GenericNameKey_BetterTyped(GenericNameKey genericNameKey)
//                {
//                    var res = new GenericTypeKey(Array.Empty<NameKey>(), genericNameKey);
//                    var context = Next(res);

//                    res.parameters = genericNameKey.Types.Select(dependentType => dependentType.SwitchReturns(
//                        x => x.Visit(context),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey>(x))).ToArray();

//                    res.primary = lookup(genericNameKey.Name).SwitchReturns(
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type>(x),
//                        x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
//                        x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
//                        x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
//                        x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
//                        x => throw new Exception("only methodType, Type, and Method can have generic parameters"));

//                    return res;
//                }

//                public IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey> ImplicitKey(ImplicitKey implicitKey)
//                {
//                    return lookup(implicitKey).SwitchReturns(
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x));
//                }

//                public IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey> NameKey(NameKey nameKey)
//                {
//                    return lookup(nameKey).SwitchReturns(
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x),
//                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError, GenericTypeKey>(x));
//                }
//            }

//            //internal class Tests
//            //{
//            //    // method [t1] [t1, t1]
//            //    // equals
//            //    // method [t2] [t2, t2]
//            //    public void GenericMethodsCanUnify()
//            //    {
//            //        var sample1 = new DoubleGenericNameKey(
//            //            new NameKey("method"),
//            //            new[] { new NameKey("T1") },
//            //            new[] {
//            //        Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T1")),
//            //        Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T1"))});

//            //        var sample2 = new DoubleGenericNameKey(
//            //            new NameKey("method"),
//            //            new[] { new NameKey("T2") },
//            //            new[] {
//            //        Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T2")),
//            //        Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T2"))});

//            //        var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), TestTpn.DefaultRootScopePopulateScope(), _ => { });

//            //        var keyVisitor = Tpn.TypeProblem2.KeyVisitor.Base(y => Tpn.TypeProblem2.LookUpOrError(problem.ModuleRoot, y), new Dictionary<Tpn.TypeProblem2.GenericTypeParameter, Tpn.TypeProblem2.GenericTypeKey.TypeParameter>());

//            //        var sample1Visited = keyVisitor.DoubleGenericNameKey(sample1).Is8OrThrow();
//            //        var sample2Visited = keyVisitor.DoubleGenericNameKey(sample2).Is8OrThrow();

//            //        Assert.Equal(sample1Visited, sample2Visited);
//            //    }

//            //    // method [t1] [t1, pair [t1] ]
//            //    // equals
//            //    // method [t2] [t2, pair [t2]]
//            //    public void GenericMethodsCanUnify_ReturnGeneric()
//            //    {
//            //        var sample1 = new DoubleGenericNameKey(
//            //            new NameKey("method"),
//            //            new[] { new NameKey("T1") },
//            //            new[] {
//            //        Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T1")),
//            //        Prototypist.Toolbox.OrType.Make<IKey,IError>(new GenericNameKey(new NameKey("pair"),new []{ Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T1")) }))});

//            //        var sample2 = new DoubleGenericNameKey(
//            //            new NameKey("method"),
//            //            new[] { new NameKey("T2") },
//            //            new[] {
//            //        Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T2")),
//            //        Prototypist.Toolbox.OrType.Make<IKey,IError>(new GenericNameKey(new NameKey("pair"),new []{ Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T2")) }))});

//            //        var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), TestTpn.DefaultRootScopePopulateScope(), _ => { });

//            //        var pairType = problem.builder.CreateGenericType(
//            //            problem.ModuleRoot,
//            //            Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("pair")),
//            //            new[]{
//            //                new Tpn.TypeAndConverter(
//            //                    new NameKey("T"),
//            //                    new WeakTypeDefinitionConverter())},
//            //            new WeakTypeDefinitionConverter());

//            //        var keyVisitor = Tpn.TypeProblem2.KeyVisitor.Base(y => Tpn.TypeProblem2.LookUpOrError(problem.ModuleRoot, y), new Dictionary<Tpn.TypeProblem2.GenericTypeParameter, Tpn.TypeProblem2.GenericTypeKey.TypeParameter>());

//            //        var sample1Visited = keyVisitor.DoubleGenericNameKey(sample1).Is8OrThrow();
//            //        var sample2Visited = keyVisitor.DoubleGenericNameKey(sample2).Is8OrThrow();

//            //        Assert.Equal(sample1Visited, sample2Visited);
//            //    }

//            //    // method [t1] [t1, method [t] [t, t1]]
//            //    // equals
//            //    // method [t2] [t2, method [t] [t, t2]]
//            //    public void GenericMethodsCanUnify_ReturnDoubleGeneric()
//            //    {
//            //        var sample1 = new DoubleGenericNameKey(
//            //            new NameKey("method"),
//            //            new[] { new NameKey("T1") },
//            //            new[] {
//            //        Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T1")),
//            //        Prototypist.Toolbox.OrType.Make<IKey,IError>(new DoubleGenericNameKey(
//            //                new NameKey("method"),
//            //                new[] { new NameKey("T") },
//            //                new[] {
//            //            Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T")),
//            //            Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T1"))}))});

//            //        var sample2 = new DoubleGenericNameKey(
//            //            new NameKey("method"),
//            //            new[] { new NameKey("T2") },
//            //            new[] {
//            //        Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T2")),
//            //        Prototypist.Toolbox.OrType.Make<IKey,IError>(new DoubleGenericNameKey(
//            //                new NameKey("method"),
//            //                new[] { new NameKey("T") },
//            //                new[] {
//            //            Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T")),
//            //            Prototypist.Toolbox.OrType.Make<IKey,IError>(new NameKey("T2"))}))});

//            //        var problem = new Tpn.TypeProblem2(new WeakScopeConverter(), TestTpn.DefaultRootScopePopulateScope(), _ => { });

//            //        var keyVisitor = Tpn.TypeProblem2.KeyVisitor.Base(y => Tpn.TypeProblem2.LookUpOrError(problem.ModuleRoot, y), new Dictionary<Tpn.TypeProblem2.GenericTypeParameter, Tpn.TypeProblem2.GenericTypeKey.TypeParameter>());

//            //        var sample1Visited = keyVisitor.DoubleGenericNameKey(sample1).Is8OrThrow();
//            //        var sample2Visited = keyVisitor.DoubleGenericNameKey(sample2).Is8OrThrow();

//            //        Assert.Equal(sample1Visited, sample2Visited);
//            //    }
//            //}
//        }
//    }

//    // trying out extremely local tests
//    // XUnit can't find tests in inner classes
//    // so I'll help
//    //public class GenericTypeKeyTester {
//    //    [Fact]
//    //    public void GenericMethodsCanUnify() {
//    //        new Tpn.TypeProblem2.Tests().GenericMethodsCanUnify();
//    //    }

//    //    [Fact]
//    //    public void GenericMethodsCanUnify_ReturnGeneric()
//    //    {
//    //        new Tpn.TypeProblem2.Tests().GenericMethodsCanUnify_ReturnGeneric();
//    //    }
//    //    [Fact]
//    //    public void GenericMethodsCanUnify_ReturnDoubleGeneric()
//    //    {
//    //        new Tpn.TypeProblem2.Tests().GenericMethodsCanUnify_ReturnDoubleGeneric();
//    //    }
//    //}
//}
