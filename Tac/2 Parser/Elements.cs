using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Parser
{
    public class Elements
    {
        public Elements(List<TryMatch> elementBuilders) => ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));

        public delegate bool TryMatch(ElementToken elementToken, out CodeElement element);

        public List<TryMatch> ElementBuilders { get; }

        public static Lazy<Elements> StandardElements = new Lazy<Elements>(() =>
        {
            return new Elements(
                new List<TryMatch> { });
        });

        public static bool LocalVar(ElementToken elementToken, out CodeElement element){
            if (elementToken.Tokens.Count() >= 2 && elementToken.Tokens.First() is AtomicToken atomicToken && atomicToken.Item == "var") {

                var readOnly = elementToken.Tokens.Count() == 3 && 
                    elementToken.Tokens.ElementAt(1) is AtomicToken readOnlyToken &&
                    readOnlyToken.Item == "readonly";

                element = new LocalDefinition

                return true;
            }
            
            element = default;
            return false;
        }

    }
}
