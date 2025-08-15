using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SilkRoad.Core.Data;
using SilkRoad.Core.Services.Interfaces;

namespace SilkRoad.Core.Services
{
    public class GameEngine : IGameEngine
    {
        private readonly GameDbContext _context;
        private readonly ICaravanProcessor _caravanProcessor;
        private readonly IMarketService _marketService;
        private readonly IRandomEventService _randomEventService;
        private readonly IGameStateSerializer _gameStateSerializer;
        private readonly ILogger<GameEngine> _logger;

        public GameEngine(
            GameDbContext context,
            ICaravanProcessor caravanProcessor,
            IMarketService marketService,
            IRandomEventService randomEventService,
            IGameStateSerializer gameStateSerializer,
            ILogger<GameEngine> logger)
        {
            _context = context;
            _caravanProcessor = caravanProcessor;
            _marketService = marketService;
            _randomEventService = randomEventService;
            _gameStateSerializer = gameStateSerializer;
            _logger = logger;
        }

        public async Task RunGameTurnAsync(long gameInstanceId)
        {
            _logger.LogInformation("Starting game turn for GameInstance {GameInstanceId}", gameInstanceId);

            await _gameStateSerializer.SerializeGameStateAsync(gameInstanceId, "start");

            var gameInstance = await _context.GameInstances
                .Include(gi => gi.Entities)
                .ThenInclude(e => e.EntityAttributes)
                .Include(gi => gi.EntityRelationships)
                .Include(gi => gi.GameStates)
                .FirstOrDefaultAsync(gi => gi.Id == gameInstanceId && gi.GameType == "silkroad");

            if (gameInstance == null)
            {
                _logger.LogError("Game instance {GameInstanceId} not found.", gameInstanceId);
                throw new InvalidOperationException("Game instance not found.");
            }

            var marketPrices = await _marketService.GetMarketPricesAsync(gameInstanceId);
            var randomEvents = await _randomEventService.GetRandomEventsAsync(gameInstanceId);

            var caravans = gameInstance.Entities
                .Where(e => e.EntityType == "Caravan" && e.EntityAttributes.Any(a => a.AttributeKey == "Status" && a.AttributeValue.Contains("InTransit")))
                .ToList();

            _logger.LogInformation("Found {CaravanCount} caravans in transit for GameInstance {GameInstanceId}", caravans.Count, gameInstanceId);

            foreach (var caravan in caravans)
            {
                await _caravanProcessor.ProcessCaravanAsync(caravan, gameInstance, marketPrices, randomEvents);
            }

            await _marketService.UpdateMarketPricesAsync(gameInstance, marketPrices);
            await _context.SaveChangesAsync();
            await _gameStateSerializer.SerializeGameStateAsync(gameInstanceId, "end");

            _logger.LogInformation("Game turn for GameInstance {GameInstanceId} completed successfully", gameInstanceId);
        }
    }
}