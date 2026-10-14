using api.Extensions.Validations;
using api.Helpers;
using api.Models.Helpers;
using Image_Processing_WwwRoot.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers
{
    public class GameController(IGameRepository gameRepository) : BaseApiController
    {
        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost("add-game")]
        public async Task<ActionResult<Games>> AddGame(GameInfoDto userIn, CancellationToken cancellationToken)
        {
            Games? games = await gameRepository.AddGameAsync(userIn, cancellationToken);

            if (games is null) return BadRequest("Game could not be created.");

            return Ok(games);
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPut("upload-game-photo/{gameId}")]
        public async Task<ActionResult<Response>> UploadGamePhoto([AllowedFileExtensions, FileSize(50_000, 4_000_000)] IFormFile formFile, string gameId, PhotoType photoType, CancellationToken cancellationToken)
        {
            if (formFile is null) return BadRequest("no file selected for this request.");

            GamePhoto? gamePhoto = await gameRepository.UploadGamePhotoAsync(formFile, photoType, gameId, cancellationToken);

            return gamePhoto is null
                ? BadRequest("Add photo failed.")
                : Ok(new Response(Message: "Photo upload successfully."));
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<Games>>> GetAll([FromQuery] GameParams gameParams, CancellationToken cancellationToken)
        {
            PagedList<Games> pagedGames = await gameRepository.GetAllGamesAsync(gameParams, cancellationToken);

            if (pagedGames.Count == 0)
                return NoContent();

            PaginationHeader paginationHeader = new(
                CurrentPage: pagedGames.CurrentPage,
                ItemsPerPage: pagedGames.PageSize,
                TotslItems: pagedGames.TotalItems,
                TotalPages: pagedGames.TotalPages
            );

            Response.AddPaginationHeader(paginationHeader);

            List<Games>? games = [];

            foreach (Games game in pagedGames)
            {
                games.Add(game);
            }

            return games;
        }
    }
}