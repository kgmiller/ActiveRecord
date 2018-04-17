using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord;

namespace ActiveRecordExample
{	
	[Table()]
	public partial class Project : ActiveRecord<Project>
	{		
		#region -Columns-
		protected long _projectId;
		[PrimaryKeyColumn]
		public long ProjectId
		{
			get {return _projectId;}
			set {_projectId = value;}
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
