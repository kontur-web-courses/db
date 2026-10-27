using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        // TODO: Спроектировать интерфейс исходя из потребностей ConsoleApp
        void Insert(GameTurnEntity turn);
        IList<GameTurnEntity> FindLastTurns(Guid gameId, int limit);
    }
}