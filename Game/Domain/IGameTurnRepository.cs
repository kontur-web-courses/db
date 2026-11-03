using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity Insert(GameTurnEntity turn);
        IList<GameTurnEntity> GetLast(Guid gameId, int limit);
    }
}
