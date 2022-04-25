using OBPostupyApi.Enums;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class CoursesToCategoryResponse
    {
        public ResponseType ResponseType { get; set; }
        public List<CourseToCategoryResponse> Categories { get; set; }
        public List<string> Courses { get; set; }
    }

    public class CourseToCategoryResponse
    {
        public string Name { get; set; }
        public string Course { get; set; }
    }
}
