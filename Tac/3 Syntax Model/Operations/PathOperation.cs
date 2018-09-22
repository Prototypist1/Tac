﻿using Prototypist.LeftToRight;
using System;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{
    public class PathOperation : BinaryOperation<ICodeElement, MemberDefinition>
    {
        public PathOperation(ICodeElement left, MemberDefinition right) : base(left, right)
        {
        }

        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            if (!left.Cast<IScoped>().Scope.TryGetMember(right.Key.Key,false,out _)){
                throw new Exception("Member should be defined");
            }
            
            return right.ReturnType(new ScopeStack(scope.ScopeTree, left.Cast<IScoped>().Scope));
        }
    }


    public class PathOperationMaker : BinaryOperationMaker<PathOperation>
    {
        public PathOperationMaker(Func<ICodeElement, ICodeElement, PathOperation> make, IElementBuilders elementBuilders) : base(".", make, elementBuilders)
        {
        }
    }
}
