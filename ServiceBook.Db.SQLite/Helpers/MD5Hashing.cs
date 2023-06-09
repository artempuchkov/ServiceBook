﻿using System.Text;
using System.Security.Cryptography;

namespace ServiceBook.Db.SQLite;
public static class MD5Hashing
{
	public static string GetMd5Hash(this string input)
	{
		MD5 md5Hasher = MD5.Create();
		byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
		StringBuilder sBuilder = new StringBuilder();

		for (int i = 0; i < data.Length; i++)
			sBuilder.Append(data[i].ToString("x2"));

		return sBuilder.ToString();
	}
}