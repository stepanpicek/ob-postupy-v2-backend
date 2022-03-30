using OBPostupyApi.Entities;
using OBPostupyApi.Enums;
using System;

namespace OBPostupyApi.Dto.Responses
{
    public class EditRaceResponse
    {
        public ResponseType ResponseType { get; set; }
        public string Key { get; set; }
        public RaceType Type { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public EditRaceResultResponse Results { get; set; }
        public EditRaceMapResponse Map { get; set; }
        public EditRaceCourseDataResponse CourseData { get; set; }

    }

    public class EditRaceResultResponse
    {
        public bool IsUploaded { get; set; }
    }

    public class EditRaceMapResponse : EditRaceResultResponse
    {
        public bool IsCalibrated { get; set; }
    }

    public class EditRaceCourseDataResponse : EditRaceResultResponse
    {
        public bool AreCoursesConnected { get; set; }
    }
}
