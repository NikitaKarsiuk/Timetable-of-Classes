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
    public partial class LessonWindow : Window
    {
        private int Id { get; }
        private int userId { get; }

        public LessonWindow(int userId, int Id = -1)
        {
            InitializeComponent();

            this.Id = Id;
            this.userId = userId;

            using (DataContext db = new DataContext())
            {
                if (Id != -1)
                {
                    var List = db.LessonName.Find(Id);
                    LessonTextBox.Text = List.Name.ToString();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DataContext db = new DataContext())
                {
                    if (LessonTextBox.Text == "" || !Regex.IsMatch(LessonTextBox.Text, @"[А-яA-z]"))
                        throw new ArgumentException("Ошибка. Вы не заполнили поле предмет");

                    var teacherLessons = db.LessonName.Where(x => x.UserId == userId).ToList();

                    if (Id == -1)
                    {
                        foreach (var lessons in teacherLessons)
                        {
                            if (lessons.Name == LessonTextBox.Text)
                                throw new ArgumentException("Ошибка. Предмет уже существует");
                        }
                    }
                    else
                    {
                        foreach (var lessons in teacherLessons)
                        {
                            if (lessons.Name == LessonTextBox.Text && Id != lessons.Id)
                                throw new ArgumentException("Ошибка. Предмет уже существует");
                        }
                    }

                    if (Id == -1)
                    {
                        db.LessonName.Add(new LessonName()
                        {
                            Name = LessonTextBox.Text,
                            UserId = userId
                        });
                        db.SaveChanges();

                        this.Close();
                    }
                    else
                    {
                        var List = db.LessonName.Find(Id);
                        List.Name = LessonTextBox.Text;
                        List.UserId = userId;
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
