namespace Inventory.Application.Common.Interfaces
{
    public interface ICorrelationIdProvider
    {
        string CorrelationId { get; }
        string FormattedCorrelationId { get; }
    }
}
