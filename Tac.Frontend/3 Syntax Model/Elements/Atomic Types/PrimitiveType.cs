﻿using Prototypist.TaskChain;
using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.SemanticModel;
using Tac.Type;

namespace Tac.SyntaxModel.Elements.AtomicTypes
{
    internal static class FrontendTypeExtensions
    {
        public static IOrType<IFrontendType<IVerifiableType>, IError> UnwrapRefrence(this IFrontendType<IVerifiableType> frontendType)
        {
            if (frontendType is RefType refType)
            {
                return refType.inner;
            }
            return OrType.Make<IFrontendType<IVerifiableType>, IError>(frontendType);
        }
    }

    internal interface IPrimitiveType : IFrontendType<IVerifiableType>
    {
    }

    internal class FrontEndOrType : IFrontendType<ITypeOr>
    {
        //internal readonly IOrType<IFrontendType<IVerifiableType>, IError> left, right;
        //private readonly IOrType<Dictionary<IKey, IOrType<IOrType<WeakMemberDefinition, IError>, No, IError>>, IError> members;
        //private readonly IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> returns;
        //private readonly IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> input;

        //public FrontEndOrType(IOrType<IFrontendType<IVerifiableType>, IError> left, IOrType<IFrontendType<IVerifiableType>, IError> right)
        //{
        //    // TODO this is wrong
        //    // I think
        //    // I don't think this should be a validation error
        //    // it should be an exception at converstion type
        //    // Idk this design is a little werid
        //    this.left = left ?? throw new ArgumentNullException(nameof(left));
        //    this.right = right ?? throw new ArgumentNullException(nameof(right));

        //    members = this.left.TransformInner(x => x.GetMembers().ToDictionary(x => x.Key, x => InitMember(x.Key)));

        //    IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> InitMember(IKey key)
        //    {
        //        return OrTypeLibrary.GetMember(
        //             key,
        //             left,
        //             right,
        //             // Func<T, IKey, List<(T, T)>, IOrType<IOrType<(T,Access), IError>, No, IError>> tryGetMember,
        //             (x, key, list) => x.TryGetMember(key, list).SwitchReturns(
        //                 y => y.SwitchReturns(
        //                     z => z.Type.GetValue().SwitchReturns(
        //                         zz => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(OrType.Make<(IFrontendType<IVerifiableType>, Access), IError>((zz, z.Access))),
        //                         zz => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(zz)),
        //                     z => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(z)),
        //                 y => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(y),
        //                 y => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(y)
        //                 ),
        //             (x, y) => new FrontEndOrType(OrType.Make<IFrontendType<IVerifiableType>, IError>(x), OrType.Make<IFrontendType<IVerifiableType>, IError>(y)),
        //             (us, them, list) => us.TheyAreUs(them, list),
        //             new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()
        //             ).SwitchReturns(
        //             x => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(x.TransformInner(y => new WeakMemberDefinition(y.Item2, key, new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(y.Item1))))),
        //             x => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(x),
        //             x => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(x));
        //    }

        //    returns = OrTypeLibrary.TryGetReturn(left, right, (x) => x.TryGetReturn(), (x, y) => new FrontEndOrType(x, y));
        //    input = OrTypeLibrary.TryGetInput(left, right, (x) => x.TryGetInput(), (x, y) => new FrontEndOrType(x, y));
        //}

        //public IBuildIntention<ITypeOr> GetBuildIntention(IConversionContext context)
        //{
        //    var (res, builder) = Tac.Model.Instantiated.TypeOr.Create();

        //    var inputType = left;
        //    var outputType = right;
        //    return new BuildIntention<ITypeOr>(res
        //        , () =>
        //        {
        //            builder.Build(
        //                inputType.Is1OrThrow().SafeCastTo<IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>>().Convert(context),
        //                outputType.Is1OrThrow().SafeCastTo<IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>>().Convert(context));
        //        });
        //}

