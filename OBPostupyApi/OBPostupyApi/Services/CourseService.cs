using OBPostupyApi.Dto.Responses;
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

        public CourseService(IRaceRepository raceRepository, IMapRepository mapRepository, ICoursesReader coursesReader,
            ICategoryRepository categoryRepository, ICourseRepository courseRepository)
        {
            _raceRepository = raceRepository;
            _mapRepository = mapRepository;
            _coursesReader = coursesReader;
            _categoryRepository = categoryRepository;
            _courseRepository = courseRepository;
        }

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

        public async Task<ResponseType> AddCoursesToCategoriesAsync(string raceKey, List<CourseToCategory> courseToCategories)
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

        public async Task<CourseResponse> GetCourseAsync(int id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if(course == null)
            {
                return new CourseResponse { ResponseType = ResponseType.BadRequest };
            }

            var controls = course.CourseControl.Select(s => new ControlResponse
            {
                Id = s.Control.Id,
                Position = new List<double> { s.Control.Coordinates.Item1, s.Control.Coordinates.Item2 },
                Order = s.Order,
                Type = s.Type
            }).ToList();

            return new CourseResponse 
            { 
                Controls = controls,
                ResponseType = ResponseType.OK
            };
        }

        public async Task<ResponseType> DeleteCoursesAsync(string raceKey)
        {
            var courseData = await _courseRepository.GetCourseDataByRaceAsync(raceKey);
            if (courseData == null)
            {
                return ResponseType.BadRequest;
            }

            await _courseRepository.DeleteRaceCoursesAsync(raceKey);
            return ResponseType.OK;
        }

        public async Task<CoursesToCategoryResponse> GetCoursesToCategoriesAsync(string raceKey)
        {
            var race = await _raceRepository.GetRaceByKeyAsync(raceKey);
            if (race == null)
            {
                return new CoursesToCategoryResponse { ResponseType = ResponseType.BadRequest };
            }

            var categories = await _categoryRepository.GetCategoriesWithCourseAsync(raceKey);
            var courses = await _courseRepository.GetCoursesAsync(raceKey);

            return new CoursesToCategoryResponse
            {
                ResponseType = ResponseType.OK,
                Categories = categories?.OrderBy(c => c.Name)?.Select(c => new CourseToCategoryResponse
                {
                    Name = c?.Name,
                    Course = c?.Course?.Name

                })?.ToList(),
                Courses = courses?.OrderBy(c => c.Name)?.Select(c => c?.Name)?.ToList()
            };
        }
    }
}
