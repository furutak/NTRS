using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Npgsql;
using System.Text;
using System.Data.OleDb;
using System.Globalization;
using System.IO;

namespace NTRS
{
    public class TfSQL
    {
        NpgsqlConnection connection;
        string conStringPqmDb = @"Server=192.168.193.2;Port=5432;User Id=pqm;Password=dbuser;Database=pqmdb; CommandTimeout=100; Timeout=100;";
        string conStringTesterDb = @"Server=192.168.193.5;Port=5432;User Id=pqm;Password=dbuser;Database=pqmdb; CommandTimeout=100; Timeout=100;";

        // 文字列スカラを返す（ＰＱＭＤＢ）
        public string sqlExecuteScalarStringPqm(string sql)
        {
            string response;
            try
            {
                connection = new NpgsqlConnection(conStringPqmDb);
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand(sql, connection);
                response = Convert.ToString(command.ExecuteScalar());
                connection.Close();
                return response;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Responce", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                connection.Close();
                return String.Empty;
            }
        }

        // 対象レコードをＤＡＴＡＴＡＢＬＥへ格納する（ＰＱＭＤＢ）
        public void sqlDataAdapterFillDatatableFromPqmDb(string sql, ref DataTable dt)
        {
            NpgsqlConnection connection = new NpgsqlConnection(conStringPqmDb);
            NpgsqlCommand command = new NpgsqlCommand();

            using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter())
            {
                command.CommandText = sql;
                command.Connection = connection;
                adapter.SelectCommand = command;
                adapter.Fill(dt);
            }
        }

        // 対象レコードをＤＡＴＡＴＡＢＬＥへ格納する（ＴＥＳＴＥＲ ＤＢ）
        public void sqlDataAdapterFillDatatableFromTesterDb(string sql, ref DataTable dt)
        {
            NpgsqlConnection connection = new NpgsqlConnection(conStringTesterDb);
            NpgsqlCommand command = new NpgsqlCommand();

            using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter())
            {
                command.CommandText = sql;
                command.Connection = connection;
                adapter.SelectCommand = command;
                adapter.Fill(dt);
            }
        }

        // ＣＳＶデータをデータテーブルに取り込む
        public DataTable getDataTableFromCsv(string path, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";

            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            string sql = @"SELECT * FROM [" + fileName + "]";

            using (OleDbConnection connection = new OleDbConnection(
                      @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                      ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
    }
}
