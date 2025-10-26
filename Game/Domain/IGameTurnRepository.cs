using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity Insert(GameTurnEntity gameTurn);
        
        /// <summary>
        /// Находит все туры для указанной игры, отсортированные по номеру тура (от новых к старым)
        /// </summary>
        List<GameTurnEntity> GetLastTurns(Guid gameId, int turnsCount);
        
        /// <summary>
        /// Находит последние N туров для указанной игры
        /// </summary>
        List<GameTurnEntity> FindByGameId(Guid gameId);
    }
}
