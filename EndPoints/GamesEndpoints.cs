using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.EndPoints;

public static class GamesEndpoints
{
    const string GetGameEndPoint = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoint(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        group.MapGet("/",async (GameStoreContext dbContext)=> 
                await dbContext.Games
                    .Include(game=>game.Genre)
                    .Select(game=>game.ToGameSummaryDto())
                    .AsNoTracking()
                    .ToListAsync()); //get all games api _ _  minimal api

        //get specific game through id
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {

            Game? game = await dbContext.Games.FindAsync(id);

            return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        }).WithName(GetGameEndPoint);

        //POST game
        group.MapPost("/",async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(GetGameEndPoint, new { id = game.Id }, game.ToGameDetailsDto());
        })
        .WithParameterValidation();

        //put
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingGame)
                .CurrentValues.
                    SetValues(updatedGame.ToEntity(id));

            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        //delete
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
                    .Where(game=>game.Id == id)
                    .ExecuteDeleteAsync();
            
           await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        return group;
    }
}
