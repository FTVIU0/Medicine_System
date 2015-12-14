using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Medicine_System
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.CanMinimize;//禁止使用最大化按钮
        }

        //登录 按钮点击事件处理
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            Salesperson salesperson = new Salesperson();
            Purchase purchase = new Purchase();
            Manage manage = new Manage();
            //Application.Current.MainWindow = salesperson;//设置应用程序的主窗口
            
            switch (cbRole.SelectedIndex)
            {
                case 1://销售员
                    Application.Current.MainWindow = salesperson;//设置应用程序的主窗口
                    this.Close();//关闭登录窗口
                    salesperson.Show();//打开Salesperson窗口
                    //Close();
                    break;
                case 2://采购员
                    Application.Current.MainWindow = purchase;//设置应用程序的主窗口
                    this.Close();//关闭登录窗口
                    purchase.Show();//打开Purchase窗口
                    break;
                case 3://管理者
                    Application.Current.MainWindow = manage;//设置应用程序的主窗口
                    this.Close();//关闭登录窗口
                    manage.Show();//打开Manage窗口
                    break;
                default:
                    MessageBox.Show("请选择角色", "提示");
                    break;
            }
        }

        //取消 按钮点击事件处理
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();//关闭登录窗口
        }

        //重置 按钮点击事件处理
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtUserName.Text = "";
            ptxtUserPassWord.Password = "";
        }
    }
}
