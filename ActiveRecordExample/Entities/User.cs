using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord;

namespace ActiveRecordExample
{	
	public partial class User : ActiveRecord<User>
	{		
		#region -Relationships-

        public HasAndBelongsToMany<Project> projects = new HasAndBelongsToMany<Project>();

        public List<Project> Projects
        {
            get { return projects.Object;}
            set { projects.Object = value;}
        }

        public HasMany<Task> tasks = new HasMany<Task>();

        public List<Task> Tasks
        {
            get {return tasks.Object;}
            set {tasks.Object = value;}
        }

        public BelongsTo<Role> role = new BelongsTo<Role>();

        public Role Role
        {
            get { return role.Object; }
            set { role.Object = value; }
        }


		#endregion
	}

}
