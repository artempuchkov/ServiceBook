using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceBook.Db.SQLite;
using ServiceBook.Db.SQLite.Models;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace ServiceBook.App.Controllers
{
    [ApiController]
    
    public class ServiceController : ControllerBase
    {
        private readonly IDataSource _dataSource;
        private readonly ILogger<ServiceController> _logger;
        private const string AuthCookie = "AutoTechCentr";
        private const string vowels = "аяуюеёыоыиэь";


        public ServiceController(IDataSource dataSource, ILogger<ServiceController> logger)
        {
            _dataSource = dataSource;
            _logger = logger;
        }

        [HttpGet]
        [Route("api/Service/GetAll")]
        public async Task<IActionResult> GetAll(int? id = null)
        {
            try
            {
                var serviceRead = await _dataSource.ReadService(id);
                return Ok(serviceRead);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/Service/GetByCategory")]
        public async Task<IActionResult> GetServiceByCategory(int category, int? id = null)
        {
            try
            {
                var serviceByCategoryRead = await _dataSource.ReadByCategoryService(category, id);
                return Ok(serviceByCategoryRead);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/Service/GetByNameOrDescription")]
        public async Task<IActionResult> GetByNameOrDescription(String searchString)
        {
            List<ServiceModel> services = new List<ServiceModel>();
            string newString = searchString;
            if (searchString.Length > 3)
            for (var i = 0; i < vowels.Length; i++)
            {
                newString = newString.Replace(vowels[i].ToString(), ".");
            }
            try
            {
                var serviceByCategoryRead = await _dataSource.ReadService();
                for (int i = 0; i < serviceByCategoryRead.Length; i++)
                {
                    if (Regex.Match(serviceByCategoryRead[i].Name, newString, RegexOptions.IgnoreCase, Regex.InfiniteMatchTimeout).Success){
                        services.Add(serviceByCategoryRead[i]);
                    }
                    else if (Regex.Match(serviceByCategoryRead[i].Description, newString, RegexOptions.IgnoreCase, Regex.InfiniteMatchTimeout).Success)
                    {
                        services.Add(serviceByCategoryRead[i]);
                    }
                }
                return Ok(services);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
                
        }
    }
}
