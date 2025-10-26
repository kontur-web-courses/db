using System;
using System.Collections.Generic;
using Game.Domain;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GameTurnRepositoryTests
    {
        private MongoGameTurnRepository repository;

        [SetUp]
        public void Setup()
        {
            var db = TestMongoDatabase.Create();
            db.DropCollection(MongoGameTurnRepository.CollectionName);
            repository = new MongoGameTurnRepository(db);
        }

        [Test]
        public void InsertGameTurn_ShouldWork()
        {
            var gameId = Guid.NewGuid();
            var turn = new GameTurnEntity
            {
                GameId = gameId,
                TurnIndex = 0,
                Decisions = new Dictionary<Guid, PlayerDecision>
                {
                    [Guid.NewGuid()] = PlayerDecision.Rock,
                    [Guid.NewGuid()] = PlayerDecision.Scissors
                },
                WinnerId = Guid.NewGuid()
            };
            
            var inserted = repository.Insert(turn);
            
            Assert.IsNotNull(inserted);
            Assert.IsNotNull(inserted.Id);
            Assert.AreNotEqual(Guid.Empty, inserted.Id);
            Assert.AreEqual(gameId, inserted.GameId);
            Assert.AreEqual(0, inserted.TurnIndex);
        }

        [Test]
        public void GetLastTurns_ShouldReturnEmpty_WhenNoTurns()
        {
            var gameId = Guid.NewGuid();
            
            var turns = repository.GetLastTurns(gameId, 5);
            
            Assert.IsNotNull(turns);
            Assert.IsEmpty(turns);
        }

        [Test]
        public void GetLastTurns_ShouldReturnTurnsOrderedByIndexDescending()
        {
            var gameId = Guid.NewGuid();
            var turn1 = new GameTurnEntity { GameId = gameId, TurnIndex = 0 };
            var turn2 = new GameTurnEntity { GameId = gameId, TurnIndex = 1 };
            var turn3 = new GameTurnEntity { GameId = gameId, TurnIndex = 2 };
            
            repository.Insert(turn1);
            repository.Insert(turn2);
            repository.Insert(turn3);
            
            var turns = repository.GetLastTurns(gameId, 5);
            
            Assert.AreEqual(3, turns.Count);
            Assert.AreEqual(2, turns[0].TurnIndex);
            Assert.AreEqual(1, turns[1].TurnIndex);
            Assert.AreEqual(0, turns[2].TurnIndex);
        }

        [Test]
        public void GetLastTurns_ShouldLimitResults()
        {
            var gameId = Guid.NewGuid();
            for (var i = 0; i < 10; i++)
            {
                repository.Insert(new GameTurnEntity { GameId = gameId, TurnIndex = i });
            }
            
            var turns = repository.GetLastTurns(gameId, 5);
            
            Assert.AreEqual(5, turns.Count);
            Assert.AreEqual(9, turns[0].TurnIndex);
            Assert.AreEqual(8, turns[1].TurnIndex);
            Assert.AreEqual(7, turns[2].TurnIndex);
            Assert.AreEqual(6, turns[3].TurnIndex);
            Assert.AreEqual(5, turns[4].TurnIndex);
        }

        [Test]
        public void GetLastTurns_ShouldReturnOnlyTurnsForSpecifiedGame()
        {
            var gameId1 = Guid.NewGuid();
            var gameId2 = Guid.NewGuid();
            
            repository.Insert(new GameTurnEntity { GameId = gameId1, TurnIndex = 0 });
            repository.Insert(new GameTurnEntity { GameId = gameId1, TurnIndex = 1 });
            repository.Insert(new GameTurnEntity { GameId = gameId2, TurnIndex = 0 });
            
            var turnsGame1 = repository.GetLastTurns(gameId1, 5);
            var turnsGame2 = repository.GetLastTurns(gameId2, 5);
            
            Assert.AreEqual(2, turnsGame1.Count);
            Assert.AreEqual(1, turnsGame2.Count);
            
            Assert.IsTrue(turnsGame1.TrueForAll(t => t.GameId == gameId1));
            Assert.IsTrue(turnsGame2.TrueForAll(t => t.GameId == gameId2));
        }

        [Test]
        public void FindByGameId_ShouldReturnAllTurnsForGame()
        {
            var gameId = Guid.NewGuid();
            var otherGameId = Guid.NewGuid();
            
            repository.Insert(new GameTurnEntity { GameId = gameId, TurnIndex = 0 });
            repository.Insert(new GameTurnEntity { GameId = gameId, TurnIndex = 1 });
            repository.Insert(new GameTurnEntity { GameId = otherGameId, TurnIndex = 0 });
            
            var turns = repository.FindByGameId(gameId);
            
            Assert.AreEqual(2, turns.Count);
            Assert.IsTrue(turns.TrueForAll(t => t.GameId == gameId));
        }

        [Test]
        public void GameTurn_ShouldStoreAllRequiredData()
        {
            var player1Id = Guid.NewGuid();
            var player2Id = Guid.NewGuid();
            var winnerId = player1Id;
            var gameId = Guid.NewGuid();
            
            var turn = new GameTurnEntity
            {
                GameId = gameId,
                TurnIndex = 5,
                Decisions = new Dictionary<Guid, PlayerDecision>
                {
                    [player1Id] = PlayerDecision.Rock,
                    [player2Id] = PlayerDecision.Scissors
                },
                WinnerId = winnerId
            };
            
            var inserted = repository.Insert(turn);
            var retrieved = repository.GetLastTurns(gameId, 1)[0];
            
            Assert.AreEqual(turn.GameId, retrieved.GameId);
            Assert.AreEqual(turn.TurnIndex, retrieved.TurnIndex);
            Assert.AreEqual(turn.WinnerId, retrieved.WinnerId);
            Assert.AreEqual(2, retrieved.Decisions.Count);
            Assert.AreEqual(PlayerDecision.Rock, retrieved.Decisions[player1Id]);
            Assert.AreEqual(PlayerDecision.Scissors, retrieved.Decisions[player2Id]);
            Assert.IsTrue(retrieved.FinishedAt > DateTime.UtcNow.AddMinutes(-1));
        }
    }
}
