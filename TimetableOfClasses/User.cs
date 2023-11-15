namespace TimetableOfClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Surname { get; set; }
        [Required]
        [StringLength(50)]
        public string Patronymic { get; set; }
        public int GroupId { get; set; }
        public int RootId { get; set; }
        public virtual Root Root { get; set; }
        public virtual Group Group { get; set; }
        [Required]
        [StringLength(50)]
        public string Login { get; set; }
        [Required]
        [StringLength(50)]
        public string Password { get; set; }

    }
}
