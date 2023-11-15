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

namespace TimetableOfClasses.Windows1
{
    /// <summary>
    /// Логика взаимодействия для ClassNumberWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        private int Id { get; }
        private User User { get; set; }

        public StudentWindow(User user, int Id = -1)
        {
            InitializeComponent();

            this.Id = Id;
            User = user;

            using (DataContext db = new DataContext())
            {
                GroupComboBox.ItemsSource = db.Group.Where(x => x.Name != "Admin group" && x.Name != "Teacher group").ToList();

                if (Id != -1)
                {
                    var List = db.User.Find(Id);

                    SurnameTextBox.Text = List.Surname.ToString();
                    NameTextBox.Text = List.Name.ToString();
                    PatronymicTextBox.Text = List.Patronymic.ToString();
                    GroupComboBox.SelectedItem = List.Group;
                    LoginTextBox.Text = List.Login.ToString();
                    PasswordTextBox.Text = List.Password.ToString();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DataContext db = new DataContext())
                {
                if (SurnameTextBox.Text == "" || !Regex.IsMatch(SurnameTextBox.Text, @"[А-яA-z]"))
                    throw new ArgumentException("Ошибка. Вы не заполнили поле фамилия");
                if (NameTextBox.Text == "" || !Regex.IsMatch(NameTextBox.Text, @"[А-яA-z]"))
                    throw new ArgumentException("Ошибка. Вы не заполнили поле имя");
                if (PatronymicTextBox.Text == "" || !Regex.IsMatch(PatronymicTextBox.Text, @"[А-яA-z]"))
                    throw new ArgumentException("Ошибка. Вы не заполнили поле отчество");
                if (GroupComboBox.Text == "")
                    throw new ArgumentException("Ошибка. Вы не выбрали группу");
                if (LoginTextBox.Text == "" || !Regex.IsMatch(LoginTextBox.Text, @"[А-яA-z]"))
                    throw new ArgumentException("Ошибка. Вы не заполнили поле логин");
                if (Id == -1)
                    if (db.User.Where(x => x.Login == LoginTextBox.Text).Count() > 0)
                    throw new ArgumentException("Данный логин уже существует");
                else
                {
                    if (db.User.Where(x => x.Login == LoginTextBox.Text).Count() > 1)
                        throw new ArgumentException("Данный логин уже существует");
                }
                if (PasswordTextBox.Text == "" || !Regex.IsMatch(PasswordTextBox.Text, @"[А-яA-z0-9]"))
                    throw new ArgumentException("Ошибка. Вы не заполнили поле пароль");

                if (Id == -1)
                {
                    
                        db.User.Add(new User()
                        {
                            Name = NameTextBox.Text,
                            Surname = SurnameTextBox.Text,
                            Patronymic = PatronymicTextBox.Text,
                            GroupId = (GroupComboBox.SelectedItem as Group).Id,
                            Login = LoginTextBox.Text,
                            Password = PasswordTextBox.Text,                            
                            RootId = db.Root.Where(x => x.Name == "Студент").Select(x => x.Id).First()
                        }) ;
                        db.SaveChanges();

                        this.Close();
                }
                else
                {
                        var List = db.User.Find(Id);
                        List.Name = NameTextBox.Text;
                        List.Surname = SurnameTextBox.Text;
                        List.Patronymic = PatronymicTextBox.Text;
                        List.GroupId = (GroupComboBox.SelectedItem as Group).Id;
                        List.Password = PasswordTextBox.Text;
                        List.Login = LoginTextBox.Text;
                        List.RootId = db.Root.Where(x => x.Name == "Студент").Select(x => x.Id).First();
                        db.SaveChanges();
                        this.Close();
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
                string pathDocument = Environment.CurrentDirectory + "\\help_admin.chm";
                System.Diagnostics.Process.Start(pathDocument);
            }
        }
    }
}
