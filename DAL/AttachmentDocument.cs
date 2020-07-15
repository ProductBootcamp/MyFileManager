using System;
using System.Collections.Generic;

namespace FileStorage.DAL
{
    public partial class AttachmentDocument
    {
        public int Id { get; set; }
        public string DocPath { get; set; }
        public string FileName { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
