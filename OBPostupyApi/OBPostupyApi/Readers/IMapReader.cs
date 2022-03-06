using OBPostupyApi.Dto.Readers;
using System.IO;
using System.Threading.Tasks;

namespace OBPostupyApi.Readers
{
    public interface IMapReader
    {
        MapData ReadKmz(Stream fileStream);
        string SaveKmzImage(Stream fileStream, string pathToImage, string rootPath);
        Task<string> SaveImageAsync(Stream stream, string fileName, string rootPath);
    }
}
