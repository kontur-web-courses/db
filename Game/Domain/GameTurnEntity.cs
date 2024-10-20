using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain;

public class GameTurnEntity
{
    [BsonElement]
    private readonly List<PlayerInfo> players;

    [BsonConstructor]
    public GameTurnEntity(Guid id, Guid gameId, int index, List<PlayerInfo> players, Guid winnerId)
    {
        GameId = gameId;
        Index = index;
        this.players = players;
        WinnerId = winnerId;
        Id = id;
    }

    public GameTurnEntity(Guid id, Guid gameId, int index, List<Player> players, Guid winnerId) : this(id, gameId,
        index, players.Select(p => new PlayerInfo(p.Name, p.Decision!.Value)).ToList(), winnerId)
    {
    }

    [BsonElement]
    public Guid GameId { get; }

    [BsonElement]
    public Guid Id { get; }

    [BsonElement]
    public int Index { get; }

    [BsonElement]
    public Guid WinnerId { get; }

    [BsonElement]
    public IReadOnlyList<PlayerInfo> PlayerInfos => players.AsReadOnly();
}

public class PlayerInfo
{
    [BsonConstructor]
    public PlayerInfo(string name, PlayerDecision decision)
    {
        Name = name;
        Decision = decision;
    }

    [BsonElement]
    public string Name { get; }

    [BsonElement]
    public PlayerDecision Decision { get; }
}