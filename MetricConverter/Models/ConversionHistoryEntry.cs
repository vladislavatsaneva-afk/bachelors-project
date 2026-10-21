using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using MetricConverter.Data;

namespace MetricConverter.Models
{
    public class ConversionHistoryEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string ConversionType { get; set; }
        [Required]
        public double InputValue { get; set; }
        [Required]
        public string FromUnit { get; set; }
        [Required]
        public string ToUnit { get; set; }
        [Required]
        public double Result { get; set; }
        [Required]
        public DateTime DateTime { get; set; }

        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public AppUser User { get; set; }
    }
}
