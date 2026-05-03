using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaterPlantApp.Models
{
    public class Store
    {
        [Key] public int StoreId { get; set; }

        [Required, MaxLength(100), Display(Name = "Store Name")]
        public string StoreName { get; set; } = string.Empty;

        [Required, MaxLength(20), Display(Name = "Store Code")]
        public string StoreCode { get; set; } = string.Empty;

        [Required, MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required, MaxLength(10), Display(Name = "Pin Code")]
        public string PinCode { get; set; } = string.Empty;

        [Required, MaxLength(20), Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(150), EmailAddress, Display(Name = "Email Address")]
        public string? EmailAddress { get; set; }

        [Required, MaxLength(100), Display(Name = "Manager Name")]
        public string ManagerName { get; set; } = string.Empty;

        [MaxLength(20), Display(Name = "Manager Phone")]
        public string? ManagerPhone { get; set; }

        [Required, MaxLength(50), Display(Name = "Store Type")]
        public string StoreType { get; set; } = "Retail";

        [MaxLength(100), Display(Name = "Operating Hours")]
        public string? OperatingHours { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Water Capacity (Litres)")]
        public int? WaterCapacityLtrs { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(10,7)")] public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(10,7)")] public decimal? Longitude { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<StoreProduct> StoreProducts { get; set; } = new List<StoreProduct>();

        [NotMapped] public string FullAddress => $"{Address}, {City}, {State} - {PinCode}";
        [NotMapped] public string StatusBadge => IsActive ? "Active" : "Inactive";
    }

    public class Product
    {
        [Key] public int ProductId { get; set; }

        [Required, MaxLength(100), Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Required, MaxLength(20), Display(Name = "Product Code")]
        public string ProductCode { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)"), Display(Name = "Size (Litres)")]
        public decimal SizeLtrs { get; set; }

        [Column(TypeName = "decimal(10,2)"), Display(Name = "Price (Rs.)")]
        public decimal PricePerUnit { get; set; }

        [MaxLength(300)] public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<StoreProduct> StoreProducts { get; set; } = new List<StoreProduct>();
    }

    public class StoreProduct
    {
        [Key] public int StoreProductId { get; set; }
        public int StoreId { get; set; }
        public int ProductId { get; set; }
        public int StockQty { get; set; }
        public bool IsAvailable { get; set; } = true;
        public Store Store { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
