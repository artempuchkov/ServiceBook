using Dapper;
using ServiceBook.EmailService;

using Flurl;
using System.Web;
using Org.BouncyCastle.Math.EC.Rfc7748;
using ServiceBook.Db.SQLite.Models;
using System.Linq;

namespace ServiceBook.Db.SQLite;
public class SQLiteDataSource : IDataSource
{
    private int _sqlCommandTimeout = 600;   // seconds

    private readonly IDbConnectionFactory _db;
    private readonly IEmailService _emailService;

    public SQLiteDataSource(IDbConnectionFactory db, IEmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    public async Task SaveRepairStatus(RepairStatus status)
    {
        if (status == null)
            return;

        const string sql = @"
			insert into repair_status (id, name)
				values (@Id, @Name)
			on conflict(id) do
			update set
				name = excluded.name;";

        await _db.CreateConnection().ExecuteAsync(sql, status, commandTimeout: _sqlCommandTimeout);
    }
    public async Task DeleteRepairStatus(int id)
    {
        await _db.CreateConnection().QueryAsync("DELETE FROM repair_status WHERE id = @id", new { id }, commandTimeout: _sqlCommandTimeout);
    }
    public async Task<RepairStatus[]> ReadRepairStatus(int? id = null)
    {
        const string sql = @"
			select
				id as Id,
				name as Name
			from repair_status
			where @IdRepairStatus is null or id = @IdRepairStatus";

        var data = await _db.CreateConnection().QueryAsync<RepairStatus>(sql, new { IdRepairStatus = id }, commandTimeout: _sqlCommandTimeout);
        return data.ToArray();
    }
    public async Task<string> SignIn(LoginViewModel model)
    {
        var userCode = await _db.CreateConnection().QuerySingleAsync<string>("select user_code from user where email = @email and password = @password", new
        {
            email = model.Email,
            password = model.Password.GetMd5Hash()
        }, commandTimeout: _sqlCommandTimeout);

        if (String.IsNullOrEmpty(userCode))
            throw new Exception("Неверен логин или пароль.");

        var confirmed = await _db.CreateConnection().QuerySingleAsync<int>("select confirm_email from user where user_code = @user_code", new
        {
            user_code = userCode
        }, commandTimeout: _sqlCommandTimeout);
        if (confirmed == 0)
            throw new Exception("Необходимо подтвердить e-mail");

        return userCode;


    }
    public async Task UserRegistration(string url, RegisterViewModel model)
    {
        var countUser = await _db.CreateConnection().QuerySingleOrDefaultAsync<int>("select count(1) from user where email = @email", new
        {
            email = model.Email
        }, commandTimeout: _sqlCommandTimeout);

        if (countUser != 0)
            throw new Exception("Пользователь с такой почтой уже зарегистрирован.");

        const string sqlAddUser = @"
			insert into bonus_card (id)
				values (@BonusCard);

			insert into user (user_code, email, password, number_phone, fio, bonus_card)
				values (@UserCode, @Email, @Password, @NumberPhone, @FIO, @BonusCard);";

        var userCode = Guid.NewGuid().ToString();

        await _db.CreateConnection().ExecuteAsync(sqlAddUser, new
        {
            UserCode = userCode,
            Email = model.Email,
            Password = model.Password.GetMd5Hash(),
            NumberPhone = model.NumberPhone,
            FIO = model.FIO,
            BonusCard = Guid.NewGuid().ToString()
        }, commandTimeout: _sqlCommandTimeout);

        await _emailService.SendEmailAsync(model.Email, "Подтверждение адреса электронной почты", CreateAccountActivationMessage(url, userCode));
    }

    public async Task ConfirmEmail(string userCode)
    {
        await _db.CreateConnection().QueryAsync("update user set confirm_email = 1 where user_code = @userCode", new { userCode });
    }

    private string CreateAccountActivationMessage(string url, string userCode)
    {
        var newUrl = url.Replace("https://", "").Replace("http://", "");

        Url urlActivation = Url.Combine(url, "api/Account/Login");
        urlActivation.QueryParams.Add("userCode", userCode);

        var urlActivationString = HttpUtility.UrlDecode(urlActivation);

        var htmlMessage = "<table style='font-family: \"Lucida Console\" Monospace' width='100%'>";
        htmlMessage += $"<tr style='height: 70px; background-color: #1E90FF; text-align: center; font-weight:700; font-size: 24px'><td>Регистрация на сайте {newUrl}</td></tr>";
        htmlMessage += "<tr style='height: 20px'></tr>";
        htmlMessage += "<tr style='font-weight:600; font-size: 20px'><td style='padding:0 50px'>Здравствуйте!</td></tr>";
        htmlMessage += "<tr style='height: 20px'></tr>";
        htmlMessage += "<tr style='font-size: 18px'><td style='padding:0 50px; text-align:justify'>Вы зарегистрировались в AutoTechCentr.</td></tr>";
        htmlMessage += "<tr style='height: 10px'></tr>";
        htmlMessage += $"<tr style='font-size: 18px'><td style='padding:0 50px; text-align:justify'>Для работы необходимо активировать учетную запись перейдя по этой <a href='{urlActivationString}'>ссылке</a></td></tr>";
        htmlMessage += "<tr style='height: 10px'></tr>";
        htmlMessage += "<tr style='font-size: 18px'><td style='padding:0 50px; text-align:justify'>Если она не работает, скопируйте этот код и вставьте в адресную строку браузера:</td></tr>";
        htmlMessage += "<tr style='height: 10px'></tr>";
        htmlMessage += $"<tr style='font-size: 16px; text-align: center'><td>{urlActivationString}</td></tr>";
        htmlMessage += "<tr style='height: 20px'></tr>";
        htmlMessage += "<tr style='font-size: 18px'><td style='padding:0 50px; text-align:justify'>Если Вы не знаете о чем речь и первый раз слышите о таком сайте, то, возможно, кто-то ошибочно указал Ваш адрес электронной почты при регстрации.</td></tr>";
        htmlMessage += "<tr style='font-size: 18px'><td style='padding:0 50px; text-align:justify'>В таком случае просто проигнорируйте это письмо.</td></tr>";
        htmlMessage += "<tr style='height: 30px'></tr>";
        htmlMessage += $"<tr style='height: 70px; background-color: #1E90FF; text-align: center; font-weight:700; font-size: 24px'><td>{newUrl}</td></tr></table>";

        return htmlMessage;
    }
}