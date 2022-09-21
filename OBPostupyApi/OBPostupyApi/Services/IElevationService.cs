using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OBPostupyApi.Entities;

namespace OBPostupyApi.Services
{
    public interface IElevationService
    {
        Task SetElevationAsync(List<Location> path);
    }
}

