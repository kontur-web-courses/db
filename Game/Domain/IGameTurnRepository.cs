using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity Insert(GameTurnEntity gameTurn);
        IList<GameTurnEntity> FindLastByGameId(Guid gameId, int limit = 5);
    }
}
