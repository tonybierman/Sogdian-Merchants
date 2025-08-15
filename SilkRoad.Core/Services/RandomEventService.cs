using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SilkRoad.Core.Data;
using SilkRoad.Core.Domain;
using SilkRoad.Core.Services.Interfaces;

namespace SilkRoad.Core.Services
{
    public class RandomEventService : IRandomEventService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<RandomEventService> _logger;

        public RandomEventService(GameDbContext context, ILogger<RandomEventService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Route.RandomEvents> GetRandomEventsAsync(long gameInstanceId)
        {
            _logger.LogDebug("Loading random events for GameInstance {GameInstanceId}", gameInstanceId);
            var eventState = await _context.GameStates
                .FirstOrDefaultAsync(gs => gs.GameInstanceId == gameInstanceId && gs.StateKey == "RandomEvents");
            var randomEvents = eventState != null ? JsonSerializer.Deserialize<Route.RandomEvents>(eventState.StateValue) : new Route.RandomEvents();
            if (!randomEvents.Events.Any())
            {
                randomEvents.Events = new Dictionary<string, Route.EventDetail>
                {
                    { "BanditAttack", new Route.EventDetail { Probability = 0.3, Impact = "Lose 50% goods" } },
                    { "Sandstorm", new Route.EventDetail { Probability = 0.2, Impact = "Lose 30% goods" } }
                };
                _logger.LogWarning("No random events found in GameState. Using default events.");
            }
            return randomEvents;
        }
    }
}