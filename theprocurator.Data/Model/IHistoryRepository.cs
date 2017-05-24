using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace theprocurator.Data.Model
{
    interface IHistoryRepository
    {
        int HistoryId { get; set; }
        string Version { get; set; }
        string Date { get; set; }
    }
}
