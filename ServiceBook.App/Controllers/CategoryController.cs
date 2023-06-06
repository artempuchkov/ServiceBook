using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceBook.Db.SQLite;

namespace ServiceBook.App.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IDataSource _dataSource;
        private readonly ILogger<CategoryController> _logger;
        private const string AuthCookie = "AutoTechCentr";

        public CategoryController(IDataSource dataSource, ILogger<CategoryController> logger)
        {
            _dataSource = dataSource;
            _logger = logger;
        }

        [HttpGet]
        [Route("api/Category/GetCategoryById")]
        public async Task<IActionResult> GetCategoryById()
        {
            try
            {
                var category = await _dataSource.GetCategories();
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
