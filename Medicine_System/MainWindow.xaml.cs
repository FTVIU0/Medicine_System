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
using MediDBDAL;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Medicine_System
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //MediDBConnDAL mediConn = new MediDBConnDAL();
        public static string userName;
        public MainWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.CanMinimize;//禁止使用最大化按钮
            
        }

        //登录 按钮点击事件处理
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(txtUserName.Text == ""||ptxtUserPassWord.Password == "")
            {
                MessageBox.Show("登录失败，请检查用户名或密码");
            }
            else
            {
                //连接数据库
                SqlConnection conn = new SqlConnection();
                string cnStr = @"Data Source = localhost;Integrated Security = SSPI; Initial Catalog = MediDB";
                string sql = "Select * From agency Where ano=" + txtUserName.Text + "And  password=" + ptxtUserPassWord.Password;
                conn.ConnectionString = cnStr;
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader sqlDataRead = cmd.ExecuteReader();

                //创建各个窗体对象
                Salesperson salesperson = new Salesperson();
                Purchase purchase = new Purchase();
                Manage manage = new Manage();

                if (sqlDataRead.HasRows)
                {
                    switch (cbRole.SelectedIndex)
                    {
                        case 1://销售员
                            userName = txtUserName.Text;
                            Application.Current.MainWindow = salesperson;//设置应用程序的主窗口
                            this.Close();//关闭登录窗口
                            salesperson.Show();//打开Salesperson窗口
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
                else
                {
                    MessageBox.Show("用户名或密码有错误！");
                }
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
