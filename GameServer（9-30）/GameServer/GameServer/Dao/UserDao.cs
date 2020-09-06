using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using MySql.Data.MySqlClient;
namespace GameServer.Dao
{
    public class UserDao
    {
        public static string connstr = "data source=localhost;database=blackjack;user id=root;password=123456;pooling=false;charset=utf8";//pooling代表是否使用连接池
        public static MySqlConnection conn = new MySqlConnection(connstr);

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="name"></param>
        /// <param name="accountId"></param>
        public void Create(string name, int accountId)
        {
            MySqlCommand command;
            conn.Open();
            command = conn.CreateCommand();
            command.CommandText = "INSERT INTO user(Name,AccountId,Been,WinCount,LoseCount,RunCount,Lv,Exp,Date) VALUES(@Name,@AccountId,10000,0,0,0,1,0,@Date)";
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@AccountId", accountId);
            command.Parameters.AddWithValue("@Date", DateTime.Now);
            command.ExecuteNonQuery();
            conn.Close();


        }

        /// <summary>
        /// 是否存在角色
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public bool IsExist(int accountId)
        {
            string sql = "select * from user where AccountId =" + accountId;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            conn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            bool result = reader.HasRows;
            conn.Close();
            return result;
            
        }

        /// <summary>
        /// 根据账号返回角色信息
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public UserModel GetModelByAccountId(int accountId)
        {
            UserModel um = new UserModel();
            string sql = "select * from user where AccountId =" + accountId;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            conn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                um.Id = reader.GetInt32("Id");
                um.Name = reader.GetString("Name");
                um.Been = reader.GetInt32("Been");
                um.WinCount = reader.GetInt32("WinCount");
                um.LoseCount = reader.GetInt32("LoseCount");
                um.RunCount = reader.GetInt32("RunCount");
                um.Lv = reader.GetInt32("Lv");
                um.Exp = reader.GetInt32("Exp");
                um.AccountId = reader.GetInt32("AccountId");
                um.date = Convert.ToDateTime(reader.GetString("Date"));

            }
            conn.Close();
            return um;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserModel GetModelById(int userId)
        {
            UserModel um = new UserModel();
            string sql = "select * from user where Id =" + userId;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            conn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                um.Id = reader.GetInt32("Id");
                um.Name = reader.GetString("Name");
                um.Been = reader.GetInt32("Been");
                um.WinCount = reader.GetInt32("WinCount");
                um.LoseCount = reader.GetInt32("LoseCount");
                um.RunCount = reader.GetInt32("RunCount");
                um.Lv = reader.GetInt32("Lv");
                um.Exp = reader.GetInt32("Exp");
                um.AccountId = reader.GetInt32("AccountId");
                um.date = Convert.ToDateTime(reader.GetString("Date"));
            }
            conn.Close();
            return um;
        }

        /// <summary>
        /// 获取角色id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public int GetId(int accountId)
        {
            string sql2 = "select Id from user where AccountId=" + accountId;
            MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
            conn.Open();
            int Id = Convert.ToInt32(cmd2.ExecuteScalar());
            conn.Close();
            return Id;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        public void Update(UserModel model)
        {
            MySqlCommand command;
            conn.Open();
            command = conn.CreateCommand();
            command.CommandText = "update user set Been = @Been, Lv = @Lv ,Exp=@Exp,Date = @Date where Id = @Id";
            command.Parameters.AddWithValue("@Been", model.Been);
            command.Parameters.AddWithValue("@Lv",model.Lv );
            command.Parameters.AddWithValue("@Exp", model.Exp);
            command.Parameters.AddWithValue("@Date", model.date);
            command.Parameters.AddWithValue("@Id", model.Id);
            command.ExecuteNonQuery();
            conn.Close();

        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        public void Update2(UserModel model)
        {
            MySqlCommand command;
            conn.Open();
            command = conn.CreateCommand();
            command.CommandText = "update user set Been = @Been, Date = @Date  where Id = @Id";
            command.Parameters.AddWithValue("@Been", model.Been);
            command.Parameters.AddWithValue("@Date", model.date);
            command.Parameters.AddWithValue("@Id", model.Id);
            command.ExecuteNonQuery();
            conn.Close();

        }
    }
}
