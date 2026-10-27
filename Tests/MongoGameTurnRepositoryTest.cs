using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Game.Domain;
using NUnit.Framework;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Tests
{
    [TestFixture]
    public class MongoGameTurnRepositoryTest
    {
        [SetUp]
        public void SetUp()
        {
            var db = TestMongoDatabase.Create();
            db.DropCollection(MongoGameRepository.CollectionName);
            repo = new MongoGameTurnRepository(db);
        }

        private MongoGameTurnRepository repo;

        [Test]
        public void FindLastTurns_ReturnsLastFiveTurnsInOrder()
        {
            var gameId =
            Guid.NewGuid();
            for (int i = 0; i < 10; i++)
            {
                var turn = new GameTurnEntity(Guid.NewGuid(), gameId, i, Guid.Empty, new List<PlayerTurnDecision>());
                repo.Insert(turn);
            }

            var lastTurns = repo.FindLastTurns(gameId, 5);

            lastTurns.Should().HaveCount(5);
            lastTurns.Select(t => t.TurnIndex).Should().ContainInOrder(9, 8, 7, 6, 5);
        }

        [Test]
        public void Repository_CreatesCompoundIndexOnGameIdAndTurnIndex()
        {
            var db = TestMongoDatabase.Create();
            var collection = db.GetCollection<GameTurnEntity>(MongoGameTurnRepository.CollectionName);

            var indexes = collection.Indexes.List().ToList();

            // Ищем индекс, содержащий поля GameId и TurnIndex
            var hasIndex = indexes.Any(index =>
            {
                var keyDoc = index["key"].AsBsonDocument;
                return keyDoc.Contains("GameId") && keyDoc.Contains("TurnIndex");
            });

            hasIndex.Should().BeTrue("составной индекс по GameId и TurnIndex необходим для быстрого поиска последних ходов");
        }

        [Test]
        public void FindLastTurns_ReturnsTurnsOnlyForSpecifiedGame()
        {
            var targetGameId = Guid.NewGuid();
            var otherGameId = Guid.NewGuid();

            for (int i = 0; i < 5; i++)
            {
                repo.Insert(new GameTurnEntity(Guid.NewGuid(), targetGameId, i, Guid.Empty, new List<PlayerTurnDecision>()));
                repo.Insert(new GameTurnEntity(Guid.NewGuid(), otherGameId, i, Guid.Empty, new List<PlayerTurnDecision>()));
            }

            var result = repo.FindLastTurns(targetGameId, 5);

            result.Should().HaveCount(5);
            result.Should().OnlyContain(t => t.GameId == targetGameId);
        }


        [Test]
        public void FindLastTurns_WhenTurnsCountLessThanLimit_ReturnsAllTurns()
        {
            var gameId = Guid.NewGuid();
            repo.Insert(new GameTurnEntity(Guid.NewGuid(), gameId, 0, Guid.Empty, new List<PlayerTurnDecision>()));
            repo.Insert(new GameTurnEntity(Guid.NewGuid(), gameId, 1, Guid.Empty, new List<PlayerTurnDecision>()));

            var result = repo.FindLastTurns(gameId, 5);

            result.Should().HaveCount(2);
            result.Select(t => t.TurnIndex).Should().ContainInOrder(1, 0);
        }
    }
}