<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SearchViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Search
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Search</h2>
    <% foreach(var view in Model.Documents){ %>
    <h3><a href="<%= view.Link %>"><%= view.Name %></a></h3>
    <p><%=view.Description %></p>
    <%} %>
</asp:Content>
