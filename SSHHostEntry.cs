using System;
using System.Collections.Generic;
using System.Text;

namespace SSHMan
{
    public class SSHHostEntry: DefaultModel
    {

        public string Icon => "pack://application:,,,/host.png";

        private string name, address;

        public string Name
        {
            get => this.name;
            set {
                this.name = value;
                this.RaisePropertyChanged(() => this.Name);
            }
        }

        public string Address
        {
            get => this.address;
            set {
                this.address = value;
                this.RaisePropertyChanged(() => this.Address);
            }
        }
    }
}
