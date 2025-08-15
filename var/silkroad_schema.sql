use silkroad;
CREATE TABLE User (
    Id BIGINT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    last_login DATETIME,
    UNIQUE (username),
    UNIQUE (email)
);

CREATE TABLE GameInstance (
    Id BIGINT PRIMARY KEY AUTO_INCREMENT,
    UserId BIGINT NOT NULL,
    game_type VARCHAR(50) NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    last_updated DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (UserId) REFERENCES User(Id) ON DELETE CASCADE,
    INDEX idx_user_id (UserId)
);

CREATE TABLE GameState (
    Id BIGINT PRIMARY KEY AUTO_INCREMENT,
    GameInstanceId BIGINT NOT NULL,
    state_key VARCHAR(100) NOT NULL,
    state_value JSON NOT NULL,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (GameInstanceId) REFERENCES GameInstance(Id) ON DELETE CASCADE,
    UNIQUE (GameInstanceId, state_key),
    INDEX idx_game_instance_id (GameInstanceId)
);

CREATE TABLE Entity (
    Id BIGINT PRIMARY KEY AUTO_INCREMENT,
    GameInstanceId BIGINT NOT NULL,
    entity_type VARCHAR(50) NOT NULL,
    name VARCHAR(100),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    last_updated DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (GameInstanceId) REFERENCES GameInstance(Id) ON DELETE CASCADE,
    INDEX idx_game_instance_id (GameInstanceId)
);

CREATE TABLE EntityAttribute (
    Id BIGINT PRIMARY KEY AUTO_INCREMENT,
    EntityId BIGINT NOT NULL,
    attribute_key VARCHAR(100) NOT NULL,
    attribute_value JSON NOT NULL,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (EntityId) REFERENCES Entity(Id) ON DELETE CASCADE,
    UNIQUE (EntityId, attribute_key),
    INDEX idx_entity_id (EntityId)
);

CREATE TABLE EntityRelationship (
    Id BIGINT PRIMARY KEY AUTO_INCREMENT,
    GameInstanceId BIGINT NOT NULL,
    SourceEntityId BIGINT NOT NULL,
    TargetEntityId BIGINT NOT NULL,
    relationship_type VARCHAR(50) NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (GameInstanceId) REFERENCES GameInstance(Id) ON DELETE CASCADE,
    FOREIGN KEY (SourceEntityId) REFERENCES Entity(Id) ON DELETE CASCADE,
    FOREIGN KEY (TargetEntityId) REFERENCES Entity(Id) ON DELETE CASCADE,
    UNIQUE (GameInstanceId, SourceEntityId, TargetEntityId, relationship_type),
    INDEX idx_game_instance_id (GameInstanceId),
    INDEX idx_source_entity_id (SourceEntityId),
    INDEX idx_target_entity_id (TargetEntityId)
);