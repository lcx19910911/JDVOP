<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Web.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
        <asp:Button ID="refreshButton" runat="server" OnClick="refreshButton_Click" Text="更新商品库存和价格" style="margin-bottom: 0px" />
        <br />
        <br />

        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="查询商品池编号" />
&nbsp;<asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="查询池内商品编号" />
&nbsp;<asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="查询商品详细信息" />
&nbsp;<asp:Button ID="Button4" runat="server" OnClick="Button4_Click" Text="查询商品上下架状态" />
&nbsp;<asp:Button ID="Button5" runat="server" OnClick="Button5_Click" Text="查询所有图片信息" />
        <br />
        <br />
        <asp:Button ID="Button6" runat="server" OnClick="Button6_Click" Text="查询商品好评度" />
&nbsp;<asp:Button ID="Button7" runat="server" OnClick="Button7_Click" Text="商品区域购买限制" />
&nbsp;<asp:Button ID="Button9" runat="server" OnClick="Button9_Click" Text="商品可售验证" />
        <br />
        <br />
        <asp:Button ID="Button8" runat="server" OnClick="Button8_Click" Text="批量查询价格" />
        <br />
        <br />
        <asp:Button ID="Button10" runat="server" OnClick="Button10_Click" Text="获取京东地址" />
        <br />
        <br />
        <asp:Button ID="Button11" runat="server" OnClick="Button11_Click" Text="检查库存（列表）" />
&nbsp;<asp:Button ID="Button12" runat="server" Height="21px" OnClick="Button12_Click" Text="检查库存（下单）" />
        <br />
        <br />
        <asp:Button ID="Button13" runat="server" OnClick="Button13_Click" Text="余额查询" />
&nbsp;<asp:Button ID="Button14" runat="server" OnClick="Button14_Click" Text="下订单" />
    
    </div>
    </form>
</body>
</html>
