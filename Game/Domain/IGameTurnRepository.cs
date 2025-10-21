using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        void SaveTurn(GameTurnEntity turn);
        IReadOnlyList<GameTurnEntity> GetLastTurns(Guid gameId, int count);
    }
}