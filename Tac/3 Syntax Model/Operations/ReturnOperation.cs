using System;
using System.Collections.Generic;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public interface IReturnOperation<ICodeElement> {

    }

    public class WeakReturnOperation : TrailingOperation, IWeakCodeElement
    {

        public const string Identifier = "return";

        public WeakReturnOperation(IWeakCodeElement result)
        {
            Result = result;
        }

        public IWeakCodeElement Result { get; }
        
        public IWeakReturnable Returns(IElementBuilders elementBuilders)
        {
            return elementBuilders.EmptyType();
        }
    }

    public class ITrailingOperion<T> {
    }

    public class TrailingOperation {
        public delegate T Make<T>(IWeakCodeElement codeElement);
    }

    public class TrailingOperationMaker<T> : IOperationMaker<T>
        where T : class, IWeakCodeElement
    {
        public TrailingOperationMaker(string name, TrailingOperation.Make<T> make)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public string Name { get; }
        private TrailingOperation.Make<T> Make { get; }

        public IResult<IPopulateScope<T>> TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsTrailingOperation(Name), out var perface, out var _)
            .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                
                return ResultExtension.Good(new TrailingPopulateScope<T>(left,Make));
            }
            return ResultExtension.Bad<IPopulateScope<T>>();
        }
        
    }

    public class TrailingPopulateScope<T> : IPopulateScope<T>
        where T : IWeakCodeElement
    {
        private readonly IPopulateScope<IWeakCodeElement> left;
        private readonly TrailingOperation.Make<T> make;
        private readonly DelegateBox<IWeakReturnable> box = new DelegateBox<IWeakReturnable>();

        public TrailingPopulateScope(IPopulateScope<IWeakCodeElement> left, TrailingOperation.Make<T> make)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IPopulateBoxes<T> Run(IPopulateScopeContext context)
        {
            return new TrailingResolveReferance<T>(left.Run(context),  make, box);
        }
    }



    public class TrailingResolveReferance<T> : IPopulateBoxes<T>
        where T : IWeakCodeElement
    {
        public readonly IPopulateBoxes<IWeakCodeElement> left;
        private readonly TrailingOperation.Make<T> make;
        private readonly DelegateBox<IWeakReturnable> box;

        public TrailingResolveReferance(IPopulateBoxes<IWeakCodeElement> resolveReferance1, TrailingOperation.Make<T> make, DelegateBox<IWeakReturnable> box)
        {
            left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }
        
        public T Run(IResolveReferanceContext context)
        {
            var res = make(left.Run(context));
            box.Set(()=>res.Returns(context.ElementBuilders));
            return res;
        }
    }


    public class ReturnOperationMaker : TrailingOperationMaker<WeakReturnOperation>
    {
        public ReturnOperationMaker(TrailingOperation.Make<WeakReturnOperation> make) : base(WeakReturnOperation.Identifier, make)
        {
        }
    }
}
