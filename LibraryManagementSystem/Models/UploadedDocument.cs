﻿namespace LibraryManagementSystem.Models
{
    public class UploadedDocument
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsEncrypted { get; set; } = true;
    }
}