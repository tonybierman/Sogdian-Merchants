namespace SilkRoad.Core.Services.Interfaces
{
    public interface IGameStateSerializer
    {
        Task SerializeGameStateAsync(long gameInstanceId, string phase);
    }
}