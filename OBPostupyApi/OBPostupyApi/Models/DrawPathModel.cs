using System.Collections.Generic;

namespace OBPostupyApi.Models
{
    public class DrawPathModel
    {
        public int PersonResultId { get; set; }
        public List<SplitPath> SplitPaths { get; set; }
    }
}
