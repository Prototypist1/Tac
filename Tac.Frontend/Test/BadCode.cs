using Prototypist.Toolbox.Object;
using System;
using Tac.Model;
using Xunit;
using Tac.Model.Operations;
using Tac.SemanticModel;
using Tac.Model.Elements;

namespace Tac.Tests
{
    public class BadCode {

        [Fact]
        public void TokenizeMissingElement()
        {
            var res = TestSupport.Tokenize("module tokenize-missing-element { 5 + + 10 =: x ; }");
            var converted = TestSupport.Convert(res);
            var line = Assert.Single(converted.StaticInitialization);
            var validLine = line.Is1OrThrow();
            var assignOperation = validLine.SafeCastTo<ICodeElement, IAssignOperation>();
            var addition = assignOperation.Left.Is1OrThrow();
            var addOperation = addition.SafeCastTo<ICodeElement, IAddOperation>();
            addOperation.Left.Is2OrThrow();
        }

        [Fact]
        public void MissingCurleyBracket()
        {
            var res = TestSupport.Tokenize("module test { 5 + 10 =: x ; ");
            var converted = TestSupport.Convert(res);

            throw new NotImplementedException();
        }

        [Fact]
        public void MissingSquareBracket()
        {
            var res = TestSupport.Tokenize(@" module tet { method [ number ; number ;  input { input return ;} =: pass-through ; }");
            var converted = TestSupport.Convert(res);

            throw new NotImplementedException();
        }

        [Fact]
        public void YouCantInvokeANumber()
        {
            var res = TestSupport.Tokenize("module test { 5 =: x ; 5 > x ; }");
            var converted = TestSupport.Convert(res);

            throw new NotImplementedException();
        }

        // I think this actaully will not error out here
        // a is used before it is assigned 
        // but that is flow analysis 
        [Fact]
        public void UndefinedVariable()
        {
            var res = TestSupport.Tokenize("module test { a + 2 =: x ; }");
            var converted = TestSupport.Convert(res);

            throw new NotImplementedException();
        }

        [Fact]
        public void UndefinedType()
        {
            var res = TestSupport.Tokenize(@" module test { method [ chicken ; number ; ] input { 1 return ;} =: chicken-to-one ; }");
            var converted = TestSupport.Convert(res);

            var lineOr = Assert.Single(converted.StaticInitialization);
            var line = lineOr.Is1OrThrow();
            var assign = line.SafeCastTo<ICodeElement, IAssignOperation>();
            var codeElement = assign.Left.Is1OrThrow();
            var method= codeElement.SafeCastTo<ICodeElement, IInternalMethodDefinition>();
            var error = method.InputType.Is2OrThrow();
            Assert.Equal(ErrorCodes.TypeNotFound, error.Code);
        }
    }
}
