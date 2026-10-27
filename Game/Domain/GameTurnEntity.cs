using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonConstructor]
        public GameTurnEntity(Guid id, Guid gameId, int turnIndex,
            Guid firstPlayerId, string firstPlayerName, PlayerDecision firstPlayerDecision,
            Guid secondPlayerId, string secondPlayerName, PlayerDecision secondPlayerDecision,
            Guid? winnerId)
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

        public Guid Id { get; private set; }
        public Guid GameId { get; }
        public int TurnIndex { get; }

        public Guid FirstPlayerId { get; }
        public string FirstPlayerName { get; }
        public PlayerDecision FirstPlayerDecision { get; }

        public Guid SecondPlayerId { get; }
        public string SecondPlayerName { get; }
        public PlayerDecision SecondPlayerDecision { get; }

        public Guid? WinnerId { get; }

        public string WinnerName =>
            WinnerId == null ? null
            : WinnerId == FirstPlayerId ? FirstPlayerName : SecondPlayerName;
    }
}