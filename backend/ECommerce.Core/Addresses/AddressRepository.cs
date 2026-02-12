using ECommerce.Core.Misc;
using ECommerce.Core.Models;
using System.Data;

namespace ECommerce.Core.Addresses;

public class AddressRepository : BaseRepository, IAddressRepository
{
    public AddressRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<Address?> GetByIdAsync(Guid id)
    {
        var sql = "SELECT * FROM addresses WHERE id = @Id";
        return await QueryFirstOrDefaultAsync(sql, MapAddress, new { Id = id });
    }

    public async Task<List<Address>> GetUserAddressesAsync(Guid userId)
    {
        var sql = "SELECT * FROM addresses WHERE user_id = @UserId ORDER BY is_default DESC, created_at DESC";
        return await QueryAsync(sql, MapAddress, new { UserId = userId });
    }

    public async Task<Address> CreateAsync(Address address)
    {
        var sql = @"
            INSERT INTO addresses (id, user_id, full_name, phone_number, address_line1, address_line2, 
                city, state, postal_code, country, is_default, address_type, created_at, updated_at)
            VALUES (@Id, @UserId, @FullName, @PhoneNumber, @AddressLine1, @AddressLine2, 
                @City, @State, @PostalCode, @Country, @IsDefault, @AddressType, @CreatedAt, @UpdatedAt)
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapAddress, new
        {
            address.Id,
            address.UserId,
            address.FullName,
            address.PhoneNumber,
            address.AddressLine1,
            address.AddressLine2,
            address.City,
            address.State,
            address.PostalCode,
            address.Country,
            address.IsDefault,
            address.AddressType,
            address.CreatedAt,
            address.UpdatedAt
        }) ?? throw new Exception("Failed to create address");
    }

    public async Task<Address> UpdateAsync(Address address)
    {
        var sql = @"
            UPDATE addresses 
            SET full_name = @FullName, phone_number = @PhoneNumber, address_line1 = @AddressLine1,
                address_line2 = @AddressLine2, city = @City, state = @State, postal_code = @PostalCode,
                country = @Country, is_default = @IsDefault, address_type = @AddressType, updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapAddress, new
        {
            address.Id,
            address.FullName,
            address.PhoneNumber,
            address.AddressLine1,
            address.AddressLine2,
            address.City,
            address.State,
            address.PostalCode,
            address.Country,
            address.IsDefault,
            address.AddressType,
            address.UpdatedAt
        }) ?? throw new Exception("Failed to update address");
    }

    public async Task DeleteAsync(Guid id)
    {
        var sql = "DELETE FROM addresses WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }

    public async Task SetDefaultAsync(Guid addressId, Guid userId)
    {
        var connection = await GetConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        
        try
        {
            var updateAllSql = "UPDATE addresses SET is_default = false WHERE user_id = @UserId";
            await using (var updateAllCmd = connection.CreateCommand())
            {
                updateAllCmd.CommandText = updateAllSql;
                updateAllCmd.Transaction = transaction as Npgsql.NpgsqlTransaction;
                var param = updateAllCmd.CreateParameter();
                param.ParameterName = "@UserId";
                param.Value = userId;
                updateAllCmd.Parameters.Add(param);
                await updateAllCmd.ExecuteNonQueryAsync();
            }

            var setDefaultSql = "UPDATE addresses SET is_default = true WHERE id = @AddressId";
            await using (var setDefaultCmd = connection.CreateCommand())
            {
                setDefaultCmd.CommandText = setDefaultSql;
                setDefaultCmd.Transaction = transaction as Npgsql.NpgsqlTransaction;
                var param = setDefaultCmd.CreateParameter();
                param.ParameterName = "@AddressId";
                param.Value = addressId;
                setDefaultCmd.Parameters.Add(param);
                await setDefaultCmd.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private Address MapAddress(IDataReader reader)
    {
        return new Address
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
            FullName = reader.GetString(reader.GetOrdinal("full_name")),
            PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number")),
            AddressLine1 = reader.GetString(reader.GetOrdinal("address_line1")),
            AddressLine2 = reader.IsDBNull(reader.GetOrdinal("address_line2")) ? null : reader.GetString(reader.GetOrdinal("address_line2")),
            City = reader.GetString(reader.GetOrdinal("city")),
            State = reader.GetString(reader.GetOrdinal("state")),
            PostalCode = reader.GetString(reader.GetOrdinal("postal_code")),
            Country = reader.GetString(reader.GetOrdinal("country")),
            IsDefault = reader.GetBoolean(reader.GetOrdinal("is_default")),
            AddressType = reader.GetString(reader.GetOrdinal("address_type")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
        };
    }
}
