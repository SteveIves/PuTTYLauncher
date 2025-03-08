
using System.ComponentModel;

namespace PuttyLauncher
{
    class ConnectionProfile : INotifyPropertyChanged
    {
        private string name = String.Empty;
        private string session = String.Empty;
        private string user = String.Empty;
        private string password = String.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Session
        {
            get => session;
            set
            {
                if (session != value)
                {
                    session = value;
                    OnPropertyChanged(nameof(Session));
                }
            }
        }

        public string User
        {
            get => user;
            set
            {
                if (user != value)
                {
                    user = value;
                    OnPropertyChanged(nameof(User));
                }
            }
        }

        public string Password
        {
            get => password;
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }
    }
}
