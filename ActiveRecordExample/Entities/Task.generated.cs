using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord;

namespace ActiveRecordExample
{	
	[Table()]
	public partial class Task : ActiveRecord<Task>
	{		
		#region -Columns-
		protected long _taskId;
		[PrimaryKeyColumn]
		public long TaskId
		{
			get {return _taskId;}
			set {_taskId = value;}
		}

		protected string _title = String.Empty;
		[Column()]
		public string Title
		{
			get {return _title;}
			set {_title = value;}
		}

		protected string _description = String.Empty;
		[Column()]
		public string Description
		{
			get {return _description;}
			set {_description = value;}
		}

		protected long _userId;
		[Column()]
		public long UserId
		{
			get {return _userId;}
			set {_userId = value;}
		}

		protected long _projectId;
		[Column()]
		public long ProjectId
		{
			get {return _projectId;}
			set {_projectId = value;}
		}
		#endregion
	}

}
