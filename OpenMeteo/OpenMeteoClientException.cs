using System.Net;
using System.Net.Http;

namespace OpenMeteo
{
    public class OpenMeteoClientException(string message, HttpStatusCode httpStatusCode) : HttpRequestException(message, null, httpStatusCode)
    {
    }
}
