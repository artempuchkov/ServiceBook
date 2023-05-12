using System.ComponentModel.DataAnnotations;

namespace ServiceBook.Db.SQLite;

public class RegisterViewModel
{
	public string FIO { get; set; }
	public string Email { get; set; }
	public string NumberPhone { get; set; }

	[DataType(DataType.Password)]
	public string Password { get; set; }

	[DataType(DataType.Password)]
	public string PasswordConfirm { get; set; }
}
