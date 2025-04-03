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
    public partial class AddUserForm : Form
    {
        public UserInfo NewUser { get; set; }
        private bool isEditMode = false;

        public AddUserForm()
        {
            InitializeComponent();
            isEditMode = false;
        }

        public AddUserForm(UserInfo user)
        {
            InitializeComponent();
            isEditMode = true;

            txtLogin.Text = user.Login;
            txtName.Text = user.Name;
            txtSurname.Text = user.Surname;
            BirthDay.Value = user.BirthDate;
            txtPassword.Text = user.Password;
            txtEmail.Text = user.Email;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Поле 'Login' обязательно для заполнения.");
                return; // Прерываем выполнение, если поле пустое
            }
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Поле 'Имя' обязательно для заполнения.");
                return; // Прерываем выполнение, если поле пустое
            }
            if (string.IsNullOrWhiteSpace(txtSurname.Text))
            {
                MessageBox.Show("Поле 'Фамилия' обязательно для заполнения.");
                return; // Прерываем выполнение, если поле пустое
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Поле 'Пароль' обязательно для заполнения.");
                return; // Прерываем выполнение, если поле пустое
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Поле 'Email' обязательно для заполнения.");
                return; // Прерываем выполнение, если поле пустое
            }

            NewUser = new UserInfo(txtLogin.Text);
            NewUser.Name = txtName.Text;
            NewUser.Surname = txtSurname.Text;
            NewUser.BirthDate = BirthDay.Value;
            NewUser.Password = txtPassword.Text;
            NewUser.Email = txtEmail.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}
