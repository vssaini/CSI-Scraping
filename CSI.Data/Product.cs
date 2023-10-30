using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSI.Data
{
    public class Product
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(Order = 1)]
        public int Batch { get; set; }

        [Column(Order = 2, TypeName = "datetime2")]
        public DateTime BatchDate { get; set; }

        [Column(Order = 3)]
        [StringLength(100)]
        public string ProductId { get; set; }

        [Column(Order = 4)]
        [StringLength(2000)]
        public string ProductName { get; set; }

        [Column(Order = 5)]
        public decimal ProductPrice { get; set; }

        [Column(Order = 6)]
        public int ProductStock { get; set; }
    }
}
