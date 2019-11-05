using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Entrega2_Equipo1
{
    public partial class iFruitUserLogin : Form
    {
        public iFruitUserLogin()
        {
            InitializeComponent();
        }



        #region AttributesAndConstantsRegion

        private List<User> users;
        // logInUser es el usuario que se logueo
        private User logInUser;
        private User activeUser;
        private string DEFAULT_USERS_INFORMATION_LOCATION = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Files\users.bin";
        private string DEFAULT_USERS_DATA_LOCATION = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Files\";

        public List<User> Users { get => this.users; set => this.users = value; }
        public User LogInUser { get => this.logInUser; set => this.logInUser = value; }
        public User ActiveUser { get => this.activeUser; set => this.activeUser = value; }

        #endregion

        // Builder
        public iFruitUserLogin(List<User> users)
        {
            InitializeComponent();
            this.Users = users;
            LoadLogInUsrNames();
        }


        #region FormEventsRelatedRegion

        private void IFruitUserLogin_Load(object sender, EventArgs e)
        {

        }

        // Metodo para cargar al combo box los datos de los usuarios que existen
        public void LoadLogInUsrNames()
        {
            foreach (User user in Users)
            {
                if (user.Usrname != null && !this.UserNamesComboBox.Items.Contains(user.Usrname))
                    this.UserNamesComboBox.Items.Add(user.Usrname);
            }
        }

        
        

            // metodo que se ejecuta cuando se hace clic en el new user label
        private void NewUserLabel_Click(object sender, EventArgs e)
        {
            if (CreateUserPanel.Visible == false)
            {
                CreateUserPanel.Visible = true;
                LogInButton.Enabled = false;
                UserNamesComboBox.Enabled = false;
                LogInPasswordTextBox.Enabled = false;
            }
            else
            {
                CreateUserPanel.Visible = false;
                LogInButton.Enabled = true;
                UserNamesComboBox.Enabled = true;
                LogInPasswordTextBox.Enabled = true;
            }
        }

        private void SignUpButton_Click(object sender, EventArgs e)
        {
            if (users.Count != 0)
            {
                // Recorremos los usuarios existentes
                foreach (User usr in users)
                {
                    // Si el nombre del usuario es diferente de null
                    if (usr.Usrname != null)
                    {
                        // Luego, tiramos error si el usuario ya existe, la contrasena es vacia o no se ha introducido
                        if (NewUserNameTextBox.Text == usr.Usrname || NewUserNameTextBox.Text == "" || NewUserNameTextBox.Text == null)
                        {
                            this.NewUsernameErrorLabel.Visible = true;
                            this.NewPasswordEmptyErrorLabel.Visible = false;
                            return;
                        }
                        else if (NewPasswordTextBox.Text == "" || NewPasswordTextBox.Text == null)
                        {
                            this.NewPasswordEmptyErrorLabel.Visible = true;
                            this.NewUsernameErrorLabel.Visible = false;
                            return;
                        }
                    }
                }
            }
            else
            {
                if (NewUserNameTextBox.Text == "" || NewUserNameTextBox.Text == null)
                {
                    this.NewUsernameErrorLabel.Visible = true;
                    this.NewPasswordEmptyErrorLabel.Visible = false;
                    return;
                }
                else if (NewPasswordTextBox.Text == "" || NewPasswordTextBox.Text == null)
                {
                    this.NewPasswordEmptyErrorLabel.Visible = true;
                    this.NewUsernameErrorLabel.Visible = false;
                    return;
                }
            }


            // Si ess valido, agregamos a la lista de usuarios el nuevo usuario
            this.users.Add(new User(NewUserNameTextBox.Text, NewPasswordTextBox.Text));


            // Cargamos los usrnames en el combo box
            this.LoadLogInUsrNames();

            // Guardamos los usuarios
            SaveUserInformation();

            // Creamos la carpeta para almacenar su library y su producer
            CreateUserDirectory(NewUserNameTextBox.Text);

            // Escondemos el panel de signup
            this.NewUsernameErrorLabel.Visible = false;
            this.NewPasswordEmptyErrorLabel.Visible = false;
            this.NewUserNameTextBox.Text = "username";
            this.NewPasswordTextBox.Text = "password";
            this.CreateUserPanel.Visible = false;

            // Habilitamos el log in
            LogInButton.Enabled = true;
            UserNamesComboBox.Enabled = true;
            LogInPasswordTextBox.Enabled = true;
        }

        // Si cambia la seleccion de combo box, cargamos el usuario seleccionado como loginuser
        private void UserNamesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox sender2 = (ComboBox)sender;
            string selecteditem = sender2.SelectedItem.ToString();
            foreach (User user in users)
            {
                if (user.Usrname == selecteditem)
                {
                    this.LogInUser = user;
                    this.UserPicturePictureBox2.BackgroundImage = user.UsrImage;
                }
            }
        }

        // Si se hace clic en cualquier lugar sobre el form
        private void IFruitUserLogin_Click(object sender, EventArgs e)
        {
            this.CreateUserPanel.Visible = false;
            LogInButton.Enabled = true;
            UserNamesComboBox.Enabled = true;
            LogInPasswordTextBox.Enabled = true;
            this.NewUserNameTextBox.Text = "username";
            this.NewPasswordTextBox.Text = "password";
            this.NewPasswordTextBox.UseSystemPasswordChar = false;
        }

        // Mouse enter sobre el new user label
        private void NewUserLabel_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.Label newuserlabel = (System.Windows.Forms.Label)sender;
            newuserlabel.Font = new Font(newuserlabel.Font.Name, newuserlabel.Font.SizeInPoints, FontStyle.Underline);
        }

        // Mouse leave sobre el new user label
        private void NewUserLabel_MouseLeave(object sender, EventArgs e)
        {
            System.Windows.Forms.Label newuserlabel = (System.Windows.Forms.Label)sender;
            newuserlabel.Font = new Font(newuserlabel.Font.Name, newuserlabel.Font.SizeInPoints, FontStyle.Regular);
        }

        // Si se hace clic en log in
        private void LogInButton_Click(object sender, EventArgs e)
        {
            // Verificamos que el combo box haya seleccionado un usuario
            if (UserNamesComboBox.SelectedItem != null)
            {
                // Mostramos error en caso de que el password sea vacio o equivocado
                this.WrongPasswordLabel.Text = "";
                this.WrongPasswordLabel.Visible = false;
                if (LogInPasswordTextBox.Text == "" || LogInPasswordTextBox.Text == null)
                {
                    this.WrongPasswordLabel.Text = "Empty password";
                    this.WrongPasswordLabel.Visible = true;
                    return;
                }
                else if (LogInPasswordTextBox.Text != logInUser.Password)
                {
                    this.WrongPasswordLabel.Text = "Wrong password";
                    this.WrongPasswordLabel.Visible = true;
                    return;
                }
                // En este caso el login es exitoso, por lo que cambiamos la propiedad current user de loginuser a true
                this.LogInUser.CurrentUser = true;
                // Guardamos
                SaveUserInformation();
                // Cerramos el login
                this.Close();
            }
        }



        #endregion

        private void LogInPasswordTextBox_Enter(object sender, EventArgs e)
        {
            if (LogInPasswordTextBox.Text == "password")
            {
                LogInPasswordTextBox.Text = "";
                LogInPasswordTextBox.UseSystemPasswordChar = true;
                LogInPasswordTextBox.ForeColor = Color.White;
            }
        }

        private void LogInPasswordTextBox_Leave(object sender, EventArgs e)
        {
            if (LogInPasswordTextBox.Text == "")
            {
                LogInPasswordTextBox.Text = "password";
                LogInPasswordTextBox.UseSystemPasswordChar = false;
                LogInPasswordTextBox.ForeColor = Color.Silver;
            }
        }

        private void NewUserNameTextBox_Enter(object sender, EventArgs e)
        {
            if (NewUserNameTextBox.Text == "username")
            {
                NewUserNameTextBox.Text = "";
                NewUserNameTextBox.ForeColor = Color.White;
            }
        }

        private void NewUserNameTextBox_Leave(object sender, EventArgs e)
        {
            if (NewUserNameTextBox.Text == "")
            {
                NewUserNameTextBox.Text = "username";
                NewUserNameTextBox.ForeColor = Color.Silver;
            }
        }

        private void NewPasswordTextBox_Enter(object sender, EventArgs e)
        {
            if (NewPasswordTextBox.Text == "password")
            {
                NewPasswordTextBox.Text = "";
                NewPasswordTextBox.UseSystemPasswordChar = true;
                NewPasswordTextBox.ForeColor = Color.White;
            }
        }

        private void NewPasswordTextBox_Leave(object sender, EventArgs e)
        {
            if (NewPasswordTextBox.Text == "")
            {
                NewPasswordTextBox.Text = "password";
                NewPasswordTextBox.UseSystemPasswordChar = false;
                NewPasswordTextBox.ForeColor = Color.Silver;
            }
        }

        #region AuxiliarMethodsRegion

        private void LoadUserInformation()
        {
            if (File.Exists(DEFAULT_USERS_INFORMATION_LOCATION))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(DEFAULT_USERS_INFORMATION_LOCATION, FileMode.Open, FileAccess.Read, FileShare.None);
                users = (List<User>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                users = new List<User>() { new User(null, null) };
            }
        }

        private void SaveUserInformation()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(DEFAULT_USERS_INFORMATION_LOCATION, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, users);
            stream.Close();
        }

        private void CreateUserDirectory(string usrname)
        {
            if (!Directory.Exists(DEFAULT_USERS_DATA_LOCATION + usrname))
            {
                Directory.CreateDirectory(DEFAULT_USERS_DATA_LOCATION + usrname);
            }
            else
            {
                string[] files = Directory.GetFiles(DEFAULT_USERS_DATA_LOCATION + usrname);
                string[] dirs = Directory.GetDirectories(DEFAULT_USERS_DATA_LOCATION + usrname);

                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    Directory.Delete(dir);
                }

                Directory.Delete(DEFAULT_USERS_DATA_LOCATION + usrname);
                Directory.CreateDirectory(DEFAULT_USERS_DATA_LOCATION + usrname);
            }
        }

        #endregion

        private void LogInPasswordTextBox_MouseEnter(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            txt.BackColor = Color.FromArgb(6, 57, 76);
        }

        private void LogInPasswordTextBox_MouseLeave(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            txt.BackColor = Color.FromArgb(7, 30, 38);
        }
    }


    #region AuxiliarFormClasses

    class OvalPictureBox : PictureBox
    {
        public OvalPictureBox()
        {
            this.BackColor = Color.DarkGray;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            using (var gp = new GraphicsPath())
            {
                gp.AddEllipse(new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                this.Region = new Region(gp);
            }
        }
    }

    #endregion

}
