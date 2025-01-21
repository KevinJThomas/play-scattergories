using PlayScattergories.Server.Helpers;
using PlayScattergories.Server.Models;
using PlayScattergories.Server.Models.Game;
using PlayScattergories.Server.Models.Player;

namespace PlayScattergories.Server.Services
{
    public static class LobbyService
    {
        private static List<Lobby> _lobbies { get; set; } = new List<Lobby>();
        private static readonly int _playerMaxPerLobby = ConfigurationHelper.config.GetValue<int>("App:PlayerMaxPerLobby");

        #region public methods

        public static Lobby NewPlayerJoined(Player player)
        {
            ClearInactiveLobbies();
            var availableLobby = FindAvailableLobby();
            if (availableLobby != null)
            {
                var index = _lobbies.FindIndex(x => x == availableLobby);
                if (index >= 0 && index < _lobbies.Count)
                {
                    _lobbies[index].Players.Add(player);
                    _lobbies[index].HostId = _lobbies[index].Players[0].Id;
                    return _lobbies[index];
                }
            }

            return null;
        }

        public static Lobby? NextRound(string playerId)
        {
            var lobbyIndex = -1;
            var lobbyId = string.Empty;

            foreach (var lobby in _lobbies)
            {
                if (lobby != null && lobby.Players != null && lobby.Players.Any() && lobby.Players[0].Id == playerId)
                {
                    lobbyId = lobby.Id;
                    lobbyIndex = GetLobbyIndexById(lobbyId);
                    break;
                }
            }

            if (lobbyIndex < 0 ||
                lobbyIndex >= _lobbies.Count ||
                _lobbies[lobbyIndex] == null ||
                _lobbies[lobbyIndex].Players == null ||
                _lobbies[lobbyIndex].Players.Count <= 0 ||
                _lobbies[lobbyIndex].Players[0].Id != playerId)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(_lobbies[lobbyIndex].Id) &&
                _lobbies[lobbyIndex].IsActive &&
                _lobbies[lobbyIndex].GameState != null)
            {
                var (newCategoryCardList, newCategoryCard) = GameService.ChooseNextCategoryCard(_lobbies[lobbyIndex].GameState.UnusedCategoryCards);
                _lobbies[lobbyIndex].GameState.UnusedCategoryCards = newCategoryCardList;
                _lobbies[lobbyIndex].GameState.CategoryCard = newCategoryCard;
                if (!string.IsNullOrWhiteSpace(_lobbies[lobbyIndex].GameState.Letter))
                {
                    _lobbies[lobbyIndex].GameState.UsedLetters.Add(_lobbies[lobbyIndex].GameState.Letter);
                }

                _lobbies[lobbyIndex].GameState.Letter = GameService.GetLetter(_lobbies[lobbyIndex].GameState.UsedLetters);
                var time = DateTime.Now.AddMinutes(ConfigurationHelper.config.GetValue<int>("App:GameLengthInMinutes")).ToUniversalTime() - new DateTime(1970, 1, 1);
                _lobbies[lobbyIndex].GameState.SubmitNextRoundTimeLimit = (long)(time.TotalMilliseconds + 0.5);
                if (_lobbies[lobbyIndex].IsWaitingToStart)
                {
                    _lobbies[lobbyIndex].GameState.RoundNumber = 1;
                    _lobbies[lobbyIndex].IsWaitingToStart = false;
                }
                else
                {
                    _lobbies[lobbyIndex].GameState.RoundNumber += 1;
                }

                return _lobbies[lobbyIndex];
            }
            else
            {
                _lobbies[lobbyIndex].FailedToStart = true;
                return _lobbies[lobbyIndex];
            }
        }

