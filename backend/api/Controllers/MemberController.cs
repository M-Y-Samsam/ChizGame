using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class MemberController(IMemberRepository memberRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<MemberDto>>> GetAllGamers(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized("You are not login. Please login again");

        List<Gamer>? gamers = await memberRepository.GetGamersAsync(cancellationToken);

        if (gamers is null)
            return NoContent();

        List<MemberDto>? members = [];

        foreach (Gamer gamer in gamers)
        {
            MemberDto member = Mappers.ConvertGamerToMemberDto(gamer);

            members.Add(member);
        }

        return members;
    }

    [HttpGet("get-by-name/{userInput}")]
    public async Task<ActionResult<MemberDto>> GetGamerByName(string userInput, CancellationToken cancellationToken)
    {
        MemberDto? memberDto = await memberRepository.GetGamerByName(userInput, cancellationToken);

        if (memberDto is null) return BadRequest("gamer not found!");

        return Ok(memberDto);
    }
}