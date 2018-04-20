using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace C_SharpBelt.Models
{
    public class User : BaseEntity
    {
        [Key]
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public User()
        {
            CreatedActivities = new List<Activitie>();
            Participating = new List<Participant>();
        }
        public List<Activitie> CreatedActivities { get; set; }
        public List<Participant> Participating { get; set; }
    }
}