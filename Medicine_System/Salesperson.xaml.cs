using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using MediDBDAL;
using System.Text;

namespace Medicine_System
{
    /// <summary>
    /// Salesperson.xaml 的交互逻辑
    /// </summary>
    public partial class Salesperson : Window
    {
        string cnStr = @"Data Source = localhost;Integrated Security = SSPI; Initial Catalog = MediDB";
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
            //清空cbCity.Items
            cbCounty.Items.Clear();

            using (SqlConnection conn = new SqlConnection(cnStr))
            {
                conn.Open();//打开数据库
                //执行SQL语句并将结果保存在DataTable dt
                string sql = "select DistrictName from District, City where District.CityID = City.CityID And CityName ='" + cbCity.SelectedItem.ToString() + "'";
                DataTable dt = new DataTable();
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
            string cnStr = @"Data Source = localhost;Integrated Security = SSPI; Initial Catalog = MediDB";
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
    }
}
