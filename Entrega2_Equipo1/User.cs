using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entrega2_Equipo1
{
    [Serializable]
    public class User
    {

        #region AttributesAndConstantsRegion

        private string DEFAULT_USER_EMPTYIMAGE_LOCATION = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\logos\emptyuser.png";
        private string name;
        private string usrname;
        private string surname;
        private DateTime birthdate;
        private ENationality nationality;
        private string description;
        private string password;
        private bool currentUser;
        private Bitmap usrImage;
        private DateTime membersince;

        #endregion

        #region BuildersAndGetSetRegion

        public string Name { get => this.name; set => this.name = value; }
        public string Surname { get => this.surname; set => this.surname = value; }
        public DateTime BirthDate { get => this.birthdate; set => this.birthdate = value; }
        public ENationality Nationality { get => this.nationality; set => this.nationality = value; }
        public string Description { get => this.description; set => this.description = value; }
        public string Password { get => this.password; set => this.password = value; }
        public bool CurrentUser { get => this.currentUser; set => this.currentUser = value; }
        public Bitmap UsrImage { get => this.usrImage; set => this.usrImage = value; }
        public string Usrname { get => this.usrname; set => this.usrname = value; }
        public DateTime Membersince { get => this.membersince; set => this.membersince = value; }

        public User(string name, string surname, DateTime birthdate, ENationality nationality, string description, string password, bool currentUser)
        {
            this.name = name;
            this.surname = surname;
            this.birthdate = birthdate;
            this.nationality = nationality;
            this.description = description;
            this.Password = password;
            this.CurrentUser = currentUser;
        }

        public User(string usrname, string password)
        {
            this.usrname = usrname;
            this.password = password;
            this.name = null;
            this.surname = null;
            this.birthdate = DateTime.Parse("01-01-1930");
            this.Nationality = ENationality.None;
            this.description = null;
            this.CurrentUser = false;
            this.usrImage = (Bitmap)System.Drawing.Image.FromFile(DEFAULT_USER_EMPTYIMAGE_LOCATION);
            this.Membersince = DateTime.Now;
        }

        #endregion
    }
}
