using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using ServiceBook.Db.SQLite;
using ServiceBook.Db.SQLite.Models;
using System.Data.Common;

namespace ServiceBook.App.Controllers
{
    [ApiController]
    public class CarController : Controller
    {
        private readonly IDataSource _dataSource;
        private readonly ILogger<CarController> _logger;
        public CarController(IDataSource dataSource, ILogger<CarController> logger)
        {
            _dataSource = dataSource;
            _logger = logger;
        }
        [HttpGet]
        [Route("api/Service/GetCars")]
        public async Task<IActionResult> GetCars(int? user_id = null)
        {
            try
            {
                var cars = await _dataSource.ReadCar(user_id);
                return Ok(cars);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("api/Request/AddCar")]
        public async Task<IActionResult> Register(CarModel model)
        {
            try
            {
                await _dataSource.AddCar(model);
                var msg = new
                {
                    message = "Успешо! Автомобиль добавлен!",
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
    }
    
}
