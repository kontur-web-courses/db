using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity Insert(GameTurnEntity user);
        List<GameTurnEntity> GetTurns(int count = 5);
    }
}