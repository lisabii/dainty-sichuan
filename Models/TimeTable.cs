using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuibao.Models
{
    public class TimeTable
    {
        [Key]
        public int TimeTableId { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:D}"), DisplayName("Date")]
        public DateTime DateTime { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { set; get; }

        [DefaultValue(false)]
        public bool Applied { get; set; }

        [DefaultValue(false)]
        public bool Assigned { get; set; }
    }
}
