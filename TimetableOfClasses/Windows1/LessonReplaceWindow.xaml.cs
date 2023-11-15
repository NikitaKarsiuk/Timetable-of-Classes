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
    public partial class LessonReplaceWindow : Window
    {
        private int Id { get; }
        private User User { get; set; }

        public LessonReplaceWindow(User user, int Id = -1)
        {
            InitializeComponent();

            this.Id = Id;
            User = user;

            using (DataContext db = new DataContext())
            {
                var root = db.Root.Where(x => x.Name == "Преподаватель").First();

                if (user.RootId == root.Id)
                {
                    TeacherComboBox.IsEnabled = false;
                    LessonComboBox.IsEnabled = false;
                }

                DayComboBox.ItemsSource = db.Day.ToList();
                LessonNumberComboBox.ItemsSource = db.LessonNumber.ToList();
                ClassNumberComboBox.ItemsSource = db.ClassNumber.ToList();
                GroupComboBox.ItemsSource = db.Group.Where(x => x.Name != "Admin group" && x.Name != "Teacher group").ToList();

                var rootId = db.Root.Where(x => x.Name == "Преподаватель").Select(x => x.Id).First();
                var items = db.User.Where(x => x.RootId == rootId).ToList();
                TeacherComboBox.ItemsSource = items;

                if (db.LessonReplace.Where(x => x.LessonId == Id).Count() == 0)
                {
                    var List = db.Lesson.Find(Id);
                    DayComboBox.SelectedItem = List.Day;
                    LessonNumberComboBox.SelectedItem = List.LessonNumber;
                    ClassNumberComboBox.SelectedItem = List.ClassNumber;
                    GroupComboBox.SelectedItem = List.Group;
                    var lessonName = db.LessonName.Find(List.LessonNameId);
                    var userInfo = db.User.Find(lessonName.UserId);
                    TeacherComboBox.SelectedItem = lessonName.User;
                }
                else
                {
                    var List = db.LessonReplace.Where(x => x.LessonId == Id).First();
                    DayComboBox.SelectedItem = List.Day;
                    LessonNumberComboBox.SelectedItem = List.LessonNumber;
                    ClassNumberComboBox.SelectedItem = List.ClassNumber;
                    GroupComboBox.SelectedItem = List.Group;
                    var lessonName = db.LessonName.Find(List.LessonNameId);
                    var userInfo = db.User.Find(lessonName.UserId);
                    TeacherComboBox.SelectedItem = lessonName.User;
                }
            }
        }

        private void TeacherComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (DataContext db = new DataContext())
            {
                var teacher = TeacherComboBox.SelectedItem as User;
                var lessons = db.LessonName.Where(x => x.UserId == teacher.Id).ToList();
                LessonComboBox.ItemsSource = lessons;

                if (db.LessonReplace.Where(x => x.LessonId == Id).Count() > 0)
                {
                    var List = db.LessonReplace.Where(x => x.LessonId == Id).First();
                    LessonComboBox.SelectedItem = List.LessonName;
                }
                else
                {
                    var List = db.Lesson.Find(Id);
                    LessonComboBox.SelectedItem = List.LessonName;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DataContext db = new DataContext())
                {
                    if (ClassNumberComboBox.Text == "")
                        throw new ArgumentException("Ошибка. Вы не выбрали номер кабинета");
                    if (TeacherComboBox.Text == "")
                        throw new ArgumentException("Ошибка. Вы не выбрали преподавателя");
                    if (LessonComboBox.Text == "")
                        throw new ArgumentException("Ошибка. Вы не выбрали предмет");

                    var dayId = (DayComboBox.SelectedItem as Day).Id;
                    var lessonNumberId = (LessonNumberComboBox.SelectedItem as LessonNumber).Id;
                    var сlassNumberId = (ClassNumberComboBox.SelectedItem as ClassNumber).Id;
                    var lessonNameId = (LessonComboBox.SelectedItem as LessonName).Id;
                    var teacherId = (TeacherComboBox.SelectedItem as User).Id;
                    var lessons = db.Lesson.Where(x => x.DayId == dayId && x.LessonNumberId == lessonNumberId).ToList();
                    var lessonsReplace = db.Lesson.Where(x => x.DayId == dayId && x.LessonNumberId == lessonNumberId).ToList();

                    var lessonNameID = (LessonComboBox.SelectedItem as LessonName).Id;
                    var classNumberID = (ClassNumberComboBox.SelectedItem as ClassNumber).Id;
                    var groupID = (GroupComboBox.SelectedItem as Group).Id;
                    var less = db.Lesson.Find(Id);

                    if (db.Lesson.Where(x => x.ClassNumberId == classNumberID && x.DayId == less.DayId && x.LessonNumberId == less.LessonNumberId && x.Id != Id).Count() > 0)
                            throw new ArgumentException("Ошибка. Кабинет занят");
                    if (db.LessonReplace.Where(x => x.ClassNumberId == classNumberID && x.DayId == less.DayId && x.LessonNumberId == less.LessonNumberId && x.LessonId != Id).Count() > 0)
                        throw new ArgumentException("Ошибка. Кабинет занят");

                    foreach (var lesson in lessons)
                    {
                        var lessonName = db.LessonName.Where(x => x.Id == lesson.LessonNameId).First();
                        var name = LessonComboBox.Text;
                        if (lessonName.UserId == teacherId && lesson.Id != Id)
                            throw new ArgumentException("Ошибка. Преподаватель в данный день уже ведет занятие");

                        if (db.Lesson.Where(x => x.ClassNumberId == classNumberID && x.DayId == less.DayId && x.LessonNumberId == less.LessonNumberId && x.Id == Id && x.GroupId == groupID && lessonName.UserId == teacherId && lessonName.Name == LessonComboBox.Text).Count() > 0)
                        {
                            throw new ArgumentException("Ошибка. Изменений не обнаружено");
                        }
                    }

                    foreach (var lesson in lessonsReplace)
                    {
                        var lessonName = db.LessonName.Where(x => x.Id == lesson.LessonNameId).First();
                        var name = LessonComboBox.Text;
                        if (lessonName.UserId == teacherId && lesson.Id != Id)
                            throw new ArgumentException("Ошибка. Преподаватель в данный день уже ведет занятие");
                    }
                    var item = db.Lesson.Find(Id);

                    LessonReplace lessonReplace;

                    if (db.LessonReplace.Where(x => x.LessonId == item.Id).Count() > 0)
                    {
                        lessonReplace = db.LessonReplace.Where(x => x.LessonId == item.Id).First();
                    }

                    if (db.LessonReplace.Where(x => x.LessonId == item.Id).Count() == 0)
                        {
                                db.LessonReplace.Add(new LessonReplace()
                                {
                                    DayId = (DayComboBox.SelectedItem as Day).Id,
                                    LessonNumberId = (LessonNumberComboBox.SelectedItem as LessonNumber).Id,
                                    ClassNumberId = (ClassNumberComboBox.SelectedItem as ClassNumber).Id,
                                    GroupId = (GroupComboBox.SelectedItem as Group).Id,
                                    LessonNameId = (LessonComboBox.SelectedItem as LessonName).Id,
                                    LessonId = Id
                                });
                                db.SaveChanges();

                                this.Close();
                        }
                        else
                        {
                            var List = db.LessonReplace.Where(x => x.LessonId == Id).First();

                            List.DayId = (DayComboBox.SelectedItem as Day).Id;
                            List.LessonNumberId = (LessonNumberComboBox.SelectedItem as LessonNumber).Id;
                            List.ClassNumberId = (ClassNumberComboBox.SelectedItem as ClassNumber).Id;
                            List.GroupId = (GroupComboBox.SelectedItem as Group).Id;
                            List.LessonNameId = (LessonComboBox.SelectedItem as LessonName).Id;
                            List.LessonId = Id;

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

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            using (DataContext db = new DataContext())
            {
                var lessonReplace = db.LessonReplace.Where(x => x.LessonId == Id).First();
                var deleteLessonReplace = db.LessonReplace.Find(lessonReplace.Id);
                db.LessonReplace.Remove(deleteLessonReplace);
                db.SaveChanges();
                this.Close();
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
