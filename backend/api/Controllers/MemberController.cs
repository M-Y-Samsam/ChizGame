namespace api.Controllers;

public class MemberController(IMemberRepository memberRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<MemberDto>>> GetAllGamers(CancellationToken cancellationToken)
    {
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

    [HttpGet("get-gamer-by-name/{userInput}")]
    public async Task<ActionResult<MemberDto>> GetGamerByName(string userInput, CancellationToken cancellationToken)
    {
        MemberDto? memberDto = await memberRepository.GetGamerByName(userInput, cancellationToken);

        if (memberDto is null) return BadRequest("gamer not foun!");

        return Ok(memberDto);
    }
}