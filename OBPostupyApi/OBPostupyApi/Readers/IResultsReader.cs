using OBPostupyApi.Entities;
using System.Collections.Generic;
using System.IO;

namespace OBPostupyApi.Readers
{
    public interface IResultsReader
    {
        List<Category> Read(Stream stream);
    }
}
