using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{

    internal class WeakReturnOperation : TrailingOperation, ICodeElement, IReturnOperation
    {

        public const string Identifier = "return";

        public WeakReturnOperation(ICodeElement result)
        {
            Result = result;
        }


        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ReturnOperation(this);
        }


        public ICodeElement Result { get; }
        
        public IType Returns()
        {
            return new EmptyType();
        }
    }

    internal class ITrailingOperion<T> {
    }

    internal class TrailingOperation {
        public delegate T Make<T>(ICodeElement codeElement);
    }

    internal class TrailingOperationMaker<T> : IOperationMaker<T>
        where T : class, ICodeElement
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

    internal class TrailingPopulateScope<T> : IPopulateScope<T>
        where T : class, ICodeElement
    {
        private readonly IPopulateScope<ICodeElement> left;
        private readonly TrailingOperation.Make<T> make;
        private readonly DelegateBox<IType> box = new DelegateBox<IType>();

        public TrailingPopulateScope(IPopulateScope<ICodeElement> left, TrailingOperation.Make<T> make)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IType> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<T> Run(IPopulateScopeContext context)
        {
            return new TrailingResolveReferance<T>(left.Run(context),  make, box);
        }
    }



    internal class TrailingResolveReferance<T> : IPopulateBoxes<T>
        where T : class, ICodeElement
    {
        public readonly IPopulateBoxes<ICodeElement> left;
        private readonly TrailingOperation.Make<T> make;
        private readonly DelegateBox<IType> box;

        public TrailingResolveReferance(IPopulateBoxes<ICodeElement> resolveReferance1, TrailingOperation.Make<T> make, DelegateBox<IType> box)
        {
            left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }
        
        public T Run(IResolveReferanceContext context)
        {
            var res = make(left.Run(context));
            box.Set(()=>res.Returns());
            return res;
        }
    }


    internal class ReturnOperationMaker : TrailingOperationMaker<WeakReturnOperation>
    {
        public ReturnOperationMaker() : base(WeakReturnOperation.Identifier, x=>new WeakReturnOperation(x))
        {
        }
    }
}
