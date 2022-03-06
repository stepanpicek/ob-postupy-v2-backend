using Microsoft.Extensions.Logging;
using OBPostupyApi.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace OBPostupyApi.Readers
{
    public class CoursesReader : ICoursesReader
    {
        private readonly CourseData _courseData = new CourseData();
        private readonly List<Course> _courses = new List<Course>();
        private readonly List<Control> _controls = new List<Control>();
        private readonly List<CourseControl> _courseControls = new List<CourseControl>();
        private readonly List<Split> _splits = new List<Split>();
        private readonly List<CourseSplit> _courseSplits = new List<CourseSplit>();
        public Map Map { get; private set; }

        private readonly ILogger<CoursesReader> _logger;

        public CoursesReader(ILogger<CoursesReader> logger)
        {
            _logger = logger;
        }

        public CourseData Read(Stream stream)
        {
            try
            {
                var courseData = ParseCourseData(stream);
                if (courseData != null)
                {
                    ParseRaceCourseData(courseData.RaceCourseData);
                    SetSplits();
                    _courseData.Courses = _courses;
                    _courseData.Controls = _controls;
                    _courseData.Splits = _splits;
                }
                return _courseData;
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error during reading courses");
            }

            return null;
        }

        private Generated.CourseData ParseCourseData(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Generated.CourseData));
            object courseData = serializer.Deserialize(stream);
            return (Generated.CourseData)courseData;
        }

        private void ParseRaceCourseData(Generated.RaceCourseData[] coursesData)
        {
            foreach (var courseData in coursesData)
            {
                Map = SetMapRelativeCoordinates(courseData.Map.FirstOrDefault());

                foreach (var control in courseData.Control)
                {
                    var coords = control.Position != null
                        ? Tuple.Create(control.Position.lat, control.Position.lng)
                        : null;
                    var mapCoordinates = control.MapPosition != null
                        ? Tuple.Create(control.MapPosition.x, control.MapPosition.y)
                        : null;

                    Control cnt = new Control
                    {
                        Code = control.Id.Value,
                        Coordinates = coords,
                        MapCoordinates = mapCoordinates,
                        CourseControl = new List<CourseControl>()
                    };
                    _controls.Add(cnt);
                }

                foreach (var course in courseData.Course)
                {
                    Course crs = new Course
                    {
                        Name = course.Name,
                        CourseControl = new List<CourseControl>(),
                        CourseSplits = new List<CourseSplit>()
                    };
                    _courses.Add(crs);
                    int i = 0;
                    foreach (var courseControl in course.CourseControl)
                    {
                        Control control = _controls.FirstOrDefault(c => c.Code == courseControl.Control.FirstOrDefault());
                        if (control != null)
                        {
                            CourseControl cc = new CourseControl
                            {
                                Control = control,
                                Course = crs,
                                Order = i,
                                Type = courseControl?.type.ToString()
                            };
                            _courseControls.Add(cc);
                            crs.CourseControl.Add(cc);
                            control.CourseControl.Add(cc);
                            i++;
                        }
                    }
                }
            }
        }

        private void SetSplits()
        {
            foreach (var course in _courses)
            {
                var courseControls = _courseControls.FindAll(cc => cc.Course == course).OrderBy(cc => cc.Order).ToList();
                for (int i = 0; i < courseControls.Count() - 1; i++)
                {
                    Control c1 = courseControls[i].Control;
                    Control c2 = courseControls[i + 1].Control;
                    var split = _splits.FirstOrDefault(s => s.FirstControl == c1 && s.SecondControl == c2);
                    if (split == null)
                    {
                        split = new Split
                        {
                            FirstControl = c1,
                            SecondControl = c2
                        };
                        _splits.Add(split);
                    }
                    CourseSplit courseSplit = new CourseSplit
                    {
                        Course = course,
                        Split = split,
                        Order = i
                    };
                    course.CourseSplits.Add(courseSplit);
                    _courseSplits.Add(courseSplit);
                }
            }
        }
        
        private Map SetMapRelativeCoordinates(Generated.Map map)
        {
            if (map == null) return null;
            return new Map
            {
                North = map.MapPositionTopLeft.y,
                West = map.MapPositionTopLeft.x,
                South = map.MapPositionBottomRight.y,
                East = map.MapPositionBottomRight.x,
                Scale = (int)map.Scale
            };
        }
    }
}
