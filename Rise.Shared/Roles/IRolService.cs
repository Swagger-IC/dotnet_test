
namespace Rise.Shared.Roles
{
    public interface IRolService
    {
        Task<IEnumerable<RolDto>> GetRolesAsync();
    }
}
