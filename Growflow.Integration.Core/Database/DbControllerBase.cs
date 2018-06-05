using Growflo.Integration.Core.Entities.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace Growflo.Integration.Core.Database
{
    public abstract class DbControllerBase
    {

        private string _databasePath;

        public DbControllerBase()
        {
            _databasePath = AppSettings.GetInstance().DatabasePath;

            if (!File.Exists(_databasePath))
                throw new Exception("Invalid database file path: " + _databasePath);
        }

        protected string GetConnectionString()
        {
            return $"Data Source={_databasePath};Version=3;";
        }

        public DataSet GetDataset(string sql)
        {
            DataSet results = new DataSet();

            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, connection))
                {
                    adapter.Fill(results);
                }
            }

            return results;
        }

        public int ExecuteNonQuery(string sql)
        {
            int result = 0;

            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                using (var command = new SQLiteCommand(sql, connection))
                {
                    connection.Open();

                    result = command.ExecuteNonQuery();
                }
            }

            return result;
        }

    }
}
