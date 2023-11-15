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
    public partial class LessonsWindow : Window
    {
        private int Id { get; }
        private User User { get; set; }

        public LessonsWindow(User user, int Id = -1)
        {
            InitializeComponent();

            this.Id = Id;
            User = user;

            using (DataContext db = new DataContext())
            {

                DayComboBox.ItemsSource = db.Day.ToList();
                LessonNumberComboBox.ItemsSource = db.LessonNumber.ToList();
                ClassNumberComboBox.ItemsSource = db.ClassNumber.ToList();
                GroupComboBox.ItemsSource = db.Group.Where(x => x.Name != "Admin group" && x.Name != "Teacher group").ToList();

                var rootId = db.Root.Where(x => x.Name == "Преподаватель").Select(x => x.Id).First();
                var items = db.User.Where(x => x.RootId == rootId).ToList();
                TeacherComboBox.ItemsSource = items;

                if (Id != -1)
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
            }
        }

        private void TeacherComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (DataContext db = new DataContext())
            {
                var teacher = TeacherComboBox.SelectedItem as User;
                var lessons = db.LessonName.Where(x => x.UserId == teacher.Id).ToList();
                LessonComboBox.ItemsSource = lessons;

                if (Id != -1)
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
                StreamReader fstream = new StreamReader("amoutofhourse.txt");
                var amoutOfHourse = fstream.ReadToEnd();
                fstream.Close();

                using (DataContext db = new DataContext())
                {
                    var dayId = (DayComboBox.SelectedItem as Day).Id;
                    var lessonNumberId = (LessonNumberComboBox.SelectedItem as LessonNumber).Id;
                    var сlassNumberId = (ClassNumberComboBox.SelectedItem as ClassNumber).Id;
                    var groupId = (GroupComboBox.SelectedItem as Group).Id;
                    var lessonNameId = (LessonComboBox.SelectedItem as LessonName).Id;
                    var teacherId = (TeacherComboBox.SelectedItem as User).Id;
                    var lessons = db.Lesson.Where(x => x.DayId == dayId && x.LessonNumberId == lessonNumberId).ToList();
                    var lessonsReplace = db.LessonReplace.Where(x => x.DayId == dayId && x.LessonNumberId == lessonNumberId).ToList();

                    if (DayComboBox.Text == "")
                        throw new ArgumentException("Ошибка. Вы не выбрали день");
                    if (LessonNumberComboBox.Text == "")
                        throw new ArgumentException("Ошибка. Вы не выбрали номер урока");
                    if (ClassNumberComboBox.Text == "")
                        throw new ArgumentException("Ошибка. Вы не выбрали номер кабинета");
                    if (GroupComboBox.Text == "")
                        throw new ArgumentException("Ошибка. Вы не выбрали группу");
                    if (TeacherComboBox.Text == "")
                        throw new ArgumentException("Ошибка. Вы не выбрали преподавателя");
                    if (LessonComboBox.Text == "")
                        throw new ArgumentException("Ошибка. Вы не выбрали предмет");
                    if ((db.Lesson.Where(x => x.GroupId == groupId).Count() * 2) + 2 > Convert.ToInt32(amoutOfHourse))
                        throw new ArgumentException("Превышено максимально допустимое количество часов в неделю");

                    if (Id == -1)
                    {
                        if (db.Lesson.Where(x => x.DayId == dayId && x.LessonNumberId == lessonNumberId && x.ClassNumberId == сlassNumberId).Count() > 0)
                            throw new ArgumentException("Ошибка. Кабинет занят");
                        if (db.LessonReplace.Where(x => x.DayId == dayId && x.LessonNumberId == lessonNumberId && x.ClassNumberId == сlassNumberId).Count() > 0)
                            throw new ArgumentException("Ошибка. Кабинет занят");
                        if (db.Lesson.Where(x => x.DayId == dayId && x.LessonNumberId == lessonNumberId && x.GroupId == groupId).Count() > 0)
                            throw new ArgumentException("Ошибка. У группы есть занятие в данный день");
                        if (db.LessonReplace.Where(x => x.DayId == dayId && x.LessonNumberId == lessonNumberId && x.GroupId == groupId).Count() > 0)
                            throw new ArgumentException("Ошибка. У группы есть занятие в данный день");
                        if ((db.Lesson.Where(x => x.GroupId == groupId).Count() * 2) + 2 > Convert.ToInt32(amoutOfHourse))
                            throw new ArgumentException("Превышено максимально допустимое количество часов в неделю");

                        foreach (var lesson in lessons)
                        {
                            var lessonName = db.LessonName.Where(x => x.Id == lesson.LessonNameId).First();
                            if(lessonName.UserId == teacherId)
                                throw new ArgumentException("Ошибка. Преподаватель в данный день уже ведет занятие");
                        }

                        foreach (var lesson in lessonsReplace)
                        {
                            var lessonName = db.LessonName.Where(x => x.Id == lesson.LessonNameId).First();
                            if (lessonName.UserId == teacherId)
                                throw new ArgumentException("Ошибка. Преподаватель в данный день уже ведет занятие");
                        }
                        db.Lesson.Add(new Lesson()
                        {
                            DayId = (DayComboBox.SelectedItem as Day).Id,
                            LessonNumberId = (LessonNumberComboBox.SelectedItem as LessonNumber).Id,
                            ClassNumberId = (ClassNumberComboBox.SelectedItem as ClassNumber).Id,
                            GroupId = (GroupComboBox.SelectedItem as Group).Id,
                            LessonNameId = (LessonComboBox.SelectedItem as LessonName).Id
                        }) ;
                        db.SaveChanges();

                        this.Close();
                    }
                else
                {
                        if (db.Lesson.Where(x => x.DayId == dayId && x.LessonNumberId == lessonNumberId && x.ClassNumberId == сlassNumberId && x.GroupId == groupId && x.LessonNameId == lessonNameId).Count() > 0)
                            throw new ArgumentException("Ошибка. Изменений не обнаружено");
                        if (db.Lesson.Where(x => x.DayId == dayId && x.LessonNumberId == lessonNumberId && x.ClassNumberId == сlassNumberId && x.Id != Id).Count() > 0)
                            throw new ArgumentException("Ошибка. Кабинет занят");
                        else
                        if (db.LessonReplace.Where(x => x.DayId == dayId && x.LessonNumberId == lessonNumberId && x.ClassNumberId == сlassNumberId && x.LessonId != Id).Count() > 0)
                            throw new ArgumentException("Ошибка. Кабинет занят");
                        if (db.Lesson.Where(x => x.DayId == dayId && x.LessonNumberId == lessonNumberId && x.GroupId == groupId && x.Id != Id).Count() > 0)
                            throw new ArgumentException("Ошибка. У группы есть занятие в данный день");
                        if (db.LessonReplace.Where(x => x.DayId == dayId && x.LessonNumberId == lessonNumberId && x.GroupId == groupId && x.LessonId != Id).Count() > 0)
                            throw new ArgumentException("Ошибка. У группы есть занятие в данный день");

                        foreach (var lesson in lessons)
                        {
                            var lessonName = db.LessonName.Where(x => x.Id == lesson.LessonNameId).First();
                            if (lessonName.UserId == teacherId && Id != lesson.Id)
                                throw new ArgumentException("Ошибка. Преподаватель в данный день уже ведет занятие");
                        }

                        foreach (var lesson in lessonsReplace)
                        {
                            var lessonName = db.LessonName.Where(x => x.Id == lesson.LessonNameId).First();
                            if (lessonName.UserId == teacherId)
                                throw new ArgumentException("Ошибка. Преподаватель в данный день уже ведет занятие");
                        }

                        var List = db.Lesson.Find(Id);

                        List.DayId = (DayComboBox.SelectedItem as Day).Id;
                        List.LessonNumberId = (LessonNumberComboBox.SelectedItem as LessonNumber).Id;
                        List.ClassNumberId = (ClassNumberComboBox.SelectedItem as ClassNumber).Id;
                        List.GroupId = (GroupComboBox.SelectedItem as Group).Id;
                        List.LessonNameId = (LessonComboBox.SelectedItem as LessonName).Id;

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
