using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GameServer.Dao
{

    public class AccountDao
    {
        public static string connstr = "data source=localhost;database=blackjack;user id=root;password=123456;pooling=false;charset=utf8";//pooling代表是否使用连接池
        public static MySqlConnection conn = new MySqlConnection(connstr);

        /// <summary>
        /// 账号是否存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool IsExist(string account)
        {
            string sql = "select * from account where Account = '"+ account+"'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            conn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            bool result= reader.HasRows;
            conn.Close();
            return result;
            

        }
        /// <summary>
        /// 创建账号
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        public void Create(string account, string password)
        {
            string sql = "insert into account (Account,Password) values ('" + account+"','"+password+"')";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        /// <summary>
        /// 账号密码是否匹配
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsMatch(string account, string password)
        {
            string sql2 = "select Password from account where Account='"+account+"'";
            MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
            conn.Open();
            string pw = cmd2.ExecuteScalar().ToString();
            conn.Close();
            return pw == password;
        }
        /// <summary>
        /// 获得Id
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public int GetId(string account)
        {
            string sql2 = "select Id from account where Account='" + account+"'";
            MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
            conn.Open();
            int Id = Convert.ToInt32(cmd2.ExecuteScalar());
            conn.Close();
            return Id;
        }
    }
}
