using Microsoft.Extensions.Logging;
using OBPostupyApi.Dto.Readers;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupyApi.Readers
{
    public class MapReader : IMapReader
    {
        private readonly ILogger<MapReader> _logger;

        public MapReader(ILogger<MapReader> logger)
        {
            _logger = logger;
        }

        public MapData ReadKmz(Stream fileStream)
        {
            using KmzFile kmzFile = KmzFile.Open(fileStream);
            Parser parser = new Parser();
            parser.ParseString(kmzFile.ReadKml(), false);
            var image = parser.Root.Flatten().OfType<GroundOverlay>().FirstOrDefault();
            var latLonBox = parser.Root.Flatten().OfType<LatLonBox>().FirstOrDefault();
            if (latLonBox != null)
            {
                return new MapData
                {
                    East = latLonBox.East,
                    West = latLonBox.West,
                    South = latLonBox.South,
                    North = latLonBox.North,
                    Rotation = latLonBox.Rotation,
                    KmzImagePath = image?.Icon?.Href?.ToString(),
                };
            }
            else
            {
                throw new ArgumentException("LatLanBox doesn't contain in Kmz file.");
            }
        }

        public string SaveKmzImage(Stream fileStream, string pathToImage, string rootPath)
        {
            using (ZipArchive archive = new ZipArchive(fileStream))
            {
                if (pathToImage != null)
                {
                    ZipArchiveEntry entry = archive.GetEntry(pathToImage);
                    string path = GetFilePath(rootPath, entry.Name);
                    string finalPath = Path.Combine(rootPath, path);
                    ZipFileExtensions.ExtractToFile(entry, finalPath);
                    return finalPath;
                }
            }

            return null;
        }

        public async Task<string> SaveImageAsync(Stream stream, string fileName, string rootPath)
        {
            string path = GetFilePath(rootPath, fileName);
            string finalPath = Path.Combine(rootPath, path);
            using var fileStream = new FileStream(finalPath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);
            return finalPath;
        }


        private string GetFilePath(string rootPath, string fileName)
        {
            string path = Path.GetRandomFileName() + "." + fileName;
            while (File.Exists(Path.Combine(rootPath, path)))
            {
                path = Path.GetRandomFileName() + "." + fileName;
            }
            return path;
        }
    }
}
