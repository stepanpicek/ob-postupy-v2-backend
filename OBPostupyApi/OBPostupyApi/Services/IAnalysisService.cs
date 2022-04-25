using OBPostupyApi.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OBPostupyApi.Services
{
    public interface IAnalysisService
    {
        double GetDistanceInMeters(Tuple<double, double> first, Tuple<double, double> second);
        List<Position> MapCornersSort(Map map);
        List<Location> InterpolationByTime(List<Location> locations, int seconds);
        List<double> GetSpeed(List<Location> locations);
        double GetPathDistance(List<Location> locations);
        (double Elevation, double Descent) GetElevation(List<Location> locations);
        List<Location> GetDrawnPath(List<List<Location>> splits, PersonResult personResult);
        byte[] GetMapWithWaterMark(string pathToImage);
    }
}
