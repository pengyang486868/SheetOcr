using System.Collections.Generic;
using System.Linq;

namespace SheetOcr.Client
{
    public class FormatConfig
    {
        public string Default { get; set; }
        public List<AssignedItem> Assigned { get; set; }

        public Dictionary<int, string> AssignDic => Assigned.ToDictionary(x => x.Row, x => x.Format);
    }

    public class AssignedItem
    {
        public int Row { get; set; }
        public string Format { get; set; }
    }
}