        //private static IIsPossibly<(IOrType<IFrontendType<IVerifiableType>, IError> fromLeft, IOrType<IFrontendType<IVerifiableType>, IError> fromRight)> IsOrType(IFrontendType<IVerifiableType> them)
        //{
        //    if (them.SafeIs(out FrontEndOrType orType))
        //    {
        //        return Possibly.Is<(IOrType<IFrontendType<IVerifiableType>, IError> fromLeft, IOrType<IFrontendType<IVerifiableType>, IError> fromRight)>((orType.left, orType.right));
        //    }
        //    return Possibly.IsNot<(IOrType<IFrontendType<IVerifiableType>, IError> fromLeft, IOrType<IFrontendType<IVerifiableType>, IError> fromRight)>();
        //}

        //public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        //{
        //    return OrTypeLibrary.CanAssign(they, IsOrType, this, left, right, (x, y, list) => x.TheyAreUs(y, list), assumeTrue);
        //}


        //public IEnumerable<IError> Validate()
        //{
        //    foreach (var item in left.SwitchReturns(x => x.Validate(), x => new[] { x }))
        //    {
        //        yield return item;
        //    }
        //    foreach (var item in right.SwitchReturns(x => x.Validate(), x => new[] { x }))
        //    {
        //        yield return item;
        //    }
        //}

        //public IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        //{
        //    if (members.Is1(out var v1))
        //    {
        //        if (v1.TryGetValue(key, out var res))
        //        {
        //            return res;
        //        }
        //        else
        //        {
        //            return OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(new No());
        //        }
        //    }
        //    else if (members.Is2(out var v2))
        //    {
        //        return OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(v2);
        //    }
        //    else {
        //        throw new Exception("or overflown!");
        //    }
        //}

        //public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => returns;

        //public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => input;

        //public IReadOnlyList<WeakMemberDefinition> GetMembers()
        //{
        //    //Is1OrThrow is slowy here
        //    // probably this interface should entertain the possiblity that a type doesn't know it's memebers because the code is jibberish
        //    return members.Is1OrThrow().Values.Select(x => x.Is1OrThrow().Is1OrThrow()).ToArray();
        //}
        public IBox<IOrType<IFrontendType<IVerifiableType>, IError>> left;
        public IBox<IOrType<IFrontendType<IVerifiableType>, IError>> right;
        private IOrType<IReadOnlyList<WeakMemberDefinition>, IError> members;
        private IIsPossibly<IBox<IOrType<IFrontendType<IVerifiableType>, IError>>> input;
        private IIsPossibly<IBox<IOrType<IFrontendType<IVerifiableType>, IError>>> output;

        public FrontEndOrType(
            IBox<IOrType<IFrontendType<IVerifiableType>, IError>> left,
            IBox<IOrType<IFrontendType<IVerifiableType>, IError>> right,
            IOrType<IReadOnlyList<WeakMemberDefinition>, IError> members,
            IIsPossibly<IBox<IOrType<IFrontendType<IVerifiableType>, IError>>> input,
            IIsPossibly<IBox<IOrType<IFrontendType<IVerifiableType>, IError>>> output)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.members = members ?? throw new ArgumentNullException(nameof(members));
            this.input = input ?? throw new ArgumentNullException(nameof(input));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public IBuildIntention<ITypeOr> GetBuildIntention(IConversionContext context)
        {
            var (res, builder) = Tac.Model.Instantiated.TypeOr.Create();

            var localLeft = left.GetValue();
            var localRight = right.GetValue();
            return new BuildIntention<ITypeOr>(res
                , () =>
                {
                    builder.Build(
                        localLeft.Is1OrThrow().SafeCastTo<IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>>().Convert(context),
                        localRight.Is1OrThrow().SafeCastTo<IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>>().Convert(context),
                        members.Is1OrThrow().Select(x=>x.Convert(context)).ToArray(),
                        input.TransformInner(x=>x.GetValue().Is1OrThrow().Convert(context)),
                        output.TransformInner(x => x.GetValue().Is1OrThrow().Convert(context))
                        );
                });
        }

        // Is1OrThrow is sloppy here
        // probably this interface should entertain the possiblity that a type doesn't know it's members because the code is jibberish
        //public IReadOnlyList<WeakMemberDefinition> GetMembers() => members.Is1OrThrow();

