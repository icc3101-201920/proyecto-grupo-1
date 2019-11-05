using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entrega2_Equipo1
{
    public class LogInEventArgs : EventArgs
    {
        User usrTryingToEnter;
        string selectedUsrname;
        string selectedPassword;

        public User UsrTryingToEnter { get => this.usrTryingToEnter; set => this.usrTryingToEnter = value; }
        public string SelectedUsrname { get => this.selectedUsrname; set => this.selectedUsrname = value; }
        public string SelectedPassword { get => this.selectedPassword; set => this.selectedPassword = value; }
    }
}
