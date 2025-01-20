using Microsoft.AspNetCore.SignalR;
using PlayScattergories.Server.Models;
using PlayScattergories.Server.Models.Player;
using PlayScattergories.Server.Services;
using System.Diagnostics;

public class MessageHub : Hub
{
    public async Task PlayerJoined(string name)
    {
        var id = Context.ConnectionId;
        var player = new Player
        {
            Id = id,
            Name = name,
            Points = 0,
            ScoreSheet = new ScoreSheet()
        };
        var lobby = LobbyService.NewPlayerJoined(player);

        var test = Groups.AddToGroupAsync(id, lobby.Id);

        await Clients.Caller.SendAsync("LobbyUpdated", lobby.Players, Context.ConnectionId);
        await Clients.Group(lobby.Id).SendAsync("LobbyUpdated", lobby.Players);
    }

    public async Task GameStarted()
    {
        var lobby = LobbyService.GameStarted(Context.ConnectionId);

        if (lobby != null)
        {
            if (!lobby.IsWaitingToStart && !lobby.FailedToStart)
            {
                await Clients.Group(lobby.Id).SendAsync("ConfirmGameStarted", lobby);
            }
            else if (lobby.FailedToStart && !string.IsNullOrWhiteSpace(lobby.Id))
            {
                await Clients.Group(lobby.Id).SendAsync("GameError", lobby);
            }
        }
    }

    public async Task WordsSubmitted(List<string> words)
    {
        var lobby = LobbyService.WordsSubmitted(Context.ConnectionId, words);

        if (lobby != null && LobbyService.IsRoundComplete(lobby))
        {
            await Clients.Group(lobby.Id).SendAsync("RoundComplete", lobby);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var lobby = LobbyService.PlayerLeft(Context.ConnectionId);

        await Clients.Group(lobby.Id).SendAsync("LobbyUpdated", lobby.Players);
    }
}
