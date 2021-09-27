using System;
using System.Collections.Generic;
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
		static void Main(string[] args)
		{
			if(args.Length == 0) return;

			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = false
			};

			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			var list = new List<Data>();
			foreach (var file in args)
			{
				using var reader = new StreamReader(file, Encoding.GetEncoding("SHIFT_JIS"));
				using var csv = new CsvReader(reader, config);
				list = csv.GetRecords<Data>().ToList();

				var desktop = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				var path = Path.Combine(desktop, $"{Path.GetFileNameWithoutExtension(file)}.yaml");
				using var fs = File.Create(path);
				fs.Close();
				using var sw = new StreamWriter(path, false, Encoding.UTF8);

				sw.WriteLine($"{GetSpace(4)}{Path.GetFileNameWithoutExtension(file)}:");
				sw.WriteLine($"{GetSpace(6)}description:");
				sw.WriteLine($"{GetSpace(6)}required:");
				foreach (var entity in list)
				{
					if (entity.Converted必須) sw.WriteLine($"{GetSpace(6)}- {entity.物理名PascalCase}");
				}
				sw.WriteLine($"{GetSpace(6)}type: object");
				sw.WriteLine($"{GetSpace(6)}properties:");
				
				foreach (var entity in list)
				{
					sw.WriteLine($"{GetSpace(8)}{entity.物理名PascalCase}:");
					sw.WriteLine($"{GetSpace(10)}description: {entity.論理名}");
					sw.WriteLine($"{GetSpace(10)}type: {entity.Converted型}");
				}
			}
		}

		public static string GetSpace(int num)
		{
			var ret = string.Empty;
			for (var i = 0; i < num; i++)
			{
				ret += " ";
			}

			return ret;
		}
	}
}
