namespace CSI.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Staging_ProductExtract
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        public int Batch { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "datetime2")]
        public DateTime BatchDate { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(100)]
        public string ProductId { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(2000)]
        public string ProductName { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(200)]
        public string ProductPrice { get; set; }
    }
}
