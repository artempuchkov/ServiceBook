using System.Text.RegularExpressions;
using ServiceBook.Db.SQLite;
using Microsoft.AspNetCore.Mvc;
using ServiceBook.Db.SQLite.Models;

namespace ServiceBook.App.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
	private readonly IDataSource _dataSource;
	private readonly ILogger<AccountController> _logger;
	private const string AuthCookie = "AutoTechCentr";

	public AccountController(IDataSource dataSource, ILogger<AccountController> logger)
	{
		_dataSource = dataSource;
		_logger = logger;
	}

	[HttpGet]
	[Route("api/Account/Login")]
	public async Task<IActionResult> ConfirmEmail(string userCode)
	{
		if (string.IsNullOrEmpty(userCode))
			return BadRequest("Ошибка при активации аккаунта.");

		try
		{
			await _dataSource.ConfirmEmail(userCode);

			return Ok("Аккаунт успешно активирован.");
		}
		catch (Exception ex)
		{
			_logger.LogError($"API: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
			return BadRequest(ex.Message);
		}
	}

	[HttpPost]
	[Route("api/Account/Register")]
	public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
	{
		try
		{
			if (string.IsNullOrEmpty(model.FIO))
				ModelState.AddModelError("", "Поле \"ФИО\" не заполнено.");

			if (!new Regex(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}").IsMatch(model.Email))
				ModelState.AddModelError("", "Некорректный адрес электронной почты.");

			if (!new Regex(@"[\d]{10}").IsMatch(model.NumberPhone))
				ModelState.AddModelError("", "Некорректный номер телефона.");

			if (string.IsNullOrEmpty(model.Password))
				ModelState.AddModelError("", "Поле \"Пароль\" не заполнено.");

			if (string.IsNullOrEmpty(model.PasswordConfirm))
				ModelState.AddModelError("", "Поле \"Пароль еще раз\" не заполнено.");

			if (!model.Password.Equals(model.PasswordConfirm))
				ModelState.AddModelError("", "Пароли не совпадают.");

			if (ModelState.IsValid)
			{
				var url = $"{Request.Scheme}://{Request.Host}";
				await _dataSource.UserRegistration(url, model);

				var msg = new
				{
					message = "Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме.",
				};
				return Ok(msg);
			}
			else
			{
				var errorMsg = new
				{
					message = "Пользователь не добавлен.",
					error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
				};

				_logger.LogError($"API: {Request.Path} || Status Code Response: 400 || Exception: {errorMsg.message}");
				return BadRequest(errorMsg);
			}
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", ex.Message);
			var errorMsg = new
			{
				message = "Пользователь не добавлен.",
				error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
			};

			_logger.LogError($"API: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
			return BadRequest(errorMsg);
		}
	}

	[HttpPost]
	[Route("api/Account/SignIn")]
	public async Task<IActionResult> SignIn([FromBody] LoginViewModel model)
	{
		try
		{
			if (!new Regex(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}").IsMatch(model.Email))
				ModelState.AddModelError("", "Некорректный адрес электронной почты.");
			if (string.IsNullOrEmpty(model.Password))
				ModelState.AddModelError("", "Поле \"Пароль\" не заполнено.");
			if (ModelState.IsValid)
			{
				var userCode = await _dataSource.SignIn(model);
				Response.Cookies.Append(AuthCookie, userCode);
				var Test = HttpContext.Request.Cookies[AuthCookie];

                var msg = new
				{
					message = "Вход выполнен.",
				};
				return Ok(msg);
			}
			else
			{
				var errorMsg = new
				{
					message = "Вход не выполнен.",
					error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
				};

				_logger.LogError($"API: {Request.Path} || Status Code Response: 400 || Exception: {errorMsg.message}");
				return BadRequest(errorMsg);
			}
		}
		catch(Exception ex)
		{
            ModelState.AddModelError("", ex.Message);
            var errorMsg = new
            {
                message = "Вход не выполнен.",
                error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
            };

            _logger.LogError($"API: {Request.Path} || Status Code Response: 400 || Exception: {ex.Message}");
            return BadRequest(errorMsg);
        }
	}
}