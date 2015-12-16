# Medicine_System
医药信息管理系统

涉及工具及技术：WPF、SQL Server 2012、Microsoft Visual Studio 2015
#省市三联动关键代码
```
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
        //市联动
        private void cbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //获取选中的省份对象
            Province province = (Province)cbProvince.SelectedItem;
            //清空cbCity.Items
            cbCity.Items.Clear();

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
    }
```
