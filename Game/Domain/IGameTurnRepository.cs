using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        void Insert(GameTurnEntity gameTurn);

        List<GameTurnEntity> GetTurns(Guid gameId);
    }
}