        public static Lobby PlayerLeft(string id)
        {
            foreach (var lobby in _lobbies)
            {
                if (lobby.IsActive && lobby.Players.Any(p => p.Id == id))
                {
                    if (lobby.IsWaitingToStart)
                    {
                        // If the lobby hasn't started yet, remove the player from the lobby
                        var index = GetPlayerIndexById(id, lobby);
                        lobby.Players.RemoveAt(index);
                        if (lobby.Players.Any())
                        {
                            lobby.HostId = lobby.Players[0].Id;
                        }

                        return lobby;
                    }
                    else
                    {
                        // If the lobby has started, mark the player as inactive
                        var index = GetPlayerIndexById(id, lobby);
                        lobby.Players[index].IsActive = false;
                        var newHost = lobby.Players.Where(x => x.IsActive).FirstOrDefault();
                        if (newHost != null)
                        {
                            lobby.HostId = newHost.Id;
                        }
                        
                        return lobby;
                    }
                }
            }

            return null;
        }

        public static Lobby WordsSubmitted(string playerId, List<string> words)
        {
            foreach (var lobby in _lobbies)
            {
                if (lobby.Players.Any(p => p.Id == playerId))
                {
                    var index = GetPlayerIndexById(playerId, lobby);
                    var wordList = new List<Word>();
                    foreach (var word in words)
                    {
                        wordList.Add(new Word { Value = word, IsValid = false });
                    }

                    lobby.Players[index].ScoreSheet[lobby.GameState.RoundNumber - 1] = wordList;
                    lobby.Players[index] = GameService.MarkPlayerAsRoundComplete(lobby.Players[index], lobby.GameState.RoundNumber);
                    return lobby;
                }
            }

            return null;
        }

        public static bool IsRoundComplete(Lobby lobby)
        {
            if (lobby != null && lobby.GameState != null)
            {
                switch (lobby.GameState.RoundNumber)
                {
                    case 1:
                        if (lobby.Players.Any(x => !x.RoundOneSubmitted && x.IsActive) || lobby.GameState.RoundOneSubmitted)
                        {
                            return false;
                        }

                        var indexOne = GetLobbyIndexById(lobby.Id);
                        if (_lobbies[indexOne] != null && _lobbies[indexOne].GameState != null && !_lobbies[indexOne].GameState.RoundOneSubmitted)
                        {
                            _lobbies[indexOne].GameState.RoundOneSubmitted = true;
                            return true;
                        }

                        return false;
                    case 2:
                        if (lobby.Players.Any(x => !x.RoundTwoSubmitted && x.IsActive) || lobby.GameState.RoundTwoSubmitted)
                        {
                            return false;
                        }

                        var indexTwo = GetLobbyIndexById(lobby.Id);
                        if (_lobbies[indexTwo] != null && _lobbies[indexTwo].GameState != null && !_lobbies[indexTwo].GameState.RoundTwoSubmitted)
                        {
                            _lobbies[indexTwo].GameState.RoundTwoSubmitted = true;
                            return true;
                        }

                        return false;
                    case 3:
                        if (lobby.Players.Any(x => !x.RoundThreeSubmitted && x.IsActive) || lobby.GameState.RoundThreeSubmitted)
                        {
                            return false;
                        }

                        var indexThree = GetLobbyIndexById(lobby.Id);
                        if (_lobbies[indexThree] != null && _lobbies[indexThree].GameState != null && !_lobbies[indexThree].GameState.RoundThreeSubmitted)
                        {
                            _lobbies[indexThree].GameState.RoundThreeSubmitted = true;
                            return true;
                        }

                        return false;
                }
            }

            return false;
        }

        public static Lobby ScoreRound(string lobbyId)
        {
            if (string.IsNullOrWhiteSpace(lobbyId))
            {
                return null;
            }

            var index = GetLobbyIndexById(lobbyId);
            if (index >= 0 &&
                index < _lobbies.Count &&
                _lobbies[index] != null &&
                _lobbies[index].GameState != null &&
                _lobbies[index].Players != null)
            {
                _lobbies[index] = GameService.ScoreRound(_lobbies[index]);

                return _lobbies[index];
            }

            return null;
        }

