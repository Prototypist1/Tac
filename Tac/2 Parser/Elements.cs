using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{
    public class Operation<T>
        where T: Delegate
    {
        public readonly T make;
        public readonly string idenifier;

        public Operation(T make, string idenifier)
        {
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.idenifier = idenifier ?? throw new ArgumentNullException(nameof(idenifier));
        }
    }

    internal class ElementMatchingContext
    {

        internal ElementMatchingContext ExpectPathPart(IBox<IVarifiableType> box) {
            return new ElementMatchingContext(operationMatchers, new IMaker<IPopulateScope<ICodeElement>>[] {
                new MemberReferanceMaker(box)
            });
        }
        
        internal ElementMatchingContext AcceptImplicit(IBox<IVarifiableType> box)
        {
            return new ElementMatchingContext(operationMatchers, new IMaker<IPopulateScope<ICodeElement>>[] {
                new BlockDefinitionMaker(),
                new ConstantNumberMaker(),
                new GenericTypeDefinitionMaker(),
                new ImplementationDefinitionMaker(),
                new MemberDefinitionMaker(),
                new MethodDefinitionMaker(),
                new ModuleDefinitionMaker(),
                new ObjectDefinitionMaker(),
                new TypeDefinitionMaker(),
                new ImplicitMemberMaker(box),
                new MemberMaker(),
            });
        }
        
        //internal ElementMatchingContext Child(ScopeStack scope)
        //{
        //    return new ElementMatchingContext(Builders,operationMatchers, elementMakers, scope);
        //}
        
        public ElementMatchingContext() : 
            this(
                new IMaker<IPopulateScope<ICodeElement>>[] {
                    new AddOperationMaker(),
                    new SubtractOperationMaker(),
                    new MultiplyOperationMaker(),
                    new IfTrueOperationMaker(),
                    new ElseOperationMaker(),
                    new LessThanOperationMaker(),
                    new NextCallOperationMaker(),
                    new AssignOperationMaker(),
                    new PathOperationMaker(),
                    new ReturnOperationMaker()
                },
                new IMaker<IPopulateScope<ICodeElement>>[] {
                    new BlockDefinitionMaker(),
                    new ConstantNumberMaker(),
                    new GenericTypeDefinitionMaker(),
                    new ImplementationDefinitionMaker(),
                    new MemberDefinitionMaker(),
                    new MethodDefinitionMaker(),
                    new ModuleDefinitionMaker(),
                    new ObjectDefinitionMaker(),
                    new TypeDefinitionMaker(),
                    new MemberMaker(),
                }){}
        
        public ElementMatchingContext(IMaker<IPopulateScope<ICodeElement>>[] operationMatchers, IMaker<IPopulateScope<ICodeElement>>[] elementMakers)
        {
            this.operationMatchers = operationMatchers ?? throw new ArgumentNullException(nameof(operationMatchers));
            this.elementMakers = elementMakers ?? throw new ArgumentNullException(nameof(elementMakers));
        }

        private readonly IMaker<IPopulateScope<ICodeElement>>[] elementMakers;
        private readonly IMaker<IPopulateScope<ICodeElement>>[] operationMatchers;
        
        #region Parse

        public IPopulateScope<ICodeElement> ParseParenthesisOrElement(IToken token)
        {
            if (token is ElementToken elementToken)
            {
                // smells
                // why did i write this agian?
                // why would an element be wrapped in parenthesis ?
                // maybe I can just remove??
                // maybe we have a parentthesis matcher?
                if (elementToken.Tokens.Count() == 1 && elementToken.Tokens.First() is ParenthesisToken parenthesisToken)
                {
                    return ParseLine(parenthesisToken.Tokens);
                }

                foreach (var tryMatch in elementMakers)
                {
                    if (TokenMatching<IPopulateScope<ICodeElement>>.Start(elementToken.Tokens,this)
                        .Has(tryMatch, out var res)
                        .Has(new DoneMaker())
                        .IsMatch)
                    {
                        return res;
                    }
                }
            }
            else if (token is ParenthesisToken parenthesisToken)
            {
                return ParseLine(parenthesisToken.Tokens);
            }

            throw new Exception("");
        }

        public IPopulateScope<ICodeElement> ParseLine(IEnumerable<IToken> tokens)
        {
            foreach (var operationMatcher in operationMatchers)
            {
                if (TokenMatching<IPopulateScope<ICodeElement>>.Start(tokens, this)
                        .Has(operationMatcher, out IPopulateScope<ICodeElement> res)
                        .IsNotMatch.Not())
                {
                    return res;
                }
            }

            if (tokens.Count() == 1)
            {
                return ParseParenthesisOrElement(tokens.Single());
            }

            throw new Exception("");
        }

        public IPopulateScope<ICodeElement>[] ParseFile(FileToken file)
        {
            return file.Tokens.Select(x => ParseLine(x.Cast<LineToken>().Tokens)).ToArray();
        }

        public IPopulateScope<ICodeElement>[] ParseBlock(CurleyBracketToken block)
        {
            return block.Tokens.Select(x =>
            {
                if (x is LineToken lineToken)
                {
                    return ParseLine(lineToken.Tokens);
                }
                throw new Exception("unexpected token type");
            }).ToArray();
        }

        #endregion

    }

    internal interface ITokenMatching
    {
        ElementMatchingContext Context { get; }
        bool IsNotMatch { get; }
        bool IsMatch { get; }
        IEnumerable<IToken> Tokens { get; }
    }

    internal interface ITokenMatching<out T>: IResult<T>, ITokenMatching
    {
    }

    internal class TokenMatching<T> :ITokenMatching<T>
    {

        private TokenMatching(IEnumerable<IToken> tokens, bool isNotMatch, ElementMatchingContext Context,T value)
        {
            IsNotMatch = isNotMatch;
            this.Context = Context ?? throw new ArgumentNullException(nameof(Context));
            this.Value = value;
            Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        }

        public bool HasValue
        {
            get
            {
                return !IsNotMatch;
            }
        }

        public bool IsMatch => !IsNotMatch;
        public bool IsNotMatch { get; }
        public IEnumerable<IToken> Tokens { get; }
        public ElementMatchingContext Context { get; }
        public T Value { get; }


        public static TokenMatching<T> Start(IEnumerable<IToken> tokens, ElementMatchingContext context)
        {
            return Match(tokens, context, default);
        }

        public static TokenMatching<T> Match(IEnumerable<IToken> tokens, ElementMatchingContext context, T value)
        {
            return new TokenMatching<T>(tokens, false, context,value);
        }

        public static TokenMatching<T> NotMatch(IEnumerable<IToken> tokens, ElementMatchingContext context)
        {
            return new TokenMatching<T>(tokens, true, context, default);
        }

    }

    internal static class ElementMatcher
    {
   

        public static ITokenMatching Has<T>(this ITokenMatching self, IMaker<T> pattern, out T t)
        {
            t = default;

            if (self.IsNotMatch)
            {
                return self;

            }

            var res = pattern.TryMake(self);
            if (res.HasValue)
            {
                t = res.Value;
            }
            return res;

        }

        public static ITokenMatching HasSquare(this ITokenMatching self, Func<ITokenMatching, ITokenMatching> inner)
        {
            if (self.IsNotMatch) {
                return self;
            }

            if (self.Tokens.First() is SquareBacketToken squareBacketToken)
            {
                if (inner(TokenMatching<object>.Start(squareBacketToken.Tokens, self.Context)).IsMatch) {
                    return TokenMatching<object>.Start(self.Tokens.Skip(1), self.Context);
                };
                return TokenMatching<object>.NotMatch(self.Tokens.Skip(1), self.Context);
            }

            return TokenMatching<object>.NotMatch(self.Tokens.Skip(1), self.Context);
        }
        
        public static ITokenMatching HasLine(this ITokenMatching self, Func<ITokenMatching, ITokenMatching> inner)
        {
            if (self.IsNotMatch)
            {
                return self;
            }

            if (self.Tokens.First() is LineToken line)
            {
                if (inner(TokenMatching<object>.Start(line.Tokens, self.Context)).IsMatch)
                {
                    return TokenMatching<object>.Start(self.Tokens.Skip(1), self.Context);
                };
                return TokenMatching<object>.NotMatch(self.Tokens.Skip(1), self.Context);
            }

            return TokenMatching<object>.NotMatch(self.Tokens.Skip(1), self.Context);
        }

        public static ITokenMatching HasElement(this ITokenMatching self, Func<ITokenMatching, ITokenMatching> inner)
        {
            if (self.IsNotMatch)
            {
                return self;
            }

            if (self.Tokens.First() is ElementToken elementToken)
            {
                if (inner(TokenMatching<object>.Start(elementToken.Tokens, self.Context)).IsMatch)
                {
                    return TokenMatching<object>.Start(self.Tokens.Skip(1), self.Context);
                };
                return TokenMatching<object>.NotMatch(self.Tokens.Skip(1), self.Context);
            }

            return TokenMatching<object>.NotMatch(self.Tokens.Skip(1), self.Context);
        }

        public static ITokenMatching Has(this ITokenMatching self, IMaker pattern)
        {
            if (self.IsNotMatch)
            {
                return self;
            }

            return pattern.TryMake(self);
        }


        public static ITokenMatching OptionalHas<T>(this ITokenMatching self, IMaker<T> pattern, out T t)
            where T : class
        {


            if (self.IsNotMatch)
            {
                t = default;
                return self;
            }

            var res = pattern.TryMake(self);
            if (res.HasValue)
            {
                t = res.Value;
                return res;
            }

            t = default;
            return self;
        }

        public static ITokenMatching OptionalHas(this ITokenMatching self, IMaker pattern)
        {
            if (self.IsNotMatch)
            {
                return self;
            }

            var next = pattern.TryMake(self);
            if (next.IsNotMatch)
            {
                return self;
            }

            return next;
        } 
    }
}
