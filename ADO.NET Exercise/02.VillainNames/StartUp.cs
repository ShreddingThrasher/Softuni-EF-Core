using System;
using System.Data.SqlClient;
using System.Text;

namespace _02.VillainNames
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            using SqlConnection sqlConnection =
                new SqlConnection(Config.ConnectionString);
            sqlConnection.Open();

            string result = GetVillainNamesWithMinionsCount(sqlConnection);

            Console.WriteLine(result);

            sqlConnection.Close();
        }

        /// <summary>
        /// Takes an open Sql Connection, connects to the database and returns all vilains
        /// with their minions count.
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <returns></returns>
        private static string GetVillainNamesWithMinionsCount(SqlConnection sqlConnection)
        {
            StringBuilder output = new StringBuilder();

            string query = @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                             FROM Villains AS v 
                             JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                             GROUP BY v.Id, v.Name 
                             HAVING COUNT(mv.VillainId) > 3 
                             ORDER BY COUNT(mv.VillainId)";

            SqlCommand command = new SqlCommand(query, sqlConnection);

            using SqlDataReader sqlDataReader = command.ExecuteReader();

            while (sqlDataReader.Read())
            {
                output.AppendLine($"{sqlDataReader["Name"]} {sqlDataReader["MinionsCount"]}");
            }

            return output.ToString().TrimEnd();
        }
    }
}
