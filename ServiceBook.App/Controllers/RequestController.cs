using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using ServiceBook.Db.SQLite;
using ServiceBook.Db.SQLite.Models;
using ServiceBook.TokenService;
using System.Text.RegularExpressions;

namespace ServiceBook.App.Controllers
{
    [ApiController]
    public class RequestController : Controller
    {
        private readonly IDataSource _dataSource;
        private readonly ILogger<RequestController> _logger;
        private Dictionary<string, int> days = new Dictionary<string, int>()
        {
            {"Monday", 1 },
            {"Tuesday", 2 },
            {"Wednesday", 3 },
            {"Thursday", 4 },
            {"Friday", 5 },
            {"Satureday", 6 },
            {"Sunday", 7 }
        };
        public RequestController(IDataSource dataSource, ILogger<RequestController> logger)
        {
            _dataSource = dataSource;
            _logger = logger;
 
        }
        [HttpPost]
        [Route("api/Request/Request")]
        public async Task<IActionResult> RepairRequest(RequestUserModel model)
        {
            try
            {
                await _dataSource.RepairRequest(model);
                var msg = new
                {
                    message = "Успешо! Ожидаем вас!",
                };
                return Ok(msg);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var errorMsg = new
                {
                    message = "Error",
                    error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                };

                _logger.LogError($"API: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(errorMsg);
            }
        }
        [HttpGet]
        [Route("api/Request/GetRequests")]
        public async Task<IActionResult> GetRequests(int? user_id = null)
        {
            try
            {
                var requests = await _dataSource.GetRequests(user_id);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("api/Request/GetRequestTimes")]
        public async Task<IActionResult> GetRequestTimes(string dates)
        {
            try
            {

                DateOnly date = new DateOnly();
                date = DateOnly.Parse(dates);
                var requestsTimes = await _dataSource.GetRequestTimes();
                List<TimeOnly> thisDayRequests = new List<TimeOnly>();
                foreach (var request in requestsTimes)
                {
                    var requestDate = DateOnly.FromDateTime(request.Date);
                    if (requestDate.CompareTo(date) == 0)
                    {
                        var time = TimeOnly.FromDateTime(request);
                        thisDayRequests.Add(time);
                    }
                }
                var dayOfWeek = date.DayOfWeek.ToString();
                int idOfDay = 0;
                days.TryGetValue(dayOfWeek, out idOfDay);
                var workMode = await _dataSource.ReadWorkingMode(idOfDay);
                var receptions = await _dataSource.ReadReception();
                int interval = receptions[0].Interval;
                int countMax = receptions.Length;
                TimeOnly start = new TimeOnly();
                start = TimeOnly.Parse(workMode[0].time_start);
                TimeOnly end = new TimeOnly();
                end = TimeOnly.Parse(workMode[0].time_end);
                List<TimeOnly> resultFreeTime = new List<TimeOnly>();
                TimeOnly i = start;
                while(i <= end)
                {
                    int index = thisDayRequests.IndexOf(i);
                    if (index > 0)
                    {
                        int count = thisDayRequests.Count(x => x == thisDayRequests[index]);
                        if(count < countMax)
                        {
                            resultFreeTime.Add(i);
                        }
                    }
                    else
                    {
                        resultFreeTime.Add(i);
                    }
                    i = i.AddMinutes(interval);
                }
                return Ok(resultFreeTime);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
