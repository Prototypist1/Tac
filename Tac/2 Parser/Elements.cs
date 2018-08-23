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
    public class ParsingContext {

    }

    public class Elements
    {
        public Elements(List<TryMatch> elementBuilders) => ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));

        public delegate bool TryMatch(ElementToken elementToken, IScope enclosingScope , out ICodeElement element);

        public List<TryMatch> ElementBuilders { get; }
        
        public static Lazy<Elements> StandardElements = new Lazy<Elements>(() =>
        {
            return new Elements(
                new List<TryMatch> {
                    MatchStaticMemberDefinition,
                    MatchObjectDefinition,
                    MatchLocalDefinition_Var,
                    MatchMethodDefinition,
                    MatchBlockDefinition,
                    MatchConstantNumber,
                    MatchReferanceOrMemberDef
                });
        });
        
        public static bool MatchLocalDefinition_Var(ElementToken elementToken, IScope enclosingScope, out ICodeElement element){
            if ((elementToken.Tokens.Count() == 2 || elementToken.Tokens.Count() == 3) &&
                elementToken.Tokens.First() is AtomicToken atomicToken &&
                atomicToken.Item == "var" &&
                elementToken.Tokens.Last() is AtomicToken nameToken) {

                var readOnly = elementToken.Tokens.Count() == 3 && 
                    elementToken.Tokens.ElementAt(1) is AtomicToken readOnlyToken &&
                    readOnlyToken.Item == "readonly";

                element = new VariableDefinition(readOnly, new ImplicitTypeReferance(), new ExplicitName(nameToken.Item));

                return true;
            }
            
            element = default;
            return false;
        }

        public static bool MatchStaticMemberDefinition(ElementToken elementToken, IScope enclosingScope, out ICodeElement element)
        {
            if ((elementToken.Tokens.Count() == 2 || elementToken.Tokens.Count() == 3) &&
                elementToken.Tokens.First() is AtomicToken atomicToken &&
                atomicToken.Item == "static" &&
                elementToken.Tokens.Last() is AtomicToken nameToken)
            {

                var readOnly = elementToken.Tokens.Count() == 3 &&
                    elementToken.Tokens.ElementAt(1) is AtomicToken readOnlyToken &&
                    readOnlyToken.Item == "readonly";

                element = new MemberDefinition(readOnly,true, new ImplicitTypeReferance(), new ExplicitName(nameToken.Item));

                return true;
            }

            element = default;
            return false;
        }
        
        public static bool MatchObjectDefinition(ElementToken elementToken, IScope enclosingScope, out ICodeElement element) {
            if (elementToken.Tokens.Count() == 3 &&
                elementToken.Tokens.First() is AtomicToken first &&
                    first.Item.StartsWith("object") &&
                elementToken.Tokens.ElementAt(1) is CurleyBacketToken block)
            {
                var scope = new ObjectScope(enclosingScope);

                var elements = TokenParser.ParseBlock(block,scope);
                
                var localDefininitions = elements.OfType<AssignOperation>().Where(x => !(x.right.TakeMemberDef() as MemberDefinition).IsStatic).ToArray();

                if (!elements.All(x => x is AssignOperation assignOperation && !(assignOperation.right.TakeMemberDef() as MemberDefinition).IsStatic)) {
                    throw new Exception("all lines in an object should be none static");
                }

                foreach (var loaclDefinition in localDefininitions)
                {
                    scope.TryAddLocalMember((loaclDefinition.right.TakeMemberDef() as MemberDefinition));
                }
                
                element = new ObjectDefinition(scope, localDefininitions);
                return true;
            }
            element = default;
            return false;
        }

        public static bool MatchModuleDefinition(ElementToken elementToken, IScope enclosingScope, out ICodeElement element) {
            if (elementToken.Tokens.Count() == 3 &&
                elementToken.Tokens.First() is AtomicToken first &&
                    first.Item.StartsWith("module") &&
                elementToken.Tokens.ElementAt(1) is AtomicToken second &&
                elementToken.Tokens.ElementAt(2) is CurleyBacketToken third) {

                var scope = new StaticScope(enclosingScope);

                var elements = TokenParser.ParseBlock(third, scope);

                var staticDefininitions = elements.OfType<AssignOperation>().Where(x => (x.right.TakeMemberDef() as MemberDefinition).IsStatic).ToArray();
                var types = elements.OfType<TypeDefinition>().ToArray();

                if (!elements.All(x=> (x is AssignOperation assignOperation && (assignOperation.right.TakeMemberDef() as MemberDefinition).IsStatic) || x is TypeDefinition)) {
                    throw new Exception($"all elements should be a static {nameof(AssignOperation)} or a {nameof(TypeDefinition)}");
                }

                foreach (var staticDefinition in staticDefininitions)
                {
                    scope.TryAddStaticMember((staticDefinition.right.TakeMemberDef() as MemberDefinition));
                }

                foreach (var type in types)
                {
                    scope.TryAddStaticType(type);
                }
                
                element = new ModuleDefinition(new ExplicitName(second.Item), scope, staticDefininitions);

            }
            element = default;
            return false;
        }

        public static bool MatchMethodDefinition(ElementToken elementToken, IScope enclosingScope, out ICodeElement element)
        {
            if (
                elementToken.Tokens.Count() == 3 && 
                elementToken.Tokens.First() is AtomicToken first &&
                    first.Item == "method" &&
                elementToken.Tokens.ElementAt(1) is ParenthesisToken typeParameters &&
                    typeParameters.Tokens.Count() == 2 &&
                    typeParameters.Tokens.ElementAt(0) is LineToken firstLine &&
                        firstLine.Tokens.Count() ==1 &&
                        firstLine.Tokens.ElementAt(0) is AtomicToken inputType &&
                    typeParameters.Tokens.ElementAt(0) is LineToken secondLine &&
                        secondLine.Tokens.Count() == 1 &&
                        secondLine.Tokens.ElementAt(0) is AtomicToken outputType &&
                elementToken.Tokens.ElementAt(2) is AtomicToken second &&
                elementToken.Tokens.ElementAt(3) is CurleyBacketToken third){
                
                var methodScope = new MethodScope(enclosingScope);

                var lines = TokenParser.ParseBlock(third, methodScope);

                var staticDefininitions = lines.OfType<AssignOperation>().Where(x => (x.right.TakeMemberDef() as MemberDefinition).IsStatic).ToArray();
                var types = lines.OfType<TypeDefinition>().ToArray();

                foreach (var staticDefinition in staticDefininitions)
                {
                    methodScope.TryAddStaticMember((staticDefinition.right.TakeMemberDef() as MemberDefinition));
                }

                foreach (var type in types)
                {
                    methodScope.TryAddStaticType(type);
                }

                element = new MethodDefinition(
                    new TypeReferance(inputType.Item),
                    new ParameterDefinition(
                        false, // TODO, the way this is hard coded is something to think about, readonly should be encoded somewhere!
                        new TypeReferance(inputType.Item),
                        new ExplicitName(second.Item)),
                    lines.Except(staticDefininitions).Except(types).ToArray(),
                    methodScope,
                    staticDefininitions);

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchBlockDefinition(ElementToken elementToken, IScope enclosingScope, out ICodeElement element)
        {
            if (
                elementToken.Tokens.Count() == 1  &&
                elementToken.Tokens.First() is CurleyBacketToken first
                )
            {
                var scope = new LocalStaticScope(enclosingScope);

                var lines = TokenParser.ParseBlock(first, scope);

                var staticDefininitions = lines.OfType<AssignOperation>().Where(x => (x.right.TakeMemberDef() as MemberDefinition).IsStatic).ToArray();
                var types = lines.OfType<TypeDefinition>().ToArray();
                
                foreach (var staticDefinition in staticDefininitions)
                {
                    scope.TryAddStaticMember((staticDefinition.right.TakeMemberDef() as MemberDefinition));
                }

                foreach (var type in types)
                {
                    scope.TryAddStaticType(type);
                }
                
                element = new BlockDefinition(
                    lines.Except(staticDefininitions).Except(types).ToArray(), scope, staticDefininitions);

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchConstantNumber(ElementToken elementToken, IScope enclosingScope, out ICodeElement element)
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

        public static bool MatchReferanceOrMemberDef(ElementToken elementToken, IScope enclosingScope, out ICodeElement element)
        {
            if (
                elementToken.Tokens.Count() == 1 &&
                elementToken.Tokens.First() is AtomicToken first &&
                !double.TryParse(first.Item, out var _)
                )
            {
                element = new ReferanceOrMemberDef(new Referance(first.Item), new MemberDefinition(false,false,new ImplicitTypeReferance(), new ExplicitName(first.Item)));

                return true;
            }

            element = default;
            return false;
        }

    }
}
