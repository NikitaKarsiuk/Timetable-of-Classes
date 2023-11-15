using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;

namespace TimetableOfClasses.Windows1
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();

            string ServerInfo = string.Empty;
            var list = RegistryValueDataReader.GetLocalSqlServerInstanceNames();
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var path = $"data source={Environment.MachineName}\\{list[0].ToString()};initial catalog=TimetableOfClassesdb;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");

            if (connectionStringsSection.ConnectionStrings["DataContext"].ConnectionString != path)
            {
                connectionStringsSection.ConnectionStrings["DataContext"].ConnectionString = path;
                config.Save();
                System.Windows.Forms.Application.Restart();
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DataContext db = new DataContext())
                {
                    var user = db.User.FirstOrDefault(x => x.Login == AuthLoginTextBox.Text && x.Password == AuthPassPasswordBox.Password);

                    if (user != null)
                    {
                        this.Hide();
                        MainWindow window = new MainWindow(user);
                        window.Closed += new EventHandler((_s, _e) => { this.ShowDialog(); });
                        window.ShowDialog();
                    }
                    else
                    {
                        throw new ArgumentException("Логин или пароль введены неверно!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                string pathDocument = Environment.CurrentDirectory + "\\help_student.chm";
                System.Diagnostics.Process.Start(pathDocument);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (DataContext db = new DataContext())
            {
                if (db.Root.Count() != 3)
                {
                    db.Root.RemoveRange(db.Root.ToList());

                    db.Root.AddRange(new List<Root>()
                    {
                        new Root()
                        {
                            Name = "Студент"
                        },
                        new Root()
                        {
                            Name = "Преподаватель"
                        },
                        new Root()
                        {
                            Name = "Администратор"
                        }
                    });

                    db.SaveChanges();
                }

                if (db.Group.Where(x => x.Name == "Admin group").Count() == 0)
                {
                    db.Group.Add(new Group() { Name = "Admin group"});
                    db.Group.Add(new Group() { Name = "Teacher group" });

                    db.SaveChanges();
                }
            }

            using (DataContext db = new DataContext())
            {
                var rootId = db.Root.Where(x => x.Name == "Администратор").First();
                var groupId = db.Group.Where(x => x.Name == "Admin group").First();
                if (db.User.Where(x => x.RootId == rootId.Id).Count() == 0)
                {
                    db.User.AddRange(new List<User>()
                    {
                        new User()
                        {
                            Name = "admin",
                            Surname = "admin",
                            Patronymic = "admin",
                            Login = "admin",
                            RootId = rootId.Id,
                            GroupId = groupId.Id,
                            Password = "admin"
                        }
                    });
                }

                db.SaveChanges();
            }
        }
    }
}
