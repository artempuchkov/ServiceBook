using Db.SQLite.Migrations;
using ServiceBook.Db.SQLite;
using ServiceBook.EmailService;

namespace ServiceBook.App;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		var dbStateManager = new SQLiteDbStateManager(builder.Configuration.GetSection("connectionStrings:default").Get<string>()!);
		dbStateManager.UpdateDatabase();

		// Add services to the container.

		builder.Services.AddControllers();

		builder.Services.AddSingleton<IDbConnectionFactory, SQLiteDatabase>();
		builder.Services.AddSingleton<IDataSource, SQLiteDataSource>();

		builder.Services.AddSingleton<IEmailService, EmailService.EmailService>();

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		var app = builder.Build();

		app.UseSwagger();
		app.UseSwaggerUI();

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}