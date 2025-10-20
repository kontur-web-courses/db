using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        // TODO: Спроектировать интерфейс исходя из потребностей ConsoleApp
        GameTurnEntity Insert(GameTurnEntity gameTurn);
        IReadOnlyList<GameTurnEntity> GetLastTurns(Guid gameId, int turnsCount);
    }
}