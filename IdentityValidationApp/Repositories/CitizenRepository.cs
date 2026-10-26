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
                INSERT INTO Citizens (PersonalId, Email, PhoneNumber) 
                VALUES (@PersonalId, @Email, @PhoneNumber);";

            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@PersonalId", citizen.PersonalId.Trim());
            cmd.Parameters.AddWithValue("@Email", citizen.Email.Trim());
            cmd.Parameters.AddWithValue("@PhoneNumber", citizen.PhoneNumber.Trim());

            int affectedRows = await cmd.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }
    }
}