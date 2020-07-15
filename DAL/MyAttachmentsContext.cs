using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FileStorage.DAL
{
    public partial class MyAttachmentsContext : DbContext
    {
        public MyAttachmentsContext()
        {
        }

        public MyAttachmentsContext(DbContextOptions<MyAttachmentsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AttachmentDocument> AttachmentDocument { get; set; }

    }
}
