using api.Helpers;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class MemberController(IMemberRepository memberRepository) : BaseApiController

{
    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllGamers([FromQuery] MemberParams memberParams, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        memberParams.UserId = userId;

        if (userId is null)
            return Unauthorized("You are not login. Please login again");

        PagedList<Gamer> PagedGamers = await memberRepository.GetGamersAsync(memberParams, cancellationToken);

        if (PagedGamers.Count == 0)
            return NoContent();

        PaginationHeader paginationHeader = new (
            CurrentPage: PagedGamers.CurrentPage,
            ItemsPerPage: PagedGamers.PageSize,
            TotalPages: PagedGamers.TotalPages,
            TotslItems: PagedGamers.TotalItems
        );

        Response.AddPaginationHeader(paginationHeader);

        List<MemberDto>? members = [];

        foreach (Gamer gamer in PagedGamers)
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