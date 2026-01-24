using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace FinFlowAPI.Services
{
    public class SqlHandlerService
    {
        private readonly IConfiguration _configuration;
        private IDbConnection _connection;

        public SqlHandlerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "" ;
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string is not configured.", nameof(connectionString));

            return connectionString;
        }

        public void OpenConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new SqlConnection(GetConnectionString());
                _connection.Open();
            }
        }
        public async Task<T> ExecuteAsync<T>(string proc, DynamicParameters param)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(GetConnectionString());
                var response = await connection.QueryAsync<T>(proc, param, commandType: CommandType.StoredProcedure, commandTimeout: 120);
                return response.SingleOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<T>> ExecuteAsyncList<T>(string proc, DynamicParameters param)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(GetConnectionString());
                var response = await connection.QueryAsync<T>(proc, param, commandType: CommandType.StoredProcedure, commandTimeout: 120);
                return response.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}