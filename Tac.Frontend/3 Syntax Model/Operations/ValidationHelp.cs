using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Model;
using Prototypist.Toolbox;
using System.Linq;
using System.Collections.Generic;


namespace Tac.SemanticModel.Operations
{
    internal static class ValidationHelp {

        public static IEnumerable<IError> TypeCheck(this IOrType<IBox<IFrontendCodeElement>, IError> self, IFrontendType targetType) {
            return GetTypeOrErrors(self).SwitchReturns<IEnumerable<IError>>(
            x => {
                if (!x.IsAssignableTo(targetType))
                {
                    return new[] { Error.Other($"left cannot be {x}") };
                }
                return System.Array.Empty<IError>();
            },
            x => new[] { x });
        }

        public static IOrType<IFrontendType, IError> GetTypeOrErrors(this IOrType<IBox<IFrontendCodeElement>, IError> self)
        {
            var intermittentLeft = self.Possibly1().AsEnummerable().Select(x => x.GetValue()).ToArray();

            foreach (var thing in intermittentLeft)
            {
                if (!(thing is IReturn))
                {
                    return OrType.Make<IFrontendType, IError>(Error.Other($"{thing} should return"));
                }
            }

            var leftList = intermittentLeft
                .OfType<IReturn>()
                .Select(x => x.Returns().Possibly1())
                .OfType<IIsDefinately<IFrontendType>>()
                .Select(x => x.Value.UnwrapRefrence())
                .ToArray();

            if (leftList.Any())
            {
                return OrType.Make<IFrontendType, IError>(leftList.First());
            }

            throw new System.Exception("there really should be something in that list...");
        }
    }
}
