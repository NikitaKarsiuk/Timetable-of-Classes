namespace TimetableOfClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LessonReplace")]
    public partial class LessonReplace
    {
        public int Id { get; set; }

        public int DayId { get; set; }
        public int ClassNumberId { get; set; }
        public int GroupId { get; set; }
        public int LessonNumberId { get; set; }
        public int LessonNameId { get; set; }
        public int LessonId { get; set; }
        public virtual Day Day { get; set; }
        public virtual Group Group { get; set; }
        public virtual LessonNumber LessonNumber { get; set; }
        public virtual LessonName LessonName { get; set; }
        public virtual ClassNumber ClassNumber { get; set; }
        public virtual Lesson Lesson { get; set; }
    }
}
