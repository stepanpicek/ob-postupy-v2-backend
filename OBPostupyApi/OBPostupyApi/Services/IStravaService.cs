using Microsoft.AspNetCore.Authentication;
using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Enums;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface IStravaService
    {
        Task<ResponseType> SetAuthTokensAsync(string userId, IEnumerable<AuthenticationToken> tokens);
        Task<StravaActivitiesResponse> GetActivityListAsync(string date, ClaimsPrincipal userClaims);
        Task<PathDataResponse> GetActivityAsync(long activityId, ClaimsPrincipal userClaims);
        Task<StravaAuthResponse> IsUserStravaAuthAsync(ClaimsPrincipal userClaims);
        Task<ResponseType> DeleteStravaAuthAsync(ClaimsPrincipal userClaims);
    }
}
