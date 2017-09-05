using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Table("SMSBatch")]
    public class SMSBatch
    {
        [Key]
        [Required]
        [MaxLength(32)]
        public string ID { get; set; }

        [MaxLength(32)]
        public string BatchNum { get; set; }

        [MaxLength(500)]
        public string Content { get; set; }

        public int SkipNum { get; set; }

        public int SendCount { get; set; }

        public int SuccessCount { get; set; }

        public int StartUserID { get; set; }
        public int EndUserID { get; set; }

        public DateTime CreatedTime { get; set; }
        [NotMapped]
        public string AppendMibole { get; set; }
    }
}
