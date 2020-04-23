﻿using Prototypist.Toolbox.Object;
using System;
using Tac.Model;
using Xunit;
using Tac.Model.Operations;
using Tac.SemanticModel;
using Tac.Model.Elements;
using Tac.SemanticModel.Operations;
using Tac.Frontend;

namespace Tac.Tests
{
    public class BadCode
    {

        [Fact]
        public void TokenizeMissingElement()
        {
            var res = TestSupport.Tokenize("module tokenize-missing-element { 5 + + 10 =: x ; }");
            var converted = TestSupport.ConvertToWeak(res);
            var line = Assert.Single(converted.StaticInitialization);
            var validLine = line.Is1OrThrow();
            var assignOperation = validLine.GetValue().SafeCastTo<IFrontendCodeElement, WeakAssignOperation>();
            var addOperation = assignOperation.Left.Is1OrThrow().GetValue().SafeCastTo<IFrontendCodeElement, WeakAddOperation>();
            addOperation.Left.Is2OrThrow();
        }

        [Fact]
        public void MissingSquareBracket()
        {
            var res = TestSupport.Tokenize(@" module tet { method [ number ; number ;  input { input return ;} =: pass-through ; }");
            var converted = TestSupport.ConvertToWeak(res);

            var lineOr = Assert.Single(converted.StaticInitialization);
            lineOr.Is2OrThrow();
        }

        //[Fact]
        //public void MissingCurleyBracket()
        //{
        //    var res = TestSupport.Tokenize("module test { 5 + 10 =: x ; ");
        //    var converted = TestSupport.ConvertToWeak(res);


        //    var lineOr = Assert.Single(converted.StaticInitialization);
        //    lineOr.Is2OrThrow();
        //}

        //[Fact]
        //public void YouCantInvokeANumber()
        //{
        //    var res = TestSupport.Tokenize("module test { 5 =: x ; 5 > x ; }");
        //    var converted = TestSupport.ConvertToWeak(res);

        //    throw new NotImplementedException();
        //}

        // I think this actaully will not error out here
        // a is used before it is assigned 
        // but that is flow analysis 
        //[Fact]
        //public void UndefinedVariable()
        //{
        //    var res = TestSupport.Tokenize("module test { a + 2 =: x ; }");
        //    var converted = TestSupport.Convert(res);

        //    throw new NotImplementedException();
        //}

        [Fact]
        public void UndefinedType()
        {
            var res = TestSupport.Tokenize(@" module test { method [ chicken ; number ; ] input { 1 return ;} =: chicken-to-one ; }");
            var converted = TestSupport.ConvertToWeak(res);

            var lineOr = Assert.Single(converted.StaticInitialization);
            var line = lineOr.Is1OrThrow().GetValue();
            var assign = line.SafeCastTo<IFrontendCodeElement, WeakAssignOperation>();
            var codeElement = assign.Left.Is1OrThrow().GetValue();
            var method = codeElement.SafeCastTo<IFrontendCodeElement, WeakMethodDefinition>();
            var error = method.InputType.Is2OrThrow();
            Assert.Equal(ErrorCodes.TypeNotFound, error.Code);
        }
    }
}