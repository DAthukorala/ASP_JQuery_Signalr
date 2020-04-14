using Dapper;
using SignalrApp.Communications.Models;
using System.Data.SqlClient;

namespace SignalrApp.Communications
{
    //just add a similar method to your existing repository pattern
    public class MessageRepository
    {
        //obviously use your current config value here
        private const string _connectionString = "Data Source=SRILANKA\\DANNYMSSQL;Initial Catalog=SignalrComms;Persist Security Info=True;User ID=sa;Password=abc123";

        public void InsertMessage(CommunicationMessage message)
        {
            string insertQuery = "INSERT INTO dbo.Messages (UserId, OrganizationId, Title, Message, CommunicationType, IsPersist, IsRead) Values (@UserId, @OrganizationId, @Title, @Message, @CommunicationType, @IsPersist, @IsRead);";
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.ExecuteAsync(insertQuery, message);
            }
        }
    }
}