using System;
using System.Linq;
using FluentAssertions;
using Game.Domain;
using MongoDB.Driver;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class TestMongoGameTurnRepository
{
    [SetUp]
    public void SetUp()
    {
        db = TestMongoDatabase.Create();
        db.DropCollection("GameTurn");
        repo = new MongoGameTurnRepository(db);
    }

    private MongoGameTurnRepository repo;
    private IMongoDatabase db;

    [Test]
    public void InsertionWorks()
    {
        var turnId = Guid.NewGuid();
        var gameId = Guid.Empty;

        var turn = new GameTurnEntity
        {
            Id = turnId,
            GameId = gameId,
            TurnNumber = 9,
            PlayerDescisions = new PlayerDecision[] { },
            Winner = null
        };

        repo.Insert(turn);
        var lastTurns = repo.GetLastTurns(gameId).ToList();

        lastTurns.Count.Should().Be(1);
        lastTurns[0].Should().BeEquivalentTo(turn);
    }
}