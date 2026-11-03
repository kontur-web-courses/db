using System;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        public Guid Id { get; private set; }
        public Guid GameId { get; private set; }
        public int TurnNumber { get; private set; }
        public Guid FirstPlayerId { get; private set; }
        public string FirstPlayerName { get; private set; }
        public PlayerDecision FirstPlayerDecision { get; private set; }
        public Guid SecondPlayerId { get; private set; }
        public string SecondPlayerName { get; private set; }
        public PlayerDecision SecondPlayerDecision { get; private set; }
        public Guid? WinnerId { get; private set; }

        public GameTurnEntity() { }

        public GameTurnEntity(Guid gameId, int turnNumber, Player first, Player second, Guid? winnerId)
        {
            Id = Guid.NewGuid(); GameId = gameId; TurnNumber = turnNumber;
            FirstPlayerId = first.UserId; FirstPlayerName = first.Name;
            FirstPlayerDecision = first.Decision ?? throw new ArgumentException();
            SecondPlayerId = second.UserId; SecondPlayerName = second.Name;
            SecondPlayerDecision = second.Decision ?? throw new ArgumentException();
            WinnerId = winnerId;
        }
    }
}
