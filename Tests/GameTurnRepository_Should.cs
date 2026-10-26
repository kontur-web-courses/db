using System;
using System.Collections.Generic;
using System.Linq;
using Game.Domain;
using MongoDB.Driver;
using NUnit.Framework;

namespace Tests
{
    public class GameTurnRepository_Should
    {
        [TestCase(PlayerDecision.Scissors, 0)]
        [TestCase(PlayerDecision.Paper, 1)]
        [TestCase(PlayerDecision.Rock, -1)]
        public void FinishTurnKeepsDecisionsAndWinner(PlayerDecision secondDecision, int winnerIndex)
        {
            var first = new Player(Guid.NewGuid(), "First");
            var second = new Player(Guid.NewGuid(), "Second");
            var game = new GameEntity(Guid.NewGuid(), GameStatus.Playing, 3, 0,
                new List<Player> { first, second });
            game.SetPlayerDecision(first.UserId, PlayerDecision.Rock);
            game.SetPlayerDecision(second.UserId, secondDecision);

            var turn = game.FinishTurn();

            Assert.AreEqual(game.Id, turn.GameId);
            Assert.AreEqual(1, turn.TurnNumber);
            Assert.AreEqual(PlayerDecision.Rock, turn.Players[0].Decision);
            Assert.AreEqual(secondDecision, turn.Players[1].Decision);
            Assert.AreEqual(winnerIndex < 0 ? Guid.Empty : game.Players[winnerIndex].UserId, turn.WinnerId);
            Assert.IsNull(first.Decision);
            Assert.IsNull(second.Decision);
            game.SetPlayerDecision(first.UserId, PlayerDecision.Paper);
            Assert.AreEqual(PlayerDecision.Rock, turn.Players[0].Decision);
        }

        [Test]
        public void FindLastTurnsReturnsFiveLatestTurnsOfSelectedGame()
        {
            var db = TestMongoDatabase.Create();
            var repo = new MongoGameTurnRepository(db);
            var gameId = Guid.NewGuid();
            var otherGameId = Guid.NewGuid();
            var playerId = Guid.NewGuid();
            try
            {
                foreach (var number in new[] { 3, 1, 7, 2, 6, 4, 5 })
                    repo.Insert(new GameTurnEntity
                    {
                        GameId = gameId,
                        TurnNumber = number,
                        Players = new List<Player>
                        {
                            new Player(playerId, "First") { Decision = PlayerDecision.Rock, Score = number }
                        },
                        WinnerId = playerId
                    });
                repo.Insert(new GameTurnEntity { GameId = otherGameId, TurnNumber = 100 });

                var turns = new MongoGameTurnRepository(db).FindLastTurns(gameId, 5);

                CollectionAssert.AreEqual(new[] { 7, 6, 5, 4, 3 }, turns.Select(t => t.TurnNumber));
                Assert.IsTrue(turns.All(t => t.GameId == gameId && t.Id != Guid.Empty));
                Assert.AreEqual(playerId, turns[0].WinnerId);
                Assert.AreEqual("First", turns[0].Players[0].Name);
                Assert.AreEqual(PlayerDecision.Rock, turns[0].Players[0].Decision);
                Assert.IsEmpty(repo.FindLastTurns(Guid.NewGuid(), 5));
            }
            finally
            {
                db.GetCollection<GameTurnEntity>(MongoGameTurnRepository.CollectionName)
                    .DeleteMany(t => t.GameId == gameId || t.GameId == otherGameId);
            }
        }
    }
}
