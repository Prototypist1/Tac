using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Model;
using Prototypist.Toolbox;
using System.Linq;
using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.SemanticModel.Operations
{
    internal static class ValidationHelp {

        public static IEnumerable<IError> TypeCheck(this IOrType<IBox<IFrontendCodeElement>, IError> self, IFrontendType<IVerifiableType> targetType) {
            return ReturnsTypeOrErrors(self).SwitchReturns<IEnumerable<IError>>(
            x => {
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
            if (self.Is2(out var v2)) {
                return OrType.Make<IFrontendType<IVerifiableType>, IError>(v2);
            }

            var intermittentLeft = self.Possibly1().AsEnumerable().Select(x => x.GetValue()).ToArray();

            foreach (var thing in intermittentLeft)
            {
                if (!(thing is IReturn))
                {
                    return OrType.Make<IFrontendType<IVerifiableType>, IError>(Error.Other($"{thing} should return"));
                }
            }

            var leftList = intermittentLeft
                .OfType<IReturn>()
                .Select(x => x.Returns().Possibly1())
                .OfType<IIsDefinately<IFrontendType<IVerifiableType>>>()
                .Select(x => x.Value.UnwrapRefrence())
                .OfType<IIsDefinately<IFrontendType<IVerifiableType>>>()
                .Select(x=>x.Value)
                .ToArray();

            if (leftList.Any())
            {
                return OrType.Make<IFrontendType<IVerifiableType>, IError>(leftList.First());
            }

            throw new System.Exception("there really should be something in that list...");
        }
    }
}
