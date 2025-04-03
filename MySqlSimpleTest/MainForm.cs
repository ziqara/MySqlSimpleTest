using Mysqlx.Expect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySqlSimpleTest
{
    public partial class MainForm : Form
    {
        private SQLUserReader userReader;
        private List<UserInfo> users;
        SQLUserReader sqlreader = new SQLUserReader();
        public MainForm()
        {
            InitializeComponent();
            userReader = new SQLUserReader();
            LoadUsers();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UserTable.DataSource = sqlreader.ReadUsers();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (UserTable.SelectedRows.Count > 0)
            {
                var selectedUser = (UserInfo)UserTable.SelectedRows[0].DataBoundItem;

                if (MessageBox.Show($"Удалить пользователя {selectedUser.Login}?",
                    "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    bool success = userReader.DeleteUser(selectedUser.Login);
                    if (success)
                    {
                        LoadUsers(); // Обновляем список
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для удаления");
            }
        }

        private void LoadUsers()
        {
            // Сохраняем логин выбранного пользователя (если есть)
            string selectedLogin = null;
            if (UserTable.SelectedRows.Count > 0 && UserTable.SelectedRows[0].DataBoundItem is UserInfo user)
            {
                selectedLogin = user.Login;
            }

            // Обновляем данные
            users = userReader.ReadUsers();
            UserTable.DataSource = null;
            UserTable.DataSource = users;

            // Восстанавливаем выбор по логину (если был выбран)
            if (!string.IsNullOrEmpty(selectedLogin))
            {
                foreach (DataGridViewRow row in UserTable.Rows)
                {
                    if (row.DataBoundItem is UserInfo currentUser && currentUser.Login == selectedLogin)
                    {
                        row.Selected = true;
                        UserTable.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            using (var addForm = new AddUserForm())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    bool success = userReader.AddUser(addForm.NewUser);
                    if (success)
                    {
                        // Обновляем таблицу после успешного добавления
                        LoadUsers();
                        MessageBox.Show("Пользователь успешно добавлен!", "Успех",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void Update_Click(object sender, EventArgs e)
        {

            if (UserTable.SelectedRows.Count > 0)
            {
                string oldLogin = UserTable.SelectedRows[0].Cells["login"].Value.ToString(); // Сохраняем старый логин

                UserInfo userToEdit = null;
                foreach (UserInfo user in sqlreader.ReadUsers()) // Ищем в текущих данных
                {
                    if (user.Login == oldLogin)
                    {
                        userToEdit = user;
                        break;
                    }
                }

                if (userToEdit != null)
                {
                    AddUserForm editForm = new AddUserForm(userToEdit);
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        UserInfo updatedUser = editForm.NewUser;

                        // Проверяем уникальность нового логина
                        if (updatedUser.Login != oldLogin && !sqlreader.IsLoginUnique(updatedUser.Login))
                        {
                            MessageBox.Show("Логин уже используется. Пожалуйста, выберите другой.");
                            return;
                        }

                        if (sqlreader.UpdateUser(updatedUser, oldLogin)) // Передаем старый логин
                        {
                            MessageBox.Show($"Пользователь с логином '{updatedUser.Login}' успешно обновлен.");
                            RefreshDataGridView();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить пользователя.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось найти пользователя для редактирования.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для редактирования.");
            }
        }
        private void RefreshDataGridView()
        {
            UserTable.DataSource = null;  // Очистите DataSource перед обновлением
            UserTable.DataSource = sqlreader.ReadUsers();
        }
    }
    
}

