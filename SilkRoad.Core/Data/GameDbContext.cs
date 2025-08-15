using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SilkRoad.Core.Data;

public partial class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Entity> Entities { get; set; }

    public virtual DbSet<EntityAttribute> EntityAttributes { get; set; }

    public virtual DbSet<EntityRelationship> EntityRelationships { get; set; }

    public virtual DbSet<GameInstance> GameInstances { get; set; }

    public virtual DbSet<GameState> GameStates { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_uca1400_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Entity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Entity");

            entity.HasIndex(e => e.GameInstanceId, "idx_game_instance_id");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.GameInstanceId).HasColumnType("bigint(20)");
            entity.Property(e => e.LastUpdated)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("last_updated");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.GameInstance).WithMany(p => p.Entities)
                .HasForeignKey(d => d.GameInstanceId)
                .HasConstraintName("Entity_ibfk_1");
        });

        modelBuilder.Entity<EntityAttribute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("EntityAttribute");

            entity.HasIndex(e => new { e.EntityId, e.AttributeKey }, "EntityId").IsUnique();

            entity.HasIndex(e => e.EntityId, "idx_entity_id");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.AttributeKey)
                .HasMaxLength(100)
                .HasColumnName("attribute_key");
            entity.Property(e => e.AttributeValue)
                .HasColumnType("json")
                .HasColumnName("attribute_value");
            entity.Property(e => e.EntityId).HasColumnType("bigint(20)");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Entity).WithMany(p => p.EntityAttributes)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("EntityAttribute_ibfk_1");
        });

        modelBuilder.Entity<EntityRelationship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("EntityRelationship");

            entity.HasIndex(e => new { e.GameInstanceId, e.SourceEntityId, e.TargetEntityId, e.RelationshipType }, "GameInstanceId").IsUnique();

            entity.HasIndex(e => e.GameInstanceId, "idx_game_instance_id");

            entity.HasIndex(e => e.SourceEntityId, "idx_source_entity_id");

            entity.HasIndex(e => e.TargetEntityId, "idx_target_entity_id");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GameInstanceId).HasColumnType("bigint(20)");
            entity.Property(e => e.RelationshipType)
                .HasMaxLength(50)
                .HasColumnName("relationship_type");
            entity.Property(e => e.SourceEntityId).HasColumnType("bigint(20)");
            entity.Property(e => e.TargetEntityId).HasColumnType("bigint(20)");

            entity.HasOne(d => d.GameInstance).WithMany(p => p.EntityRelationships)
                .HasForeignKey(d => d.GameInstanceId)
                .HasConstraintName("EntityRelationship_ibfk_1");

            entity.HasOne(d => d.SourceEntity).WithMany(p => p.EntityRelationshipSourceEntities)
                .HasForeignKey(d => d.SourceEntityId)
                .HasConstraintName("EntityRelationship_ibfk_2");

            entity.HasOne(d => d.TargetEntity).WithMany(p => p.EntityRelationshipTargetEntities)
                .HasForeignKey(d => d.TargetEntityId)
                .HasConstraintName("EntityRelationship_ibfk_3");
        });

        modelBuilder.Entity<GameInstance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("GameInstance");

            entity.HasIndex(e => e.UserId, "idx_user_id");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GameType)
                .HasMaxLength(50)
                .HasColumnName("game_type");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.LastUpdated)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("last_updated");
            entity.Property(e => e.UserId).HasColumnType("bigint(20)");

            entity.HasOne(d => d.User).WithMany(p => p.GameInstances)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("GameInstance_ibfk_1");
        });

        modelBuilder.Entity<GameState>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("GameState");

            entity.HasIndex(e => new { e.GameInstanceId, e.StateKey }, "GameInstanceId").IsUnique();

            entity.HasIndex(e => e.GameInstanceId, "idx_game_instance_id");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.GameInstanceId).HasColumnType("bigint(20)");
            entity.Property(e => e.StateKey)
                .HasMaxLength(100)
                .HasColumnName("state_key");
            entity.Property(e => e.StateValue)
                .HasColumnType("json")
                .HasColumnName("state_value");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.GameInstance).WithMany(p => p.GameStates)
                .HasForeignKey(d => d.GameInstanceId)
                .HasConstraintName("GameState_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.Username, "username").IsUnique();

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.LastLogin)
                .HasColumnType("datetime")
                .HasColumnName("last_login");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
