using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using MediDBDAL;
using System.Text;
using Microsoft.Office.Interop.Excel;
//using Microsoft.Office.Interop.Excel;

namespace Medicine_System
{
    /// <summary>
    /// Salesperson.xaml 的交互逻辑
    /// </summary>
    public partial class Salesperson : System.Windows.Window
    {
        string cnStr = @"Data Source = localhost;Integrated Security = SSPI; Initial Catalog = MediDB";
        bool tf = false;
        public Salesperson()
        {
            InitializeComponent();
            
        }

        //省份
        class Province
        {
            public string ProcinceName { get; set; }
            public int ProvinceID { get; set; }
            public override string ToString()
            {
                return ProcinceName;
            }
        }

        //城市
        class City
        {
            public string CityName { get; set; }
            public int ProvinceID { get; set; }
            public int CityID { get; set; }
            public override string ToString()
            {
                return CityName;
            }
        }

        //Salesperson窗口Load事件
        private void Salesperson_Load(object sender, RoutedEventArgs e)
        {
            cbCity.Visibility = Visibility.Hidden;
            lbCity.Visibility = Visibility.Hidden;
            cbCounty.Visibility = Visibility.Hidden;
            lbCounty.Visibility = Visibility.Hidden;

            tbAgency.Text = MainWindow.userName;//从登录窗口传递UserName到经办人编号
            //初始化年份、月份
            for (int i = 2015; i >= 1920; i--)
            {
                cbYear.Items.Add(Convert.ToString(i));//往ConboBox添加Item
            }
            for (int i = 1; i <= 12; i++)
            {
                cbMonth.Items.Add(Convert.ToString(i));//往ConboBox添加Item
            }
            //初始化省份
            using (SqlConnection conn = new SqlConnection(cnStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from Province";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Province province = new Province();
                            province.ProvinceID = Convert.ToInt32(reader["ProvinceID"].ToString());//获取省份ID
                            province.ProcinceName = reader["ProvinceName"].ToString();//获取省份名
                            //填充cbProvince.Items
                            cbProvince.Items.Add(province); 
                        }
                    }
                }
                conn.Close();
            }
            
        }

        //cbProvince的SelectionChanged事件
        //省市联动
        //问题：存在事件触发冲突问题
        private void cbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbCity.Visibility = Visibility.Visible;
            lbCity.Visibility = Visibility.Visible;

            //获取选中的省份对象
            Province province = (Province)cbProvince.SelectedItem;
            //清空cbCity.Items
            cbCity.Items.Clear();

            //cbCounty.Items.Clear();

            using (SqlConnection conn = new SqlConnection(cnStr))//连接数据库
            {
                conn.Open();//打开数据库
                //执行SQL语句并将结果保存在DataSet dataSet
                string sql =  "select * from City";
                DataSet dataSet = new DataSet();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                sda.Fill(dataSet);

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    //获取dataSet中的ProvinceID和CityName
                    int ProvinceID = int.Parse(dataSet.Tables[0].Rows[i][3].ToString());
                    string CityName = dataSet.Tables[0].Rows[i][1].ToString();
                    //填充cbCity.Items
                    if (ProvinceID == province.ProvinceID)
                    {
                        cbCity.Items.Add(CityName);
                    }
                }   
            }
        }

        //cbCity的SelectionChanged事件
        //县区联动
        private void cbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbCounty.Visibility = Visibility.Visible;
            lbCounty.Visibility = Visibility.Visible;
            //清空cbCity.Items
            cbCounty.Items.Clear();

            using (SqlConnection conn = new SqlConnection(cnStr))
            {
                conn.Open();//打开数据库
                            //执行SQL语句并将结果保存在DataTable dt             
                string sql = "select DistrictName from District, City where District.CityID = City.CityID And CityName ='" + cbCity.SelectedItem.ToString() + "'";
                System.Data.DataTable dt = new System.Data.DataTable();
                SqlCommand com = new SqlCommand(sql, conn);
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                sda.Fill(dt);
                //填充cbCounty.Items
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cbCounty.Items.Add(dt.Rows[i][0]);
                }
            }

        }
        //顾客信息类
        public class Client
        {
            public string cno { get; set; }//顾客编号
            public string cname { get; set; }//姓名
            public string csex { get; set; }//性别
            public int cage { get; set; }//年龄
            public string caddress { get; set; }//住址
            public string cphone { get; set; }//电话
            public string csymptom { get; set; }//症状
            public string mno { get; set; }//已购药品
            public string ano { get; set; }//经办人
            public string cremark { get; set; }//备注
            public DateTime cdate { get; set; }//录入日期
            public System.Windows.Controls.Button btndelete = new System.Windows.Controls.Button();
            

        }
        //初始化Client类
        private Client InitClient()
        {
            Client client = new Client();
            client.cno = tbNum.Text;//顾客编号
            client.cname = tbCName.Text;//姓名
            if ((bool)rbMan.IsChecked)//性别
            {
                client.csex = rbMan.Content.ToString();
            }
            else if ((bool)rbWomen.IsChecked)
            {
                client.csex = rbWomen.Content.ToString();
            }
            else
            {
                MessageBox.Show("请选择性别");
            }
            DateTime now = DateTime.Now;//获取系统时间 
            
            client.cage = now.Year - int.Parse(cbYear.SelectedItem.ToString());//年龄
            client.caddress = cbProvince.SelectedItem.ToString()+
                cbCity.SelectedItem.ToString()+
                cbCounty.SelectedItem.ToString() +
                tbAddress.Text;//住址
            client.cphone = tbPhoneNum.Text;//电话
            client.csymptom = tbSymptom.Text;//症状
            client.mno = tbMediNum.Text;//药品编号
            client.ano = tbNum.Text;//经办人编号
            client.cremark = tbRemark.Text;//备注
            client.cdate = DateTime.UtcNow;
            return client;
        }
        //点击”添加“按钮事件
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbNum.Text == "")
            {
                MessageBox.Show("请输入顾客编号！", "温馨提示");
            }
            else
            {
                Client client = InitClient();
                //动态数据集合
                //ObservableCollection<Client> clientList = new ObservableCollection<Client>();
                //clientList.Add(client);
                //dataGridClient.ItemsSource = clientList;
                //添加数据到顾客信息窗口（DataGrid）
                //将对象放到List<>,在用DataGrid把数据显示出来
                List<Client> clientL = new List<Client>();
                clientL.Add(client);
                dataGridClient.ItemsSource = clientL;
            }
            
        }
        //保存按钮点击事件处理
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //连接数据库
            //string cnStr = @"Data Source = localhost;Integrated Security = SSPI; Initial Catalog = MediDB";
            SqlConnection con = new SqlConnection(cnStr);
            //打开数据库
            con.Open();
            //插入命令
            StringBuilder sql = new StringBuilder();
                sql.Append("Insert into Client");
                sql.Append("(cno, cname, csex, cage, caddress, cphone, csymptom, mno, ano, cdata, cremark)");
                sql.Append("values");
                sql.Append("(@cno, @cname, @csex, @cage, @caddress, @cphone, @csymptom, @mno, @ano, @cdata, @cremark)");
            SqlCommand mycom = new SqlCommand(sql.ToString(), con);
            //添加参数
            mycom.Parameters.Add(new SqlParameter("@cno", SqlDbType.NChar, 10));
            mycom.Parameters.Add(new SqlParameter("@cname", SqlDbType.NVarChar, 8));
            mycom.Parameters.Add(new SqlParameter("@csex", SqlDbType.NVarChar, 1));
            mycom.Parameters.Add(new SqlParameter("@cage", SqlDbType.Int, 3));
            mycom.Parameters.Add(new SqlParameter("@caddress", SqlDbType.NVarChar, 50));
            mycom.Parameters.Add(new SqlParameter("@cphone", SqlDbType.NVarChar, 20));
            mycom.Parameters.Add(new SqlParameter("@csymptom", SqlDbType.VarChar, 50));
            mycom.Parameters.Add(new SqlParameter("@mno", SqlDbType.Char, 12));
            mycom.Parameters.Add(new SqlParameter("@ano", SqlDbType.Char, 12));
            mycom.Parameters.Add(new SqlParameter("@cdata", SqlDbType.DateTime));
            mycom.Parameters.Add(new SqlParameter("@cremark", SqlDbType.NVarChar, 50));

            //给参数赋值
            mycom.Parameters["@cno"].Value = tbNum.Text;
            mycom.Parameters["@cname"].Value = tbCName.Text;
            if ((bool)rbMan.IsChecked)//性别
            {
                mycom.Parameters["@csex"].Value = rbMan.Content.ToString();
            }
            else
            {
                mycom.Parameters["@csex"].Value = rbWomen.Content.ToString();
            }
            DateTime now = DateTime.Now;//获取系统时间 
            mycom.Parameters["@cage"].Value = now.Year - int.Parse(cbYear.SelectedItem.ToString());
            mycom.Parameters["@caddress"].Value = cbProvince.SelectedItem.ToString() +
                cbCity.SelectedItem.ToString() +
                cbCounty.SelectedItem.ToString() +
                tbAddress.Text;
            mycom.Parameters["@cphone"].Value = tbPhoneNum.Text;
            mycom.Parameters["@csymptom"].Value = tbSymptom.Text;
            mycom.Parameters["@mno"].Value = tbMediNum.Text;
            mycom.Parameters["@ano"].Value = tbNum.Text;
            mycom.Parameters["@cdata"].Value = DateTime.UtcNow;
            mycom.Parameters["@cremark"].Value = tbRemark.Text;
            //执行添加语句 
            int i = mycom.ExecuteNonQuery();
            if (i>=1)
            {
                MessageBox.Show("保存成功");
            }
            //关闭数据库
            con.Close();
        }
        //查询按钮点击事件
        private void btnMInquiry_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(cnStr))
            {
                conn.Open();//打开数据库
                //执行SQL语句并将结果保存在DataTable dt
                string sql = "select * from medicine where mno ='" + tbMediNum1.Text + "'";
                DataSet dataSet = new DataSet();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                sda.Fill(dataSet);
                //填充DataGrid
                dataGridMedi.ItemsSource = dataSet.Tables[0].DefaultView;
                conn.Close();

            }
        }
      
        private void btnCInquiry_Click(object sender, RoutedEventArgs e)
        {
            if (tbCName1.Text !="")
            {
                using (SqlConnection conn = new SqlConnection(cnStr))
                {
                    conn.Open();//打开数据库
                                //执行SQL语句并将结果保存在DataTable dt
                    string sql = "select * from client where cno ='" + tbCName1.Text + "'";
                    DataSet dataSet = new DataSet();
                    SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                    sda.Fill(dataSet);
                    //填充DataGrid
                    dataGridClient1.ItemsSource = dataSet.Tables[0].DefaultView;

                }
            }
            else
            {
                MessageBox.Show("请输入");
            }
            
        }
        //导出按钮点击事件
        private void btnMExport_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection conn = new SqlConnection(cnStr);
            conn.Open();//打开数据库
            //执行SQL语句并将结果保存在DataTable dt
            string sql = "select * from medicine where mno ='" + tbMediNum1.Text + "'";
            System.Data.DataTable dt = new System.Data.DataTable();
            SqlCommand com = new SqlCommand(sql, conn);
            SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
            sda.Fill(dt);
            //创建Excel  
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook excelWB = excelApp.Workbooks.Add(System.Type.Missing);    //创建工作簿（WorkBook：即Excel文件主体本身）  
            Worksheet excelWS = (Worksheet)excelWB.Worksheets[1];   //创建工作表（即Excel里的子表sheet） 1表示在子表sheet1里进行数据导出 

            //将数据导入到工作表的单元格  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    excelWS.Cells[i + 1, j + 1] = dt.Rows[i][j].ToString();   //Excel单元格第一个从索引1开始  
                }
            }

            excelWB.SaveAs("F:\\sanjiawan.xlsx");  //将其进行保存到指定的路径
            MessageBox.Show("成功导出文件到F:\\sanjiawan.xlsx");  
            excelWB.Close();
            excelApp.Quit();  //
            KillAllExcel(excelApp); //释放可能还没释放的进程  
        }

        //释放Excel进程
        public bool KillAllExcel(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            try
            {
                if (excelApp != null)
                {
                    excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    //释放COM组件，其实就是将其引用计数减1     
                    //System.Diagnostics.Process theProc;     
                    foreach (System.Diagnostics.Process theProc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                    {
                        //先关闭图形窗口。如果关闭失败.有的时候在状态里看不到图形窗口的excel了，     
                        //但是在进程里仍然有EXCEL.EXE的进程存在，那么就需要释放它     
                        if (theProc.CloseMainWindow() == false)
                        {
                            theProc.Kill();
                        }
                    }
                    excelApp = null;
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void btnCExport_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection conn = new SqlConnection(cnStr);
            conn.Open();//打开数据库
            //执行SQL语句并将结果保存在DataTable dt
            string sql = "select * from client where cno ='" + tbCName1.Text + "'";
            System.Data.DataTable dt = new System.Data.DataTable();
            SqlCommand com = new SqlCommand(sql, conn);
            SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
            sda.Fill(dt);
            //创建Excel  
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook excelWB = excelApp.Workbooks.Add(System.Type.Missing);    //创建工作簿（WorkBook：即Excel文件主体本身）  
            Worksheet excelWS = (Worksheet)excelWB.Worksheets[1];   //创建工作表（即Excel里的子表sheet） 1表示在子表sheet1里进行数据导出 

            //将数据导入到工作表的单元格  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    excelWS.Cells[i + 1, j + 1] = dt.Rows[i][j].ToString();   //Excel单元格第一个从索引1开始  
                }
            }

            excelWB.SaveAs("F:\\sanjiawan1.xlsx");  //将其进行保存到指定的路径 
            MessageBox.Show("成功导出文件到F:\\sanjiawan1.xlsx");
            excelWB.Close();
            excelApp.Quit();  //
            KillAllExcel(excelApp); //释放可能还没释放的进程
        }
        //退出按钮点击事件
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        //清空输入按钮点击事件
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            //tbNum.Text = "";
            //tbCName.Text = "";
            //cbYear.SelectedIndex = -1;
            //cbMonth.SelectedIndex = -1;
            //cbProvince.SelectedIndex = -1;
            //cbCity.SelectedIndex = -1;
            //cbCounty.SelectedIndex = -1;
            //tbAddress.Text = "";
            //tbPhoneNum.Text = "";
            //tbMediNum.Text = "";
            //tbSymptom.Text = "";
            //tbRemark.Text = "";
        }
        
    }
}
