using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ActiveRecordExample
{
    public partial class ViewProject : System.Web.UI.Page
    {
        Project project;
        long projectId;

        protected void Page_Load(object sender, EventArgs e)
        {            
            InitializeEvents();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            InitializeDataSources();
        }

        void InitializeEvents()
        {
            UsersDataGrid.ItemDataBound += new DataGridItemEventHandler(UsersDataGrid_ItemDataBound);        
        }

        void InitializeDataSources()
        {
            projectId = (long)HttpContext.Current.Items["ProjectId"];
            project = Project.Find(projectId);

            //Note - Since we are not using weak references yet, records are cached in memory.
            //a call to refresh ensures we are getting the latest from the DB.
            project.Refresh();

            UsersDataGrid.DataSource = project.Users;
            UsersDataGrid.DataBind();

            ProjectName.Text = project.Title;        
        }

        void UsersDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                Repeater userTasks = (Repeater)e.Item.FindControl("UserTasksRepeater");
                long userId = (long)UsersDataGrid.DataKeys[e.Item.DataSetIndex];

                User user = ActiveRecordExample.User.Find(userId);

                userTasks.DataSource = user.Tasks;
                userTasks.DataBind();
            }
        }

    }
}
