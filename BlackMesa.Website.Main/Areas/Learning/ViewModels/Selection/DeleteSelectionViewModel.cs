﻿using System.Collections.Generic;

namespace BlackMesa.Website.Main.Areas.Learning.ViewModels.Selection
{
    public class DeleteSelectionViewModel
    {
        public string Id { get; set; }
        public List<Models.Learning.Folder> SelectedFolders { get; set; }
        public List<Models.Learning.IndexCard> SelectedIndexCards { get; set; }
        public int AffectedFolders { get; set; }
        public int AffectedIndexCards { get; set; }
    }
}