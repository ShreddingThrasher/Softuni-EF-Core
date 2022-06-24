using System;
using System.Data.SqlClient;

namespace _04.AddMinion
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

            string[] minionInput = Console.ReadLine().Split(' ');
            string[] villainInput = Console.ReadLine().Split(' ');

            string minionName = minionInput[1];
            int minionAge = int.Parse(minionInput[2]);
            string minionTown = minionInput[3];
            string villainName = villainInput[1];

            string result = AddRecordsToDatabase(sqlConnection, minionName, minionAge, minionTown, villainName);

            Console.WriteLine(result);

            sqlConnection.Close();
        }

        /// <summary>
        /// Using an open Sql Connection checks and add all the needed records to the database
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <param name="minionName">Minion name</param>
        /// <param name="minionAge">Minion age</param>
        /// <param name="minionTown">Minion town</param>
        /// <param name="villainName">Villain name</param>
        /// <returns></returns>
        private static string AddRecordsToDatabase(SqlConnection sqlConnection, string minionName,
            int minionAge, string minionTown, string villainName)
        {
            string checkTownQuery = @"SELECT Id FROM Towns WHERE Name = @townName";

            SqlCommand checkTownCommand = new SqlCommand(checkTownQuery, sqlConnection);
            checkTownCommand.Parameters.AddWithValue("@townName", minionTown);

            if(checkTownCommand.ExecuteScalar() == null)
            {
                AddTownToDatabase(sqlConnection, minionTown);
            }

            string checkVillainQuery = @"SELECT Id FROM Villains WHERE Name = @Name";

            SqlCommand checkVillainCommand = new SqlCommand(checkVillainQuery, sqlConnection);
            checkVillainCommand.Parameters.AddWithValue("@Name", villainName);

            if(checkVillainCommand.ExecuteScalar() == null)
            {
                AddVillainToDatabase(sqlConnection, villainName);
            }

            AddMinionToDatabase(sqlConnection, minionName, minionAge, minionTown);

            int minionId = GetMinionIdFromDatabase(sqlConnection, minionName);
            int villainId = GetVillainIdFromTheDatabase(sqlConnection, villainName);

            AddMinionsVillainsToDatabase(sqlConnection, minionId, villainId);

            return $"Successfully added {minionName} to be minion of {villainName}.";
        }

        /// <summary>
        /// Using an open Sql Connection adds a town to the database.
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <param name="townName">Town name</param>
        /// <returns></returns>
        private static void AddTownToDatabase(SqlConnection sqlConnection, string townName)
        {
            string insertTownQuery = @"INSERT INTO Towns (Name) VALUES (@townName)";

            SqlCommand insertTownCommand = new SqlCommand(insertTownQuery, sqlConnection);
            insertTownCommand.Parameters.AddWithValue("@townName", townName);

            insertTownCommand.ExecuteNonQuery();

            Console.WriteLine($"Town {townName} was added to the database.");
        }

        /// <summary>
        /// Using an open Sql Connection adds a villain to the database.
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <param name="villainName">Villain name</param>
        private static void AddVillainToDatabase(SqlConnection sqlConnection, string villainName)
        {
            string addVillainQuery = @"INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";

            SqlCommand insertVillainCommand = new SqlCommand(addVillainQuery, sqlConnection);
            insertVillainCommand.Parameters.AddWithValue("@villainName", villainName);

            insertVillainCommand.ExecuteNonQuery();

            Console.WriteLine($"Villain {villainName} was added to the database.");
        }

        /// <summary>
        /// Using an open Sql Connection adds a minion to the database
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="minionName"></param>
        /// <param name="minionAge"></param>
        /// <param name="minionTown"></param>
        private static void AddMinionToDatabase(SqlConnection sqlConnection, string minionName,
            int minionAge, string minionTown)
        {
            string addMinionQuery = @"INSERT INTO Minions (Name, Age, TownId) VALUES (@nam, @age, @townId)";

            int townId = GetTownIdFromDatabase(sqlConnection, minionTown);

            SqlCommand addMinionCommand = new SqlCommand(addMinionQuery, sqlConnection);
            addMinionCommand.Parameters.AddWithValue("@nam", minionName);
            addMinionCommand.Parameters.AddWithValue("@age", minionAge);
            addMinionCommand.Parameters.AddWithValue("townId", townId);

            addMinionCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Using an open Sql Connection returns a town Id from the database
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <param name="townName">Town name</param>
        /// <returns></returns>
        private static int GetTownIdFromDatabase(SqlConnection sqlConnection, string townName)
        {
            string townIdQuery = @"SELECT Id FROM Towns WHERE Name = @townName";

            SqlCommand getTownIdCommand = new SqlCommand(townIdQuery, sqlConnection);
            getTownIdCommand.Parameters.AddWithValue("@townName", townName);

            return (int)getTownIdCommand.ExecuteScalar();
        }

        /// <summary>
        /// Using an open Sql Connection returns a minion Id from the database
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <param name="minionName">Minion name</param>
        /// <returns></returns>
        private static int GetMinionIdFromDatabase(SqlConnection sqlConnection, string minionName)
        {
            string minionIdQuery = @"SELECT Id FROM Minions WHERE Name = @Name";

            SqlCommand minionIdCommand = new SqlCommand(minionIdQuery, sqlConnection);
            minionIdCommand.Parameters.AddWithValue("@Name", minionName);

            return (int)minionIdCommand.ExecuteScalar();
        }

        /// <summary>
        /// Using an open SqlConnection returns a villain Id from the database
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <param name="villainName">Villain name</param>
        /// <returns></returns>
        private static int GetVillainIdFromTheDatabase(SqlConnection sqlConnection, string villainName)
        {
            string villainIdQuery = @"SELECT Id FROM Villains WHERE Name = @Name";

            SqlCommand villainIdCommand = new SqlCommand(villainIdQuery, sqlConnection);
            villainIdCommand.Parameters.AddWithValue("@Name", villainName);

            return (int)villainIdCommand.ExecuteScalar();
        }

        /// <summary>
        /// Using an open Sql Connection adds a record in the MinionsVillainsTable
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <param name="minionId">Minion Id</param>
        /// <param name="villainId">Villain Id</param>
        private static void AddMinionsVillainsToDatabase(SqlConnection sqlConnection, int minionId, int villainId)
        {
            string query = @"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)";

            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.Parameters.AddWithValue("@minionId", minionId);
            command.Parameters.AddWithValue("@villainId", villainId);
        }
    }
}
