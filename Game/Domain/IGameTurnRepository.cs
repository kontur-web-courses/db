using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity FindById(Guid gameId);

        List<GameTurnEntity> FindByGameId(Guid gameId, int count);
        
        GameTurnEntity Insert(GameTurnEntity game);
    }
}