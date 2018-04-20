using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace C_SharpBelt.Models
{
    public class Activitie : BaseEntity
    {
        [Key]
        public int id { get; set; }
        public string title { get; set; }
        public DateTime startDate { get; set; }
        public string duration { get; set; }
        public string description { get; set; }
        public int userId { get; set; }
        public User user { get; set; }
        public Activitie()
        {
            Participants = new List<Participant>();
        }
        public List<Participant> Participants { get; set; }
    }
}