using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord;

namespace ActiveRecordExample
{	
	public partial class Project : ActiveRecord<Project>
	{		
		#region -Relationships-
        
        
        public HasAndBelongsToMany<User> users = new HasAndBelongsToMany<User>();
        
        //By defining this Users List object we can access the list of users for this project
        //using a project.Users syntax rather than having to use project.Users.Object. 
        //Note that the users (lowercase) object above must be publicly accessible (this requirement may be
        //removed in the future)
        public List<User> Users
        {
            get { return users.Object; }
            set { users.Object = value; }
        }

        #endregion
	}

}
