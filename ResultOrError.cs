using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace MyData.Logic.Thor
{
    public class ResultOrError<TResult, TError>
    {
        public ResultOrError(TResult result)
        {
            this.Result = result;
            this.IsError = false;
        }

        public ResultOrError(TError error)
        {
            this.Error = error;
            this.IsError = true;
        }

        public TResult Result { get; }
        public TError Error { get; }
        public bool IsError { get; }
        
        public void Match(Action<TResult> onSuccess, Action<TError> onError)
        {
            if (this.IsError)
            {
                onError(this.Error);
            }
            else
            {
                onSuccess(this.Result);
            }
        }

        public T Match<T>(Func<TResult, T> resultFunc, Func<TError, T> errorFunc) => this.IsError
            ? errorFunc(this.Error)
            : resultFunc(this.Result);

#pragma warning disable CA2225 // Operator overloads have named alternates
        public static implicit operator ResultOrError<TResult, TError>(TResult result) => FromResult(result);
        public static implicit operator ResultOrError<TResult, TError>(TError error) => FromError(error);
#pragma warning restore CA2225 // Operator overloads have named alternates

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static ResultOrError<TResult, TError> FromResult(TResult result)
        {
            return new ResultOrError<TResult, TError>(result);
        }

        public static ResultOrError<TResult, TError> FromError(TError error)
        {
            return new ResultOrError<TResult, TError>(error);
        }
#pragma warning restore CA1000 // Do not declare static members on generic types
    }

    public static class ResultOrModelError
    {
        public static ResultOrModelError<TResult> Create<TResult>(TResult result)
        {
            return new ResultOrModelError<TResult>(result);
        }
    }

    public class ResultOrModelError<TResult> : ResultOrError<TResult, IDictionary<string, string>>
    {
        public ResultOrModelError(TResult result)
            : base(result)
        {
        }

        public ResultOrModelError(string memberName, string error)
            : base(CreateErrorDictionary(memberName, error))
        {
        }

        public ResultOrModelError(IDictionary<string, string> error)
            : base(error)
        {
        }

        private static IDictionary<string, string> CreateErrorDictionary(string memberName, string error)
        {
            return new Dictionary<string, string>
            {
                [memberName] = error
            };
        }

        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "To make code cleaner")]
        public static implicit operator ResultOrModelError<TResult>(TResult result) => FromResult(result);

        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "To make code cleaner")]
        public static implicit operator ResultOrModelError<TResult>((string memberName, string error) error) => FromError(CreateErrorDictionary(error.memberName, error.error));

        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Factory methody")]
        public new static ResultOrModelError<TResult> FromResult(TResult result)
        {
            return new ResultOrModelError<TResult>(result);
        }

        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Factory methody")]
        public new static ResultOrModelError<TResult> FromError(IDictionary<string, string> error)
        {
            return new ResultOrModelError<TResult>(error);
        }
    }



    public class ResultOrError<TValue, TError>
    {
        public bool IsResult { get; }
        public TValue Result { get; }
        public TError Error { get; }

        private ResultOrError(TValue result)
        {
            IsResult = true;
            Result = result;
        }

        private ResultOrError(TError error)
        {
            IsResult = false;
            Error = error;
        }

        public static ResultOrError<TValue, TError> CreateResult(TValue result)
        {
            return new ResultOrError<TValue, TError>(result);
        }

        public static ResultOrError<TValue, TError> CreateError(TError error)
        {
            return new ResultOrError<TValue, TError>(error);
        }
    }

}
