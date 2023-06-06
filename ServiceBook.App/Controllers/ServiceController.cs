using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceBook.Db.SQLite;
using System.Text.RegularExpressions;

namespace ServiceBook.App.Controllers
{
    [ApiController]
    
    public class ServiceController : ControllerBase
    {
        private readonly IDataSource _dataSource;
        private readonly ILogger<ServiceController> _logger;
        private const string AuthCookie = "AutoTechCentr";
        private const string vowels = "ауоыиэь";

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
            searchString = searchString.Insert(0, "%");
            searchString = searchString.Insert(searchString.Length, "%");
            string res = searchString;
            foreach (char vowel in vowels)
            {
                searchString = searchString.Replace(vowel, '_');
            }
            try
            {
                var serviceByCategoryRead = await _dataSource.ReadServiceSearch(searchString);
                return Ok(serviceByCategoryRead);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
                
        }
    }
}
