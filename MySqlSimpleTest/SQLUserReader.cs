using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySqlSimpleTest
{
    public class SQLUserReader
    {
        private string MyConnectionString = "server=127.0.0.1;uid=root;pwd=vertrigo;database=users";
        public List<UserInfo> ReadUsers()
        {
            List<UserInfo> result = new List<UserInfo>();
            MySqlConnection conn;
            string MyConnectionString = "server=127.0.0.1; uid=root;pwd=vertrigo; database=users;";
            try
            {
                conn = new MySqlConnection(MyConnectionString);
                conn.Open();

                const string quary = "SELECT name, surname, login, password, email, birthdate from users;";
                MySqlCommand command = new MySqlCommand(quary, conn);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string Login = reader.GetString("login");

                        UserInfo us = new UserInfo(Login);
                        us.Name = reader.GetString("name");
                        us.Surname = reader.GetString("surname");
                        us.Password = reader.GetString("password");
                        us.Email = reader.GetString("email");
                        us.BirthDate = reader.GetDateTime("birthdate");
                        result.Add(us);
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return result;
            }
            return result;
        }

        public bool DeleteUser(string login)
        {
            MySqlConnection conn;
            string MyConnectionString = "server=127.0.0.1; uid=root;pwd=vertrigo; database=users;";

            try
            {
                conn = new MySqlConnection(MyConnectionString);
                conn.Open();

                string query = "DELETE FROM users WHERE login = @login;";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@login", login);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0; // Возвращает true, если пользователь был удален
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Ошибка при удалении пользователя: " + ex.Message);
                return false;
            }
        }

        public bool AddUser(UserInfo user)
        {
            MySqlConnection conn;
            string MyConnectionString = "server=127.0.0.1; uid=root;pwd=vertrigo; database=users;";

            try
            {
                conn = new MySqlConnection(MyConnectionString);
                conn.Open();

                string query = @"INSERT INTO users (login, name, surname, password, email, birthdate) 
                        VALUES (@login, @name, @surname, @password, @email, @birthdate);";

                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@login", user.Login);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@surname", user.Surname);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@birthdate", user.BirthDate);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Ошибка при добавлении пользователя: " + ex.Message);
                return false;
            }
        }

        public bool UpdateUser(UserInfo user, string oldLogin)  // oldLogin для поиска
        {
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(MyConnectionString);
                conn.Open();

                string query = "UPDATE users SET login = @newLogin, name = @name, surname = @surname, birthdate = @birthdate, " +
                               "password = @password, email = @email WHERE login = @oldLogin";  // Используем oldLogin в WHERE
                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@newLogin", user.Login); // Новый логин
                command.Parameters.AddWithValue("@oldLogin", oldLogin);   // Старый логин для WHERE
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@surname", user.Surname);
                command.Parameters.AddWithValue("@birthdate", user.BirthDate);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@email", user.Email);

                int rowsAffected = command.ExecuteNonQuery();
                conn.Close();

                return rowsAffected > 0;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public bool IsLoginUnique(string login)  // Метод для проверки уникальности логина
        {
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(MyConnectionString);
                conn.Open();

                string query = "SELECT COUNT(*) FROM users WHERE login = @login";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@login", login);

                long count = (long)command.ExecuteScalar();
                conn.Close();

                return count == 0; // Возвращает true, если логин не существует
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false; // В случае ошибки считаем, что логин не уникален
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
}