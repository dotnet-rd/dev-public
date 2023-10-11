
using System;
using System.Net;

namespace Kmd.Logic.Cpr.Api.Handlers
{
    public class CprProviderError
    {
        public CprProviderErrorReason Reason { get; }
        public string Message { get; }

        public CprProviderError(CprProviderErrorReason reason, string message)
        {
            Reason = reason;
            Message = message;
        }

        public static CprProviderError AsExternalServiceFailure(string source, string reason)
        {
            return new CprProviderError(CprProviderErrorReason.ExternalServiceFailure, $"[{source}] Request failed: {reason}.");
        }

        public static CprProviderError AsExternalServiceFailure(string source, HttpStatusCode statusCode, string reason)
        {
            return new CprProviderError(CprProviderErrorReason.ExternalServiceFailure, $"[{source}] Failed with status code {statusCode}: {reason}.");
        }

        public static CprProviderError AsActionNotSupported(string source, string reason)
        {
            return new CprProviderError(CprProviderErrorReason.ActionNotSupported, $"[{source}] {reason}.");
        }

        public static CprProviderError AsCitizenNotFound(string source)
        {
            return new CprProviderError(CprProviderErrorReason.CitizenNotFound, $"[{source}] Citizen not found.");
        }

        public static CprProviderError AsBadProviderRequest(string source, HttpStatusCode statusCode, string reason)
        {
            return new CprProviderError(CprProviderErrorReason.BadProviderRequest, $"[{source}] Bad request ({statusCode}): {reason}.");
        }
    }
}