using Dapper;
using SignalrApp.Communications.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SignalrApp.Communications
{
    //use interfaces and DI (SOL"ID")
    //I have just used a class with object creation for simplicity and since I dont have an idea about the existing architecture
    public class MessageRepository
    {
        //obviously use your current config value here
        private const string _connectionString = "Data Source=SRILANKA\\DANNYMSSQL;Initial Catalog=FileDownloader;Persist Security Info=True;User ID=sa;Password=abc123";

        public async Task InsertMessage(CommunicationMessage message)
        {
            string insertQuery = "INSERT INTO dbo.Messages (UserId, OrganizationId, Title, Message, CommunicationType, IsPersist, IsRead) Values (@UserId, @OrganizationId, @Title, @Message, @CommunicationType, @IsPersist, @IsRead);";
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(insertQuery, message);
            }
        }
    }
}