        public static Lobby VotesSubmitted(string playerId, List<string> words)
        {
            foreach (var lobby in _lobbies)
            {
                if (lobby.Players.Any(p => p.Id == playerId) && lobby.GameState != null)
                {
                    if (lobby.GameState.Votes == null)
                    {
                        lobby.GameState.Votes = new List<Vote>();
                    }

                    foreach (var word in words)
                    {
                        if (lobby.GameState.Votes.Any(x => x.Word.ToLower() == word.ToLower()))
                        {
                            // If the word has already been voted for, increment count
                            var vote = lobby.GameState.Votes.Where(x => x.Word == word).FirstOrDefault();
                            var voteIndex = lobby.GameState.Votes.FindIndex(x => x == vote);
                            lobby.GameState.Votes[voteIndex].Count += 1;
                        }
                        else
                        {
                            // If the word hasn't already been voted for, add it to the list
                            lobby.GameState.Votes.Add(new Vote { Word = word, Count = 1 });
                        }
                    }

                    var index = GetPlayerIndexById(playerId, lobby);
                    lobby.Players[index] = GameService.MarkPlayerAsVotingComplete(lobby.Players[index], lobby.GameState.RoundNumber);
                    return lobby;
                }
            }

            return null;
        }

        public static bool IsVotingComplete(Lobby lobby)
        {
            if (lobby != null && lobby.GameState != null)
            {
                switch (lobby.GameState.RoundNumber)
                {
                    case 1:
                        return !lobby.Players.Any(x => !x.RoundOneVoted && x.IsActive);
                    case 2:
                        return !lobby.Players.Any(x => !x.RoundTwoVoted && x.IsActive);
                    case 3:
                        return !lobby.Players.Any(x => !x.RoundThreeVoted && x.IsActive);
                }
            }

            return false;
        }

        public static Lobby ScoreVotes(string lobbyId)
        {
            if (string.IsNullOrWhiteSpace(lobbyId))
            {
                return null;
            }

            var index = GetLobbyIndexById(lobbyId);
            if (index >= 0 &&
                index < _lobbies.Count &&
                _lobbies[index] != null &&
                _lobbies[index].GameState != null &&
                _lobbies[index].Players != null)
            {
                _lobbies[index] = GameService.ScoreVotes(_lobbies[index]);

                return _lobbies[index];
            }

            return null;
        }

        public static Lobby GetLobbyByPlayerId(string playerId)
        {
            foreach (var lobby in _lobbies)
            {
                if (lobby.IsActive && lobby.Players.Any(p => p.Id == playerId))
                {
                    return lobby;
                }
            }

            return null;
        }

        #endregion

        #region private methods

        private static Lobby CreateNewLobby()
        {
            var id = Guid.NewGuid().ToString();
            var lobby = new Lobby(id);
            _lobbies.Add(lobby);
            return lobby;
        }

        private static void ClearInactiveLobbies()
        {
            var newLobbiesList = new List<Lobby>();

            foreach (var lobby in _lobbies)
            {
                if (lobby.IsActive)
                {
                    newLobbiesList.Add(lobby);
                }
            }

            _lobbies = newLobbiesList;
        }

        private static Lobby FindAvailableLobby()
        {
            var nextAvailableLobby = _lobbies.Where(x => x.IsActive && x.IsWaitingToStart && x.Players.Count < _playerMaxPerLobby).FirstOrDefault();

            if (nextAvailableLobby == null || string.IsNullOrWhiteSpace(nextAvailableLobby.Id))
            {
                // If there isn't a valid lobby to join, create a new one
                nextAvailableLobby = CreateNewLobby();
            }

            return nextAvailableLobby;
        }

        private static int GetLobbyIndexById(string id)
        {
            var lobby = _lobbies.Where(x => x.Id == id).FirstOrDefault();
            if (lobby == null)
            {
                return -1;
            }

            return _lobbies.FindIndex(x => x == lobby);
        }

        private static int GetPlayerIndexById(string id, Lobby lobby)
        {
            var player = lobby.Players.Where(x => x.Id == id).FirstOrDefault();
            if (player == null)
            {
                return -1;
            }

            return lobby.Players.FindIndex(x => x == player);
        }

        #endregion
    }
}
