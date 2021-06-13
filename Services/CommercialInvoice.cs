
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using newkilibraries;

namespace newki_inventory_commercialinvoice
{
    public interface ICommercialInvoiceService
    {
        List<CommercialInvoice> GetCommercialInvoices();
        CommercialInvoice GetCommercialInvoice(int id);
        CommercialInvoice Insert(CommercialInvoice CommercialInvoice);
        CommercialInvoice Update(CommercialInvoice CommercialInvoice);
        int Delete(int CommercialInvoiceId);
    }
    public class CommercialInvoiceService : ICommercialInvoiceService
    {
        private readonly ApplicationDbContext _context;
        private const string bucketName = "newki";

        public CommercialInvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<CommercialInvoice> GetCommercialInvoices()
        {
            List<CommercialInvoice> Items = _context.CommercialInvoice
            .OrderByDescending(p => p.CommercialInvoiceDate)
            .Include(p => p.Files).ThenInclude(w => w.File).ToList();
            return Items;
        }

        public CommercialInvoice GetCommercialInvoice(int id)
        {
            var Item = _context.CommercialInvoice
            .Include(p => p.Files).ThenInclude(w => w.File)
            .FirstOrDefault(p => p.CommercialInvoiceId == id);
            return Item;
        }

        public CommercialInvoice Insert(CommercialInvoice CommercialInvoice)
        {



            _context.CommercialInvoice.Add(CommercialInvoice);
            _context.SaveChanges();
            return _context.CommercialInvoice
                            .FirstOrDefault(p => p.CommercialInvoiceId == CommercialInvoice.CommercialInvoiceId);
        }


        public CommercialInvoice Update(CommercialInvoice CommercialInvoice)
        {
            var oldCommercialInvoice = _context.CommercialInvoice.Include(p => p.Extras)
                                                                 .Include(p => p.Files).ThenInclude(p => p.File)
                                                                 .FirstOrDefault(p => p.CommercialInvoiceId == CommercialInvoice.CommercialInvoiceId);

            foreach (var oldInvoiceDocument in oldCommercialInvoice.Files)
            {
                _context.DocumentFile.Remove(oldInvoiceDocument.File);
            }
            _context.CommercialInvoiceDocumentFile.RemoveRange(oldCommercialInvoice.Files);
            foreach (var extra in oldCommercialInvoice.Extras.ToList())
            {
                oldCommercialInvoice.Extras.Remove(extra);
                _context.CommercialInvoiceExtra.Remove(extra);
            }

            _context.SaveChanges();

            if (CommercialInvoice.Extras != null)
            {
                foreach (var extra in CommercialInvoice.Extras?.ToList())
                {
                    _context.CommercialInvoiceExtra.Add(extra);
                    oldCommercialInvoice.Extras.Add(extra);
                }
            }

            foreach (var file in CommercialInvoice.Files)
            {
                var oldFile = _context.DocumentFile.FirstOrDefault(p => p.DocumentFileId == file.DocumentFileId);
                if (oldFile == null)
                {
                    oldCommercialInvoice.Files.Add(file);
                }
                else
                {
                    var newCommercialInvoiceFile = new CommercialInvoiceDocumentFile();
                    newCommercialInvoiceFile.DocumentFileId = oldFile.DocumentFileId;
                    newCommercialInvoiceFile.CommercialInvoiceId = CommercialInvoice.CommercialInvoiceId;
                    oldCommercialInvoice.Files.Add(newCommercialInvoiceFile);
                }
            }


            oldCommercialInvoice.ExchangeRate = CommercialInvoice.ExchangeRate;
            oldCommercialInvoice.CommercialInvoiceDate = CommercialInvoice.CommercialInvoiceDate.ToLocalTime();
            oldCommercialInvoice.CommercialInvoiceDueDate = CommercialInvoice.CommercialInvoiceDueDate.ToLocalTime();
            oldCommercialInvoice.Paid = CommercialInvoice.Paid;
            oldCommercialInvoice.TotalUsd = CommercialInvoice.TotalUsd;
            oldCommercialInvoice.Kdv = CommercialInvoice.Kdv;
            oldCommercialInvoice.Tax = CommercialInvoice.Tax;
            oldCommercialInvoice.Discount = CommercialInvoice.Discount;
            oldCommercialInvoice.Currency = CommercialInvoice.Currency;
            oldCommercialInvoice.Comment = CommercialInvoice.Comment;
            oldCommercialInvoice.Seller = CommercialInvoice.Seller;
            oldCommercialInvoice.Buyer = CommercialInvoice.Buyer;
            oldCommercialInvoice.Consignee = CommercialInvoice.Consignee;
            oldCommercialInvoice.CountryOfBeneficiary = CommercialInvoice.CountryOfBeneficiary;
            oldCommercialInvoice.CountryOfDestination = CommercialInvoice.CountryOfDestination;
            oldCommercialInvoice.CountryOfOrigin = CommercialInvoice.CountryOfOrigin;
            oldCommercialInvoice.HsCode = CommercialInvoice.HsCode;
            oldCommercialInvoice.PackageDescription = CommercialInvoice.PackageDescription;
            oldCommercialInvoice.FreightForwarder = CommercialInvoice.FreightForwarder;
            oldCommercialInvoice.PartialShipment = CommercialInvoice.PartialShipment;
            oldCommercialInvoice.RelevantLocation = CommercialInvoice.RelevantLocation;
            oldCommercialInvoice.Size = CommercialInvoice.Size;
            oldCommercialInvoice.Port = CommercialInvoice.Port;
            oldCommercialInvoice.TermsOfDelivery = CommercialInvoice.TermsOfDelivery;
            oldCommercialInvoice.TermsOfPayment = CommercialInvoice.TermsOfPayment;
            oldCommercialInvoice.TotalGross = CommercialInvoice.TotalGross;
            oldCommercialInvoice.TransportBy = CommercialInvoice.TransportBy;
            oldCommercialInvoice.LoadingDate = CommercialInvoice.LoadingDate;
            oldCommercialInvoice.Extras = CommercialInvoice.Extras;

            _context.CommercialInvoice.Update(oldCommercialInvoice);

            _context.SaveChanges();
            return _context.CommercialInvoice
                        .FirstOrDefault(p => p.CommercialInvoiceId == oldCommercialInvoice.CommercialInvoiceId);
        }

        public int Delete(int CommercialInvoiceId)
        {
            var CommercialInvoiceOld = _context.CommercialInvoice
                .Where(x => x.CommercialInvoiceId == CommercialInvoiceId)
                .FirstOrDefault();

            _context.CommercialInvoice.Remove(CommercialInvoiceOld);
            _context.SaveChanges();
            return CommercialInvoiceId;

        }

    }
}
