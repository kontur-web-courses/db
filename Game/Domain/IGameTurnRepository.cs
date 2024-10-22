using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        void Insert(GameTurnEntity turn);
        List<GameTurnEntity> GetLastTurns(Guid gameId, int count);
    }
}