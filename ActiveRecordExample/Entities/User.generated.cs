using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord;

namespace ActiveRecordExample
{	
	[Table()]
	public partial class User : ActiveRecord<User>
	{		
		#region -Columns-
		protected long _userId;
		[PrimaryKeyColumn]
		public long UserId
		{
			get {return _userId;}
			set {_userId = value;}
		}

		protected string _firstName = String.Empty;
		[Column()]
		public string FirstName
		{
			get {return _firstName;}
			set {_firstName = value;}
		}

		protected string _lastName = String.Empty;
		[Column()]
		public string LastName
		{
			get {return _lastName;}
			set {_lastName = value;}
		}

		protected string _username = String.Empty;
		[Column()]
		public string Username
		{
			get {return _username;}
			set {_username = value;}
		}

		protected string _password = String.Empty;
		[Column()]
		public string Password
		{
			get {return _password;}
			set {_password = value;}
		}

		protected long _roleId;
		[Column()]
		public long RoleId
		{
			get {return _roleId;}
			set {_roleId = value;}
		}
		#endregion
	}

}
