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
    
    internal abstract class BinaryOperation
    {
        public delegate IIsPossibly<T> Make<out T>(IIsPossibly<IFrontendCodeElement<ICodeElement>> left, IIsPossibly<IFrontendCodeElement<ICodeElement>> right);
    }

    internal abstract class BinaryOperation<TLeft, TRight,TCodeElement> : BinaryOperation, IFrontendCodeElement<TCodeElement>
        where TLeft : class, IFrontendCodeElement<ICodeElement>
        where TRight : class, IFrontendCodeElement<ICodeElement>
        where TCodeElement: class, ICodeElement
    {
        public IIsPossibly<TLeft> Left { get; }
        public IIsPossibly<TRight> Right { get; }
        public IIsPossibly<IFrontendCodeElement<ICodeElement>>[] Operands
        {
            get
            {
                return new IIsPossibly<IFrontendCodeElement<ICodeElement>>[] { Left, Right };
            }
        }
        
        public BinaryOperation(IIsPossibly<TLeft> left, IIsPossibly<TRight> right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }
        
        public abstract IIsPossibly<IFrontendType<IVerifiableType>> Returns();

        public abstract IBuildIntention<TCodeElement> GetBuildIntention(TransformerExtensions.ConversionContext context);
    }


    internal class BinaryOperationMaker<TFrontendCodeElement,TCodeElement> : IMaker<IPopulateScope<TFrontendCodeElement>>
        where TFrontendCodeElement : class, IFrontendCodeElement<TCodeElement>
        where TCodeElement: class, ICodeElement
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
                    new BinaryPopulateScope<TFrontendCodeElement, TCodeElement>(left, right, Make));
            }

            return TokenMatching<IPopulateScope<TFrontendCodeElement>>.MakeNotMatch(
                    matching.Context);
        }

    }


    internal class BinaryPopulateScope<TFrontendCodeElement, TCodeElement> : IPopulateScope<TFrontendCodeElement>
        where TFrontendCodeElement : class, IFrontendCodeElement<TCodeElement>
        where TCodeElement : class, ICodeElement
    {
        private readonly IPopulateScope<IFrontendCodeElement<ICodeElement>> left;
        private readonly IPopulateScope<IFrontendCodeElement<ICodeElement>> right;
        private readonly BinaryOperation.Make<TFrontendCodeElement> make;
        private readonly DelegateBox<IIsPossibly<IFrontendType<IVerifiableType>>> box = new DelegateBox<IIsPossibly<IFrontendType<IVerifiableType>>>();

        public BinaryPopulateScope(IPopulateScope<IFrontendCodeElement<ICodeElement>> left,
            IPopulateScope<IFrontendCodeElement<ICodeElement>> right,
            BinaryOperation.Make<TFrontendCodeElement> make)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IIsPossibly<IFrontendType<IVerifiableType>>> GetReturnType()
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

            return new BinaryResolveReferance<TFrontendCodeElement,TCodeElement>(
                left.Run(context),
                rightres,
                make,
                box);
        }
    }



    internal class BinaryResolveReferance<TFrontendCodeElement,TCodeElement> : IPopulateBoxes<TFrontendCodeElement>
        where TFrontendCodeElement : class, IFrontendCodeElement<TCodeElement>
        where TCodeElement : class, ICodeElement
    {
        public readonly IPopulateBoxes<IFrontendCodeElement<ICodeElement>> left;
        public readonly IPopulateBoxes<IFrontendCodeElement<ICodeElement>> right;
        private readonly BinaryOperation.Make<TFrontendCodeElement> make;
        private readonly DelegateBox<IIsPossibly<IFrontendType<IVerifiableType>>> box;

        public BinaryResolveReferance(
            IPopulateBoxes<IFrontendCodeElement<ICodeElement>> resolveReferance1,
            IPopulateBoxes<IFrontendCodeElement<ICodeElement>> resolveReferance2,
            BinaryOperation.Make<TFrontendCodeElement> make,
            DelegateBox<IIsPossibly<IFrontendType<IVerifiableType>>> box)
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
                else {

                    return Possibly.IsNot<IFrontendType<IVerifiableType>>(no);
                }
            });
            return res;
        }
    }
}
