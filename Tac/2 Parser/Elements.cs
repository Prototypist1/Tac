using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{
    public class Elements
    {
        public Elements(List<TryMatch> elementBuilders) => ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));

        public delegate bool TryMatch(ElementToken elementToken, out ICodeElement element);

        public List<TryMatch> ElementBuilders { get; }

        // TODO different commands are allowed in different scopes 
        public static Lazy<Elements> StandardElements = new Lazy<Elements>(() =>
        {
            return new Elements(
                new List<TryMatch> {
                    MatchLocalDefinition_Var,
                    MatchMethodDefinition,
                    MatchBlockDefinition,
                    MatchConstantNumber,
                    MatchMemberReferance
                });
        });

        public static bool MatchLocalDefinition_Var(ElementToken elementToken, out ICodeElement element){
            if ((elementToken.Tokens.Count() == 2 || elementToken.Tokens.Count() == 3) && elementToken.Tokens.First() is AtomicToken atomicToken && atomicToken.Item == "var") {

                var readOnly = elementToken.Tokens.Count() == 3 && 
                    elementToken.Tokens.ElementAt(1) is AtomicToken readOnlyToken &&
                    readOnlyToken.Item == "readonly";

                element = new LocalDefinition(readOnly, new ImplicitTypeReferance(), new ExplicitName("var"));

                return true;
            }
            
            element = default;
            return false;
        }
        
        public static bool MatchMethodDefinition(ElementToken elementToken, out ICodeElement element)
        {
            if (
                elementToken.Tokens.Count() == 3 && 
                elementToken.Tokens.First() is AtomicToken first &&
                    first.Item.StartsWith("method") &&
                    first.Item.Split('|').Count() == 3 &&
                    first.Item.Split('|').All(x=>x.Length!=0) &&
                elementToken.Tokens.ElementAt(1) is AtomicToken second &&
                elementToken.Tokens.ElementAt(2) is CurleyBacketToken third
                )
            {
                var split = first.Item.Split('|');
                element = new MethodDefinition(
                    new TypeReferance(split[2]),
                    new ParameterDefinition(
                        false, // TODO, the way this is hard coded is something to think about, readonly should be encoded somewhere!
                        new TypeReferance(split[1]),
                        new ExplicitName(second.Item)),
                    TokenParser.ParseBlock(third));

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchBlockDefinition(ElementToken elementToken, out ICodeElement element)
        {
            if (
                elementToken.Tokens.Count() == 1  &&
                elementToken.Tokens.First() is CurleyBacketToken first
                )
            {
                element = new BlockDefinition(
                    TokenParser.ParseBlock(first));

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchConstantNumber(ElementToken elementToken, out ICodeElement element)
        {
            if (
                elementToken.Tokens.Count() == 1 &&
                elementToken.Tokens.First() is AtomicToken first &&
                double.TryParse(first.Item,out var dub)
                )
            {
                element = new ConstantNumber(dub);

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchMemberReferance(ElementToken elementToken, out ICodeElement element)
        {
            if (
                elementToken.Tokens.Count() == 1 &&
                elementToken.Tokens.First() is AtomicToken first &&
                !double.TryParse(first.Item, out var _)
                )
            {
                element = new MemberReferance(first.Item);

                return true;
            }

            element = default;
            return false;
        }

    }
}
