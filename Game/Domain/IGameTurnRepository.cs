using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity Insert(GameTurnEntity gameTurnEntity);
        IReadOnlyCollection<GameTurnEntity> FindLatest(int limit = 5);
    }
}