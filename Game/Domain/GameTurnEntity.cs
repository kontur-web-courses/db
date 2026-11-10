using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonConstructor]
        public GameTurnEntity(
            Guid id,
            Guid gameId,
            int turnIndex,
            Guid firstPlayerId,
            string firstPlayerName,
            PlayerDecision? firstPlayerDecision,
            Guid secondPlayerId,
            string secondPlayerName,
            PlayerDecision? secondPlayerDecision,
            Guid winnerId)
        {
            Id = id;
            GameId = gameId;
            TurnIndex = turnIndex;
            FirstPlayerId = firstPlayerId;
            FirstPlayerName = firstPlayerName;
            FirstPlayerDecision = firstPlayerDecision;
            SecondPlayerId = secondPlayerId;
            SecondPlayerName = secondPlayerName;
            SecondPlayerDecision = secondPlayerDecision;
            WinnerId = winnerId;
        }

        [BsonId]
        public Guid Id { get; }

        [BsonElement]
        public Guid GameId { get; }

        [BsonElement]
        public int TurnIndex { get; }

        [BsonElement]
        public Guid FirstPlayerId { get; }

        [BsonElement]
        public string FirstPlayerName { get; }

        [BsonElement]
        public PlayerDecision? FirstPlayerDecision { get; }

        [BsonElement]
        public Guid SecondPlayerId { get; }

        [BsonElement]
        public string SecondPlayerName { get; }

        [BsonElement]
        public PlayerDecision? SecondPlayerDecision { get; }

        [BsonElement]
        public Guid WinnerId { get; }

        public override string ToString()
        {
            return $"{nameof(GameId)}: {GameId}, {nameof(TurnIndex)}: {TurnIndex}, {nameof(WinnerId)}: {WinnerId}";
        }
    }
}
