using OBPostupyApi.Entities;
using System.IO;

namespace OBPostupyApi.Readers
{
    public interface ICoursesReader
    {
        Map Map { get; }
        CourseData Read(Stream stream);
    }
}