        private static IIsPossibly<(IOrType<IFrontendType<IVerifiableType>, IError> fromLeft, IOrType<IFrontendType<IVerifiableType>, IError> fromRight)> IsOrType(IFrontendType<IVerifiableType> them)
        {
            if (them.SafeIs(out FrontEndOrType orType))
            {
                return Possibly.Is<(IOrType<IFrontendType<IVerifiableType>, IError> fromLeft, IOrType<IFrontendType<IVerifiableType>, IError> fromRight)>((orType.left.GetValue(), orType.right.GetValue()));
            }
            return Possibly.IsNot<(IOrType<IFrontendType<IVerifiableType>, IError> fromLeft, IOrType<IFrontendType<IVerifiableType>, IError> fromRight)>();
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            return OrTypeLibrary.CanAssign(they, IsOrType, this, left.GetValue(), right.GetValue(), (x, y, list) => x.TheyAreUs(y, list), assumeTrue);
        }

        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput()
        {
            return input.IfElseReturn(x => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(x.GetValue()),
                () => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No()));
        }

        public IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            return members.SwitchReturns(
                x =>
                {
                    if (!x.Any(y => y.Key.Equals(key)))
                    {
                        return OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(new No());
                    }
                    return OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(
                        OrType.Make<WeakMemberDefinition, IError>(x.Single(y => y.Key.Equals(key))));
                },
                error => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(error));
        }

        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn()
        {
            return output.IfElseReturn(x => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(x.GetValue()),
                () => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No()));
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var item in left.GetValue().SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return item;
            }
            foreach (var item in right.GetValue().SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return item;
            }
            // we might need to validate more, but I think if an Or's componets are Ok, it is OK
        }
    }

    // EXTERNAL TYPES | 6D97B5C3-BFB5-4F1F-AE91-8955AD8277AD
    //
    // the external types represent mebers or types created by another assembly
    // they are disignishable from internal 
    // but when they build instead of creating a new instance they return one provided from outside the compilation
    internal class ExternalHasMembersType : HasMembersTypeAbstract
    {
        private readonly IInterfaceType result;
        private readonly IReadOnlyList<WeakExternslMemberDefinition> members;

        public ExternalHasMembersType(IInterfaceType result, IReadOnlyList<WeakExternslMemberDefinition> members)
        {
            this.result = result ?? throw new ArgumentNullException(nameof(result));
            this.members = members ?? throw new ArgumentNullException(nameof(members));
        }

        public override IBuildIntention<IInterfaceType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IInterfaceType>(result, () => { });
        }

        public override IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            return HasMembersLibrary.CanAssign(
                they,
                this,
                members.Select(x => (x.Key, x.Type.GetValue().TransformInner(y => (y, x.Access)))).ToList(),
                (target, key) =>
                {
                    //jesus these data structures
                    return target.TryGetMember(key, assumeTrue).SwitchReturns(
                        x => x.SwitchReturns(
                            y => y.Type.GetValue().SwitchReturns(
                                z => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(OrType.Make<(IFrontendType<IVerifiableType>, Access), IError>((z, y.Access))),
                                z => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(z)),
                            y => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(y)),
                        x => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(x),
                        x => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(x));
                }
                ,
                (target, other, assumes) => target.TheyAreUs(other, assumes),
                assumeTrue
                );
        }

        public override IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            return HasMembersLibrary.TryGetMember(key, members.Select(x => (
                key: x.Key,
                type: (IOrType<WeakMemberDefinition, IError>)OrType.Make<WeakMemberDefinition, IError>(x))).ToList());
        }

        public override IEnumerable<IError> Validate() => Array.Empty<IError>();
    }


    internal abstract class HasMembersTypeAbstract : IFrontendType<IInterfaceType>
    {
        public abstract IBuildIntention<IInterfaceType> GetBuildIntention(IConversionContext context);
        public abstract IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue);
        public abstract IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue);
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public abstract IEnumerable<IError> Validate();
    }

    internal class HasMembersType : HasMembersTypeAbstract
    {
        public override IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            return HasMembersLibrary.TryGetMember(key, weakScope.membersList.Select(x => (
                key: x.Key,
                type: (IOrType<WeakMemberDefinition, IError>)OrType.Make<WeakMemberDefinition, IError>(x))).ToList());
        }

        public override IBuildIntention<IInterfaceType> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = InterfaceType.Create();
            return new BuildIntention<IInterfaceType>(toBuild, () =>
            {
                maker.Build(weakScope.Convert(context).Members.Values.Select(x => x.Value).ToArray());
            });
        }

        public override IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            return HasMembersLibrary.CanAssign(
                they,
                this,
                weakScope.membersList.Select(x => (x.Key, x.Type.GetValue().TransformInner(y => (y, x.Access)))).ToList(),
                (target, key) =>
                {
                    //jesus these data structures
                    return target.TryGetMember(key, assumeTrue).SwitchReturns(
                        x => x.SwitchReturns(
                            y => y.Type.GetValue().SwitchReturns(
                                z => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(OrType.Make<(IFrontendType<IVerifiableType>, Access), IError>((z, y.Access))),
                                z => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(z)),
                            y => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(y)),
                        x => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(x),
                        x => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(x));
                }
                ,
                (target, other, assumes) => target.TheyAreUs(other, assumes),
                assumeTrue
                );
        }

        public override IEnumerable<IError> Validate() => weakScope.membersList.Select(x => x.Type.GetValue().Possibly1()).OfType<IIsDefinately<IFrontendType<IVerifiableType>>>().SelectMany(x => x.Value.Validate());

        public readonly WeakScope weakScope;

        public HasMembersType(WeakScope weakScope)
        {
            this.weakScope = weakScope ?? throw new ArgumentNullException(nameof(weakScope));
        }
    }

    // reference is a type!
    // but it probably does not mean what you think it means
    // it really means you can assign to it
    // this is what is returned by member and member reference 

    // I am not sure this is doing me any good
    // and it stinks to have to unwrap it 
    // I think the place it would be useful is in validate of assignment
    // but I am not even using it there
    // it could be a real type tho
    // Method [ref[int], empty] x { x := 5 } =: set-to-five
    //
    // var a = 2;
    // a > set-to-five
    // a > (out.write-number)
    // 
    // would print 5
    internal class RefType : IFrontendType<IVerifiableType>, IPrimitiveType
    {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IReferanceType>(new Model.Instantiated.ReferanceType(), () => { });
        }

        public IEnumerable<IError> Validate() => inner.SwitchReturns(x => x.Validate(), x => Array.Empty<IError>());

        public IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(new No());

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            // the method calling this
            // is in charge of unwrapping
            throw new Exception("I don't think this should ever happen");

            // this is a bit of a smell
            // a lot of a smell
        }

        public readonly IOrType<IFrontendType<IVerifiableType>, IError> inner;

        public RefType(IOrType<IFrontendType<IVerifiableType>, IError> inner)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());

        //public IReadOnlyList<WeakMemberDefinition> GetMembers() => Array.Empty<WeakMemberDefinition>();
    }

    // should this really exist?
    // is it a type?
    // it is an error...
    // it is not convertable
    //internal class IndeterminateType : IFrontendType<IVerifiableType>
    //{
    //    private readonly IError error;

    //    public IndeterminateType(IError error)
    //    {
    //        this.error = error ?? throw new ArgumentNullException(nameof(error));
    //    }

    //    public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
    //    {
    //        return OrType.Make<bool, IError>(ReferenceEquals(this, they));
    //    }

    //    public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
    //    public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetMember(IKey key) => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
    //    public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());

    //    public IEnumerable<IError> Validate()
    //    {
    //        return new[] { error };
    //    }
    //}

    internal struct BlockType : IFrontendType<IVerifiableType>, IPrimitiveType
    {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IBlockType>(new Model.Instantiated.BlockType(), () => { });
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<bool, IError>(they is BlockType);

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(new No());


        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        //public IReadOnlyList<WeakMemberDefinition> GetMembers() => Array.Empty<WeakMemberDefinition>();
    }

    internal struct StringType : IFrontendType<IVerifiableType>, IPrimitiveType
    {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IStringType>(new Model.Instantiated.StringType(), () => { });
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<bool, IError>(they is StringType);
        public IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(new No());

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        //public IReadOnlyList<WeakMemberDefinition> GetMembers() => Array.Empty<WeakMemberDefinition>();
    }
    internal struct EmptyType : IFrontendType<IVerifiableType>, IPrimitiveType
    {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IEmptyType>(new Model.Instantiated.EmptyType(), () => { });
        }
        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<bool, IError>(they is EmptyType);
        public IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(new No());
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        //public IReadOnlyList<WeakMemberDefinition> GetMembers() => Array.Empty<WeakMemberDefinition>();
    }

    internal struct NumberType : IFrontendType<IVerifiableType>, IPrimitiveType
    {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<INumberType>(new Model.Instantiated.NumberType(), () => { });
        }
        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<bool, IError>(they is NumberType);
        public IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(new No());
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        //public IReadOnlyList<WeakMemberDefinition> GetMembers() => Array.Empty<WeakMemberDefinition>();
    }

    // I don't think this is a type....
    // placeholders are effectively defined by constraints really just metadata
    // 
    // do I need this interface?
    internal interface IGenericTypeParameterPlacholder : IFrontendType<IVerifiableType>
    { 
        int Index { get; }
        IBox<IOrType<IFrontendType<IVerifiableType>, IError>> Constraint { get; }
    }

    internal class GenericTypeParameterPlacholder : IGenericTypeParameterPlacholder
    {
        public GenericTypeParameterPlacholder(int index, IBox<IOrType<IFrontendType<IVerifiableType>, IError>> constraint)
        {
            this.Index = index;
            Constraint = constraint ?? throw new ArgumentNullException(nameof(constraint));
        }

        public int Index { get; }
        public IBox<IOrType<IFrontendType<IVerifiableType>, IError>> Constraint { get; }


        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            var (res ,build)= GenericTypeParameter.Create();

            return new BuildIntention<IVerifiableType>(res, () => build.Build(Index, Constraint.GetValue().Is1OrThrow().Convert(context)) );
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            if (assumeTrue.Contains((this, they)))
            {
                return OrType.Make<bool, IError>(true);
            }
            assumeTrue.Add((this, they));

            return Constraint.GetValue().TransformInner(x=>x.TheyAreUs(they, assumeTrue));
        }

        public IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => Constraint.GetValue().SwitchReturns(x=>x.TryGetMember(key, assumeTrue), x=> OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(x));
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => Constraint.GetValue().SwitchReturns(x => x.TryGetReturn(), x=> OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(x));
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => Constraint.GetValue().SwitchReturns(x => x.TryGetInput(), x=> OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(x));

