using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        // TODO: Спроектировать интерфейс исходя из потребностей ConsoleApp
        GameTurnEntity Insert(GameTurnEntity turn);
        IReadOnlyList<GameTurnEntity> FindLastTurns(Guid gameId, int maxCount = 5);
    }
}