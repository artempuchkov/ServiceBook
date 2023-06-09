using System.ComponentModel.DataAnnotations;

namespace ServiceBook.Db.SQLite;

public class RegisterViewModel
{
	[Required]
	public string FIO { get; set; }
	[Required]
	public string Email { get; set; }
	[Required]
	public string NumberPhone { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public string Password { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public string PasswordConfirm { get; set; }
}
