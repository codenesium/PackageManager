using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codenesium.PackageManagement.DeploymentService
{
    public class SqlLiteManager
    {
        public const string UPGRADE_STATUS_CREATED = "CREATED";
        public const string UPGRADE_STATUS_RUNNING = "RUNNING";
        public const string UPGRADE_STATUS_COMPLETE = "COMPLETE";
        public const string UPGRADE_STATUS_FAILED = "FAILED";

        private static SqlLiteManager _sqlLightManager;
        private static string _filename;

        public static SqlLiteManager GetInstance()
        {
            if (_sqlLightManager == null)
            {
                _sqlLightManager = new SqlLiteManager();
            }
            return _sqlLightManager;
        }

        public string ConnectionString
        {
            get
            {
                return String.Format("Data Source={0};Version=3;", _filename);
            }
        }

        public void Migrate(string filename)
        {
            _filename = filename;
            if (!File.Exists(filename))
            {
                _filename = filename;
                if (!File.Exists(filename))
                {
                    CreateDatabase(filename);
                    InitialConfig();
                }
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                try
                {
                    if (GetSystemVersion() == 0)
                    {
                        Migration1();
                        UpdateSystemVersion(1);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void UpdateSystemVersion(int version)
        {
            string updateConfig = String.Format(@"update systemConfig
            set systemVersion = {0}", version.ToString());
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                UpdateQuery(updateConfig, connection);
            }
        }

        private void CreateDatabase(string filename)
        {
            SQLiteConnection.CreateFile(filename);
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                connection.Close();
            }
        }

        public void InitialConfig()
        {
            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string createConfigTable = @"create table systemConfig
                (
                    id INTEGER PRIMARY KEY ASC,
                    systemVersion int
                 )";
                    InsertQuery(createConfigTable, connection);

                    string insertConfig = String.Format(@"insert into systemConfig
            (
            systemVersion
            )
            values
            (
            '{0}'
            )",
                "0");
                    InsertQuery(insertConfig, connection);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int InsertQuery(string query, SQLiteConnection connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
            string idQuery = String.Format(@"SELECT last_insert_rowid();");
            int id = 0;
            using (SQLiteCommand idCommand = new SQLiteCommand(idQuery, connection))
            {
                id = Convert.ToInt32(idCommand.ExecuteScalar());
            }
            return id;
        }

        private void UpdateQuery(string query, SQLiteConnection connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void UpdateStatus(string status, string packageName)
        {
            string query = String.Format(
                @"UPDATE upgrade set
                status='{0}'
                where
                packageName='{1}'",
                status,
                packageName);

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                UpdateQuery(query, connection);
            }
        }

        public void InsertPackage(string packageName)
        {
            string query = String.Format(
                @"INSERT INTO UPGRADE
                (
                packageName
                )
                values
                (
                '{0}'
                )",
                packageName);

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                InsertQuery(query, connection);
            }
        }

        public int GetSystemVersion()
        {
            string query = String.Format(@"select systemVersion from systemConfig");

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return Convert.ToInt32(reader["systemVersion"].ToString());
                        }
                    }
                }
            }
            throw new InvalidOperationException();
        }

        public bool GetPackageExists(string packageName)
        {
            string query = String.Format(@"select
                                            id
                                            from
                                            upgrade
                                            where
                                            packageName='{0}'
                                            ", packageName);

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void Migration1()
        {
            string createGenerationOutputTable = @"create table upgrade
                (
                    id INTEGER PRIMARY KEY ASC,
                    packageName varchar(256),
                    status varchar(32),
                    projectGuid varchar(36)

                 )";
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                InsertQuery(createGenerationOutputTable, connection);
            }
        }
    }
}