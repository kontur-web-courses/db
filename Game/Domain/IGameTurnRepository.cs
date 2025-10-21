using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity Insert(GameTurnEntity entity);
        List<GameTurnEntity> GetLastTurns(Guid gameId, int maxCountTurns);
    }
}