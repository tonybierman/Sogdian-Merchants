namespace SilkRoad.Core.Services.Interfaces
{
    public interface IGameEngine
    {
        Task RunGameTurnAsync(long gameInstanceId);
    }
}