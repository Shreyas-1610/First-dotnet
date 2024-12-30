using System;
using GameStore.Api.Dtos;

namespace GameStore.Api.EndPoints;

public static class GamesEndpoints
{
    const string GetGameEndPoint = "GetGame";

    private static readonly List<GameDto> games = [
        new(1,"GTA 5","Action", 800.90M, new DateOnly(2013,7,20)),
        new(2,"Forza","Racing", 1800.90M, new DateOnly(2017,9,22)),
        new(3,"Red Dead","Fighting", 2800.90M, new DateOnly(2019,6,20))
];

    public static RouteGroupBuilder MapGamesEndpoint(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        group.MapGet("/", () => games); //get all games api _ _  minimal api

        //get specific game through id
        group.MapGet("/{id}", (int id) =>
        {

            GameDto? game = games.Find(game => game.Id == id);

            return game is null ? Results.NotFound() : Results.Ok(game);
        }).WithName(GetGameEndPoint);

        //POST game
        group.MapPost("/", (CreateGameDto newGame) =>
        {
            GameDto game = new(
                games.Count + 1,
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleaseDate);

            games.Add(game);

            return Results.CreatedAtRoute(GetGameEndPoint, new { id = game.Id }, game);
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

            games[index] = new GameDto(
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
