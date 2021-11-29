using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        // TODO: Спроектировать интерфейс исходя из потребностей ConsoleApp
        public GameTurnEntity Insert(GameTurnEntity gameTurn);

        public List<GameTurnEntity> FindLast(Guid gameId, int count);
    }
}