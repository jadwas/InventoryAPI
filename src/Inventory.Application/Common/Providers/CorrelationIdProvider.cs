using Inventory.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Inventory.Application.Common.Providers
{
    public class CorrelationIdProvider : ICorrelationIdProvider
    {
        public string CorrelationId { get; }

        public string FormattedCorrelationId => $"[CID:{CorrelationId}]";

        public CorrelationIdProvider(IHttpContextAccessor accessor)
        {
            const string header = "X-Correlation-ID";

            var context = accessor.HttpContext;

            if (context == null)
            {
                CorrelationId = Guid.NewGuid().ToString();
                return;
            }

            if (context.Request.Headers.TryGetValue(header, out var cid) &&
                !string.IsNullOrWhiteSpace(cid))
            {
                CorrelationId = cid!;
            }
            else
            {
                CorrelationId = Guid.NewGuid().ToString();
                context.Response.Headers[header] = CorrelationId;
                context.Items[header] = CorrelationId;
            }
        }
    }
}
