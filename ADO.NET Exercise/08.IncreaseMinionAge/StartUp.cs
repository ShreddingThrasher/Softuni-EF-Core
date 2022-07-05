using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace _08.IncreaseMinionAge
{
    class StartUp
    {
        static void Main(string[] args)
        {
            const string _connectionString =
                @"Server=.;Database=MinionsDB;Integrated Security=True;";

            int[] minionIds = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            using SqlConnection sqlConnection =
                new SqlConnection(_connectionString);
            sqlConnection.Open();

            for (int i = 0; i < minionIds.Length; i++)
            {
                UpdateMinion(sqlConnection, minionIds[i]);
            }

            string result = MinionsNamesAndAge(sqlConnection);

            Console.WriteLine(result);

            sqlConnection.Close();
        }

        /// <summary>
        /// Usng an open Sql Connection updates a minion record in the database.
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <param name="minionId">Minion Id</param>
        static void UpdateMinion(SqlConnection sqlConnection, int minionId)
        {
            string updateQuery = @"UPDATE Minions
                                   SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                   WHERE Id = @Id";

            SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection);
            updateCommand.Parameters.AddWithValue("@Id", minionId);

            updateCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Using an open Sql Connection retrieves name and age for all minions from the database.
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <returns></returns>
        static string MinionsNamesAndAge(SqlConnection sqlConnection)
        {
            StringBuilder output = new StringBuilder();

            string query = @"SELECT Name, Age FROM Minions";

            SqlCommand command = new SqlCommand(query, sqlConnection);

            using SqlDataReader sqlDataReader = command.ExecuteReader();

            while (sqlDataReader.Read())
            {
                output.AppendLine($"{sqlDataReader["Name"]} {sqlDataReader["Age"]}");
            }

            return output.ToString().TrimEnd();
        }
    }
}
