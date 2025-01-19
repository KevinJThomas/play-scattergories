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
        var playerList = LobbyService.NewPlayerJoined(player);

        await Clients.All.SendAsync("ConfirmPlayerJoined", playerList);
    }

    public async Task StartLobby(string lobbyId)
    {
        var lobby = LobbyService.StartLobby(lobbyId);

        await Clients.All.SendAsync("ConfirmStartLobby", lobby);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var playerList = LobbyService.PlayerLeft(Context.ConnectionId);

        await Clients.All.SendAsync("PlayerLeft", playerList);
    }
}
