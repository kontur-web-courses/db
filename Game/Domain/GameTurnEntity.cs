using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        //TODO: Придумать какие свойства должны быть в этом классе, чтобы сохранять всю информацию о закончившемся туре.
        [BsonConstructor]
        public GameTurnEntity(Guid gameId, List<PlayerTurn> players, Guid winnerId, string winnerName)
        {
            Id = Guid.NewGuid();
            GameId = gameId;
            WinnerId = winnerId;
            WinnerName = winnerName;
            IsDraw = winnerId == Guid.Empty;
            FinishedAt = DateTime.UtcNow;
            this.players = players;
        }

        [BsonElement]
        private readonly List<PlayerTurn> players;

        public Guid Id { get; private set; }
        public Guid GameId { get; private set; }
        public Guid WinnerId { get; private set; }
        public String WinnerName { get; private set; }
        public bool IsDraw { get; private set; }
        public DateTime FinishedAt { get; private set; }
        public IReadOnlyList<PlayerTurn> Players => players.AsReadOnly();

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var player in players)
            {
                sb.AppendLine($"{player.Name}-> {player.Decision}");
            }
            if (!IsDraw)
            {
                sb.AppendLine($"Winner: {WinnerName}");
                sb.AppendLine($"WinnerId: {WinnerId}");
            }
            else
            {
                sb.AppendLine($"Draw");
            }

            return sb.ToString();
        }
    }
}