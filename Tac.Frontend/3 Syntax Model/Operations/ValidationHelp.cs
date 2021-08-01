using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Model;
using Prototypist.Toolbox;
using System.Linq;
using System.Collections.Generic;
using Tac.Model.Elements;
using Prototypist.Toolbox.Object;

namespace Tac.SemanticModel.Operations
{
    internal static class ValidationHelp
    {

        public static IEnumerable<IError> TypeCheck(this IOrType<IBox<IFrontendCodeElement>, IError> self, IFrontendType<IVerifiableType> targetType)
        {
            return ReturnsTypeOrErrors(self).SwitchReturns<IEnumerable<IError>>(
            x =>
            {
                if (!x.TheyAreUs(targetType, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>()).SwitchReturns(x => x, x => false))
                {
                    return new[] { Error.Other($"left cannot be {x}") };
                }
                return System.Array.Empty<IError>();
            },
            x => new[] { x });
        }

        public static IOrType<IFrontendType<IVerifiableType>, IError> ReturnsTypeOrErrors(this IOrType<IBox<IFrontendCodeElement>, IError> self)
        {
            if (self.Is2(out var v2))
            {
                return OrType.Make<IFrontendType<IVerifiableType>, IError>(v2);
            }

            var intermittentLeft = self.Is1OrThrow().GetValue();

            if (intermittentLeft is not IReturn returns)
            {
                return OrType.Make<IFrontendType<IVerifiableType>, IError>(Error.Other($"{intermittentLeft} should return"));
            }

            return returns.Returns().TransformAndFlatten(x => x.UnwrapRefrence());
        }
    }
}
