﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tac._3_Syntax_Model.Elements.Atomic_Types;
//using Tac.Frontend._2_Parser;
//using Tac.Model.Elements;
//using Tac.Model.Instantiated.Elements;
//using Tac.New;
//using Tac.Parser;
//using Tac.Semantic_Model;
//using static Tac.Frontend.TransformerExtensions;

//namespace Tac.Frontend._3_Syntax_Model.Elements
//{
//    // what is the empty instance...
//    // I though empty was a type
//    // more analgous to something like a bool with a single value
//    // than the null reference 
//    internal class WeakEmptyInstance : IConvertableFrontendCodeElement<IEmptyInstance>
//    {
//        public WeakEmptyInstance()
//        {
//        }

//        public IBuildIntention<IEmptyInstance> GetBuildIntention(ConversionContext context)
//        {
//            var (toBuild, maker) = EmptyInstance.Create();
//            return new BuildIntention<IEmptyInstance>(toBuild, () =>
//            {
//                maker.Build();
//            });
//        }

//        public IIsPossibly<IConvertableFrontendType<IVerifiableType>> Returns()
//        {
//            return Possibly.Is<IConvertableFrontendType<IVerifiableType>>(PrimitiveTypes.CreateEmptyType());
//        }
//    }

//    internal class EmptyInstanceMaker : IMaker<IPopulateScope<WeakEmptyInstance>>
//    {
//        public EmptyInstanceMaker() { }

//        public ITokenMatching<IPopulateScope<WeakEmptyInstance>> TryMake(IMatchedTokenMatching tokenMatching)
//        {
//            var match = tokenMatching
//                .Has(new KeyWordMaker("new-empty"), out var _);

//            if (match
//                 is IMatchedTokenMatching matched)
//            {
//                return TokenMatching<IPopulateScope<WeakEmptyInstance>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new EmptyInstancePopulateScope());
//            }
//            return TokenMatching<IPopulateScope<WeakEmptyInstance>>.MakeNotMatch(tokenMatching.Context);
//        }

//        public static IPopulateScope<WeakEmptyInstance> PopulateScope()
//        {
//            return new EmptyInstancePopulateScope();
//        }
//        public static IPopulateBoxes<WeakEmptyInstance> PopulateBoxes()
//        {
//            return new EmptyInstanceResolveReferance();
//        }

//        private class EmptyInstancePopulateScope : IPopulateScope<WeakEmptyInstance>
//        {

//            public EmptyInstancePopulateScope() { }

//            public IPopulateBoxes<WeakEmptyInstance> Run(IPopulateScopeContext context)
//            {
//                return new EmptyInstanceResolveReferance();
//            }

//            public IBox<IIsPossibly<IFrontendType>> GetReturnType()
//            {
//                return new Box<IIsPossibly<IFrontendType>>(Possibly.Is<IFrontendType>(PrimitiveTypes.CreateEmptyType()));
//            }
//        }

//        private class EmptyInstanceResolveReferance : IPopulateBoxes<WeakEmptyInstance>
//        {
//            public EmptyInstanceResolveReferance()
//            {
//            }

//            public IIsPossibly<WeakEmptyInstance> Run(IResolveReferenceContext context)
//            {
//                return Possibly.Is(new WeakEmptyInstance());
//            }
//        }
//    }
//}
