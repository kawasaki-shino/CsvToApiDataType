using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace CsvToApiDataType
{
	class Program
	{
		private static readonly string TARGET = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CSVImport.csv");

		static void Main(string[] args)
		{
			var files = args.ToList();
			if (args.Length == 0 && File.Exists(TARGET))			{
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
				using var reader = new StreamReader(file, Encoding.GetEncoding("SHIFT_JIS"));
				using var csv = new CsvReader(reader, config);
				var list = csv.GetRecords<Data>().ToList();

				var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				var path = Path.Combine(desktop, $"{Path.GetFileNameWithoutExtension(file)}.yaml");
				using var fs = File.Create(path);
				fs.Close();
				using var sw = new StreamWriter(path, false, Encoding.UTF8);

				sw.WriteLine($"{new string(' ', 4)}{Path.GetFileNameWithoutExtension(file)}:");
				sw.WriteLine($"{new string(' ', 6)}description:");
				sw.WriteLine($"{new string(' ', 6)}required:");
				foreach (var entity in list)
				{
					if (entity.Converted必須) sw.WriteLine($"{new string(' ', 6)}- {entity.物理名PascalCase}");
				}
				sw.WriteLine($"{new string(' ', 6)}type: object");
				sw.WriteLine($"{new string(' ', 6)}properties:");

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
