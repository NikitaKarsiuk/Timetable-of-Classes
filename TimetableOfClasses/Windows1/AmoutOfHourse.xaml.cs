using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class AmoutOfHourseWindow : Window
    {
        private int Id { get; }
        private User User { get; set; }

        public AmoutOfHourseWindow(User user, int ID = -1)
        {
            InitializeComponent();

            this.Id = ID;
            User = user;
            StreamReader fstream = new StreamReader("amoutofhourse.txt");

            AmoutOfHourseTextBox.Text = fstream.ReadToEnd();
            fstream.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                if (AmoutOfHourseTextBox.Text == "" || !Regex.IsMatch(AmoutOfHourseTextBox.Text, @"[0-9]"))
                    throw new ArgumentException("Ошибка. Вы не заполнили поле");
                using (DataContext db = new DataContext())
                {
                    var groups = db.Group.ToList();
                    var lesson = db.Lesson.ToList();

                    foreach(var group in groups)
                    {
                        if(db.Lesson.Where(x => x.GroupId == group.Id).Count() * 2 > Convert.ToInt32(AmoutOfHourseTextBox.Text))
                            throw new ArgumentException("Ошибка. Невозможно изменить количество часов");
                    }
                }

                StreamWriter fstream = new StreamWriter("amoutofhourse.txt");
                fstream.Write(AmoutOfHourseTextBox.Text);
                fstream.Close();
                this.Close();
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
