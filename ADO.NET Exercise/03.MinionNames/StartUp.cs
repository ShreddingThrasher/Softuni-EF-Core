using System;
using System.Data.SqlClient;
using System.Text;

namespace _03.MinionNames
{
    class StartUp
    {
        static void Main(string[] args)
        {
            const string _connectionString = 
                @"Server=.;Database=MinionsDB;Integrated Security=True;";

            using SqlConnection sqlConnection =
                new SqlConnection(_connectionString);
            sqlConnection.Open();

            int villainId = int.Parse(Console.ReadLine());

            string result = GetMinionNames(sqlConnection, villainId);
            Console.WriteLine(result);

            sqlConnection.Close();
        }

        /// <summary>
        /// Gets all minions for a given villain
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <param name="villainId">Id of villain</param>
        /// <returns></returns>
        private static string GetMinionNames(SqlConnection sqlConnection, int villainId)
        {
            StringBuilder output = new StringBuilder();
            string villainNameQuery = @"SELECT Name FROM Villains WHERE Id = @Id";

            SqlCommand getVillainNameCommand = new SqlCommand(villainNameQuery, sqlConnection);
            getVillainNameCommand.Parameters.AddWithValue("@Id", villainId);

            string villainName = (string)getVillainNameCommand.ExecuteScalar();

            if(villainName == null)
            {
                return $"No villain with ID {villainId} exists in the database.";
            }

            string minionNamesQuery = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                        FROM MinionsVillains AS mv
                                        JOIN Minions As m ON mv.MinionId = m.Id
                                        WHERE mv.VillainId = @Id
                                        ORDER BY m.Name";

            SqlCommand getMinionNamesCommand = new SqlCommand(minionNamesQuery, sqlConnection);
            getMinionNamesCommand.Parameters.AddWithValue(@"Id", villainId);

            using SqlDataReader minionsReader = getMinionNamesCommand.ExecuteReader();

            output.AppendLine($"Villain: {villainName}");

            if (minionsReader.HasRows)
            {
                while (minionsReader.Read())
                {
                    output.AppendLine($"{minionsReader["RowNum"]}. {minionsReader["Name"]} {minionsReader["Age"]}");
                }
            }
            else
            {
                output.AppendLine("(no minions)");
            }

            return output.ToString().TrimEnd();
        }
    }
}
