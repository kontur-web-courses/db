using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity Insert(GameTurnEntity turn);
        List<GameTurnEntity> GetLastTurns(Guid gameId);
    }
}