using OBPostupyApi.Dto.Readers;
using OBPostupyApi.Entities;
using System;
using System.Collections.Generic;

namespace OBPostupyApi.Extensions
{
    public static class MapExtensions
    {
        public static void SetCorners(this Map map, MapData mapData)
        {
            if (mapData != null && map != null)
            {
                if (mapData.North != null &&
                    mapData.South != null &&
                    mapData.East != null &&
                    mapData.West != null)
                {
                    map.Corners = mapData.Rotation != null ? GetCornersWithRotation(mapData) : GetCornersWithoutRotation(mapData);
                    map.Rotation = mapData.Rotation;
                }
            }
        }

        private static List<Position> GetCornersWithRotation(MapData mapData)
        {
            double rotation = mapData.Rotation.Value;

            double a = (mapData.East.Value + mapData.West.Value) / 2.0;
            double b = (mapData.North.Value + mapData.South.Value) / 2.0;
            double squish = Math.Cos(DegToRad(b));
            double x = squish * (mapData.East.Value - mapData.West.Value) / 2.0;
            double y = (mapData.North.Value - mapData.South.Value) / 2.0;

            double X, Y;
            List<Position> corners = new List<Position>();
            X = b - (x * Math.Sin(DegToRad(rotation)) - y * Math.Cos(DegToRad(rotation)));
            Y = a - (x * Math.Cos(DegToRad(rotation)) + y * Math.Sin(DegToRad(rotation))) / squish;
            corners.Add(new Position(X, Y));
            X = b + (x * Math.Sin(DegToRad(rotation)) + y * Math.Cos(DegToRad(rotation)));
            Y = a + (x * Math.Cos(DegToRad(rotation)) - y * Math.Sin(DegToRad(rotation))) / squish;
            corners.Add(new Position(X, Y));
            X = b - (x * Math.Sin(DegToRad(rotation)) + y * Math.Cos(DegToRad(rotation)));
            Y = a - (x * Math.Cos(DegToRad(rotation)) - y * Math.Sin(DegToRad(rotation))) / squish;
            corners.Add(new Position(X, Y));
            X = b + (x * Math.Sin(DegToRad(rotation)) - y * Math.Cos(DegToRad(rotation)));
            Y = a + (x * Math.Cos(DegToRad(rotation)) + y * Math.Sin(DegToRad(rotation))) / squish;
            corners.Add(new Position(X, Y));
            X = b - (x * Math.Sin(DegToRad(rotation)) - y * Math.Cos(DegToRad(rotation)));
            Y = a - (x * Math.Cos(DegToRad(rotation)) + y * Math.Sin(DegToRad(rotation))) / squish;
            corners.Add(new Position(X, Y));
            return corners;
        }

        private static List<Position> GetCornersWithoutRotation(MapData mapData) => new List<Position>
        {
                new Position(mapData.North.Value, mapData.West.Value),
                new Position(mapData.North.Value, mapData.East.Value),
                new Position(mapData.South.Value, mapData.East.Value),
                new Position(mapData.South.Value, mapData.West.Value),
                new Position(mapData.North.Value, mapData.West.Value)
        };

        private static double DegToRad(double angle) =>  (Math.PI / 180) * angle;
    }
}
