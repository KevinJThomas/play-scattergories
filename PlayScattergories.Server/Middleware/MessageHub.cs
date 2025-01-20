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

        await Clients.Caller.SendAsync("LobbyUpdated", lobby.Players, true);
        await Clients.Group(lobby.Id).SendAsync("LobbyUpdated", lobby.Players);
    }

    public async Task StartLobby(string lobbyId)
    {
        var lobby = LobbyService.StartLobby(lobbyId);

        await Clients.All.SendAsync("ConfirmStartLobby", lobby);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var lobby = LobbyService.PlayerLeft(Context.ConnectionId);

        await Clients.Group(lobby.Id).SendAsync("LobbyUpdated", lobby.Players);
    }
}
