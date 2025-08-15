using Microsoft.EntityFrameworkCore;
using SilkRoad.Core.Data;
using SilkRoad.Core.Services.Interfaces;

namespace SilkRoad.Core.Services;
public class GameService : IGameService
{
    private readonly GameDbContext _context;

    public GameService(GameDbContext context)
    {
        _context = context;
    }

    public async Task CreateCharacter(long userId, string name, string gameType = "rpg")
    {
        var gameInstance = await _context.GameInstances
            .FirstOrDefaultAsync(gi => gi.UserId == userId && gi.GameType == gameType);

        if (gameInstance == null)
        {
            gameInstance = new GameInstance
            {
                UserId = userId,
                GameType = gameType,
                IsActive = true
            };
            _context.GameInstances.Add(gameInstance);
            await _context.SaveChangesAsync();
        }

        var entity = new Entity
        {
            GameInstanceId = gameInstance.Id,
            EntityType = "character",
            Name = name
        };
        _context.Entities.Add(entity);
        await _context.SaveChangesAsync();

        var stats = new EntityAttribute
        {
            EntityId = entity.Id,
            AttributeKey = "stats",
            AttributeValue = "{\"hp\": 100, \"strength\": 10}"
        };
        _context.EntityAttributes.Add(stats);
        await _context.SaveChangesAsync();
    }
}