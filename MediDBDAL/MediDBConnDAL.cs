using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace MediDBDAL
{
    public class MediDBConnDAL
    {
        private SqlConnection sqlCn = null;//被所有方法调用
        //连接数据库
        public void ConnectionSql(string connectionString)
        {
            sqlCn = new SqlConnection();
            sqlCn.ConnectionString = connectionString;
        }

        //打开数据库
        public void Opening()
        {
            sqlCn.Open();
        }

        //关闭数据库
        public void Closing()
        {
            sqlCn.Close();
        }

        //增加插入逻辑
        public void Insert(string sql)
        {
            sqlCn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, this.sqlCn))
            {
                cmd.ExecuteNonQuery();
            }
            sqlCn.Close();
        }

        //增加删除逻辑
        public void Delete(string sql)
        {
            sqlCn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, this.sqlCn))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Exception error = new Exception("不存在此记录", e);
                    throw error;
                }
            }
            sqlCn.Close();
        }

        //增加更新逻辑
        public void Update(string sql)
        {
            sqlCn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, this.sqlCn))
            {
                cmd.ExecuteNonQuery();
            }
            sqlCn.Close();
        }

        //增加查询遍历逻辑
        public DataSet Select(string sql)
        {
            sqlCn.Open();
            DataSet dataSet = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter(sql, sqlCn);
            sda.Fill(dataSet);
            sqlCn.Close();
            return dataSet;
        }

    }
    
}
