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
    public partial class TimetableOfLessonWindow : Window
    {
        private int userId { get; }

        public TimetableOfLessonWindow(int userId)
        {
            InitializeComponent();

            this.userId = userId;
            using (DataContext db = new DataContext())
            {
                var items = db.LessonName.Where(x => x.UserId == userId).ToList();
                lessonDataGrid.ItemsSource = items;
            }
        }

        private void LessonAddButton_Click(object sender, RoutedEventArgs e)
        {
            LessonWindow window = new LessonWindow(userId);
            window.Closed += new EventHandler((_s, _e) =>
            {
                using (DataContext db = new DataContext())
                {
                    var items = db.LessonName.Where(x => x.UserId == userId).ToList();
                    lessonDataGrid.ItemsSource = items;
                }
            });
            window.Show();
        }

        private void LessonDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lessonDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = lessonDataGrid.ItemsSource as List<LessonName>;
                var item = lessonDataGrid.SelectedItem as LessonName;
                
                using (DataContext db = new DataContext())
                {
                    var lesson = db.LessonName.Find(item.Id);
                    var lessons = db.Lesson.Where(x => item.Id == x.LessonNameId).ToList();
                    if (lessons.Count > 0) 
                    {
                        MessageBox.Show("Нельза удалить выбранный предмет");
                        return;
                    }
                    db.LessonName.Remove(lesson);
                    db.SaveChanges();
                }

                items.Remove(item);

                lessonDataGrid.ItemsSource = items;
                lessonDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LessonChangeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lessonDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = lessonDataGrid.ItemsSource as List<LessonName>;
                var item = lessonDataGrid.SelectedItem as LessonName;

                LessonWindow window = new LessonWindow(userId,item.Id);
                window.Closed += new EventHandler((_s, _e) =>
                {
                    using (DataContext db = new DataContext())
                    {
                        var lessons = db.LessonName.Where(x => x.UserId == userId).ToList();
                        lessonDataGrid.ItemsSource = lessons;
                    }
                });
                window.Show();
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
