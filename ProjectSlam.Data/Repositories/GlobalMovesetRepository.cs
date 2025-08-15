using ProjectSlam.Data.Interfaces;
using ProjectSlam.Data.Models;

namespace ProjectSlam.Data.Repositories;

public class GlobalMovesetRepository : BaseRepository, IGlobalMovesetRepository
{

    public GlobalMovesetRepository(DbConfig dbConfig) : base(dbConfig)
    {  
    }

    public async Task<IEnumerable<GlobalMoveset>> GetAllAsync()
    {
        const string sql = @"SELECT MoveId, MoveCategory, MoveName, DamageAmount, IsFinisher, IsSignature
                               FROM GlobalMoveset ORDER BY MoveCategory, MoveName";
        return await QueryAsync<GlobalMoveset>(sql);
    }

    public async Task<IEnumerable<GlobalMoveset>> GetByMoveCategoryAsync(string moveCategory)
    {
        const string sql = @"SELECT MoveId, MoveCategory, MoveName, DamageAmount, IsFinisher, IsSignature
                            FROM GlobalMoveset
                            WHERE MoveCategory = @moveCategory
                        ORDER BY MoveName;";
        return await QueryAsync<GlobalMoveset>(sql, new { moveCategory });
    }

    public async Task<GlobalMoveset?> GetByIdAsync(int id)
    {
        const string sql = @"SELECT MoveId, MoveCategory, MoveName, DamageAmount, IsFinisher, IsSignature
                               FROM GlobalMoveset WHERE MoveId = @id;";
        return await QuerySingleOrDefaultAsync<GlobalMoveset>(sql, new { id });
    }

    public async Task<int> AddAsync(GlobalMoveset entity)
    {
        const string sql = @"INSERT INTO GlobalMoveset (MoveCategory, MoveName, DamageAmount, IsFinisher, IsSignature)
                             VALUES (@MoveCategory, @MoveName, @DamageAmount, @IsFinisher, @IsSignature);
                             RETURNING MoveId;";

        return await ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<bool> UpdateAsync(GlobalMoveset entity)
    {
        const string sql = @"UPDATE GlobalMoveset
                                SET MoveCategory = @MoveCategory,
                                    MoveName = @MoveName,
                                    DamageAmount = @DamageAmount,
                                    IsFinisher = @IsFinisher,
                                    IsSignature = @IsSignature
                              WHERE MoveId = @MoveId;";
        var rowsAffected = await ExecuteAsync(sql, entity);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = @"DELETE FROM GlobalMoveset 
                              WHERE MoveId = @MoveId;";
        var rowsAffected = await ExecuteAsync(sql, new { MoveId = id });
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<GlobalMoveset>> SearchAsync(string searchTerm)
    {
        const string sql = @"SELECT MoveId, MoveCategory, MoveName, DamageAmount, IsFinisher, IsSignature
                               FROM GlobalMoveset 
                              WHERE MoveCategory LIKE @SearchTerm OR MoveName LIKE @SearchTerm
                           ORDER BY MoveCategory, MoveName;";

        return await QueryAsync<GlobalMoveset>(sql, new { SearchTerm = $"%{searchTerm}%" });
    }
}
