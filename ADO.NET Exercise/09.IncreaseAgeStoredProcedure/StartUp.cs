using System;
using System.Data.SqlClient;

namespace _09.IncreaseAgeStoredProcedure
{
    class StartUp
    {
        static void Main(string[] args)
        {
            const string _connectionString =
                @"Server=DESKTOP-4A8B05S;Database=MinionsDB;Integrated Security=True;";

            int minionId = int.Parse(Console.ReadLine());

            using SqlConnection sqlConnection =
                new SqlConnection(_connectionString);
            sqlConnection.Open();

            ExecuteUpdateProcedure(sqlConnection, minionId);

            string result = UpdatedMinionInfo(sqlConnection, minionId);

            Console.WriteLine(result);

            sqlConnection.Close();
        }

        /// <summary>
        /// Using an open Sql Connection executes usp_GetOlder procedure for a given minion by Id.
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <param name="minionId">Minion Id</param>
        static void ExecuteUpdateProcedure(SqlConnection sqlConnection, int minionId)
        {
            string query = @"EXEC usp_GetOlder @minionId";

            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.Parameters.AddWithValue("@minionId", minionId);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Using an open Sql Connection retrieves info for the updated minion from the database.
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <param name="minionId">Minion Id</param>
        /// <returns></returns>
        static string UpdatedMinionInfo(SqlConnection sqlConnection, int minionId)
        {
            string output = "";

            string query = @"SELECT Name, Age FROM Minions WHERE Id = @Id";

            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.Parameters.AddWithValue("@Id", minionId);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                output = $"{reader["Name"]} - {reader["Age"]} years old";
            }

            return output;
        }
    }
}
