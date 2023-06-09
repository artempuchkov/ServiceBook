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
    public async Task<WorkingModeModel[]> ReadWorkingMode(int? id = null)
    {
        const string sql = @" select 
                                Id as Id,
                                day as day,
                                time_start as time_start,
                                time_end as time_end
                              from working_mode
                              where @Id is null or id = @Id";
        var data = await _db.CreateConnection().QueryAsync<WorkingModeModel>(sql, new { Id = id }, commandTimeout: _sqlCommandTimeout);
        return data.ToArray();
    }
    public async Task SaveWorkingModel(WorkingModeModel model)
    {
        if (model == null)
            return;

        const string sql = @"
			insert into working_mode (Id, day, time_start, time_end)
				values (@Id, @day, @time_start, @time_end)
			on conflict(id) do
			update set
				day = excluded.day,
                time_start = excluded.time_start,
                time_end = excluded.time_end;";

        await _db.CreateConnection().ExecuteAsync(sql, model, commandTimeout: _sqlCommandTimeout);
    }
    public async Task<ServiceModel[]> ReadService(int? id = null)
    {

        const string sql = @"
			select
				service_id as Id,
				name as Name,
                description as Description,
                price as Price,
                img as Img,
                category as Category
			from service
			where @Id is null or service_id = @Id";
        var data = await _db.CreateConnection().QueryAsync<ServiceModel>(sql, new { Id = id }, commandTimeout: _sqlCommandTimeout);
        return data.ToArray();
    }
    public async Task<ReceptionModel[]> ReadReception(int? id = null)
    {
        const string sqlReception = @"select
                                        reception.id as Id,
                                        reception.name as Name,
                                        reception.capacity as Capacity,
                                        master.fio as Master,
                                        master.interval as Interval
                                    from reception
                                    join master on reception.master = master.id;";
        var data = await _db.CreateConnection().QueryAsync<ReceptionModel>(sqlReception, new { Id = id }, commandTimeout: _sqlCommandTimeout);
        return data.ToArray();
    }
    public async Task<MasterModel[]> ReadMaster(int? id = null)
    {

        const string sql = @"
			select
				id as Id,
				fio as FIO,
                interval as Interval,
                phone as PhoneNumber
			from master
			where @Id is null or id = @Id";
        var data = await _db.CreateConnection().QueryAsync<MasterModel>(sql, new { Id = id }, commandTimeout: _sqlCommandTimeout);
        return data.ToArray();
    }
    public async Task SaveMaster(MasterModel model)
    {
        if (model == null)
            return;

        const string sql = @"
			insert into master (id, fio, interval, phone)
				values (@Id, @FIO, @Interval, @PhoneNumber)
			on conflict(id) do
			update set
				fio = excluded.fio,
                interval = excluded.interval,
                phone = excluded.phone;";

        await _db.CreateConnection().ExecuteAsync(sql, model, commandTimeout: _sqlCommandTimeout);
    }
    public async Task<ServiceModel[]> ReadServiceSearch(string search)
    {
        const string sql = @"
              select service_id as Id,
				name as Name,
                description as Description,
                price as Price,
                img as Img,
                category as Category 
                from service where lower(name) LIKE lower(@search)
                or lower(description) LIKE lower(@search)
                ";
        var data = await _db.CreateConnection().QueryAsync<ServiceModel>(sql, new { search = search }, commandTimeout: _sqlCommandTimeout);
        return data.ToArray();

    }
    public async Task<ServiceModel[]> ReadByCategoryService(int category, int? service_id = null)
    {
        const string sql = @"
            select 
                service_id as Id,
                name as Name,
                description as Description,
                price as Price,
                img as Img,
                category as Category
            from service
            where category = @Category and (@Id is null or service_id = @Id)";
        var data = await _db.CreateConnection().QueryAsync<ServiceModel>(sql, new {Category = category, Id = service_id}, commandTimeout: _sqlCommandTimeout);
        return data.ToArray();
    }
    public async Task<CategoryModel[]> GetCategories(int? parent)
    {
        const string sql = @"
            select 
                category_id as Id,
                category_name as Name,
                parent_category as Parent,
                image_url as ImageUrl
            from category
            where @Parent is null or parent_category = @Parent";
        var data = await _db.CreateConnection().QueryAsync<CategoryModel>(sql, new { Parent = parent }, commandTimeout: _sqlCommandTimeout);
        return data.ToArray();
    }
    public async Task<UserInfoModel[]> ReadUserInfo(string userCode)
    {
        var data = await _db.CreateConnection().QueryAsync<UserInfoModel>("select" +
            " email as Email," +
            " number_phone as PhoneNumber," +
            " bonus_card as BonusCardId " +
            "from user where user_code = @Code", new
            {
                Code = userCode
            }, commandTimeout: _sqlCommandTimeout);
        return data.ToArray();
    }
    

    public async Task<UserIdentityModel> SignIn(LoginViewModel model)
    {
        var user = await _db.CreateConnection().QuerySingleAsync<UserIdentityModel>("select user_code as Id, " +
            "fio as UserName, email as Email," +
            "confirm_email as confirm from user where email = @email and password = @password", new
        {
            email = model.Email,
            password = model.Password.GetMd5Hash()
        }, commandTimeout: _sqlCommandTimeout);

        if (user == null)
            throw new Exception("Неверен логин или пароль.");

        if (user.Confirm == 0)
            throw new Exception("Необходимо подтвердить e-mail");

        return user;


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