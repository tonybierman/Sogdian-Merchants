using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SilkRoad.Core.Data;
using SilkRoad.Core.Services;
using SilkRoad.Core.Services.Interfaces;
using System;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Build configuration
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Set up DI
        var services = new ServiceCollection();
        services.AddDbContext<GameDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("GameDbContext"),
                new MySqlServerVersion(new Version(8, 0, 21)),
                mysqlOptions => mysqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            ));
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<ISeedData, SilkRoadDataSeeder>();
        services.AddScoped<IGameEngine, GameEngine>();
        services.AddScoped<ICaravanProcessor, CaravanProcessor>();
        services.AddScoped<IMarketService, MarketService>();
        services.AddScoped<IRandomEventService, RandomEventService>();
        services.AddScoped<IGameStateSerializer, GameStateSerializer>();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
        });

        services.AddAutoMapper(typeof(SilkRoad.Core.Mapping.AutoMapperProfile));

        try
        {
            // Build service provider
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
            var seeder = scope.ServiceProvider.GetRequiredService<ISeedData>();
            var engine = scope.ServiceProvider.GetRequiredService<IGameEngine>();
            var random = new Random();

            // Reset database for fresh game
            logger.LogInformation("Resetting database for fresh game...");
            await EnsureDatabaseMigratedAsync(context, logger);

            // Seed initial data
            logger.LogInformation("Starting data seeding...");
            await seeder.SeedDataAsync();
            logger.LogInformation("Data seeding completed successfully!");

            // Get the GameInstance ID for the silkroad game
            logger.LogInformation("Retrieving active silkroad game instance...");
            var gameInstance = await context.GameInstances
                .FirstOrDefaultAsync(gi => gi.GameType == "silkroad" && gi.IsActive.Value);

            if (gameInstance == null)
            {
                logger.LogWarning("No active silkroad game instance found.");
                return;
            }

            // Main game loop
            while (true)
            {
                logger.LogInformation("Running game turn for GameInstance {GameInstanceId}...", gameInstance.Id);
                await engine.RunGameTurnAsync(gameInstance.Id);
                logger.LogInformation("Game turn for GameInstance {GameInstanceId} completed successfully!", gameInstance.Id);

                // Check for InTransit caravans
                var inTransitCount = await context.Entities
                    .Where(e => e.GameInstanceId == gameInstance.Id && e.EntityType == "Caravan")
                    .Join(context.EntityAttributes,
                        e => e.Id,
                        ea => ea.EntityId,
                        (e, ea) => new { e, ea })
                    .Where(x => x.ea.AttributeKey == "Status" && x.ea.AttributeValue.Contains("InTransit"))
                    .CountAsync();

                if (inTransitCount == 0)
                {
                    logger.LogInformation("No caravans in transit. Creating 1-2 new caravans...");
                    int newCaravanCount = random.Next(1, 3); // Randomly 1 or 2 caravans
                    for (int i = 0; i < newCaravanCount; i++)
                    {
                        string caravanName = $"SG-{DateTime.UtcNow.Ticks % 10000 + i}";
                        var caravan = new Entity
                        {
                            GameInstanceId = gameInstance.Id,
                            Name = caravanName,
                            EntityType = "Caravan",
                            CreatedAt = DateTime.UtcNow
                        };
                        context.Entities.Add(caravan);
                        await context.SaveChangesAsync();

                        string goodsType = i % 2 == 0 ? "Silk" : "Spices";
                        string marketName = i % 2 == 0 ? "ChangAnMarket" : "DamascusMarket";
                        var attributes = new[]
                        {
                            new EntityAttribute
                            {
                                EntityId = caravan.Id,
                                AttributeKey = "Goods",
                                AttributeValue = JsonSerializer.Serialize(new { Type = goodsType, Quantity = i % 2 == 0 ? 50 : 20 }),
                                UpdatedAt = DateTime.UtcNow
                            },
                            new EntityAttribute
                            {
                                EntityId = caravan.Id,
                                AttributeKey = "Route",
                                AttributeValue = JsonSerializer.Serialize(new { Start = i % 2 == 0 ? "Samarkand" : "Bukhara", End = i % 2 == 0 ? "ChangAn" : "Damascus", RiskLevel = i % 2 == 0 ? "High" : "Medium" }),
                                UpdatedAt = DateTime.UtcNow
                            },
                            new EntityAttribute
                            {
                                EntityId = caravan.Id,
                                AttributeKey = "Investment",
                                AttributeValue = JsonSerializer.Serialize(new { Value = i % 2 == 0 ? 200.0 : 150.0 }),
                                UpdatedAt = DateTime.UtcNow
                            },
                            new EntityAttribute
                            {
                                EntityId = caravan.Id,
                                AttributeKey = "Status",
                                AttributeValue = JsonSerializer.Serialize(new { Value = "InTransit" }),
                                UpdatedAt = DateTime.UtcNow
                            }
                        };
                        context.EntityAttributes.AddRange(attributes);

                        // Add relationships
                        var market = await context.Entities
                            .FirstOrDefaultAsync(e => e.GameInstanceId == gameInstance.Id && e.EntityType == "Market" && e.Name == marketName);
                        if (market == null)
                        {
                            market = new Entity
                            {
                                GameInstanceId = gameInstance.Id,
                                Name = marketName,
                                EntityType = "Market",
                                CreatedAt = DateTime.UtcNow
                            };
                            context.Entities.Add(market);
                            await context.SaveChangesAsync();
                            context.EntityAttributes.Add(new EntityAttribute
                            {
                                EntityId = market.Id,
                                AttributeKey = "Demand",
                                AttributeValue = JsonSerializer.Serialize(new
                                {
                                    Silk = new { price = marketName == "ChangAnMarket" ? 12.0 : 30.0, maxQuantity = marketName == "ChangAnMarket" ? 100 : 80 },
                                    Spices = new { price = marketName == "ChangAnMarket" ? 8.0 : 15.0, maxQuantity = marketName == "ChangAnMarket" ? 50 : 40 }
                                }),
                                UpdatedAt = DateTime.UtcNow
                            });
                            await context.SaveChangesAsync();
                            logger.LogInformation("Created new market {MarketName} with demand data.", marketName);
                        }

                        context.EntityRelationships.Add(new EntityRelationship
                        {
                            GameInstanceId = gameInstance.Id,
                            SourceEntityId = caravan.Id,
                            TargetEntityId = market.Id,
                            RelationshipType = "Trade",
                            CreatedAt = DateTime.UtcNow
                        });

                        var tribe = await context.Entities
                            .FirstOrDefaultAsync(e => e.GameInstanceId == gameInstance.Id && e.EntityType == "Tribe" && e.Name == "TurkicTribe");
                        if (tribe != null)
                        {
                            context.EntityRelationships.Add(new EntityRelationship
                            {
                                GameInstanceId = gameInstance.Id,
                                SourceEntityId = caravan.Id,
                                TargetEntityId = tribe.Id,
                                RelationshipType = "TollNegotiation",
                                CreatedAt = DateTime.UtcNow
                            });
                        }

                        await context.SaveChangesAsync();
                        logger.LogInformation("Created new caravan {CaravanName} with goods {GoodsType} to {MarketName}.",
                            caravanName, goodsType, marketName);
                    }
                }

                Console.WriteLine("Press Enter to run the next turn, or Ctrl+C to exit...");
                if (Console.ReadKey(true).Key != ConsoleKey.Enter)
                    break;
            }
        }
        catch (Exception ex)
        {
            // Create a new service provider for logging in the catch block
            using var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger?.LogError(ex, "An error occurred during execution: {ErrorMessage}", ex.Message);
            Console.ReadLine();
        }
    }

    private static async Task EnsureDatabaseMigratedAsync(GameDbContext context, ILogger logger)
    {
        try
        {

            context.Database.ExecuteSqlRaw(@"DROP DATABASE IF EXISTS silkroad;
CREATE DATABASE silkroad;");
            await context.SaveChangesAsync();

            bool tableExists = context.Database.ExecuteSqlRaw(@"
                    SELECT COUNT(*) FROM information_schema.tables 
                    WHERE table_schema = DATABASE() 
                    AND table_name = 'User';") > 0;

            if (!tableExists)
            {
                RelationalDatabaseCreator databaseCreator =
                    (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
                await databaseCreator.CreateTablesAsync();
                await context.SaveChangesAsync();
            }

            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync(); // Applies migrations to create the database schema
            await context.SaveChangesAsync();
            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply database migrations.");
            throw; // Re-throw to let the caller handle the failure
        }
    }
}