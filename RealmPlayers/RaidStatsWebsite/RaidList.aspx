﻿<%@ Page Title="" Language="C#" MasterPageFile="~/RaidDamageMasterFrame.Master" AutoEventWireup="true" CodeBehind="RaidList.aspx.cs" Inherits="VF.RaidDamageWebsite.RaidList" %>

<%@OutputCache Duration="60" VaryByParam="*" %>
<%--<%@OutputCache Location="Server" Duration="60" VaryByParam="*" VaryByCustom="UserID" %>--%>

<%@ Register Src="RealmControl.ascx" TagPrefix="uc1" TagName="RealmControl" %>
<%@ Register Src="InstanceControl.ascx" TagPrefix="uc1" TagName="InstanceControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
    </ul><!--/.breadcrumb -->
    
    <header class="page-header">  
        <div class="row">
          <div class="span8">
              <%= m_RaidListInfoHTML %>
          </div>
          <div class="span4" style="min-width:200px;">
              <div style="margin: 0px 0px 0px 10px; float:right; ">
                    <uc1:RealmControl runat="server" ID="RealmControl" />
                </div>
              <div style="margin: 0px 0px 0px 0px; float:right; ">
                    <uc1:InstanceControl runat="server" ID="InstanceControl" />
                </div>
          </div>
        </div>
    </header>
    
    <%= m_PaginationHTML %>

    <table id="characters-table" class="table">
        <thead>
            <%= m_TableHeadHTML %>
        </thead>
        <tbody>
            <%= m_TableBodyHTML %>
        </tbody>
    </table>
    
    <%= m_PaginationHTML %>
</asp:Content>
