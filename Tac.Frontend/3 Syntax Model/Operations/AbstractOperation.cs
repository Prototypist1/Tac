using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model.CodeStuff
{
    public static class Symbols{
        public static string[] GetSymbols()=> 
            AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(ISymbols).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
            .Select(x=> Activator.CreateInstance(x)
            .Cast<ISymbols>().Symbols)
            .ToArray();
    }

    public interface ISymbols {
        string Symbols { get; }
    }
    
    internal class BinaryOperation
    {
        public delegate IIsPossibly<T> Make<out T>(IIsPossibly<IFrontendCodeElement> left, IIsPossibly<IFrontendCodeElement> right);
        public delegate IIsPossibly<T> MakeBinaryType<out T>(IIsPossibly<IFrontendType> left, IIsPossibly<IFrontendType> right);
    }

    internal abstract class BinaryOperation<TLeft, TRight,TCodeElement> :  IConvertableFrontendCodeElement<TCodeElement>
        where TLeft : class, IFrontendCodeElement
        where TRight : class, IFrontendCodeElement
        where TCodeElement: class, ICodeElement
    {
        public IIsPossibly<TLeft> Left { get; }
        public IIsPossibly<TRight> Right { get; }
        public IIsPossibly<IFrontendCodeElement>[] Operands
        {
            get
            {
                return new IIsPossibly<IFrontendCodeElement>[] { Left, Right };
            }
        }
        
        public BinaryOperation(IIsPossibly<TLeft> left, IIsPossibly<TRight> right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }
        
        public abstract IIsPossibly<IFrontendType> Returns();

        public abstract IBuildIntention<TCodeElement> GetBuildIntention(TransformerExtensions.ConversionContext context);
    }



    internal abstract class BinaryTypeOperation<TLeft, TRight, TType> : IConvertableFrontendType<TType>
        where TLeft : IFrontendType
        where TRight : IFrontendType
        where TType: IVerifiableType 
    { 
        public IIsPossibly<TLeft> Left { get; }
        public IIsPossibly<TRight> Right { get; }
        public IEnumerable<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> Operands { get {
                // this make me sad,
                // if we could mark TLeft, TRight as classes and I would need these ugly casts
                // but a lot of types are structs
                // so I have casts
                // I assume this has something to do with boxing
                // the cast boxes
                yield return (IIsPossibly<IConvertableFrontendType<IVerifiableType>>)Left;
                yield return (IIsPossibly<IConvertableFrontendType<IVerifiableType >>)Right;
            }
        }

        public BinaryTypeOperation(IIsPossibly<TLeft> left, IIsPossibly<TRight> right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }


        public abstract IBuildIntention<TType> GetBuildIntention(TransformerExtensions.ConversionContext context);
    }



    internal class BinaryOperationMaker<TFrontendCodeElement, TCodeElement> : IMaker<IPopulateScope<TFrontendCodeElement>>
        where TFrontendCodeElement : class, IConvertableFrontendCodeElement<TCodeElement>
        where TCodeElement : class, ICodeElement
    {

        public BinaryOperationMaker(ISymbols name, BinaryOperation.Make<TFrontendCodeElement> make
            )
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public ISymbols Name { get; }
        private BinaryOperation.Make<TFrontendCodeElement> Make { get; }

        public ITokenMatching<IPopulateScope<TFrontendCodeElement>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(Name.Symbols), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(match.perface);
                var right = matching.Context.ParseParenthesisOrElement(match.rhs);

                return TokenMatching<IPopulateScope<TFrontendCodeElement>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new BinaryPopulateScope(left, right, Make));
            }

            return TokenMatching<IPopulateScope<TFrontendCodeElement>>.MakeNotMatch(
                    matching.Context);
        }


        public static IPopulateScope<TFrontendCodeElement> PopulateScope(IPopulateScope<IFrontendCodeElement> left,
                IPopulateScope<IFrontendCodeElement> right,
                BinaryOperation.Make<TFrontendCodeElement> make)
        {
            return new BinaryPopulateScope(left,
                 right,
                 make);
        }
        public static IPopulateBoxes<TFrontendCodeElement> PopulateBoxes(IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>> resolveReferance1,
                IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>> resolveReferance2,
                BinaryOperation.Make<TFrontendCodeElement> make,
                DelegateBox<IIsPossibly<IFrontendType>> box)
        {
            return new BinaryResolveReferance(resolveReferance1,
                resolveReferance2,
                make,
                box);
        }



        private class BinaryPopulateScope : IPopulateScope<TFrontendCodeElement>
        {
            private readonly IPopulateScope<IFrontendCodeElement> left;
            private readonly IPopulateScope<IFrontendCodeElement> right;
            private readonly BinaryOperation.Make<TFrontendCodeElement> make;
            private readonly DelegateBox<IIsPossibly<IFrontendType>> box = new DelegateBox<IIsPossibly<IFrontendType>>();

            public BinaryPopulateScope(IPopulateScope<IFrontendCodeElement> left,
                IPopulateScope<IFrontendCodeElement> right,
                BinaryOperation.Make<TFrontendCodeElement> make)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public IBox<IIsPossibly<IFrontendType>> GetReturnType()
            {
                return box;
            }

            public IPopulateBoxes<TFrontendCodeElement> Run(IPopulateScopeContext context)
            {
                // TODO
                // this is something I don't much like
                // right runs first because of assign
                // in assign you might have something like
                // method [int;int] input { input < ? 2 if { 1 return; } else { input - 1 > fac * input return; } } =: fac
                // if the left runs first than fac will not be found
                // and so it will add it to the scope
                // but if the right is run first 
                // fac works
                // if I add an assign that goes the other way...
                // this will break

                // part of me just thinks 
                // force 'var' on member definition 
                var rightres = right.Run(context);

                return new BinaryResolveReferance(
                    left.Run(context),
                    rightres,
                    make,
                    box);
            }
        }

        private class BinaryResolveReferance : IPopulateBoxes<TFrontendCodeElement>
        {
            public readonly IPopulateBoxes<IFrontendCodeElement> left;
            public readonly IPopulateBoxes<IFrontendCodeElement> right;
            private readonly BinaryOperation.Make<TFrontendCodeElement> make;
            private readonly DelegateBox<IIsPossibly<IFrontendType>> box;

            public BinaryResolveReferance(
                IPopulateBoxes<IFrontendCodeElement> resolveReferance1,
                IPopulateBoxes<IFrontendCodeElement> resolveReferance2,
                BinaryOperation.Make<TFrontendCodeElement> make,
                DelegateBox<IIsPossibly<IFrontendType>> box)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
            }


            public IIsPossibly<TFrontendCodeElement> Run(IResolveReferenceContext context)
            {
                var res = make(
                    left.Run(context),
                    right.Run(context));
                box.Set(() => {
                    if (res.IsDefinately(out var yes, out var no))
                    {
                        return yes.Value.Returns();
                    }
                    else
                    {

                        return Possibly.IsNot<IConvertableFrontendType<IVerifiableType>>(no);
                    }
                });
                return res;
            }
        }
    }


    internal class BinaryTypeMaker : IMaker<IPopulateScope<IWeakTypeReference>>
    {

        public BinaryTypeMaker(ISymbols name, BinaryOperation.MakeBinaryType<IWeakTypeReference> make
            )
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public ISymbols Name { get; }
        private BinaryOperation.MakeBinaryType<IWeakTypeReference> Make { get; }

        public ITokenMatching<IPopulateScope<IWeakTypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(Name.Symbols), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseTypeLine(match.perface);
                var right = matching.Context.ParseParenthesisOrElementType(match.rhs);

                return TokenMatching<IPopulateScope<IWeakTypeReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new BinaryPopulateScope(left, right, Make));
            }

            return TokenMatching<IPopulateScope<IWeakTypeReference>>.MakeNotMatch(
                    matching.Context);
        }


        public static IPopulateScope<IWeakTypeReference> PopulateScope(IPopulateScope<IConvertableFrontendType<IVerifiableType>> left,
                IPopulateScope<IConvertableFrontendType<IVerifiableType>> right,
                BinaryOperation.MakeBinaryType<IWeakTypeReference> make)
        {
            return new BinaryPopulateScope( left,
                 right,
                 make);
        }
        public static IPopulateBoxes<IWeakTypeReference> PopulateBoxes(IPopulateBoxes<IConvertableFrontendType<IVerifiableType>> resolveReferance1,
                IPopulateBoxes<IConvertableFrontendType<IVerifiableType>> resolveReferance2,
                BinaryOperation.MakeBinaryType<IWeakTypeReference> make)
        {
            return new BinaryResolveReferance(resolveReferance1,
                resolveReferance2,
                make);
        }



        private class BinaryPopulateScope : IPopulateScope<IWeakTypeReference>
        {
            private readonly IPopulateScope<IConvertableFrontendType<IVerifiableType>> left;
            private readonly IPopulateScope<IConvertableFrontendType<IVerifiableType>> right;
            private readonly BinaryOperation.MakeBinaryType<IWeakTypeReference> make;
            private readonly IBox<IIsPossibly<IFrontendType>> box = new Box<IIsPossibly<IFrontendType>>(new TypeType());

            public BinaryPopulateScope(IPopulateScope<IConvertableFrontendType<IVerifiableType>> left,
                IPopulateScope<IConvertableFrontendType<IVerifiableType>> right,
                BinaryOperation.MakeBinaryType<IWeakTypeReference> make)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public IBox<IIsPossibly<IFrontendType>> GetReturnType()
            {
                return box;
            }

            public IPopulateBoxes<IWeakTypeReference> Run(IPopulateScopeContext context)
            {
                // TODO
                // this is something I don't much like
                // right runs first because of assign
                // in assign you might have something like
                // method [int;int] input { input < ? 2 if { 1 return; } else { input - 1 > fac * input return; } } =: fac
                // if the left runs first than fac will not be found
                // and so it will add it to the scope
                // but if the right is run first 
                // fac works
                // if I add an assign that goes the other way...
                // this will break

                // part of me just thinks 
                // force 'var' on member definition 
                var rightres = right.Run(context);

                return new BinaryResolveReferance(
                    left.Run(context),
                    rightres,
                    make);
            }
        }

        private class BinaryResolveReferance : IPopulateBoxes<IWeakTypeReference>
        {
            public readonly IPopulateBoxes<IConvertableFrontendType<IVerifiableType>> left;
            public readonly IPopulateBoxes<IConvertableFrontendType<IVerifiableType>> right;
            private readonly BinaryOperation.MakeBinaryType<IWeakTypeReference> make;

            public BinaryResolveReferance(
                IPopulateBoxes<IConvertableFrontendType<IVerifiableType>> resolveReferance1,
                IPopulateBoxes<IConvertableFrontendType<IVerifiableType>> resolveReferance2,
                BinaryOperation.MakeBinaryType<IWeakTypeReference> make)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }


            public IIsPossibly<IWeakTypeReference> Run(IResolveReferenceContext context)
            {
                var res = make(
                    left.Run(context),
                    right.Run(context));

                return res;
            }
        }
    }

}