#pragma warning disable CA1822 // Mark members as static
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
#pragma warning restore CA1822 // Mark members as static
    }

    internal struct AnyType : IFrontendType<IVerifiableType>, IPrimitiveType
    {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IAnyType>(new Model.Instantiated.AnyType(), () => { });
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<bool, IError>(true);
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        //public IReadOnlyList<WeakMemberDefinition> GetMembers() => Array.Empty<WeakMemberDefinition>();
    }

    internal struct BooleanType : IFrontendType<IVerifiableType>, IPrimitiveType
    {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IBooleanType>(new Tac.Model.Instantiated.BooleanType(), () => { });
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<bool, IError>(they is BooleanType);

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        //public IReadOnlyList<WeakMemberDefinition> GetMembers() => Array.Empty<WeakMemberDefinition>();
    }

    internal class MethodType : IFrontendType<IVerifiableType>, IPrimitiveType
    {

        private struct Yes { }

        // is the meta-data here worth capturing
        public static MethodType ImplementationType(
            IBox<IOrType<IFrontendType<IVerifiableType>, IError>> inputType,
            IBox<IOrType<IFrontendType<IVerifiableType>, IError>> outputType,
            IBox<IOrType<IFrontendType<IVerifiableType>, IError>> contextType)
        {
            return new MethodType(
                contextType,
                new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new MethodType(inputType, outputType))));
        }

        public MethodType(
            IBox<IOrType<IFrontendType<IVerifiableType>, IError>> inputType,
            IBox<IOrType<IFrontendType<IVerifiableType>, IError>> outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }

        public IBox<IOrType<IFrontendType<IVerifiableType>, IError>> InputType { get; }
        public IBox<IOrType<IFrontendType<IVerifiableType>, IError>> OutputType { get; }

        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            var (res, builder) = Tac.Model.Instantiated.MethodType.Create();

            // this is stack allocated and might be GC'ed so we need to create locals
            // to feed to the lambda
            var inputType = InputType;
            var outputType = OutputType;
            return new BuildIntention<IMethodType>(res
                , () =>
                {
                    builder.Build(
                        inputType.GetValue().Is1OrThrow().SafeCastTo<IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>>().Convert(context),
                        outputType.GetValue().Is1OrThrow().SafeCastTo<IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>>().Convert(context));
                });
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var error in InputType.GetValue().SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return error;
            }
            foreach (var error in OutputType.GetValue().SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return error;
            }
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            return MethodLibrary.CanAssign(
                they,
                this,
                InputType.GetValue(),
                OutputType.GetValue(),
                x => x.TryGetInput(),
                x => x.TryGetReturn(),
                (target, other, list) => target.TheyAreUs(other, list),
                assumeTrue);
        }
        public IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(new No());


        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(OutputType.GetValue());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(InputType.GetValue());
        //public IReadOnlyList<WeakMemberDefinition> GetMembers() => Array.Empty<WeakMemberDefinition>();
    }

    internal class GenericMethodType : IFrontendType<IVerifiableType> //,IFrontendGenericType
    {
        public readonly IBox<IOrType<IFrontendType<IVerifiableType>, IError>> inputType;
        public readonly IBox<IOrType<IFrontendType<IVerifiableType>, IError>> outputType;
        public readonly IOrType<IGenericTypeParameterPlacholder, IError>[] typeParameterDefinitions;

        public GenericMethodType(IBox<IOrType<IFrontendType<IVerifiableType>, IError>> inputType, Box<IOrType<IFrontendType<IVerifiableType>, IError>> outputType, IOrType<IGenericTypeParameterPlacholder, IError>[] typeParameterDefinitions)
        {
            this.inputType = inputType;
            this.outputType = outputType;
            this.typeParameterDefinitions = typeParameterDefinitions;
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var error in inputType.GetValue().SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return error;
            }
            foreach (var error in outputType.GetValue().SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return error;
            }
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            if (!they.SafeIs(out GenericMethodType genericMethod))
            {
                return OrType.Make<bool, IError>(false);
            }

            if (typeParameterDefinitions.Length != genericMethod.typeParameterDefinitions.Length)
            {
                return OrType.Make<bool, IError>(false); ;
            }

            return MethodLibrary.CanAssign(
                they,
                this,
                inputType.GetValue(),
                outputType.GetValue(),
                x => x.TryGetInput(),
                x => x.TryGetReturn(),
                (target, other, list) => target.TheyAreUs(other, list),
                assumeTrue);
        }
        public IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(new No());


        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(outputType.GetValue());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(inputType.GetValue());

        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            var (res, builder) = Tac.Model.Instantiated.GenericMethodType.Create();

            // this is stack allocated and might be GC'ed so we need to create locals
            // to feed to the lambda
            var inputType = this.inputType;
            var outputType = this.outputType;
            return new BuildIntention<IGenericMethodType>(res
                , () =>
                    builder.Build(
                        inputType.GetValue().Is1OrThrow().SafeCastTo<IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>>().Convert(context),
                        outputType.GetValue().Is1OrThrow().SafeCastTo<IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>>().Convert(context),
                        typeParameterDefinitions
                            .Select(x=> x.Is1OrThrow().Convert(context).SafeCastTo(out GenericTypeParameter _)/* can I push this earlier? it would be nice to cast earlier. it's a little painful becuase of the unwrapping, I don't know if I was careful with the types the whole way */)
                            .ToArray()));
        }
    }
}
