using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace C_SharpBelt.Models
{
    public class ActivitieCreate
    {
        [Required]
        [MinLength(2, ErrorMessage = "Activity title must be longer than 2 characters")]
        [Display(Name = "Title: ")]
        public string title { get; set; }
        [Required]
        [Display(Name = "Date: ")]
        public DateTime startDate { get; set; }
        [Required]
        [Display(Name = "Duration: ")]
        public int duration { get; set; }
        [Required]
        [MinLength(10, ErrorMessage = "Description must be longer than 10 characters")]
        [Display(Name = "Description: ")]
        public string description { get; set; }
    }
}