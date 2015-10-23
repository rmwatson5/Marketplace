namespace SimpleAudit.Models
{
	using System;
	using System.Collections.Generic;

	public class ItemInformation
	{
		public Guid ItemId { get; set; }
		public string UserName { get; set; }
		public DateTime DateModified { get; set; }
		public IList<FieldInformation> FieldsModified { get; set; } 
	}
}
