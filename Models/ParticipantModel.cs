using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace C_SharpBelt.Models
{
    public class Participant : BaseEntity
    {
        [Key]
        public int id { get; set; }
        public int usersId { get; set; }
        public User users { get; set; }
        public int activitiesId { get; set; }
        public Activitie activities { get; set; }
    }
}