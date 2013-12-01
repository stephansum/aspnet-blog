﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BlackMesa.Blog.Main.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [DefaultValue("Anon")]
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(254)]
        public string Email { get; set; }

        [Required]
        public int EntryId { get; set; }

        [Required]
        public Entry Entry { get; set; }
    }
}