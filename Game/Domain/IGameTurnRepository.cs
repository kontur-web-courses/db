using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity Insert(GameTurnEntity gameTurn);
        IList<GameTurnEntity> GetLastTurns(int limit, Guid gameId);
    }
}