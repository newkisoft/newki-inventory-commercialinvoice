using Microsoft.EntityFrameworkCore;
using newkilibraries;

namespace newki_inventory_commercialinvoice
{
     public class ApplicationDbContext : DbContext
     {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AgentCustomer>().HasKey(sc => new { sc.CustomerId, sc.AgentId });
            builder.Entity<CommercialInvoiceDocumentFile>().HasKey(sc => new { sc.CommercialInvoiceId, sc.DocumentFileId });
            builder.Entity<CommercialInvoiceDataView>().HasKey(sc => new { sc.CommercialInvoiceId });
            builder.Entity<CommercialInvoiceExtra>().HasKey(p=>p.CommercialInvoiceExtraId);
        }
        public DbSet<CommercialInvoice> CommercialInvoice { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<DocumentFile> DocumentFile{get;set;}
        public DbSet<CommercialInvoiceDocumentFile> CommercialInvoiceDocumentFile{get;set;}
        public DbSet<CommercialInvoiceDataView> CommercialInvoiceDataView{get;set;}
        public DbSet<CommercialInvoiceExtra> CommercialInvoiceExtra{get;set;}
        public DbSet<RequestStatus> RequestStatus{get;set;}
        
    }
}