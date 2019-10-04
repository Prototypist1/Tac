using Tac.Parser;

namespace Tac.New
{
    internal interface IMaker
    {
        ITokenMatching TryMake(ITokenMatching elementToken);
    }


    internal interface IMaker<out TCodeElement>
    {
        ITokenMatching<TCodeElement> TryMake(IMatchedTokenMatching elementToken);
    }
}
