using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AF_UploadClientsHaceLaCuenta.Model
{
    public class EntriesProcessingResult
    {
        public int TotalEntries { get;set; }
        public int InsertedEntries { get;set; }
        public List<ProcessedEntry> ProcessedEntries { get; set; }=new List<ProcessedEntry>();
    }
    public class ProcessedEntry
    {
        public string Id { get; set; }
        public bool Inserted { get; set; }
    }
}
