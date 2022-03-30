using OBPostupyApi.Enums;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class CategoriesResponse
    {
        public ResponseType ResponseType { get; set; }
        public List<CategoryResponse> Categories { get; set; }
    }

    public class CategoryResponse
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public string Name { get; set; }
    }
}
