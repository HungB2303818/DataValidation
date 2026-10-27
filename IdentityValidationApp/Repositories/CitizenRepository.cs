using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using IdentityValidationApp.Models;

namespace IdentityValidationApp.Repositories
{
    public class CitizenRepository
    {
        private readonly string _connectionString;

        public CitizenRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> InsertCitizenAsync(CitizenModel citizen)
        {
            const string sql = @"
                INSERT INTO Citizens (FullName, PersonalId, Email, PhoneNumber, StartDate) 
                VALUES (@FullName, @PersonalId, @Email, @PhoneNumber, @StartDate);";

            await using var conn = new MySqlConnection(_connectionString);
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FullName", citizen.FullName.Trim());
            cmd.Parameters.AddWithValue("@PersonalId", citizen.PersonalId.Trim());
            cmd.Parameters.AddWithValue("@Email", citizen.Email.Trim());
            cmd.Parameters.AddWithValue("@PhoneNumber", citizen.PhoneNumber.Trim());
            cmd.Parameters.AddWithValue("@StartDate", citizen.StartDate.Trim());

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<List<CitizenModel>> GetAllCitizensAsync()
        {
            var list = new List<CitizenModel>();
            const string sql = @"
                SELECT Id, FullName, PersonalId, Email, PhoneNumber, DATE_FORMAT(StartDate, '%Y-%m-%d') as StartDate 
                FROM Citizens 
                ORDER BY Id DESC;";

            await using var conn = new MySqlConnection(_connectionString);
            await using var cmd = new MySqlCommand(sql, conn);
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new CitizenModel
                {
                    Id = reader.GetInt32("Id"),
                    FullName = reader.GetString("FullName"),
                    PersonalId = reader.GetString("PersonalId"),
                    Email = reader.GetString("Email"),
                    PhoneNumber = reader.GetString("PhoneNumber"),
                    StartDate = reader.GetString("StartDate")
                });
            }
            return list;
        }

        public async Task<bool> UpdateCitizenAsync(CitizenModel citizen)
        {
            const string sql = @"
                UPDATE Citizens 
                SET FullName = @FullName, 
                    PersonalId = @PersonalId, 
                    Email = @Email, 
                    PhoneNumber = @PhoneNumber, 
                    StartDate = @StartDate 
                WHERE Id = @Id;";

            await using var conn = new MySqlConnection(_connectionString);
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", citizen.Id);
            cmd.Parameters.AddWithValue("@FullName", citizen.FullName.Trim());
            cmd.Parameters.AddWithValue("@PersonalId", citizen.PersonalId.Trim());
            cmd.Parameters.AddWithValue("@Email", citizen.Email.Trim());
            cmd.Parameters.AddWithValue("@PhoneNumber", citizen.PhoneNumber.Trim());
            cmd.Parameters.AddWithValue("@StartDate", citizen.StartDate.Trim());

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteCitizenAsync(int id)
        {
            const string sql = "DELETE FROM Citizens WHERE Id = @Id;";

            await using var conn = new MySqlConnection(_connectionString);
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}