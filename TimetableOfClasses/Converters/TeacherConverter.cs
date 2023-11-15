using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TimetableOfClasses.Converters
{
    class TeacherConverter : IValueConverter
    {
        int id;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            id = (int)value;

            using (DataContext db = new DataContext())
                return $"{db.User.Find(id).Surname + " " + db.User.Find(id).Name[0] + "." + db.User.Find(id).Patronymic[0] + "."}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return id;
        }
    }
}
