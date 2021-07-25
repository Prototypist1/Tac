using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.SemanticModel;
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
        IOrType<IFrontendType<IVerifiableType>, IError> Returns();
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


    internal interface IFrontendType<out TTargetType>: IFrontendCodeElement, IConvertable<TTargetType>
        where TTargetType: IVerifiableType
    {
        


        // we need to pass around a list of assumed true
        // TheyAreUs is often called inside TheyAreUs
        // and the parameters to the inner can be the same as the outer
        // this happens in cases like: A { A x }  TheyAreUs B { B x }
        // this are actaully the same
        // another case A { B x },  B { C x },  C { A x }
        // these are the same as well
        IOrType<bool,IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue);
        /// <summary>
        /// the outer error means we don't know if we have the member
        /// the inner error means we have the memeber but we don't know details about it
        /// </summary>
        IOrType<IOrType<WeakMemberDefinition, IError>, No,IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue);
        /// <summary>
        /// the outer error means we don't know if we return
        /// the inner error means we return something but we don't know what type it is
        /// </summary>
        IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn();
        /// <summary>
        /// the outer error means we don't know if we have input
        /// the inner error means we have input but we don't know what type it is
        /// </summary>
        IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput();
        //IReadOnlyList<WeakMemberDefinition> GetMembers();
    }

    //internal static class IFrontendTypeStatic
    //{

    //    public static TTargetType ConvertTypeOrThrow<TTargetType>(this IFrontendType<TTargetType> self, IConversionContext context)
    //        where TTargetType : IVerifiableType
    //    {
    //        if (self is IFrontendType<TTargetType> convertable)
    //        {
    //            return convertable.Convert(context);
    //        }
    //        throw new Exception("could not be converted");
    //    }
    //}
}
