using OBPostupyApi.Enums;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class SearchResultsResponse
    {
        public ResponseType ResponseType { get; set; }
        public List<SearchCategoryResultsResponse> Categories { get; set; }
    }

    public class SearchCategoryResultsResponse
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public List<SearchPersonResultsResponse> People { get; set; }
    }

    public class SearchPersonResultsResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Position { get; set; }
        public string RegNumber { get; set; }
        public string Status { get; set; }
    }
}
