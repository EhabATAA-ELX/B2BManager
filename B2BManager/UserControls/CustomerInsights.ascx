<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CustomerInsights.ascx.vb" Inherits="UserControls_CustomerInsights" %>

<div class="row" style="margin: 5px">
    <!-- /.col -->
    <div class="col-12 col-sm-6 col-md-3">
        <div class="info-box mb-3">
            <span class="info-box-icon bg-pink elevation-1"><i class="fas fa-users"></i></span>

            <div class="info-box-content">
                <span class="info-box-text">Total User(s)<br />
                    &nbsp</span>
                <span class="info-box-number" id="usersCount"><img width="22" src="Images/Loading.gif" /></span>
            </div>
            <!-- /.info-box-content -->
        </div>
        <!-- /.info-box -->
    </div>
    <!-- /.col -->
    <div class="col-12 col-sm-6 col-md-3">
        <div class="info-box">
            <span class="info-box-icon bg-info elevation-1"><i class="fas fa-user-ninja"></i></span>

            <div class="info-box-content">
                <span class="info-box-text">Linked super user(s)<br />
                    &nbsp</span>
                <span class="info-box-number" id="superUsersCount"><img width="22" src="Images/Loading.gif" /></span>
            </div>
            <!-- /.info-box-content -->
        </div>
        <!-- /.info-box -->
    </div>
    <!-- /.col -->
    <div class="col-12 col-sm-6 col-md-3">
        <div class="info-box mb-3">
            <span class="info-box-icon bg-teal elevation-1"><i class="fas fa-user"></i></span>

            <div class="info-box-content">
                <span class="info-box-text">Most active user<br />
                    <small>In last 6 months</small></span>
                <span class="info-box-number" id="MostActiveUser"><img width="22" src="Images/Loading.gif" /></span>
            </div>
            <!-- /.info-box-content -->
        </div>
        <!-- /.info-box -->
    </div>
    <!-- /.col -->

    <!-- fix for small devices only -->
    <div class="clearfix hidden-md-up"></div>

    <div class="col-12 col-sm-6 col-md-3">
        <div class="info-box mb-3">
            <span class="info-box-icon bg-success elevation-1"><i class="fas fa-shopping-cart"></i></span>

            <div class="info-box-content">
                <span class="info-box-text">Orders placed<br />
                    <small>In last 6 months</small></span>
                <span class="info-box-number" id="OrdersPlaced"><img width="22" src="Images/Loading.gif" /></span>
            </div>
            <!-- /.info-box-content -->
        </div>
        <!-- /.info-box -->
    </div>
</div>
