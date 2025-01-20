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

        public static Lobby? StartLobby(string id)
        {
            var index = GetLobbyIndexById(id);

            if (index >= 0 &&
                index < _lobbies.Count &&
                _lobbies[index] != null &&
                !string.IsNullOrWhiteSpace(_lobbies[index].Id) &&
                _lobbies[index].IsWaitingToStart &&
                _lobbies[index].IsActive &&
                _lobbies[index].GameState != null)
            {
                var (newCategoryCardList, newCategoryCard) = GameService.ChooseNextCategoryCard(_lobbies[index].GameState.UnusedCategoryCards);
                _lobbies[index].GameState.UnusedCategoryCards = newCategoryCardList;
                _lobbies[index].GameState.CategoryCard = newCategoryCard;
                _lobbies[index].GameState.Letter = GameService.GetLetter();
                _lobbies[index].GameState.RoundNumber = 1;
                _lobbies[index].IsWaitingToStart = false;

                return _lobbies[index];
            }

            return null;
        }

        public static List<Player> PlayerLeft(string id)
        {
            foreach (var lobby in _lobbies)
            {
                if (lobby.Players.Any(p => p.Id == id))
                {
                    var index = GetPlayerIndexById(id, lobby);
                    lobby.Players.RemoveAt(index);
                    return lobby.Players;
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
