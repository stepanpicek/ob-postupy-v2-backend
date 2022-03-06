using Microsoft.Extensions.Logging;
using OBPostupyApi.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace OBPostupyApi.Readers
{
    public class ResultsReader : IResultsReader
    {
        private readonly List<Category> _categories = new List<Category>();
        private readonly ILogger<ResultsReader> _logger;

        public ResultsReader(ILogger<ResultsReader> logger)
        {
            _logger = logger;
        }

        public List<Category> Read(Stream stream)
        {
            try
            {
                XElement results = XElement.Load(stream);
                Read(results);
                return _categories;
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error during reading results");
                return null;
            }
        }

        private void Read(XElement root)
        {
            var classResults = root.Elements().Where(e => e.Name.LocalName == "ClassResult").ToList();
            foreach (var classResult in classResults)
            {
                Category category = new Category { Name = GetCategoryName(classResult), PersonResults = new List<PersonResult>() };
                var personResults = classResult?.Elements()?.Where(e => e.Name.LocalName == "PersonResult")?.ToList();
                foreach (var personResult in personResults)
                {
                    var person = new Person { 
                        FirstName = GetFirstName(personResult), 
                        LastName = GetLastName(personResult), 
                        RegNumbers = new List<string> { GetUserReg(personResult) } 
                    };
                    var result = personResult?.Elements()?.FirstOrDefault(p => p.Name.LocalName == "Result");
                    PersonResult pr = new PersonResult {
                        StartTime = GetStartTime(result),
                        FinishTime = GetFinishTime(result),
                        Position = GetPosition(result),
                        Status = GetStatus(result),
                        Category = category,
                        Person = person,
                        SplitTimes = new List<SplitTime>()
                    };
                    var splitTimes = result?.Elements()?.Where(e => e.Name.LocalName == "SplitTime")?.ToList();
                    int timeBefore = 0;
                    foreach (var splitTime in splitTimes)
                    {
                        var code = GetControlCode(splitTime);
                        var time = GetSplitTime(splitTime);

                        SplitTime st = new SplitTime { 
                            Time = pr.StartTime.AddSeconds(time), 
                            PersonResult = pr, 
                            Code = code, 
                            TimeSpan = time - timeBefore 
                        };
                        timeBefore = time;
                    }
                    category.PersonResults.Add(pr);
                }
                _categories.Add(category);
            }
        }

        private string GetCategoryName(XElement classResult) => classResult?
            .Elements()?
            .FirstOrDefault(e => e.Name.LocalName == "Class")?
            .Elements()?
            .FirstOrDefault(e => e.Name.LocalName == "Name")?.Value;

        private string GetFirstName(XElement personResult) => personResult?
            .Elements()?
            .FirstOrDefault(e => e.Name.LocalName == "Person")?
            .Elements()?
            .FirstOrDefault(e => e.Name.LocalName == "Name")?
            .Elements()?
            .FirstOrDefault(e => e.Name.LocalName == "Given")?.Value;

        private string GetLastName(XElement personResult) => personResult?
            .Elements()?
            .FirstOrDefault(e => e.Name.LocalName == "Person")?
            .Elements()?
            .FirstOrDefault(e => e.Name.LocalName == "Name")?
            .Elements()?
            .FirstOrDefault(e => e.Name.LocalName == "Family")?.Value;

        private string GetUserReg(XElement personResult)
        {
            var ids = personResult?
                .Elements()?
                .FirstOrDefault(e => e.Name.LocalName == "Person")?
                .Elements()?
                .Where(e => e.Name.LocalName == "Id")
                .ToList();

            if (ids?.Count == 1) return ids?.FirstOrDefault()?.Value;

            return ids?.FirstOrDefault(i => i?.Attributes()?.FirstOrDefault(a => a.Name.LocalName == "CZE") != null)?.Value;
        }

        private DateTime GetStartTime(XElement result)
        {
            var startTime = result.Elements().FirstOrDefault(e => e.Name.LocalName == "StartTime");
            if (DateTime.TryParse(startTime.Value, out DateTime dateTime))
            {
                return dateTime;
            }

            return default;
        }

        private DateTime GetFinishTime(XElement result)
        {
            var startTime = result?.Elements()?.FirstOrDefault(e => e.Name.LocalName == "FinishTime");
            if (DateTime.TryParse(startTime?.Value, out DateTime dateTime))
            {
                return dateTime;
            }

            return default;
        }

        private string GetStatus(XElement result) => result?.Elements()?.FirstOrDefault(e => e.Name.LocalName == "Status")?.Value;

        private int GetPosition(XElement result)
        {
            var positionString = result?.Elements()?.FirstOrDefault(e => e.Name.LocalName == "Position");
            if (int.TryParse(positionString?.Value, out int position))
            {
                return position;
            }

            return int.MaxValue - 1;
        }

        private string GetControlCode(XElement splitTime) => splitTime?.Elements()?.FirstOrDefault(e => e.Name.LocalName == "ControlCode")?.Value;
        private int GetSplitTime(XElement splitTime)
        {
            var timeString = splitTime?.Elements()?.FirstOrDefault(e => e.Name.LocalName == "Time")?.Value;
            if (int.TryParse(timeString, out int time))
            {
                return time;
            }

            return 0;
        }
    }
}
