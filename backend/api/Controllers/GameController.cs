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
        [HttpPut("upload-game-cover/{gameId}")]
        public async Task<ActionResult<Response>> UploadGameCover([AllowedFileExtensions] IFormFile formFile, string gameId, CancellationToken cancellationToken)
        {
            if (formFile is null) return BadRequest("no file selected for this request.");

            GamePhoto? gamePhoto = await gameRepository.UploadGamePhotoAsync(formFile, PhotoType.Cover, gameId, cancellationToken);

            return gamePhoto is null
                ? BadRequest("Add photo failed.")
                : Ok(new Response(Message: "Photo upload successfully."));
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPut("upload-game-banner/{gameId}")]
        public async Task<ActionResult<Response>> UploadGameBanner([AllowedFileExtensions] IFormFile formFile, string gameId, CancellationToken cancellationToken)
        {
            if (formFile is null) return BadRequest("no file selected for this request.");

            GamePhoto? gamePhoto = await gameRepository.UploadGamePhotoAsync(formFile, PhotoType.Banner, gameId, cancellationToken);

            return gamePhoto is null
                ? BadRequest("Add photo failed.")
                : Ok(new Response(Message: "Photo upload successfully."));
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPut("upload-game-logo/{gameId}")]
        public async Task<ActionResult<Response>> UploadGameLogo([AllowedFileExtensions] IFormFile formFile, string gameId, CancellationToken cancellationToken)
        {
            if (formFile is null) return BadRequest("no file selected for this request.");

            GamePhoto? gamePhoto = await gameRepository.UploadGamePhotoAsync(formFile, PhotoType.Logo, gameId, cancellationToken);

            return gamePhoto is null
                ? BadRequest("Add photo failed.")
                : Ok(new Response(Message: "Photo upload successfully."));
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPut("upload-game-screenshots/{gameId}")]
        public async Task<ActionResult<Response>> UploadGameScreenshots(
        List<IFormFile> formFiles, string gameId, CancellationToken cancellationToken)
        {
            if (formFiles is null || formFiles.Count == 0 ) return BadRequest("no file selected for this request.");

            if (formFiles.Count > 10) return BadRequest("you can upload at most 10 screenshots at once.");

            List<GamePhoto>? gamePhoto = await gameRepository.UploadScreenshotsAsync(formFiles, PhotoType.ScreenShots, gameId, cancellationToken);

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
                TotalItems: pagedGames.TotalItems,
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