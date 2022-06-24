using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace _07.PrintAllMinionNames
{
    class StartUp
    {
        static void Main(string[] args)
        {
            const string _connectionString =
                @"Server=DESKTOP-4A8B05S;Database=MinionsDB;Integrated Security=True;";

            using SqlConnection sqlConnection =
                new SqlConnection(_connectionString);
            sqlConnection.Open();

            string[] minions = GetAllMinionNames(sqlConnection);

            for (int i = 0; i < minions.Length / 2; i++)
            {
                Console.WriteLine(minions[i]);
                Console.WriteLine(minions[(minions.Length - 1) - i]);
            }

            if(minions.Length % 2 == 1)
            {
                Console.WriteLine(minions[minions.Length / 2]);
            }

            sqlConnection.Close();
        }

        static string[] GetAllMinionNames(SqlConnection sqlConnection)
        {
            List<string> minions = new List<string>();

            string minionsQuery = @"SELECT Name FROM Minions";

            SqlCommand minionsCommand = new SqlCommand(minionsQuery, sqlConnection);

            using SqlDataReader minionsReader = minionsCommand.ExecuteReader();

            while (minionsReader.Read())
            {
                minions.Add((string)minionsReader["Name"]);
            }

            return minions.ToArray();
        }
    }
}
