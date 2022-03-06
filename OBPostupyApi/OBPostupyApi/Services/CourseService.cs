using OBPostupyApi.Entities;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using OBPostupyApi.Readers;
using OBPostupyApi.Repositories;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public class CourseService : ICourseService
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IMapRepository _mapRepository;
        private readonly ICoursesReader _coursesReader;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICourseRepository _courseRepository;

        public async Task<ResponseType> SaveCoursesAsync(string raceKey, Stream fileStream)
        {
            var race = await _raceRepository.GetRaceByKeyAsync(raceKey);
            if (race == null)
            {
                return ResponseType.BadRequest;
            }

            var courseData = _coursesReader.Read(fileStream);
            race.CourseData = courseData;
            await SetMap(race, _coursesReader.Map);
            await _raceRepository.SaveAsync();
            return ResponseType.OK;
        }

        public async Task<ResponseType> AddCoursesToCategories(string raceKey, List<CourseToCategory> courseToCategories)
        {
            var race = await _raceRepository.GetRaceByKeyAsync(raceKey);
            if (race == null)
            {
                return ResponseType.BadRequest;
            }

            var categories = await _categoryRepository.GetCategoriesAsync(raceKey);
            var courses = (await _courseRepository.GetCourseDataByRaceAsync(raceKey))?.Courses;

            foreach (var courseToCategory in courseToCategories)
            {
                Category category = categories.Find(c => c.Name == courseToCategory.Category);
                Course course = courses.Find(c => c.Name == courseToCategory.Course);
                if (category != null && course != null)
                {
                    category.Course = course;
                    SetSplitsToResults(category, course);
                }
            }

            await _raceRepository.SaveAsync();
            return ResponseType.OK;
        }

        private void SetSplitsToResults(Category category, Course course)
        {
            foreach (var result in category.PersonResults)
            {
                var splitTimes = result.SplitTimes.OrderBy(s => s.Time).ToList();
                var courseSplits = course.CourseSplits.OrderBy(cs => cs.Order).ToList();

                int i = 0;
                foreach (var courseSplit in courseSplits)
                {
                    if (i < splitTimes.Count && splitTimes[i].Code == courseSplit.Split.SecondControl.Code)
                    {
                        splitTimes[i].Split = courseSplit.Split;
                        i++;
                    }
                }
            }
        }

        private async Task SetMap(Race race, Map map)
        {
            var currentMap = await _mapRepository.GetMapByRaceAsync(race.Key);
            if (currentMap == null)
            {
                race.Maps = new List<Map> { map };
            }
            else
            {
                currentMap.West = map.West;
                currentMap.East = map.East;
                currentMap.North = map.North;
                currentMap.South = map.South;
                currentMap.Scale = map.Scale;
            }
        }
    }
}
