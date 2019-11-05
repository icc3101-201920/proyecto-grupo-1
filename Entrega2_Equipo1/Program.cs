using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Entrega2_Equipo1
{
    class Program
    {
		[STAThread]
		static void Main(string[] args)
        {
            // Detalles
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                // Creamos un loader para cargar la info de usuarios
                Loader loader = new Loader();

                // Obtenemos los forms creados por el loader
                MainWindow mainWindow = (MainWindow)loader.MainWindow;
                iFruitUserLogin userLoginForm = (iFruitUserLogin)loader.UserLoginForm;

                // Users es la lista de usuerios que existen
                userLoginForm.Users = loader.Users;


                // Si el active user es diferente de null, no necesitamos el userLoginForm,  corremos directamente la aplicacion
                if (loader.ActiveUser != null)
                {
                    mainWindow.UserLoggedIn = loader.ActiveUser;
                    Application.Run(mainWindow);
                    // Una vez que se cierre, podemos verificar si el usuario efectivamente queria salir o no
                    bool decision = mainWindow.Exit;

                    // Si queria salir, podemos salir del programa
                    if (decision == true)
                    {
                        loader.SaveUserInformation();
                        break;
                    } 
                    // Si no, hacemos otra iteracion
                    else
                    {
                        // Revisamos si desea eliminar su cuenta
                        if (mainWindow.Deleteaccount == true)
                        {
                            loader.Users.Remove(mainWindow.UserLoggedIn);
                        }


                        loader.SaveUserInformation();
                        continue;
                    }
                }
                // si no, debemos correr el userLoginForm
                else
                {
                    // Mostramos el userloginform
                    Application.Run(userLoginForm);
                    // El userlogin form obtiene el LogInUser, que es el usuario que se loguea, y se lo pasa al mainwindow
                    mainWindow.UserLoggedIn = userLoginForm.LogInUser;

                    // Ya podemos correr el mainWindow
                    Application.Run(mainWindow);

                    // Una vez que se cierre, podemos verificar si el usuario efectivamente queria salir o no
                    bool decision = mainWindow.Exit;

                    // Si queria salir, podemos salir del programa
                    if (decision == true)
                    {
                        loader.SaveUserInformation();
                        break;
                    } 
                    // Si no, hacemos otra iteracion
                    else
                    {
                        // Revisamos si desea eliminar su cuenta
                        if (mainWindow.Deleteaccount == true)
                        {
                            loader.Users.Remove(mainWindow.UserLoggedIn);
                        }
                        loader.SaveUserInformation();
                        continue;
                    }
                }
            }
		}
    }


    #region AuxiliarUserLoaderRegion

    public class Loader
    {

        #region AttributesAndConstantsRegion

        private MainWindow mainWindow;
        private iFruitUserLogin userLoginForm;
        private string DEFAULT_USERS_INFORMATION_LOCATION = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Files\users.bin";
        private List<User> users;
        private User activeUser = null;

        #endregion



        #region BuilderAndGetSetRegion

        public Loader()
        {
            LoadUserInformation();
            this.MainWindow = new MainWindow();
            this.UserLoginForm = new iFruitUserLogin(Users);
        }

        public MainWindow MainWindow { get => this.mainWindow; set => this.mainWindow = value; }

        public iFruitUserLogin UserLoginForm { get => this.userLoginForm; set => this.userLoginForm = value; }

        public List<User> Users { get => this.users; set => this.users = value; }

        public User ActiveUser { get => this.activeUser; set => this.activeUser = value; }




        #endregion



        #region AuxiliarMethodsRegion

        private void LoadUserInformation()
        {
            if (File.Exists(DEFAULT_USERS_INFORMATION_LOCATION))
            {
                // Cargamos los datos del usuario
                BinaryFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(DEFAULT_USERS_INFORMATION_LOCATION, FileMode.Open, FileAccess.Read, FileShare.None);
                Users = (List<User>)formatter.Deserialize(stream);
                stream.Close();

                // Buscamos si hay algun usuario con la sesion iniciada
                foreach (User user in Users)
                {
                    if (user.CurrentUser == true)
                    {
                        ActiveUser = user;
                    }
                }
            }
            else
            {
                Users = new List<User>() { };
                ActiveUser = null;
            }
        }
        public void SaveUserInformation()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(DEFAULT_USERS_INFORMATION_LOCATION, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, users);
            stream.Close();
        }


        #endregion

    }

    #endregion

}
