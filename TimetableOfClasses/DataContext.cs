using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableOfClasses
{
    class DataContext : DbContext
    {
        public DataContext()
            : base("name=DataContext")
        {
        }

        public virtual DbSet<ClassNumber> ClassNumber { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<Lesson> Lesson { get; set; }
        public virtual DbSet<LessonName> LessonName { get; set; }
        public virtual DbSet<LessonReplace> LessonReplace { get; set; }
        public virtual DbSet<LessonNumber> LessonNumber { get; set; }
        public virtual DbSet<Root> Root { get; set; }
        public virtual DbSet<Day> Day { get; set; }
        public virtual DbSet<User> User { get; set; }
    }
}
