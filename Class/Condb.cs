using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PostSap_GR_TR.Class
{
    class Condb
    {
        public DataTable GetQuery(string sql)
        {
            var dt = new DataTable();

            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            int settingTimeout = Int32.Parse(ConfigurationManager.AppSettings["settingTimeout"]);
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }

            SqlConnection conn = new SqlConnection(connString);
            using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                cmd.CommandTimeout = settingTimeout;
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                    conn.Close();
                    da.Dispose();
                }
            }
            return dt;
        }
    }
}
