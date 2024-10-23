using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity Insert(GameTurnEntity gameTurnEntity);
        IReadOnlyList<GameTurnEntity> FindLastTurns(Guid gameId, int maxCount);
    }
}