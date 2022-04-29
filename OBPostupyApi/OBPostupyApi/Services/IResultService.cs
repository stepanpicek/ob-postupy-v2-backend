using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Enums;
using System.IO;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface IResultService
    {
        Task<ResponseType> SaveOrisResultsAsync(string raceId, string orisId);
        Task<ResponseType> SaveResultsAsync(string raceId, Stream fileStream);
        Task<CategoriesResponse> GetCategoriesAsync(string raceId);
        Task<CategoryResultsResponse> GetCategoryResultsAsync(int id);
        Task<ResponseType> DeleteResults(string raceId);
        Task<ResultsResponse> GetRaceResults(string raceId);
        Task<SearchResultsResponse> SearchRaceResultsAsync(string raceId, string term);
    }
}
