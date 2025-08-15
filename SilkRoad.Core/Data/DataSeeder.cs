using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkRoad.Core.Data
{
    using Microsoft.EntityFrameworkCore;
    using SilkRoad.Core.Data;
    using SilkRoad.Core.Services.Interfaces;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ISeedData
    {
        Task SeedDataAsync();
    }

    public class DataSeeder : ISeedData
    {
        protected readonly GameDbContext _context;
        protected readonly IGameService _gameService;

        public DataSeeder(GameDbContext context, IGameService gameService)
        {
            _context = context;
            _gameService = gameService;
        }

        public virtual async Task SeedDataAsync()
        {
            // Check and seed User
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == "player1");
            if (user == null)
            {
                user = new User
                {
                    Username = "player1",
                    Email = "player1@example.com",
                    PasswordHash = "hashed_password"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Check and seed GameInstance
            var gameInstance = await _context.GameInstances
                .FirstOrDefaultAsync(gi => gi.UserId == user.Id && gi.GameType == "rpg");
            if (gameInstance == null)
            {
                gameInstance = new GameInstance
                {
                    UserId = user.Id,
                    GameType = "rpg",
                    IsActive = true
                };
                _context.GameInstances.Add(gameInstance);
                await _context.SaveChangesAsync();
            }

            // Check and seed Entity (character)
            var entity = await _context.Entities
                .FirstOrDefaultAsync(e => e.GameInstanceId == gameInstance.Id && e.Name == "Hero");
            if (entity == null)
            {
                await _gameService.CreateCharacter(user.Id, "Hero");
            }
        }
    }
}
