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
    public partial class ClassNumberWindow : Window
    {
        private int Id { get; }
        private User User { get; set; }
        public ClassNumberWindow()
        {
            InitializeComponent();
        }

        public ClassNumberWindow(User user, int ID = -1)
        {
            InitializeComponent();

            this.Id = ID;
            User = user;

            if (ID != -1)
            {
                using (DataContext db = new DataContext())
                {
                    var List = db.ClassNumber.Find(Id);

                    ClassNumberTextBox.Text = List.Number.ToString();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DataContext db = new DataContext())
                {
                    if (ClassNumberTextBox.Text == "" || !Regex.IsMatch(ClassNumberTextBox.Text, @"^[0-9]+$"))
                    throw new ArgumentException("Ошибка. Вы не заполнили поле кабинет");
                    if (Id == -1)
                        if (db.ClassNumber.Where(x => x.Number.ToString() == ClassNumberTextBox.Text).Count() > 0)
                        throw new ArgumentException("Данный кабинет уже существует");
                    else
                        {
                            if (db.ClassNumber.Where(x => x.Number.ToString() == ClassNumberTextBox.Text).Count() > 1)
                                throw new ArgumentException("Данный кабинет уже существует");
                        }
                    if (Id == -1)
                    {
                    
                            db.ClassNumber.Add(new ClassNumber()
                            {
                                Number = Int32.Parse(ClassNumberTextBox.Text)
                            });
                            db.SaveChanges();

                            this.Close();
                    }
                    else
                    {
                        var List = db.ClassNumber.Find(Id);
                        List.Number = Int32.Parse(ClassNumberTextBox.Text);
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
