using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SilkRoad.Core.Data;
using SilkRoad.Core.Domain;
using SilkRoad.Core.Services.Interfaces;

namespace SilkRoad.Core.Services
{
    public class GameStateSerializer : IGameStateSerializer
    {
        private readonly GameDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GameStateSerializer> _logger;

        public GameStateSerializer(GameDbContext context, IMapper mapper, ILogger<GameStateSerializer> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task SerializeGameStateAsync(long gameInstanceId, string phase)
        {
            try
            {
                var gameInstance = await _context.GameInstances
                    .Include(gi => gi.Entities)
                    .ThenInclude(e => e.EntityAttributes)
                    .Include(gi => gi.EntityRelationships)
                    .Include(gi => gi.GameStates)
                    .Include(gi => gi.User)
                    .FirstOrDefaultAsync(gi => gi.Id == gameInstanceId);

                if (gameInstance == null)
                {
                    _logger.LogError("Game instance {GameInstanceId} not found for serialization.", gameInstanceId);
                    return;
                }

                var gameInstanceDto = _mapper.Map<GameInstanceDto>(gameInstance);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                };
                string json = JsonSerializer.Serialize(gameInstanceDto, options);
                string fileName = $"GameInstance_{gameInstanceId}_{phase}_{DateTime.UtcNow:yyyyMMddHHmmss}.json";
                await File.WriteAllTextAsync(fileName, json);
                _logger.LogInformation("Serialized game state to {FileName}", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to serialize game state for GameInstance {GameInstanceId} at {Phase}", gameInstanceId, phase);
            }
        }
    }
}