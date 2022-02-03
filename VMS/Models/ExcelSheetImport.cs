using System;
using System.Collections.Generic;

#nullable disable

namespace VMS.Models
{
    public partial class ExcelSheetImport
    {
        public int Id { get; set; }
        public string SheetName { get; set; }
        public int? RowsCount { get; set; }
        public int? RowsImported { get; set; }
        public int? RowsFailed { get; set; }
        public string ImportedJson { get; set; }
        public string FailedJson { get; set; }
        public DateTime? ImportDate { get; set; }
        public int? ImportedBy { get; set; }
    }
}
