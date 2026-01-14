using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonConstructor]
        public GameTurnEntity(List<EasyPlayer> players, Guid winnerId, Guid gameId, int index)
        {
            Players = players;
            WinnerId = winnerId;
            GameId = gameId;
            Index = index;
        }
        
        public Guid Id { get; set; }

        [BsonElement]
        private List<EasyPlayer> Players { get; }
        [BsonElement]
        private Guid WinnerId { get; }
        [BsonElement]
        public Guid GameId { get; }
        [BsonElement]
        public int Index { get; }

        public override string ToString()
        {
            if (Players == null || Players.Count == 0)
                return "";
            return $"Ход {Index}  Победил {Players.FirstOrDefault(p => p.Id == WinnerId)?.Name ?? "Никто"}\t\tВыбрали: {string.Join(" \t ", Players.Select(p => $"{p.Name}: {p.Decision}"))}";
        }
    }

    public class EasyPlayer
    {
        [BsonConstructor]
        public EasyPlayer(Guid id, string name, PlayerDecision decision)
        {
            Id = id;
            Name = name;
            Decision = decision;
        }        
        
        public EasyPlayer(Player player)
        {
            Id = player.UserId;
            Name = player.Name;
            Decision = player.Decision.Value;
        }

        [BsonElement]
        public Guid Id { get; set; }
        [BsonElement]
        public string Name { get; set; }     
        [BsonElement]
        public PlayerDecision Decision { get; set; }
    }
}