using PlayScattergories.Server.Helpers;
using PlayScattergories.Server.Models;
using PlayScattergories.Server.Models.Player;
using System.Numerics;

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
                    return _lobbies[index];
                }
            }

            return null;
        }

        public static Lobby? GameStarted(string playerId)
        {
            var lobbyIndex = -1;
            var lobbyId = string.Empty;

            foreach (var lobby in _lobbies)
            {
                if (lobby.Players[0].Id == playerId)
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
                _lobbies[lobbyIndex].IsWaitingToStart &&
                _lobbies[lobbyIndex].IsActive &&
                _lobbies[lobbyIndex].GameState != null)
            {
                var (newCategoryCardList, newCategoryCard) = GameService.ChooseNextCategoryCard(_lobbies[lobbyIndex].GameState.UnusedCategoryCards);
                _lobbies[lobbyIndex].GameState.UnusedCategoryCards = newCategoryCardList;
                _lobbies[lobbyIndex].GameState.CategoryCard = newCategoryCard;
                _lobbies[lobbyIndex].GameState.Letter = GameService.GetLetter();
                _lobbies[lobbyIndex].GameState.RoundNumber = 1;
                _lobbies[lobbyIndex].IsWaitingToStart = false;
                var time = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1).AddMinutes(3);
                _lobbies[lobbyIndex].GameState.SubmitNextRoundTimeLimit = (long)(time.TotalMilliseconds + 0.5);

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
                if (lobby.Players.Any(p => p.Id == id))
                {
                    var index = GetPlayerIndexById(id, lobby);
                    lobby.Players.RemoveAt(index);
                    return lobby;
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
                    lobby.Players[index].ScoreSheet = GameService.PopulateScoreSheet(words, lobby.Players[index].ScoreSheet, lobby.GameState.RoundNumber);
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
                        if (lobby.Players.Any(x => !x.RoundOneSubmitted) || lobby.GameState.RoundOneSubmitted)
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
                        if (lobby.Players.Any(x => !x.RoundTwoSubmitted))
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
                        if (lobby.Players.Any(x => !x.RoundThreeSubmitted))
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
                _lobbies[index].GameState = GameService.ScoreRound(_lobbies[index]);
                return _lobbies[index];
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
