namespace FraudStream.Application.Common
{
    /// <summary>
    /// Padrão Result para evitar uso de exceções como controle de fluxo.
    /// Handlers retornam Result em vez de lançar exceções para casos esperados.
    /// </summary>
    public sealed class Result<T>
    {
        public T? Value { get; }
        public string? Error { get; }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        private Result(T value)
        {
            Value = value;
            IsSuccess = true;
        }

        private Result(string error)
        {
            Error = error;
            IsSuccess = false;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(string error) => new(error);

        public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<string, TOut> onFailure)
            => IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    }

    public sealed class Result
    {
        public string? Error { get; }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        private Result(bool success, string? error = null)
        {
            IsSuccess = success;
            Error = error;
        }

        public static Result Success() => new(true);
        public static Result Failure(string error) => new(false, error);
    }
}
