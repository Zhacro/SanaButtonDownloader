using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SanaButtonDownloader
{
	class SanaButtonDownloader
	{
		const string soundRoot = "https://www.natorisana.love/sounds/";
		static void Main(string[] args)
		{
			var wc = new WebClient();
			string html = Encoding.UTF8.GetString(wc.DownloadData("https://www.natorisana.love"));
			if (!CheckUpdate(html))
			{
				Console.WriteLine("更新はありません。");
				return;
			}
			Console.Write("OutputDirectory: ");
			var output = Console.ReadLine();
			//どうせボタンしか取得しないからこれでなんとかなる
			var re = new Regex(@"<button type=""button"" class=""sounds"" data-file=""(.+?)"">.+?</button>");
			foreach (var item in re.Matches(html).Cast<Match>().Select(i => i.Groups.Cast<Group>().ElementAt(1).Value))
			{
				Process(item, output);
			}
		}

		static void Process(string fileName,string targetDir)
		{
			var wc = new WebClient();
			var target = $"{targetDir}\\{fileName}.mp3";
			if (File.Exists(target)) return;
			//このままだと階層が2段以上になると死亡
			if (fileName.Contains("/")) Directory.CreateDirectory(targetDir + "\\" + fileName.Split('/')[0]);
			wc.DownloadFile(soundRoot + fileName+".mp3", target);
		}

		static bool CheckUpdate(string html)
		{
			if (File.Exists("LastUpdate.txt"))
			{
				var date = File.ReadAllText("LastUpdate.txt");
				//エクストリームガバガバ更新確認
				var dateReg = new Regex(@"\d?\d/\d?\d");
				if (dateReg.Match(html).Value == date) return false;
				Console.WriteLine("更新を確認しました。");
				return true;
			}
			else
			{
				Console.WriteLine("前回の使用記録が存在しません。作成します。");
				var log = File.CreateText("LastUpdate.txt");
				var dateReg = new Regex(@"\d?\d/\d?\d");
				log.Write(dateReg.Match(html).Value);
				log.Close();
				return true;
			}
		}
	}
}
