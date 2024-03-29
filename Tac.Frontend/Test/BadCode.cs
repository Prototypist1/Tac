﻿using Prototypist.Toolbox.Object;
using System;
using Tac.Model;
using Xunit;
using Tac.Model.Operations;
using Tac.SemanticModel;
using Tac.Model.Elements;
using Tac.SemanticModel.Operations;
using Tac.Frontend;
using System.Linq;

namespace Tac.Tests
{

    public class BadCode
    {

        [Fact]
        public void TokenizeMissingElement()
        {
            var res = TestSupport.Tokenize("5 + + 10 =: x ;");
            var converted = TestSupport.ConvertToWeak(res);

            var errors = converted.Validate().ToArray();

            Assert.NotEmpty(errors);

            var line = Assert.Single(converted.Assignments.GetValue());
            line.Is2OrThrow();
        }

        [Fact]
        public void MissingSquareBracket()
        {
            var res = TestSupport.Tokenize(@"  method [ number ; number ;  input { input return ;} =: pass-through ; ");
            var converted = TestSupport.ConvertToWeak(res);

            var errors = converted.Validate().ToArray();

            Assert.NotEmpty(errors);

            var lineOr = Assert.Single(converted.Assignments.GetValue());
            lineOr.Is2OrThrow();
        }

        [Fact]
        public void ImpossibleIs()
        {
            var res = TestSupport.Tokenize(
@"entry-point  [empty, empty] input {
    5 =: i;
    i is type { number x; number y; } t { };
}");
            var converted = TestSupport.ConvertToWeak(res);

            var errors = converted.Validate().ToArray();

            var error = Assert.Single(errors);

            Assert.Equal(ErrorCodes.AssignmentMustBePossible, error.Code);

            //var lineOr = Assert.Single(converted.StaticInitialization);
            //lineOr.Is2OrThrow();
        }



        //[Fact]
        //public void MissingCurleyBracket()
        //{
        //    var res = TestSupport.Tokenize("module test { 5 + 10 =: x ; ");
        //    var converted = TestSupport.ConvertToWeak(res);


        //    var lineOr = Assert.Single(converted.StaticInitialization);
        //    lineOr.Is2OrThrow();
        //}

        [Fact]
        public void YouCantInvokeANumber()
        {
            var res = TestSupport.Tokenize("entry-point [empty, empty] input { 5 =: x ; 5 > x ; }");
            var converted = TestSupport.ConvertToWeak(res);

            var db = converted.Validate().ToArray();

            // there is more to test here !
            Assert.NotEmpty(db);

        }

        [Fact]
        public void UndefinedVariable()
        {
            var res = TestSupport.Tokenize("a + 2 =: x ; ");
            var converted = TestSupport.ConvertToWeak(res);

            var errors = converted.Validate().ToArray();

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void UndefinedType()
        {
            var res = TestSupport.Tokenize(@"method [ chicken ; number ; ] input { 1 return ;} =: chicken-to-one ;");
            var converted = TestSupport.ConvertToWeak(res);

            var errors = converted.Validate().ToArray();

            Assert.NotEmpty(errors);

            var lineOr = Assert.Single(converted.Assignments.GetValue());
            var line = lineOr.Is1OrThrow().GetValue();
            var assign = line.SafeCastTo<IFrontendCodeElement, WeakAssignOperation>();
            var codeElement = assign.Left.Is1OrThrow().GetValue();
            var method = codeElement.SafeCastTo<IFrontendCodeElement, WeakMethodDefinition>();
            var error = method.InputType.GetValue().Is2OrThrow();
            Assert.Equal(ErrorCodes.TypeNotFound, error.Code);
        }
    }
}
