using System;
using System.ComponentModel.DataAnnotations;

namespace C_SharpBelt.Models
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
        }
        [DataType(DataType.Date)]     
        public DateTime created_at { get; set; }

        [DataType(DataType.Date)]      
        public DateTime updated_at { get; set; }
    }
}