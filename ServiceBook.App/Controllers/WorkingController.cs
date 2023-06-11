using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceBook.Db.SQLite;
using ServiceBook.Db.SQLite.Models;

namespace ServiceBook.App.Controllers
{
    [ApiController]
    public class WorkingController : Controller
    {
        private readonly IDataSource _dataSource;
        private readonly ILogger<WorkingController> _logger;
        private const string AuthCookie = "AutoTechCentr";

        public WorkingController(IDataSource dataSource, ILogger<WorkingController> logger)
        {
            _dataSource = dataSource;
            _logger = logger;
        }

        [HttpGet]
        [Route("api/WorkingMode/GetWorkingMode")]
        public async Task<IActionResult> GetWorkingMode(int? id = null)
        {
            try
            {
                var workinMode = await _dataSource.ReadWorkingMode(id);
                return Ok(workinMode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("api/WorkingMode/GetReception")]
        public async Task<IActionResult> GetReception(int? id = null)
        {
            try
            {
                var reception = await _dataSource.ReadReception(id);
                return Ok(reception);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("api/WorkingMode/GetMaster")]
        public async Task<IActionResult> GetMaster(int? id = null)
        {
            try
            {
                var master = await _dataSource.ReadMaster(id);
                return Ok(master);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("api/WorkingMode/UpdateWorkingMode")]
        public async Task<IActionResult> UpdateWorkingMode([FromBody] WorkingModeModel model)
        {
            try
            {
                await _dataSource.SaveWorkingModel(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("api/WorkingMode/UpdateMaster")]
        public async Task<IActionResult> UpdateMaster([FromBody] MasterModel model)
        {
            try
            {
                await _dataSource.SaveMaster(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
