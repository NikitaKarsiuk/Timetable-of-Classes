using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;
using TimetableOfClasses.Windows1;
using System.Data;
using System.Windows.Media;
using System.Collections;
using System.Reflection;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace TimetableOfClasses
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private User User { get; set; }

        public MainWindow(User user)
        {
            InitializeComponent();
            try
            {
                this.User = user;

                using (DataContext db = new DataContext())
                {
                    var UserId = db.User.Find(user.Id);
                    var GroupId = db.Group.Find(UserId.GroupId);
                    var LessonsList = db.Lesson.Where(x => x.GroupId == GroupId.Id).ToList();
                    var LessonsListReplace = db.LessonReplace.Where(x => x.GroupId == GroupId.Id).ToList();
                    var studentRoot = db.Root.Where(x => x.Name == "Студент").First();
                    var teacherRoot = db.Root.Where(x => x.Name == "Преподаватель").First();
                    var adminRoot = db.Root.Where(x => x.Name == "Администратор").First();

                    ClearTimetableOfClasses();
                    FillTimetableOfClasses(LessonsList);
                    FillTimetableOfClasses(LessonsListReplace);
                    FillStudentInformation();

                    if (user != null)
                    {
                        TextBoxUserTimetable.Text = user.Surname + " " + user.Name[0] + "." + user.Patronymic[0] + ".";
                        TextBoxUserPersonalAccount.Text = user.Surname + " " + user.Name[0] + "." + user.Patronymic[0] + ".";
                        TextBoxUserAdmin.Text = user.Surname + " " + user.Name[0] + "." + user.Patronymic[0] + ".";
                    }
                    else
                    {
                        throw new ArgumentException("");
                    }

                    if (user.RootId == studentRoot.Id)
                    {

                        foreach (TabItem item in TabControl.Items)
                        {
                            if (item.Header.ToString() == "Админ панель")
                            {
                                item.Visibility = Visibility.Hidden;
                            }
                        }

                        GroupStackPanel.Visibility = Visibility.Visible;
                    }
                    else
                    if (user.RootId == teacherRoot.Id)
                    {
                        foreach (TabItem item in TabControl.Items)
                        {
                            if (item.Header.ToString() == "Админ панель")
                            {
                                item.Header = "Панель";
                                foreach(TabItem tab in ThirdTabControl.Items)
                                {
                                    if (tab.Header.ToString() != "Занятия")
                                    {

                                        tab.Visibility = Visibility.Hidden;
                                    }
                                }
                            }
                        }


                        LessonAddButton.Visibility = Visibility.Hidden;
                        LessonDeleteButton.Visibility = Visibility.Hidden;
                        LessonChangeButton.Visibility = Visibility.Hidden;
                        GroupStackPanel.Visibility = Visibility.Hidden;
                    }
                    else
                    if (user.RootId == adminRoot.Id)
                    {
                        foreach (TabItem item in TabControl.Items)
                        {
                            if (item.Header.ToString() == "Админ панель")
                            {
                                item.Visibility = Visibility.Visible;
                                
                            }
                        }
                        GroupStackPanel.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExitTabItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MainWindow window = new MainWindow(User);
                this.Close();
            }
            catch { }
        }

        private void OpenLessonWindow_Click(object sender, RoutedEventArgs e)
        {
            var item = teacherDataGrid.SelectedItem as User;

            TimetableOfLessonWindow window = new TimetableOfLessonWindow(item.Id);
            window.Closed += new EventHandler((_s, _e) =>
            {
                DirectoryTabItemFill("Преподаватель");
            });
            window.Show();
        }

        private void OpenLessonReplaceWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lessonDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = lessonDataGrid.ItemsSource as List<Lesson>;
                var item = lessonDataGrid.SelectedItem as Lesson;

                LessonReplaceWindow window = new LessonReplaceWindow(User, item.Id);
                window.Closed += new EventHandler((_s, _e) =>
                {
                    DirectoryTabItemFill("Занятия");
                });
                window.Show();

                using (DataContext db = new DataContext())
                {
                    var UserId = db.User.Find(User.Id);
                    var GroupId = db.Group.Find(UserId.GroupId);
                    var LessonsList = db.Lesson.Where(x => x.GroupId == GroupId.Id).ToList();
                    var LessonsListReplace = db.LessonReplace.Where(x => x.GroupId == GroupId.Id).ToList();
                    ClearTimetableOfClasses();
                    FillTimetableOfClasses(LessonsList);
                    FillTimetableOfClasses(LessonsListReplace);
                    FillStudentInformation();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PersonalAreaTabItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FillStudentInformation();
        }

        private void TimetableTabItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var header = (sender as TabItem).Header.ToString();
            
            if (header != "Занятия" || header != "Выход")
            {
                DirectoryTabItemFill(header);
            }
            if(header == "Панель" || header == "Админ панель")
            {
                DirectoryTabItemFill("Занятия");
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            using (DataContext db = new DataContext())
            {
                List<Lesson> lessonsList;
                List<LessonReplace> lessonsListReplace;
                if (db.Group.Where(x => x.Name == SearchTextBox.Text).Count() > 0)
                {
                    var group = db.Group.Where(x => x.Name == SearchTextBox.Text).First();
                    lessonsList = db.Lesson.Where(x => x.GroupId == group.Id).ToList();
                    lessonsListReplace = db.LessonReplace.Where(x => x.GroupId == group.Id).ToList();
                    ClearTimetableOfClasses();
                    FillTimetableOfClasses(lessonsList);
                    FillTimetableOfClasses(lessonsListReplace);
                }
                else
                if (db.ClassNumber.Where(x => x.Number.ToString() == SearchTextBox.Text).Count() > 0)
                {
                    var classNumber = db.ClassNumber.Where(x => x.Number.ToString() == SearchTextBox.Text).First();
                    lessonsList = db.Lesson.Where(x => x.ClassNumberId == classNumber.Id).ToList();
                    lessonsListReplace = db.LessonReplace.Where(x => x.ClassNumberId == classNumber.Id).ToList();
                    ClearTimetableOfClasses();
                    FillTimetableOfClasses(lessonsList);
                    FillTimetableOfClasses(lessonsListReplace);
                }
                else
                if (db.User.Where(x => x.Surname == SearchTextBox.Text).Count() > 0)
                {
                     var userId = db.User.Where(x => x.Surname == SearchTextBox.Text).First();
                        var lessons = db.LessonName.Where(x => x.UserId == userId.Id).ToList();

                        ClearTimetableOfClasses();

                        foreach (var less in lessons)
                        {
                            lessonsList = db.Lesson.Where(x => x.LessonNameId == less.Id && db.LessonReplace.Where(p => p.LessonId == x.Id).Count() == 0).ToList();
                            lessonsListReplace = db.LessonReplace.Where(x => x.LessonNameId == less.Id).ToList();
                            FillTimetableOfClasses(lessonsList);
                            FillTimetableOfClasses(lessonsListReplace);
                        }
                }
                else
                if (db.LessonName.Where(x => x.Name == SearchTextBox.Text).Count() > 0)
                {
                    var lessonName = db.LessonName.Where(x => x.Name == SearchTextBox.Text).First();
                    if (db.Lesson.Where(x => x.LessonNameId == lessonName.Id).Count() > 0)
                    {
                        var lesson = db.Lesson.Where(x => x.LessonNameId == lessonName.Id).First();
                        lessonsList = db.Lesson.Where(x => x.LessonNameId == lesson.LessonNameId && x.GroupId == lesson.GroupId && db.LessonReplace.Where(p => p.LessonId == x.Id).Count() == 0).ToList();
                        lessonsListReplace = db.LessonReplace.Where(x => x.LessonNameId == lesson.LessonNameId && x.GroupId == lesson.GroupId).ToList();
                        ClearTimetableOfClasses();
                        FillTimetableOfClasses(lessonsList);
                        FillTimetableOfClasses(lessonsListReplace);
                    }
                    else
                        MessageBox.Show("Заданный параметр не существует");
                }
                else
                   MessageBox.Show("Заданный параметр не существует");
            }
        }

        private void ClearTimetableOfClasses()
        {
            try
            {
                foreach (var control in TimetableGrid.Children)
                {
                    if (control is TextBox)
                    {
                        TextBox textBox = (TextBox)control;
                        textBox.Background = Brushes.White;
                        textBox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void FillTimetableOfClasses(List<Lesson> lessonsList)
        {
            using (DataContext db = new DataContext())
            {
                foreach (var item in lessonsList)
                {

                    foreach (var control in TimetableGrid.Children)
                    {
                        if (control is FrameworkElement)
                        {
                            if (control is TextBox)
                            {
                                TextBox textBox = (TextBox)control;

                                string dayName = DayConverter(db.Day.Where(x => x.Id == item.DayId).First());
                                int numberLesson = LessonNumberConverter(db.LessonNumber.Where(x => x.Id == item.LessonNumberId).First());

                                if (textBox.Name == "IdTextBox" + dayName + numberLesson)
                                {
                                    textBox.Text = item.Id.ToString();
                                }
                                if (textBox.Name == "LessonNameTextBox" + dayName + numberLesson)
                                {
                                    var lessonName = db.LessonName.Find(item.LessonNameId);
                                    textBox.Text = lessonName.Name;
                                }
                                if (textBox.Name == "GroupTextBox" + dayName + numberLesson)
                                {
                                    var groupName = db.Group.Find(item.GroupId);
                                    textBox.Text = groupName.Name;
                                }
                                if (textBox.Name == "ClassNumberTextBox" + dayName + numberLesson)
                                {
                                    var classNumberName = db.ClassNumber.Find(item.ClassNumberId);
                                    textBox.Text = classNumberName.Number.ToString();
                                }
                                if (textBox.Name == "TeacherNameTextBox" + dayName + numberLesson)
                                {
                                    var teacherId = db.LessonName.Find(item.LessonNameId);
                                    var teacherName = db.User.Find(teacherId.UserId);
                                    textBox.Text = teacherName.Surname + " " + teacherName.Name[0] + "." + teacherName.Patronymic[0] + ".";
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FillTimetableOfClasses(List<LessonReplace> lessonsList)
        {
            using (DataContext db = new DataContext())
            {
                foreach (var item in lessonsList)
                {

                    foreach (var control in TimetableGrid.Children)
                    {
                        if (control is FrameworkElement)
                        {
                            if (control is TextBox)
                            {
                                TextBox textBox = (TextBox)control;

                                string dayName = DayConverter(db.Day.Where(x => x.Id == item.DayId).First());
                                int numberLesson = LessonNumberConverter(db.LessonNumber.Where(x => x.Id == item.LessonNumberId).First());

                                if (textBox.Name == "IdTextBox" + dayName + numberLesson)
                                {
                                    textBox.Text = item.Id.ToString();
                                    textBox.Background = Brushes.LightGreen;
                                }
                                if (textBox.Name == "LessonNameTextBox" + dayName + numberLesson)
                                {
                                    var lessonName = db.LessonName.Find(item.LessonNameId);
                                    textBox.Text = lessonName.Name;
                                        textBox.Background = Brushes.LightGreen;
                                }
                                if (textBox.Name == "GroupTextBox" + dayName + numberLesson)
                                {
                                    var groupName = db.Group.Find(item.GroupId);
                                    textBox.Text = groupName.Name;
                                        textBox.Background = Brushes.LightGreen;
                                }
                                if (textBox.Name == "ClassNumberTextBox" + dayName + numberLesson)
                                {
                                    var classNumberName = db.ClassNumber.Find(item.ClassNumberId);
                                    textBox.Text = classNumberName.Number.ToString();
                                        textBox.Background = Brushes.LightGreen;
                                }
                                if (textBox.Name == "TeacherNameTextBox" + dayName + numberLesson)
                                {
                                    var teacherId = db.LessonName.Find(item.LessonNameId);
                                    var teacherName = db.User.Find(teacherId.UserId);
                                        textBox.Background = Brushes.LightGreen;
                                    textBox.Text = teacherName.Surname + " " + teacherName.Name[0] + "." + teacherName.Patronymic[0] + ".";
                                }
                            }
                        }
                    }
                }
            }
        }
         private void SearchTimetable_Click(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text != "")
            {
                using (DataContext db = new DataContext())
                {
                    List<Lesson> lessonsList;
                    List<LessonReplace> lessonsListReplace;
                    if (textBox.Name[0] == 'G')
                    {
                        var group = db.Group.Where(x => x.Name == textBox.Text).First();
                        lessonsList = db.Lesson.Where(x => x.GroupId == group.Id).ToList();
                        lessonsListReplace = db.LessonReplace.Where(x => x.GroupId == group.Id).ToList();
                        ClearTimetableOfClasses();
                        FillTimetableOfClasses(lessonsList);
                        FillTimetableOfClasses(lessonsListReplace);
                    }
                    else
                    if (textBox.Name[0] == 'C')
                    {
                        var classNumber = db.ClassNumber.Where(x => x.Number.ToString() == textBox.Text).First();
                        lessonsList = db.Lesson.Where(x => x.ClassNumberId == classNumber.Id).ToList();
                        lessonsListReplace = db.LessonReplace.Where(x => x.ClassNumberId == classNumber.Id).ToList();
                        ClearTimetableOfClasses();
                        FillTimetableOfClasses(lessonsList);
                        FillTimetableOfClasses(lessonsListReplace);
                    }
                    else
                    if (textBox.Name[0] == 'L')
                    {
                        var date = textBox.Name.Remove(0,17);
                        var TextBoxId = (TextBox)this.FindName(string.Concat("IdTextBox", date));

                        if (TextBoxId.Background.ToString() == "#FFFFFFFF")
                        {
                            var lesson = db.Lesson.Where(x => x.Id.ToString() == TextBoxId.Text).First();
                            lessonsList = db.Lesson.Where(x => x.LessonNameId == lesson.LessonNameId && x.GroupId == lesson.GroupId && db.LessonReplace.Where(p => p.LessonId == x.Id).Count() == 0).ToList();
                            lessonsListReplace = db.LessonReplace.Where(x => x.LessonNameId == lesson.LessonNameId && x.GroupId == lesson.GroupId).ToList();
                            ClearTimetableOfClasses();
                            FillTimetableOfClasses(lessonsList);
                            FillTimetableOfClasses(lessonsListReplace);
                        }
                        else 
                        if(TextBoxId.Background.ToString() == "#FF90EE90")
                        {
                            var lesson = db.LessonReplace.Where(x => x.Id.ToString() == TextBoxId.Text).First();
                            lessonsList = db.Lesson.Where(x => x.LessonNameId == lesson.LessonNameId && x.GroupId == lesson.GroupId && db.LessonReplace.Where(p => p.LessonId == x.Id).Count() == 0).ToList();
                            lessonsListReplace = db.LessonReplace.Where(x => x.LessonNameId == lesson.LessonNameId && x.GroupId == lesson.GroupId).ToList();
                            ClearTimetableOfClasses();
                            FillTimetableOfClasses(lessonsList);
                            FillTimetableOfClasses(lessonsListReplace);
                        }
                    }
                    else
                    if (textBox.Name[0] == 'T'){
                        var date = textBox.Name.Remove(0, 18);
                        var TextBoxId = (TextBox)this.FindName(string.Concat("IdTextBox", date));
                        if (TextBoxId.Background.ToString() == "#FFFFFFFF")
                        {
                            var lesson = db.Lesson.Where(x => x.Id.ToString() == TextBoxId.Text).First();
                            var teacherId = db.LessonName.Where(x => x.Id == lesson.LessonNameId).First();
                            var lessons = db.LessonName.Where(x => x.UserId == teacherId.UserId).ToList();

                            ClearTimetableOfClasses();

                            foreach (var less in lessons)
                            {
                                lessonsList = db.Lesson.Where(x => x.LessonNameId == less.Id && db.LessonReplace.Where(p => p.LessonId == x.Id).Count() == 0).ToList();
                                lessonsListReplace = db.LessonReplace.Where(x => x.LessonNameId == less.Id).ToList();
                                FillTimetableOfClasses(lessonsList);
                                FillTimetableOfClasses(lessonsListReplace);
                            }
                        }
                        else
                        if (TextBoxId.Background.ToString() == "#FF90EE90")
                        {
                            var lesson = db.LessonReplace.Where(x => x.Id.ToString() == TextBoxId.Text).First();
                            var teacherId = db.LessonName.Where(x => x.Id == lesson.LessonNameId).First();
                            var lessons = db.LessonName.Where(x => x.UserId == teacherId.UserId).ToList();

                            ClearTimetableOfClasses();

                            foreach (var less in lessons)
                            {
                                lessonsList = db.Lesson.Where(x => x.LessonNameId == less.Id && db.LessonReplace.Where(p => p.LessonId == x.Id).Count() == 0).ToList();
                                lessonsListReplace = db.LessonReplace.Where(x => x.LessonNameId == less.Id).ToList();
                                FillTimetableOfClasses(lessonsList);
                                FillTimetableOfClasses(lessonsListReplace);
                            }
                        }
                    }
                }
            }
            e.Handled = false;
        }

        private string DayConverter(Day lesson)
        {
            string day = "";

            if (lesson.Name == "Понедельник")
                return "Mon";
            else
            if(lesson.Name == "Вторник")
                return "Tue";
            else
            if (lesson.Name == "Среда")
                return "Wed";
            else
            if (lesson.Name == "Четверг")
                return "Thu";
            else
            if (lesson.Name == "Пятница")
                return "Fri";
            else
            if (lesson.Name == "Суббота")
                return "Sat";

            return day;
        }

        private int LessonNumberConverter(LessonNumber lesson)
        {
            int Number = 0;

            if (lesson.Number == 1)
                return 1;
            else
            if (lesson.Number == 2)
                return 2;
            else
            if (lesson.Number == 3)
                return 3;
            else
            if (lesson.Number == 4)
                return 4;
            else
            if (lesson.Number == 5)
                return 5;
            else
            if (lesson.Number == 6)
                return 6;
            else
            if (lesson.Number == 7)
                return 7;
            else
            if (lesson.Number == 8)
                return 8;

            return Number;
        }
        private void ClassNumberAddButton_Click(object sender, RoutedEventArgs e)
        {
            ClassNumberWindow window = new ClassNumberWindow(User);
            window.Closed += new EventHandler((_s, _e) =>
            {
                DirectoryTabItemFill("Кабинет");
            });
            window.Show();
            classNumberDataGrid.Items.Refresh();
        }



        private void ClassNumberDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (classNumberDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = classNumberDataGrid.ItemsSource as List<ClassNumber>;
                var item = classNumberDataGrid.SelectedItem as ClassNumber;

                using (DataContext db = new DataContext())
                {
                    var classNumber = db.ClassNumber.Find(item.Id);
                    if (db.Lesson.Where(x => x.ClassNumberId == item.Id).Count() > 0)
                        throw new ArgumentException("Удалить выбранный вами класс удалить невозможно");
                    db.ClassNumber.Remove(classNumber);
                    db.SaveChanges();
                }

                items.Remove(item);

                classNumberDataGrid.ItemsSource = items;
                classNumberDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClassNumberChangeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (classNumberDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = classNumberDataGrid.ItemsSource as List<ClassNumber>;
                var item = classNumberDataGrid.SelectedItem as ClassNumber;

                ClassNumberWindow window = new ClassNumberWindow(User, item.Id);
                window.Closed += new EventHandler((_s, _e) =>
                {
                    DirectoryTabItemFill("Кабинет");
                });
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GroupAddButton_Click(object sender, RoutedEventArgs e)
        {
            GroupWindow window = new GroupWindow(User);
            window.Closed += new EventHandler((_s, _e) =>
            {
                DirectoryTabItemFill("Группа");
            });
            window.Show();
            groupDataGrid.Items.Refresh();
        }

        private void GroupDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (groupDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = groupDataGrid.ItemsSource as List<GroupInformation>;
                var item = groupDataGrid.SelectedItem as GroupInformation;

                using (DataContext db = new DataContext())
                {
                    if (db.Lesson.Where(x => x.GroupId == item.Id).Count() > 0)
                        throw new ArgumentException("Выбранную вами группу удалить невозможно");

                    var group = db.Group.Find(item.Id);

                    db.Group.Remove(group);
                    db.SaveChanges();
                }

                items.Remove(item);

                groupDataGrid.ItemsSource = items;
                groupDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void GroupChangeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (groupDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = groupDataGrid.ItemsSource as List<GroupInformation>;
                var item = groupDataGrid.SelectedItem as GroupInformation;

                GroupWindow window = new GroupWindow(User, item.Id);
                window.Closed += new EventHandler((_s, _e) =>
                {
                    DirectoryTabItemFill("Группа");
                });
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StudentAddButton_Click(object sender, RoutedEventArgs e)
        {
            StudentWindow window = new StudentWindow(User);
            window.Closed += new EventHandler((_s, _e) =>
            {
                DirectoryTabItemFill("Студент");
            });
            window.Show();
            studentDataGrid.Items.Refresh();
        }

        private void StudentDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (studentDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = studentDataGrid.ItemsSource as List<User>;
                var item = studentDataGrid.SelectedItem as User;

                using (DataContext db = new DataContext())
                {

                    var student = db.User.Find(item.Id);

                    db.User.Remove(student);
                    db.SaveChanges();
                }

                items.Remove(item);

                studentDataGrid.ItemsSource = items;
                studentDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StudentChangeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (studentDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = studentDataGrid.ItemsSource as List<User>;
                var item = studentDataGrid.SelectedItem as User;

                StudentWindow window = new StudentWindow(User, item.Id);
                window.Closed += new EventHandler((_s, _e) =>
                {
                    DirectoryTabItemFill("Студент");
                });
                window.Show();
                studentDataGrid.ItemsSource = items;
                studentDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TeacherAddButton_Click(object sender, RoutedEventArgs e)
        {
            TeacherWindow window = new TeacherWindow(User);
            window.Closed += new EventHandler((_s, _e) =>
            {
                DirectoryTabItemFill("Преподаватель");
            });
            window.Show();
            teacherDataGrid.Items.Refresh();
        }

        private void TeacherDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (studentDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = teacherDataGrid.ItemsSource as List<User>;
                var item = teacherDataGrid.SelectedItem as User;

                using (DataContext db = new DataContext())
                {
                    var teacher = db.User.Find(item.Id);

                    db.User.Remove(teacher);
                    db.SaveChanges();
                }

                items.Remove(item);

                teacherDataGrid.ItemsSource = items;
                teacherDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TeacherChangeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (teacherDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = teacherDataGrid.ItemsSource as List<User>;
                var item = teacherDataGrid.SelectedItem as User;

                TeacherWindow window = new TeacherWindow(User, item.Id);
                window.Closed += new EventHandler((_s, _e) =>
                {
                    DirectoryTabItemFill("Преподаватель");
                });
                window.Show();
                teacherDataGrid.ItemsSource = items;
                teacherDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AdminAddButton_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow window = new AdminWindow(User);
            window.Closed += new EventHandler((_s, _e) =>
            {
                DirectoryTabItemFill("Администратор");
            });
            window.Show();
            adminDataGrid.Items.Refresh();
        }

        private void AdminDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (adminDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = adminDataGrid.ItemsSource as List<User>;
                var item = adminDataGrid.SelectedItem as User;

                using (DataContext db = new DataContext())
                {
                    var admin = db.User.Find(item.Id);

                    db.User.Remove(admin);
                    db.SaveChanges();
                }

                items.Remove(item);

                adminDataGrid.ItemsSource = items;
                adminDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AdminChangeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (adminDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = adminDataGrid.ItemsSource as List<User>;
                var item = adminDataGrid.SelectedItem as User;

                AdminWindow window = new AdminWindow(User, item.Id);
                window.Closed += new EventHandler((_s, _e) =>
                {
                    DirectoryTabItemFill("Администратор");
                });
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LessonAddButton_Click(object sender, RoutedEventArgs e)
        {
            LessonsWindow window = new LessonsWindow(User);
            window.Closed += new EventHandler((_s, _e) =>
            {
                DirectoryTabItemFill("Занятия");
            });
            window.Show();
            lessonDataGrid.Items.Refresh();

            using (DataContext db = new DataContext())
            {
                var UserId = db.User.Find(User.Id);
                var GroupId = db.Group.Find(UserId.GroupId);
                var LessonsList = db.Lesson.Where(x => x.GroupId == GroupId.Id).ToList();
                var LessonsListReplace = db.LessonReplace.Where(x => x.GroupId == GroupId.Id).ToList();
                ClearTimetableOfClasses();
                FillTimetableOfClasses(LessonsList);
                FillTimetableOfClasses(LessonsListReplace);
                FillStudentInformation();
            }
        }

        private void LessonDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lessonDataGrid.SelectedItem == null)
                    throw new ArgumentException("Выберите строку");

                var items = lessonDataGrid.ItemsSource as List<Lesson>;
                var item = lessonDataGrid.SelectedItem as Lesson;

                using (DataContext db = new DataContext())
                {
                    var lesson = db.Lesson.Find(item.Id);

                    if (db.LessonReplace.Where(x => x.LessonId == item.Id).Count() > 0)
                        throw new ArgumentException("Ошибка. Невозможно удалить выбранное вами занятие.");

                    db.Lesson.Remove(lesson);
                    db.SaveChanges();

                    var UserId = db.User.Find(User.Id);
                    var GroupId = db.Group.Find(UserId.GroupId);
                    var LessonsList = db.Lesson.Where(x => x.GroupId == GroupId.Id).ToList();
                    var LessonsListReplace = db.LessonReplace.Where(x => x.GroupId == GroupId.Id).ToList();
                    ClearTimetableOfClasses();
                    FillTimetableOfClasses(LessonsList);
                    FillTimetableOfClasses(LessonsListReplace);
                    FillStudentInformation();
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

                var items = lessonDataGrid.ItemsSource as List<Lesson>;
                var item = lessonDataGrid.SelectedItem as Lesson;

                LessonsWindow window = new LessonsWindow(User, item.Id);
                window.Closed += new EventHandler((_s, _e) =>
                {
                    DirectoryTabItemFill("Занятия");
                });
                window.Show();

                using (DataContext db = new DataContext())
                {
                    var UserId = db.User.Find(User.Id);
                    var GroupId = db.Group.Find(UserId.GroupId);
                    var LessonsList = db.Lesson.Where(x => x.GroupId == GroupId.Id).ToList();
                    var LessonsListReplace = db.LessonReplace.Where(x => x.GroupId == GroupId.Id).ToList();
                    ClearTimetableOfClasses();
                    FillTimetableOfClasses(LessonsList);
                    FillTimetableOfClasses(LessonsListReplace);
                    FillStudentInformation();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AmoutOfHourseButton_Click(object sender, RoutedEventArgs e)
        {
            AmoutOfHourseWindow window = new AmoutOfHourseWindow(User);
            window.Closed += new EventHandler((_s, _e) =>
            {
                DirectoryTabItemFill("Группа");
            });
            groupDataGrid.Items.Refresh();
            window.Show();
        }

        private void FillStudentInformation()
        {
            try
            {
                using (DataContext db = new DataContext())
                {
                    var userInfo = db.User.FirstOrDefault(x => x.Id == User.Id);

                    if (userInfo != null)
                    {
                        SurnameTextBox.Text = userInfo.Surname;
                        SurnameTextBox.IsReadOnly = true;
                        NameTextBox.Text = userInfo.Name;
                        NameTextBox.IsReadOnly = true;
                        PatronymicTextBox.Text = userInfo.Patronymic;
                        PatronymicTextBox.IsReadOnly = true;
                        GroupTextBox.Text = db.Group.Find(userInfo.GroupId).Name;
                        GroupTextBox.IsReadOnly = true;
                    }
                    else
                    {
                        throw new ArgumentException(" ");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void DirectoryTabItemFill(string header)
        {
            using (DataContext db = new DataContext())
            {
                if (header == "Кабинет")
                {
                    var items = db.ClassNumber.ToList();
                    classNumberDataGrid.ItemsSource = items;
                }
                else
                if (header == "Группа")
                {
                    var items = db.Group.Where(x => x.Name != "Admin group" && x.Name != "Teacher group").ToList();
                    var groupInformation = new List<GroupInformation>();
                    foreach (var item in items)
                    {
                        var count = db.Lesson.Where(x => x.GroupId == item.Id).Count();
                        groupInformation.Add(new GroupInformation
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Count = count * 2
                        });
                    }

                    groupDataGrid.ItemsSource = groupInformation;
                }
                else
                if (header == "Ученик")
                {
                    var rootId = db.Root.Where(x => x.Name == "Студент").Select(x => x.Id).First();
                    var items = db.User.Where(x => x.RootId == rootId).ToList();
                    studentDataGrid.ItemsSource = items;
                }
                else
                if (header == "Преподаватель")
                {
                    var rootId = db.Root.Where(x => x.Name == "Преподаватель").Select(x => x.Id).First();
                    var items = db.User.Where(x => x.RootId == rootId).ToList();
                    teacherDataGrid.ItemsSource = items;
                }
                else
                if (header == "Администратор")
                {
                    var rootId = db.Root.Where(x => x.Name == "Администратор").Select(x => x.Id).First();
                    var items = db.User.Where(x => x.RootId == rootId).ToList();
                    adminDataGrid.ItemsSource = items;
                }
                else
                if (header == "Занятия")
                {
                    if (User.RootId == db.Root.Where(x => x.Name == "Администратор").First().Id) 
                    {
                        var items = db.Lesson.ToList();
                        lessonDataGrid.ItemsSource = items;

                        var lessonReplace = db.LessonReplace.ToList();
                        
                        foreach (var lessReplace in lessonReplace)
                        {
                            foreach (var item in items)
                            {
                                if (lessReplace.LessonId == item.Id)
                                {
                                    lessonDataGrid.UpdateLayout();
                                    var row = lessonDataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;

                                    if (row != null)
                                    {
                                        row.Background = Brushes.LightGreen;
                                    }
                                }
                            }
                        }
                        
                    }
                    else
                    if (User.RootId == db.Root.Where(x => x.Name == "Преподаватель").First().Id)
                    {
                        var teacherLessons = db.LessonName.Where(i => i.UserId == User.Id).ToList();
                        var lessons = db.Lesson.ToList();
                        List<Lesson> teacherLess =  new List<Lesson>();
                        foreach(var teacherLesson in teacherLessons)
                        {
                            foreach(var lesson in lessons)
                            {
                                if(lesson.LessonNameId == teacherLesson.Id)
                                {
                                    teacherLess.Add(lesson);
                                }
                            }
                        }

                        lessonDataGrid.ItemsSource = teacherLess;
                         
                        var lessonReplace = db.LessonReplace.ToList();

                        foreach (var lessReplace in lessonReplace)
                        {
                            foreach (var item in teacherLess)
                            {
                                if (lessReplace.LessonId == item.Id)
                                {
                                    lessonDataGrid.UpdateLayout();
                                    var row = lessonDataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                                    if (row != null)
                                    {
                                        row.Background = Brushes.LightGreen;
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (DataContext db = new DataContext())
            {
                if (db.Day.Count() != 6)
                {
                    db.Day.RemoveRange(db.Day.ToList());

                    db.Day.AddRange(new List<Day>() {new Day()
                    {
                        Name = "Понедельник"
                    },
                    new Day()
                    {
                        Name = "Вторник"
                    },

                    new Day()
                    {
                        Name = "Среда"
                    },
                    new Day()
                    {
                        Name = "Четверг"
                    },
                    new Day()
                    {
                        Name = "Пятница"
                    },
                    new Day()
                    {
                        Name = "Суббота"
                    }});

                    db.SaveChanges();
                }

                if (db.LessonNumber.Count() != 8)
                {
                    db.LessonNumber.RemoveRange(db.LessonNumber.ToList());

                    db.LessonNumber.AddRange(new List<LessonNumber>()
                    {
                        new LessonNumber()
                        {
                            Number = 1
                        },
                        new LessonNumber()
                        {
                            Number = 2
                        },
                        new LessonNumber()
                        {
                            Number = 3
                        },
                        new LessonNumber()
                        {
                            Number = 4
                        },
                        new LessonNumber()
                        {
                            Number = 5
                        },
                        new LessonNumber()
                        {
                            Number = 6
                        },
                        new LessonNumber()
                        {
                            Number = 7
                        },
                        new LessonNumber()
                        {
                            Number = 8
                        }
                    });

                    db.SaveChanges();
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            using (DataContext db = new DataContext())
            {
                Root root = db.Root.Where(x => User.RootId == x.Id).First();
                if (root.Name == "Студент")
                {
                    if (e.Key == Key.F1)
                    {
                        string pathDocument = Environment.CurrentDirectory + "\\help_student.chm";
                        System.Diagnostics.Process.Start(pathDocument);
                    }
                }
                else
            if (root.Name == "Преподаватель")
                {
                    if (e.Key == Key.F1)
                    {
                        string pathDocument = Environment.CurrentDirectory + "\\help_teacher.chm";
                        System.Diagnostics.Process.Start(pathDocument);
                    }
                }
                else
            if (root.Name == "Администратор")
                {
                if (e.Key == Key.F1)
                    {
                        string pathDocument = Environment.CurrentDirectory + "\\help_admin.chm";
                        System.Diagnostics.Process.Start(pathDocument);
                    }
                }
            }
        }
        }
    }

