using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord;

namespace ActiveRecordExample
{	
	public partial class Task : ActiveRecord<Task>
	{		
		#region -Relationships-
        
        public BelongsTo<User> user = new BelongsTo<User>();

        public User User
        {
            get { return user.Object; }
            set { user.Object = value; }
        }

        #endregion
	}

}
