using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        public void Insert(GameTurnEntity gameTurn);
        public IEnumerable<GameTurnEntity> GetLastTurns(Guid gameId);
    }
}