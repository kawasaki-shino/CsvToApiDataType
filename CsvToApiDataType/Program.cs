using CsvHelper;
using CsvHelper.Configuration;
using Hnx8.ReadJEnc;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CsvToApiDataType
{
	class Program
	{
		private static readonly string TARGET = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CSVImport.csv");

		static void Main(string[] args)
		{
			var files = args.ToList();
			if (args.Length == 0 && File.Exists(TARGET))
			{
				files.Add(TARGET);
			}

			if (files.Count == 0)
			{
				Console.WriteLine("[エラー] ファイルの指定がありません。EXE ファイルに CSV ファイルをドロップするか、デスクトップ上に CSVImport.csv を用意してください。");
				Console.ReadLine();
				return;
			}

			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = false
			};


			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			foreach (var file in files)
			{
				// 文字コード判定
				byte[] bytes;
				using (var filecheck = new FileStream(file, FileMode.Open))
				{
					bytes = new byte[filecheck.Length];
					filecheck.Read(bytes, 0, bytes.Length);
				}

				var encode = ReadJEnc.JP.GetEncoding(bytes, bytes.Length, out _);

				var enc = string.Empty;
				switch (encode.Name)
				{
					case "UTF-8N":
					case "UTF-8":
						enc = "UTF-8";
						break;
					case "ShiftJIS":
						enc = "SHIFT-JIS";
						break;
				}

				if (string.IsNullOrWhiteSpace(enc))
				{
					Console.WriteLine("[エラー] 文字コードは、UTF-8N, UTF-8, SHIFT-JIS のいずれかにしてください。");
					return;
				}

				// 読み取り
				using var reader = new StreamReader(file, Encoding.GetEncoding(enc));
				using var csv = new CsvReader(reader, config);
				var list = csv.GetRecords<Data>().ToList();

				// 書き込み
				var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				var path = Path.Combine(desktop, $"{Path.GetFileNameWithoutExtension(file)}.yaml");
				using var fs = File.Create(path);
				fs.Close();
				using var sw = new StreamWriter(path, false, Encoding.UTF8);

				sw.WriteLine($"{new string(' ', 4)}{Path.GetFileNameWithoutExtension(file)}:");
				sw.WriteLine($"{new string(' ', 6)}description:");
				sw.WriteLine($"{new string(' ', 6)}required:");

				// 必須部分書き込み
				foreach (var entity in list)
				{
					if (entity.Converted必須) sw.WriteLine($"{new string(' ', 6)}- {entity.物理名PascalCase}");
				}
				sw.WriteLine($"{new string(' ', 6)}type: object");
				sw.WriteLine($"{new string(' ', 6)}properties:");

				// プロパティ部分書き込み
				foreach (var entity in list)
				{
					sw.WriteLine($"{new string(' ', 8)}{entity.物理名PascalCase}:");
					sw.WriteLine($"{new string(' ', 10)}description: {entity.論理名}");
					sw.WriteLine($"{new string(' ', 10)}type: {entity.Converted型}");
				}
			}

		}
	}
}
