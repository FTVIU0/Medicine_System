using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace Medicine_System
{
    /// <summary>
    /// Purchase.xaml 的交互逻辑
    /// </summary>
    public partial class Purchase : System.Windows.Window
    {
        //连接字符串
        string cnStr = @"Data Source = localhost;Integrated Security = SSPI; Initial Catalog = MediDB";
        public Purchase()
        {
            InitializeComponent();
        }
        //药品信息类
        public class Medicine
        {
            public string mno { get; set; }//药品编号
            public string mname { get; set; }//药品名字
            public string mmode { get; set; }//服用方法
            public string mefficacy { get; set; }//功效
        }
        //初始化药品类
        public Medicine InitMedi()
        {
            Medicine medi = new Medicine();
            medi.mno = tbMNum.Text;
            medi.mname = tbMName.Text;
            if ((bool)rbIn.IsChecked)//性别
            {
                medi.mmode = rbIn.Content.ToString();
            }
            else if ((bool)rbOut.IsChecked)
            {
                medi.mmode = rbOut.Content.ToString();
            }
            else
            {
                MessageBox.Show("请选择服用方法");
            }
            medi.mefficacy = tbEffect.Text;
            return medi;
        }
        //添加按钮点击事件
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbMNum.Text == "")
            {
                MessageBox.Show("请输入药品编号！", "温馨提示");
            }
            else
            {
                //创建并初始化Medi对象
                Medicine medi = InitMedi();
                
                List<Medicine> clientL = new List<Medicine>();
                clientL.Add(medi);
                dataGridMedi.ItemsSource = clientL;//填充DataGrid
            }
        }
        //导入按钮点击事件
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {

        }
        //保存按钮点击事件
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //连接数据库
            SqlConnection con = new SqlConnection(cnStr);
            //打开数据库
            con.Open();
            //插入命令
            StringBuilder sql = new StringBuilder();
            sql.Append("Insert into Medicine");
            sql.Append("(mno, mname, mmode, mefficacy)");
            sql.Append("values");
            sql.Append("(@mno, @mname, @mmode, @mefficacy)");
            SqlCommand mycom = new SqlCommand(sql.ToString(), con);
            //添加参数
            mycom.Parameters.Add(new SqlParameter("@mno", SqlDbType.VarChar, 12));
            mycom.Parameters.Add(new SqlParameter("@mname", SqlDbType.NVarChar, 50));
            mycom.Parameters.Add(new SqlParameter("@mmode", SqlDbType.NChar, 2));
            mycom.Parameters.Add(new SqlParameter("@mefficacy", SqlDbType.NChar, 10));

            //给参数赋值
            mycom.Parameters["@mno"].Value = tbMNum.Text;
            mycom.Parameters["@mname"].Value = tbMName.Text;
            if ((bool)rbIn.IsChecked)//性别
            {
                mycom.Parameters["@mmode"].Value = rbIn.Content.ToString();
            }
            else
            {
                mycom.Parameters["@mmode"].Value = rbOut.Content.ToString();
            }
            mycom.Parameters["@mefficacy"].Value = tbEffect.Text;
            //执行添加语句 
            int i = mycom.ExecuteNonQuery();
            if (i >= 1)
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
                string sql = "select * from medicine where mno ='" + tbMNum1.Text + "'";
                DataSet dataSet = new DataSet();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                sda.Fill(dataSet);
                //填充DataGrid
                dataGridMediI.ItemsSource = dataSet.Tables[0].DefaultView;
                conn.Close();

            }
        }
        //导出按钮点击事件
        private void btnMExport_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection conn = new SqlConnection(cnStr);
            conn.Open();//打开数据库
            //执行SQL语句并将结果保存在DataTable dt
            string sql = "select * from medicine where mno ='" + tbMNum1.Text + "'";
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

            excelWB.SaveAs("F:\\CCsanjiawan.xlsx");  //将其进行保存到指定的路径  
            MessageBox.Show("成功导出文件到F:\\CCsanjiawan.xlsx");
            excelWB.Close();
            excelApp.Quit();  //
            KillAllExcel(excelApp); //释放可能还没释放的进程
        }
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

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }
    }
}
