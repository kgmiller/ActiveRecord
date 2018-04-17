<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewProject.aspx.cs" Inherits="ActiveRecordExample.ViewProject" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h2>Project: <asp:Label ID='ProjectName' runat='server'></asp:Label></h2>
    
    <h2>Users</h2>
    <asp:DataGrid ID='UsersDataGrid' runat='server' DataKeyField='UserId' AutoGenerateColumns='false' ShowHeader="false" BorderStyle='None' BorderWidth='0'>
    <Columns>
    <asp:TemplateColumn>
        <ItemTemplate>
            <h3><asp:Label ID="User" runat="server" Text='<%# Eval("FirstName") + " " + Eval("LastName") %>'></asp:Label></h3>
            <asp:Repeater ID="UserTasksRepeater" runat="server">
            <ItemTemplate>
                <b>Task #<asp:Literal ID="Literal2" runat="server" Text='<%# Eval("TaskId")%>'></asp:Literal> <asp:Literal runat="server" Text='<%# Eval("Title")%>'></asp:Literal></b><br />
                <asp:Literal ID="Literal1" runat="server" Text='<%# Eval("Description")%>'></asp:Literal><br />
            </ItemTemplate>
            </asp:Repeater>        
        </ItemTemplate>
    </asp:TemplateColumn>
    </Columns>
    </asp:DataGrid>

    </div>
    </form>
</body>
</html>
