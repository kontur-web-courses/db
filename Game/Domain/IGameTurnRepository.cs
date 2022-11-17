using System.Collections.Generic;
using System;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity Insert(GameTurnEntity turn);
        List<GameTurnEntity> GetLastTurns(Guid gameId);
    }
}