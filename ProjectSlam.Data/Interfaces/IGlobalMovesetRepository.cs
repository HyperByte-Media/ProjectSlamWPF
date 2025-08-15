using ProjectSlam.Data.Models;

namespace ProjectSlam.Data.Interfaces;

public interface IGlobalMovesetRepository : IRepository<GlobalMoveset>
{
    Task<IEnumerable<GlobalMoveset>> GetByMoveCategoryAsync(string moveCategory);
    Task<IEnumerable<GlobalMoveset>> SearchAsync(string searchTerm);
}
