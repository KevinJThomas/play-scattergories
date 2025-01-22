using Microsoft.AspNetCore.SignalR;
using PlayScattergories.Server.Models;
using PlayScattergories.Server.Models.Player;
using PlayScattergories.Server.Services;

public class MessageHub : Hub
{
    private readonly ILogger _logger;

    public MessageHub(ILogger<MessageHub> logger)
    {
        _logger = logger;
    }

    public async Task PlayerJoined(string name)
    {
        if (!LobbyService.Initialized)
        {
            LobbyService.Initialize(_logger);
        }

        var id = Context.ConnectionId;
        var player = new Player(id, name);
        var lobby = LobbyService.NewPlayerJoined(player);

        if (lobby != null)
        {
            await Groups.AddToGroupAsync(id, lobby.Id);
            await Clients.Caller.SendAsync("LobbyUpdated", lobby, Context.ConnectionId);
            await Clients.Group(lobby.Id).SendAsync("LobbyUpdated", lobby);
        }
    }

    public async Task NextRound()
    {
        var lobby = LobbyService.NextRound(Context.ConnectionId);

        if (lobby != null)
        {
            if (!lobby.IsWaitingToStart && !lobby.FailedToStart && lobby.IsActive)
            {
                await Clients.Group(lobby.Id).SendAsync("ConfirmNextRound", lobby);
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

    public async Task VoteSubmitted(List<string> words)
    {
        var lobby = LobbyService.VotesSubmitted(Context.ConnectionId, words);

        if (lobby != null && LobbyService.IsVotingComplete(lobby))
        {
            var scoredLobby = LobbyService.ScoreVotes(lobby.Id);

            if (scoredLobby != null)
            {
                await Clients.Group(lobby.Id).SendAsync("VoteComplete", scoredLobby);
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

        if (lobby != null && lobby.IsActive && lobby.IsWaitingToStart && !string.IsNullOrWhiteSpace(lobby.Id) && lobby.Players != null && lobby.Players.Any())
        {
            await Clients.Group(lobby.Id).SendAsync("LobbyUpdated", lobby);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendChat(string message)
    {
        var lobby = LobbyService.GetLobbyByPlayerId(Context.ConnectionId);

        if (lobby != null && lobby.IsActive && !string.IsNullOrWhiteSpace(lobby.Id) && lobby.Players != null && lobby.Players.Any())
        {
            var player = lobby.Players.Where(x => x.Id == Context.ConnectionId).FirstOrDefault();
            if (player != null)
            {
                var returnMessage = new Message
                {
                    Id =  Guid.NewGuid().ToString(),
                    Value = message,
                    Name = player.Name
                };
                await Clients.Group(lobby.Id).SendAsync("ChatReceived", returnMessage);
            }
        }
    }
}
