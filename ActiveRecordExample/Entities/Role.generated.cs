using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord;

namespace ActiveRecordExample
{	
	[Table()]
	public partial class Role : ActiveRecord<Role>
	{		
		#region -Columns-
		protected long _roleId;
		[PrimaryKeyColumn]
		public long RoleId
		{
			get {return _roleId;}
			set {_roleId = value;}
		}

		protected string _title = String.Empty;
		[Column()]
		public string Title
		{
			get {return _title;}
			set {_title = value;}
		}
		#endregion
	}

}
