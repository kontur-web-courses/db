using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        void Insert(GameTurnEntity turn);
        IList<GameTurnEntity> FindLastTurns(Guid gameId, int limit);
    }
}
