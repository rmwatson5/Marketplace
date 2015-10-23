namespace SimpleAudit
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using Newtonsoft.Json;

	using SimpleAudit.Models;

	public class AuditLogger
	{
		protected const string SimpleAuditFolderName = "Simple Audit";
		protected readonly string SimpleAuditFolderPath = Sitecore.Configuration.Settings.DataFolder + @"\" + SimpleAuditFolderName;
		public void LogAudit(ItemInformation itemInformation)
		{
			var itemLogPath = itemInformation.ItemId.ToString();

			try
			{
				var itemLog = this.GetItemLogFile(itemLogPath);
				this.LogNewEntry(itemLog, itemInformation);
			}
			catch (IOException e)
			{
			}
		}

		protected FileInfo GetItemLogFile(string itemPath)
		{
			var directoryInfo = new DirectoryInfo(this.SimpleAuditFolderPath);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}

			var itemLog = directoryInfo.GetFiles(itemPath).FirstOrDefault();
            if (itemLog == null)
			{
				var filePath = this.SimpleAuditFolderPath + @"\" + itemPath;
				itemLog = new FileInfo(filePath);
				itemLog.Create();
			}

			return itemLog;
		}

		protected void LogNewEntry(FileInfo itemLog, ItemInformation itemInformation)
		{
			var currentItemInfo = this.GetItemLog(itemLog.FullName);
            currentItemInfo.Add(itemInformation);
			var currentItemText = JsonConvert.SerializeObject(currentItemInfo);
			File.WriteAllText(itemLog.FullName, currentItemText);
		}

		protected IList<ItemInformation> GetItemLog(string itemPath)
		{
			var itemtext = File.ReadAllText(itemPath);
			return JsonConvert.DeserializeObject<IList<ItemInformation>>(itemtext) ?? new List<ItemInformation>();
		}
	}
}
