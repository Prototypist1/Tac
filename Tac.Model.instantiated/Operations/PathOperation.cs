﻿using System;
using System.Collections.Generic;
using Prototypist.Toolbox;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class PathOperation : IPathOperation, IBinaryOperationBuilder
    {
        private readonly Buildable<OrType<ICodeElement, IError>> buildableLeft = new Buildable<OrType<ICodeElement, IError>>();
        private readonly Buildable<OrType<ICodeElement, IError>> buildableRight = new Buildable<OrType<ICodeElement, IError>>();

        public void Build(OrType<ICodeElement, IError> left, OrType<ICodeElement, IError> right)
        {
            buildableLeft.Set(left);
            buildableRight.Set(right);
        }

        public OrType<ICodeElement, IError> Left => buildableLeft.Get();
        public OrType<ICodeElement, IError> Right => buildableRight.Get();
        public IReadOnlyList<OrType<ICodeElement, IError>> Operands => new[] { Left, Right };

        private PathOperation() { }

        public static (IPathOperation, IBinaryOperationBuilder) Create()
        {
            var res = new PathOperation();
            return (res, res);
        }

        // this two methods Convert and Returns are interesting
        // they could almost be implemented as extensions
        // I mean they are going to look the same in every set of implemenation of the ICodeElements
        // but... I want to ensure they are there so I include them on the interface
        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.PathOperation(this);
        }
        
        public OrType<IVerifiableType, IError> Returns()
        {
            return Right.Convert(x => x.Returns()).Flatten();
        }
        
        public static IPathOperation CreateAndBuild(OrType<ICodeElement, IError> left, OrType<ICodeElement, IError> right)
        {
            var (x, y) = Create();
            y.Build(left, right);
            return x;
        }
    }
}
