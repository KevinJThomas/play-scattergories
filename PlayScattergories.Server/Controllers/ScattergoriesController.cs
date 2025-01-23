using Microsoft.AspNetCore.Mvc;
using PlayScattergories.Server.Services;

namespace PlayScattergories.Server.Controllers
{
    [ApiController]
    [Route("scattergories")]
    public class ScattergoriesController : Controller
    {
        public ScattergoriesController()
        {

        }

        [HttpGet]
        [Route("lobby")]
        public IActionResult GetLobby(string lobbyId)
        {
            return Json(LobbyService.GetLobbyByLobbyId(lobbyId));
        }

        [HttpGet]
        [Route("player")]
        public IActionResult GetPlayer(string playerId)
        {
            return Json(LobbyService.GetPlayerByPlayerId(playerId));
        }
    }
}
