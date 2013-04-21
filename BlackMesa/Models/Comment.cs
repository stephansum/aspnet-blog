﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlackMesa.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        
        public string Email { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public int EntryId { get; set; }  // Having both the ForeignKey and the navigation property in place, will make EntryId a not nullable ForeignKey in the database
        public virtual Entry Entry { get; set; }
    }
}