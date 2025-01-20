using Microsoft.AspNetCore.SignalR;
using PlayScattergories.Server.Models.Player;
using PlayScattergories.Server.Services;

public class MessageHub : Hub
{
    public async Task PlayerJoined(string name)
    {
        var id = Context.ConnectionId;
        var player = new Player(id, name);
        var lobby = LobbyService.NewPlayerJoined(player);

        if (lobby != null)
        {
            await Groups.AddToGroupAsync(id, lobby.Id);
            await Clients.Caller.SendAsync("LobbyUpdated", lobby.Players, Context.ConnectionId);
            await Clients.Group(lobby.Id).SendAsync("LobbyUpdated", lobby.Players);
        }
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
            var scoredLobby = LobbyService.ScoreRound(lobby.Id);

            if (scoredLobby != null)
            {
                await Clients.Group(lobby.Id).SendAsync("RoundComplete", scoredLobby);
            }
            else
            {
                await Clients.Group(lobby.Id).SendAsync("GameError", lobby);
            }
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var lobby = LobbyService.PlayerLeft(Context.ConnectionId);

        if (lobby != null && !string.IsNullOrWhiteSpace(lobby.Id) && lobby.Players != null && lobby.Players.Any())
        {
            await Clients.Group(lobby.Id).SendAsync("LobbyUpdated", lobby.Players);
        }
    }
}
