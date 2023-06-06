using Microsoft.AspNetCore.Mvc;
using MimeKit;
using ServiceBook.Db.SQLite;

namespace ServiceBook.App.Controllers;

[ApiController]
[Route("[controller]")]
public class RepairStatusController : ControllerBase
{
	private readonly IDataSource _dataSource;
	private readonly ILogger<RepairStatusController> _logger;
    private const string AuthCookie = "AutoTechCentr";

    public RepairStatusController(IDataSource dataSource, ILogger<RepairStatusController> logger)
	{
		_dataSource = dataSource;
		_logger = logger;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll(int? id = null)
	{
		try
		{
            var repairStatus = await _dataSource.ReadRepairStatus(id);
			return Ok(repairStatus);
		}
		catch (Exception ex)
		{
			_logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
			return BadRequest(ex.Message);
		}
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] RepairStatus status)
	{
		try
		{
			if (string.IsNullOrEmpty(status.Name))
			{
				_logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400");
				return BadRequest("Ошибка при создании статуса.");
			}

			await _dataSource.SaveRepairStatus(status);
			return Ok();
		}
		catch (Exception ex)
		{
			_logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
			return BadRequest(ex.Message);
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update([FromRoute] int id, [FromBody] RepairStatus status)
	{
		try
		{
			status.Id = id;
			await _dataSource.SaveRepairStatus(status);
			return Ok();
		}
		catch (Exception ex)
		{
			_logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
			return BadRequest(ex.Message);
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete([FromRoute] int id)
	{
		try
		{
			await _dataSource.DeleteRepairStatus(id);
			return Ok();
		}
		catch (Exception ex)
		{
			_logger.LogError($"API {Request.Method}: {Request.Path} || Status Code Response: 404 || Exception: {ex.Message}");
			return BadRequest(ex.Message);
		}
	}
}