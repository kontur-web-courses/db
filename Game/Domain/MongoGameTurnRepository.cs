using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        public void Insert(GameTurnEntity gameTurn)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GameTurnEntity> GetLastTurns(Guid gameId)
        {
            throw new NotImplementedException();
        }
    }
}