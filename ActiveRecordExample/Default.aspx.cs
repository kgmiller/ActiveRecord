using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ActiveRecordExample
{
    public partial class _Default : System.Web.UI.Page
    {
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
            ProjectRepeater.ItemCommand += new RepeaterCommandEventHandler(ProjectRepeater_ItemCommand);
            CreateNewProjectButton.Click += new EventHandler(CreateNewProjectButton_Click);
        }

        void InitializeDataSources()
        {
            List<Project> projects = Project.FindAll();
            ProjectRepeater.DataSource = projects;
            ProjectRepeater.DataBind();
        }

        void CreateNewProjectButton_Click(object sender, EventArgs e)
        {
            Project newProject = new Project();
            newProject.Title = NewProjectTitle.Text;
            newProject.Save();
            //Simple as that.
        }

        protected void ProjectRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "View Project")
            {
                HttpContext.Current.Items.Add("ProjectId", long.Parse(e.CommandArgument.ToString()));
                Server.Transfer("ViewProject.aspx");
            }
        }
    }
}
