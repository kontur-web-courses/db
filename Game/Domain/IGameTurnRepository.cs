using System.Collections.Generic;
using System;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        // TODO: Спроектировать интерфейс исходя из потребностей ConsoleApp
        GameTurnEntity Insert(GameTurnEntity turn);
        List<GameTurnEntity> GetLastTurns(Guid gameId);
    }
}