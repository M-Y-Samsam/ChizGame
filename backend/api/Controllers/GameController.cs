using api.Extensions.Validations;
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
        public async Task<ActionResult<Response>> UploadGamePhoto([AllowedFileExtensions, FileSize(50_000, 4_000_000)]IFormFile formFile, string gameId, PhotoType photoType, CancellationToken cancellationToken)
        {
            if (formFile is null) return BadRequest("no file selected for this request.");

            GamePhoto? gamePhoto = await gameRepository.UploadGamePhotoAsync(formFile, photoType, gameId, cancellationToken);

            return gamePhoto is null
                ? BadRequest("Add photo failed.")
                : Ok (new Response (Message: "Photo upload successfully."));
        }
    }
}