using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Type;

namespace Tac.Frontend
{

    internal interface IValidate {
        // we can do better, but what does it get us?
        // let's wait and see
        IEnumerable<IError> Validate();
    }


    // ref
    internal interface IReturn {
        IOrType<IFrontendType,IError> Returns();
    }

    // not every frontend code element is convertable. type definitions are not.
    internal interface IFrontendCodeElement: IValidate
    {

    }

    internal static class IFrontendCodeElementStatic{

        public static ICodeElement ConvertElementOrThrow(this IFrontendCodeElement self, IConversionContext context) {
            if (self is IConvertableFrontendCodeElement<ICodeElement> convertable) {
                return convertable.Convert(context);
            }
            throw new Exception("could not be converted");
        }


        public static IIsPossibly<ICodeElement> PossiblyConvert(this IFrontendCodeElement self, IConversionContext context)
        {
            if (self is IConvertableFrontendCodeElement<ICodeElement> convertable)
            {
                return Possibly.Is(convertable.Convert(context));
            }
            return Possibly.IsNot<ICodeElement>();
        }
    }

    internal interface IConvertableFrontendCodeElement<out T>: IFrontendCodeElement, IConvertable<T> 
        where T: ICodeElement
    {
    }


    internal interface IFrontendType: IValidate
    {
        // we need to pass around a list of assumed true
        // TheyAreUs is often called inside TheyAreUs
        // and the parameters to the inner can be the same as the outer
        // this happens in cases like: A { A x }  TheyAreUs B { B x }
        // this are actaully the same
        // another case A { B x },  B { C x },  C { A x }
        // these are the same as well
        IOrType<bool,IError> TheyAreUs(IFrontendType they, List<(IFrontendType, IFrontendType)> assumeTrue);
        IOrType<IOrType<IFrontendType, IError>, No,IError> TryGetMember(IKey key);
        IOrType<IOrType<IFrontendType, IError>, No, IError> TryGetReturn();
        IOrType<IOrType<IFrontendType, IError>, No, IError> TryGetInput();
    }


    internal static class IFrontendTypeStatic
    {

        public static IVerifiableType ConvertTypeOrThrow(this IFrontendType self, IConversionContext context)
        {
            if (self is IConvertableFrontendType<IVerifiableType> convertable)
            {
                return convertable.Convert(context);
            }
            throw new Exception("could not be converted");
        }
    }

    // TODO, some of these transform to specific types!
    internal interface IConvertableFrontendType<out T>: IFrontendType, IConvertable<T>
        where T: IVerifiableType
    {
    }
}
