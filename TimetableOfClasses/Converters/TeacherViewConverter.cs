using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TimetableOfClasses.Converters
{
    class TeacherViewConverter : IValueConverter
    {
        int id;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            id = (int)value;

            using (DataContext db = new DataContext())
            {
                var userId = db.LessonName.Find(id).UserId;
                var user = db.User.Where(x => x.Id == userId).First();
                return user.Surname + " " + user.Name[0] + "." + user.Patronymic[0] + ".";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return id;
        }
    }
}
