using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity Insert(GameTurnEntity game);
        List<GameTurnEntity> GetLastTurns(int limit);
    }
}