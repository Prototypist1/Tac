namespace Tac.Frontend.New.CrzayNamespace
{
    public interface IConvertTo<out TConvertsTo>
    {
        TConvertsTo Convert();
    }
}