
namespace SilkRoad.Core.Services.Interfaces;
public interface IGameService
{
    Task CreateCharacter(long userId, string name, string gameType = "rpg");
}