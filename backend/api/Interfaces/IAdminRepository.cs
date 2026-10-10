namespace api.Interfaces;

public interface IAdminRepository
{
    public Task<IEnumerable<UserWithRoleDto>> GetUsersWithRolesAsync();    
    public Task<DeleteResult?> DeletUserAsync(string targetUserName, CancellationToken cancellationToken);
}