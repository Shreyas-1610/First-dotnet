using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;

namespace GameStore.Api.EndPoints;

public static class GamesEndpoints
{
    const string GetGameEndPoint = "GetGame";

    private static readonly List<GameSummaryDto> games = [
        new(1,"GTA 5","Action", 800.90M, new DateOnly(2013,7,20)),
        new(2,"Forza","Racing", 1800.90M, new DateOnly(2017,9,22)),
        new(3,"Red Dead","Fighting", 2800.90M, new DateOnly(2019,6,20))
];

    public static RouteGroupBuilder MapGamesEndpoint(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        group.MapGet("/", () => games); //get all games api _ _  minimal api

        //get specific game through id
        group.MapGet("/{id}", (int id, GameStoreContext dbContext) =>
        {

            Game? game = dbContext.Games.Find(id);

            return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        }).WithName(GetGameEndPoint);

        //POST game
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute(GetGameEndPoint, new { id = game.Id }, game.ToGameDetailsDto());
        })
        .WithParameterValidation();

        //put
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
        {
            var index = games.FindIndex(game => game.Id == id);

            if (index == -1)
            {
                return Results.NotFound();
            }

            games[index] = new GameSummaryDto(
                id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            );

            return Results.NoContent();
        });

        //delete
        group.MapDelete("/{id}", (int id) =>
        {
            games.RemoveAll(game => game.Id == id);

            return Results.NoContent();
        });

        return group;
    }
}
