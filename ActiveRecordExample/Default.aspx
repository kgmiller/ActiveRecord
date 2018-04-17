<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ActiveRecordExample._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h2>Projects</h2>
    
    <h3>Create New Project</h3>
    <b>Project Title:</b>
    <br />
    <asp:TextBox ID='NewProjectTitle' runat='server'></asp:TextBox>
    <br />
    <asp:Button ID='CreateNewProjectButton' runat='Server' Text="Create"/>
    <br />
    
    <h3>Current Projects</h3>
    <asp:Repeater ID="ProjectRepeater" runat="server">
        <ItemTemplate>
            <div>
                <asp:LinkButton ID="ProjectTitle" runat="server" Text='<%# Eval("Title") %>' CommandArgument='<%# Eval("ProjectId") %>' CommandName='View Project'></asp:LinkButton>        
            </div>
        </ItemTemplate>
    </asp:Repeater>
    
    </div>
    </form>
</body>
</html>
