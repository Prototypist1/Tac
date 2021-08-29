namespace Tac.Model
{
    public interface IKey {
        // this was added as part of support for generic-method
        // generic-methods are of the shape generic-method [T] [T,T]
        // but generic-method [T] [T,T] is the same as generic-method [T1] [T1,T1]
        // so when we construct one we transform it to standard parameters
        //IKey Replace((IKey, IKey)[] replacements);
    }
}
