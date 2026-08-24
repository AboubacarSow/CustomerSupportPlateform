using Polly;
using Polly.Retry;

namespace CustomerSupportPlateform.Infrastructure.Policies; 


public class OllamaPolicy 
{

    public static AsyncRetryPolicy<HttpResponseMessage> ImmediatRetry =>
        Policy.HandleResult<HttpResponseMessage>(res =>!res.IsSuccessStatusCode)
              .RetryAsync(7);
}