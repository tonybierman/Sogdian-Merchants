using SilkRoad.Core.Domain;

namespace SilkRoad.Core.Services.Interfaces
{
    public interface IRandomEventService
    {
        Task<Route.RandomEvents> GetRandomEventsAsync(long gameInstanceId);
    }
}