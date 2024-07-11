using com.danliris.support.lib.Models;
using com.danliris.support.lib.Models.Ceisa.Master.HS;
using com.danliris.support.lib.Models.Ceisa.PEB;
using com.danliris.support.lib.Models.Ceisa.TPB;
using com.danliris.support.lib.Models.Machine;
using com.danliris.support.lib.ViewModel;
using Com.Moonlay.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace com.danliris.support.lib
{
    public class SupportDbContext : DbContext
    {
        public  DbSet<BEACUKAI_TEMP> BeacukaiTemp { get; set; }
        public DbSet<FactBeacukai> FactBeacukai { get; set; }
		public DbSet<ScrapViewModel> ViewScrap{ get; set; }
        public DbSet<ViewFactBeacukai> ViewFactBeacukai { get; set; }
		public DbSet<WIPViewModel> ViewWIP { get; set; }
        public   DbSet<HOrder> HOrder { get; set; }
        public DbSet<Machine> Machine { get; set; }
        public DbSet<MachineBrand> MachineBrand { get; set; }
        public DbSet<MachineCategory> MachineCategory { get; set; }
        public DbSet<MachineMutation> MachineMutation { get; set; }
        //PEB
        public DbSet<PEBHeader> PEBHeader { get; set; }
        public DbSet<PEBBarang> PEBBarang { get; set; }
        public DbSet<PEBDokumen> PEBDokumen { get; set; }
        public DbSet<PEBEntitas> PEBEntitas { get; set; }
        public DbSet<PEBKemasan> PEBKemasan { get; set; }
        public DbSet<BEACUKAI_ADDED> BEACUKAI_ADDED { get; set; }
        public DbSet<BEACUKAI_ADDED_DETAIL> BEACUKAI_ADDED_DETAIL { get; set; }
        //TPB
        public DbSet<TPBHeader> TPBHeader { get; set; }
        public DbSet<TPBDokumen>  TPBDokumen { get; set; }
        public DbSet<TPBBarang> TPBBarang { get; set; }
        public DbSet<TPBEntitas> TPBEntitas { get; set; }
        public DbSet<BeacukaiDocumentsModel> BeacukaiDocuments { get; set; }

        public DbSet<TPBBarang_Tarif> TPBBarang_Tarif { get; set; }
        public DbSet<TPBBarang_Dokumen> TPBBarang_Dokumen { get; set; }
        public DbSet<TPBBarang_BahanBaku> TPBBarang_BahanBaku { get; set; }
        public DbSet<TPBBarang_BahanBakuTarif> TPBBarang_BahanBakuTarif { get; set; }




        public DbSet<HSModel> HSCodes { get; set; }
        public SupportDbContext(DbContextOptions<SupportDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BEACUKAI_TEMP>(entity =>
            {
                entity.ToTable("BEACUKAI_TEMP");

    //            entity.Property(e => e.ID)
    //                .HasColumnName("ID")
    //                .ValueGeneratedNever();

    //            entity.Property(e => e.Barang)
    //                .HasMaxLength(100)
    //                .IsUnicode(false);

				//entity.Property(e => e.Tipe)
				//   .HasMaxLength(100)
				//   .IsUnicode(false);

				//entity.Property(e => e.BCId)
    //                .HasColumnName("BCId")
    //                .HasMaxLength(50)
    //                .IsUnicode(false);

    //            entity.Property(e => e.BCNo)
    //                .HasColumnName("BCNo")
    //                .HasMaxLength(50)
    //                .IsUnicode(false);

    //            entity.Property(e => e.Bruto).HasColumnType("decimal(12, 2)");

    //            entity.Property(e => e.CIF)
    //                .HasColumnName("CIF")
    //                .HasColumnType("decimal(20, 2)");

    //            entity.Property(e => e.CIF_Rupiah)
    //                .HasColumnName("CIF_Rupiah")
    //                .HasColumnType("decimal(20, 2)");

    //            entity.Property(e => e.Hari).HasColumnType("datetime");

    //            entity.Property(e => e.IDHeader).HasColumnName("IDHeader");

    //            entity.Property(e => e.JenisBC)
    //                .HasColumnName("JenisBC")
    //                .HasMaxLength(50)
    //                .IsUnicode(false);

    //            entity.Property(e => e.JenisDokumen)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);

    //            entity.Property(e => e.JumlahSatBarang).HasColumnType("decimal(12, 2)");

    //            entity.Property(e => e.KodeBarang)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);

    //            entity.Property(e => e.KodeDokumenPabean)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);

    //            entity.Property(e => e.KodeKemasan)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);

    //            entity.Property(e => e.KodeSupplier)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);

    //            entity.Property(e => e.NamaKemasan)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);

    //            entity.Property(e => e.NamaSupplier)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);

    //            entity.Property(e => e.Netto).HasColumnType("decimal(12, 2)");

    //            entity.Property(e => e.NoAju)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);

    //            entity.Property(e => e.NomorDokumen)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);

    //            entity.Property(e => e.Sat)
    //                .HasMaxLength(5)
    //                .IsUnicode(false);

    //            entity.Property(e => e.TanggalDokumen).HasColumnType("datetime");

    //            entity.Property(e => e.TglBCNo)
    //                .HasColumnName("TglBCNo")
    //                .HasColumnType("datetime");

    //            entity.Property(e => e.TglDaftarAju).HasColumnType("datetime");

    //            entity.Property(e => e.TglDatang).HasColumnType("datetime");

    //            entity.Property(e => e.Valuta)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);
    //            entity.Property(e => e.Vendor)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);
            });

            modelBuilder.Entity<FactBeacukai>(entity =>
            {
                entity.HasKey(e => e.DetailshippingOrderId);

                entity.Property(e => e.DetailshippingOrderId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.BCDate)
                    .HasColumnName("BCDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.BCId)
                    .HasColumnName("BCId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BCNo)
                    .HasColumnName("BCNo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BCType)
                    .HasColumnName("BCType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BonDate).HasColumnType("datetime");

                entity.Property(e => e.BonNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.ItemCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ItemName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nominal).HasColumnType("money");

                entity.Property(e => e.PricePo).HasColumnType("money");

                entity.Property(e => e.Rate).HasColumnType("money");

                entity.Property(e => e.SupllierCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UnitQtyName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

			modelBuilder.Entity<ScrapViewModel>(entity =>
			{
				 
				entity.Property(e => e.ClassificationId)
					.HasColumnName("ClassificationId");

				entity.Property(e => e.ClassificationId);
				 

				entity.Property(e => e.ClassificationCode);

				entity.Property(e => e.ClassificationName);

				entity.Property(e => e.DestinationId);

				entity.Property(e => e.UnitQtyName);

				entity.Property(e => e.SaldoAwal) ;

				entity.Property(e => e.Pemasukan) ;

				entity.Property(e => e.Pengeluaran);

				entity.Property(e => e.Penyesuaian) ;

				entity.Property(e => e.StockOpname);

				entity.Property(e => e.Selisih);

				entity.Property(e => e.StockId);

				entity.Property(e => e.SaldoBuku);

			
			});

			modelBuilder.Entity<WIPViewModel>(entity =>
			{

				entity.Property(e => e.Kode);
				entity.Property(e => e.Comodity);
				entity.Property(e => e.WIP);
				entity.Property(e => e.UnitQtyName );

			});
		}
    }
}

