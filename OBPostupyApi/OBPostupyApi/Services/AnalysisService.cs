using OBPostupyApi.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OBPostupyApi.Services
{
    public class AnalysisService : IAnalysisService
    {
        public double GetDistanceInMeters(Tuple<double, double> first, Tuple<double, double> second)
        {
            if (first == null || second == null) return 0;

            var earthRadius = 6371000;

            var dLat = DegreesToRadians(second.Item1 - first.Item1);
            var dLon = DegreesToRadians(second.Item2 - first.Item2);

            var lat1 = DegreesToRadians(first.Item1);
            var lat2 = DegreesToRadians(second.Item1);
            var a = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Pow(Math.Sin(dLon / 2), 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var completeDist = earthRadius * c;
            return completeDist;
        }

        private double DegreesToRadians(double degree)
        {
            return degree * Math.PI / 180;
        }

        public List<Position> MapCornersSort(Map map)
        {
            List<Position> orderByLat = map.Corners.OrderBy(c => c.lat).ToList();
            Position pos = null;
            for (int i = 0; i < orderByLat.Count - 1; i++)
            {
                if (orderByLat[i].lat == orderByLat[i + 1].lat && orderByLat[i].lon == orderByLat[i + 1].lon)
                {
                    pos = orderByLat[i];
                    break;
                }
            }

            if (pos != null)
            {
                orderByLat.Remove(pos);
            }

            List<Position> ordered = new List<Position>();

            if (orderByLat[0].lon < orderByLat[1].lon)
            {
                ordered.Add(orderByLat[0]);
                ordered.Add(orderByLat[1]);
            }
            else
            {
                ordered.Add(orderByLat[1]);
                ordered.Add(orderByLat[0]);
            }

            if (orderByLat[2].lon < orderByLat[3].lon)
            {
                ordered.Add(orderByLat[3]);
                ordered.Add(orderByLat[2]);
            }
            else
            {
                ordered.Add(orderByLat[2]);
                ordered.Add(orderByLat[3]);
            }

            return ordered;
        }

        public List<Location> InterpolationByTime(List<Location> locations, int seconds)
        {
            if (locations == null) return null;
            if (locations.Count < 2) return null;
            var sortedLocations = locations.OrderBy(l => l.Time).ToList();
            List<Location> finalLocations = new List<Location>();
            Location actualLocation = sortedLocations[0];
            finalLocations.Add(actualLocation);
            foreach (var location in sortedLocations)
            {
                if (actualLocation == location) continue;
                if (location.Time != null && actualLocation.Time != null)
                {
                    TimeSpan diff = (TimeSpan)(location.Time - actualLocation.Time);
                    if (diff.TotalSeconds < seconds) continue;
                    if (diff.TotalSeconds == seconds)
                    {
                        finalLocations.Add(location);
                        actualLocation = location;
                        continue;
                    }

                    while (diff.TotalSeconds >= seconds)
                    {
                        Location interpolated = GetInterpolatedLocationByTime(actualLocation, location, seconds);
                        if (interpolated != null)
                        {
                            finalLocations.Add(interpolated);
                            actualLocation = interpolated;
                            diff = (TimeSpan)(location.Time - actualLocation.Time);
                        }
                    }
                }
            }
            return finalLocations;
        }

        private Location GetInterpolatedLocationByTime(Location firstPoint, Location secondPoint, int seconds)
        {
            if (firstPoint.Time != null && secondPoint.Time != null)
            {
                TimeSpan diff = (TimeSpan)(secondPoint.Time - firstPoint.Time);
                DateTime firstPointTime = (DateTime)firstPoint.Time;
                double diffLat = (secondPoint.Position.Item1 - firstPoint.Position.Item1) / diff.TotalSeconds;
                double diffLon = (secondPoint.Position.Item2 - firstPoint.Position.Item2) / diff.TotalSeconds;
                double newLat = firstPoint.Position.Item1 + (diffLat * seconds);
                double newLon = firstPoint.Position.Item2 + (diffLon * seconds);
                Location newLocation = new Location
                {
                    Position = Tuple.Create(newLat, newLon),
                    Time = firstPointTime.AddSeconds(seconds)
                };

                if (firstPoint.Elevation != null && secondPoint.Elevation != null)
                {
                    double diffEle = (double)(secondPoint.Elevation - firstPoint.Elevation) / diff.TotalSeconds;
                    double newEle = (double)firstPoint.Elevation + (seconds * diffEle);
                    newLocation.Elevation = newEle;
                }

                return newLocation;
            }
            return null;
        }

        public List<double> GetSpeed(List<Location> locations)
        {
            if (locations == null) return null;
            var orderedLocations = locations.OrderBy(l => l.Time).ToList();
            List<double> speed = new List<double>();
            speed.Add(0);
            for (int i = 1; i < orderedLocations.Count; i++)
            {
                var distance = GetDistanceInMeters(orderedLocations[i - 1].Position, orderedLocations[i].Position);
                double actualSpeed = (distance / 1000) / (((TimeSpan)(orderedLocations[i].Time - orderedLocations[i - 1].Time)).TotalHours);
                speed.Add(actualSpeed);
            }
            return speed;
        }

        public double GetPathDistance(List<Location> locations)
        {
            double distance = 0;

            for (int i = 0; i < locations.Count - 1; i++)
            {
                distance += GetDistanceInMeters(locations[i].Position, locations[i + 1].Position);
            }

            return distance;
        }

        public (double Elevation, double Descent) GetElevation(List<Location> locations)
        {
            double elevation = 0;
            double descent = 0;

            for (int i = 0; i < locations.Count - 1; i++)
            {
                double? elevation1 = locations[i].Elevation;
                double? elevation2 = locations[i + 1].Elevation;
                if (elevation1 != null && elevation2 != null)
                {
                    var eleDiff = elevation2 - elevation1;
                    if (eleDiff > 0)
                    {
                        elevation += (double)eleDiff;
                    }
                    else if (eleDiff < 0)
                    {
                        descent += (double)eleDiff;
                    }
                }
            }

            return (elevation, descent);
        }

        public List<Location> GetDrawnPath(List<List<Location>> splits, PersonResult personResult)
        {
            var splitTimes = personResult.SplitTimes.OrderBy(st => st.Time).ToList();

            var start = splits[0].FirstOrDefault();
            if (start != null) start.Time = personResult.StartTime;

            var finnish = splits[splits.Count - 1].LastOrDefault();
            if (finnish != null) finnish.Time = personResult.FinishTime;

            for (int i = 0; i < splitTimes.Count; i++)
            {
                if (splits.Count <= i)
                {
                    continue;
                }

                if (i != 0)
                {
                    var first = splits[i].FirstOrDefault();
                    if (first != null) first.Time = splits[i - 1].LastOrDefault()?.Time;
                }

                if (i != splits.Count - 1)
                {
                    var last = splits[i].LastOrDefault();
                    if (last != null) last.Time = splitTimes[i].Time;
                }
            }

            var lastSplit = splits[splits.Count - 1].FirstOrDefault();
            if (lastSplit != null && !lastSplit.Time.HasValue) lastSplit.Time = splitTimes.LastOrDefault()?.Time;

            List<Location> locations = new List<Location>();
            foreach (var split in splits)
            {
                var distance = GetPathDistance(split);
                var time = ((TimeSpan)(split.LastOrDefault()?.Time - split.FirstOrDefault()?.Time)).TotalSeconds;
                var distancePerSecond = distance / time;
                for (int i = 1; i < split.Count - 1; i++)
                {
                    var locationDistance = GetDistanceInMeters(split[i - 1].Position, split[i].Position);
                    var seconds = locationDistance / distancePerSecond;
                    var dateTime = split[i - 1].Time;
                    if (dateTime != null) split[i].Time = ((DateTime)dateTime).AddSeconds(seconds);
                }

                locations.AddRange(split);
            }

            return locations;
        }

        public Image GetMapWithWaterMark(string pathToImage)
        {
            Image image = Image.FromFile(pathToImage);
            Graphics graph = Graphics.FromImage(image);
            Font drawFont = new Font("Arial", 50, FontStyle.Bold);
            SolidBrush drawBrush = new SolidBrush(Color.FromArgb(70, 0, 0, 255));


            string drawString = "OB POSTUPY ČSOS";
            for (int j = 50; j < image.Height; j += 400)
            {
                for (int i = 50; i < image.Width; i += 1500)
                {
                    PointF drawPoint = new PointF(i, j);
                    graph.DrawString(drawString, drawFont, drawBrush, drawPoint);
                }
            }

            return image;
        }
    }
}
