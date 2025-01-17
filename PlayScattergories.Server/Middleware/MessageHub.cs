using Microsoft.AspNetCore.SignalR;
using PlayScattergories.Server.Models.Player;
using PlayScattergories.Server.Services;

public class MessageHub : Hub
{
    // TODO: delete this
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }

    public async Task PlayerJoined(string name)
    {
        var id = Guid.NewGuid().ToString();
        var player = new Player
        {
            Id = id,
            Name = name,
            Points = 0,
            ScoreSheet = new ScoreSheet()
        };
        var success = LobbyService.NewPlayerJoined(player);

        await Clients.All.SendAsync("ConfirmPlayerJoined", success ? id : string.Empty);
    }

    public async Task StartLobby(string lobbyId)
    {
        var lobby = LobbyService.StartLobby(lobbyId);

        await Clients.All.SendAsync("ConfirmStartLobby", lobby);
    }
}
