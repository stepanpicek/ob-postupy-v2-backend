using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface IPathService
    {
        Task<ResponseType> SavePathAsync(int personResultId, List<PathData> pathData);
        Task<ResponseType> DeletePathAsync(int personResultId);
        Task<ResponseType> DrawPathAsync(int personResultId, List<SplitPath> pathData);
        Task<PathResponse> GetPathAsync(int personResultId);
        Task<PathWithSpeedResponse> GetPathWithSpeedAsync(int personResultId);
        Task<ResponseType> RemovePathAsync(int personResultId);
        Task<PathAnalysisResponse> GetPathAnalysisAsync(int personResultId);
    }
}
