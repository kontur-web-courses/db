using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        void Insert(GameTurnEntity turn);
        List<GameTurnEntity> GetLastTurns(Guid gameId, int limit = 5);
    }
}