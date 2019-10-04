namespace Tac.New
{
    public class Result<T> : IResult<T>
    {
        public Result(bool hasResult, T value)
        {
            HasValue = hasResult;
            Value = value;
        }

        public bool HasValue { get;}
        public T Value {get;}

    }

    public static class ResultExtension
    {
        public static bool TryGetValue<T>(this IResult<T> self, out T res)
        {
            if (self.HasValue)
            {
                res = self.Value;
                return true;

            }
            res = default;
            return false;
        }

        public static Result<T> Good<T>(T value)
        {
            return new Result<T>(true, value);
        }

        public static Result<T> Bad<T>()
        {
            return new Result<T>(false, default);
        }
    }

    public interface IResult<out T>
    {
        bool HasValue { get; }
        T Value { get; }
    }


}
