namespace CsvToApiDataType
{
	public class Data
	{
		public string 論理名 { get; set; }

		public string 物理名SnakeCase { get; set; }

		public string 型 { get; set; }

		public string 必須 { get; set; }

		public string 物理名PascalCase => SnakeCaseToPascalCase(物理名SnakeCase);

		public string Converted型
		{
			get
			{
				return 型 switch
				{
					"VARCHAR2" => "string",
					"CHAR" => "string",
					"DATE" => "string",
					"NUMBER" => "number",
					_ => string.Empty
				};
			}
		}

		public bool Converted必須 => 必須 == "〇" || 必須 == "○";

		public string SnakeCaseToPascalCase(string name)
		{
			var ret = string.Empty;

			var list = name.Split('_');
			foreach (var s in list)
			{
				ret += s[0];
				if (2 <= s.Length) ret += s[1..].ToLower();
			}

			return ret;
		}
	}
}
