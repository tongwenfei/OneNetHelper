﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace OneNetHelper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public DataTable req_para;
        public DataRow dr;
        public String req_content;
        public Thread req_thread;
        public Thread Update_thread;
        delegate void MyDelegate(String text);
        public Uri uri;
        public String APIKEY;
        public String HostName,LastVaule;
        JObject CMDTABLE = new JObject();
        public MainWindow()
        {
            InitializeComponent();
            CMDTABLE["msgType"] = 0;
            CMDTABLE["cmdType"] = 1;
            CMDTABLE["SN"] = "102000217340000224";
           
            comb_method.Items.Add("POST");
            comb_method.Items.Add("GET");
            comb_method.Items.Add("PUT");
            comb_method.Items.Add("DELET");
            comb_method.SelectedIndex = 0;
            req_para = new DataTable();
            req_para.Columns.Add("参数", System.Type.GetType("System.String"));
           
            req_para.Columns.Add("参数值", System.Type.GetType("System.String"));
            string device_id = ConfigurationManager.AppSettings["device_id"];
            string timeout = ConfigurationManager.AppSettings["timeout"];
            string type = ConfigurationManager.AppSettings["type"];
            string qos = ConfigurationManager.AppSettings["qos"];
            DataRow dr1 = req_para.NewRow();
            DataRow dr2 = req_para.NewRow();
            DataRow dr3 = req_para.NewRow();
            DataRow dr4 = req_para.NewRow();
            dr1["参数"] = "device_id";
            dr1["参数值"] = device_id;
            dr2["参数"] = "timeout";
            dr2["参数值"] = timeout;
            dr3["参数"] = "type";
            dr3["参数值"] = type;
            dr4["参数"] = "qos";
            dr4["参数值"] = qos;
            req_para.Rows.Add(dr1);
            req_para.Rows.Add(dr2);
            req_para.Rows.Add(dr3);
            req_para.Rows.Add(dr4);
            Dg_Req_Para.ItemsSource = req_para.DefaultView;
           
        }

        private void Btn_Add_Para_Click(object sender, RoutedEventArgs e)
        {
            AddTable AddWindow = new AddTable
            {
                Owner = this
            };
            AddWindow.ChangeTextEvent += new ChangeTextHandler(Frm_ChangeTextEvent);
            AddWindow.Show();
        }

        private void Btn_Send_Req_Click(object sender, RoutedEventArgs e)
        {
            switch(cmd_table.SelectedIndex)
            {
                case 0:
                    {
                        CMDTABLE.RemoveAll();
                        CMDTABLE["msgType"] = 0;
                        CMDTABLE["cmdType"] = 1;
                        CMDTABLE["SN"] = "102000217340000224";
                    }
                    break;
                case 1:
                    {
                        CMDTABLE.RemoveAll();
                        CMDTABLE["msgType"] = 0;
                        CMDTABLE["cmdType"] = 2;
                        CMDTABLE["SN"] = "102000217340000224";
                    }
                    break;
                case 2:
                    {
                        CMDTABLE.RemoveAll();
                        CMDTABLE["msgType"] = 0;
                        CMDTABLE["cmdType"] = 3;
                        CMDTABLE["SN"] = "102000217340000224";
                    }
                    break;
                case 3:
                    {
                        CMDTABLE.RemoveAll();
                        CMDTABLE["msgType"] = 0;
                        CMDTABLE["cmdType"] = 4;
                        CMDTABLE["SN"] = "102000217340000224";
                    }
                    break;
                case 4:
                    {
                        CMDTABLE.RemoveAll();
                        CMDTABLE["msgType"] = 0;
                        CMDTABLE["cmdType"] = 5;
                        CMDTABLE["SN"] = "102000217340000224";
                    }
                    break;
                case 5:
                    {
                        CMDTABLE.RemoveAll();
                        CMDTABLE["msgType"] = 0;
                        CMDTABLE["cmdType"] = 6;
                        CMDTABLE["SN"] = "102000217340000224";
                    }
                    break;
            }
            req_content = CMDTABLE.ToString();
            Text_Send.Text = req_content;
            String uri_tmp= ApiUrl.Text;
            HostName = ApiUrl.Text;
            if (req_para.Rows.Count>0)
            {
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                for (int i = 0; i < req_para.Rows.Count; i++)
                {
                    if (i == 0) uri_tmp +="?";
                    uri_tmp+=req_para.Rows[i]["参数"].ToString();
                   
                    uri_tmp += "=";
                    uri_tmp += req_para.Rows[i]["参数值"].ToString();
                    if (i < req_para.Rows.Count-1) uri_tmp += "&";
                    cfa.AppSettings.Settings[req_para.Rows[i]["参数"].ToString()].Value = req_para.Rows[i]["参数值"].ToString();
                   
                }
                cfa.Save();
            }
            APIKEY = "api-key: "+ ApiKey.Text;
            uri = new Uri(uri_tmp);
            Text_Receive.Text = "";
            req_thread = new Thread(Send_req);
            req_thread.Start();
        }

        private void Send_req()
        {
            string uuid="";
            MyDelegate d = new MyDelegate(UpdateText);
            HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(uri);
            WebHeaderCollection headers = wbRequest.Headers;
            headers.Add(APIKEY);
            wbRequest.Headers = headers;
            wbRequest.ContentType = "application/json";
            wbRequest.AllowAutoRedirect = false;
            wbRequest.Timeout = 5000;
            wbRequest.Method = "POST";
            try
            {
                Stream myRequestStream = wbRequest.GetRequestStream();
                StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
                myStreamWriter.Write(req_content);
                myStreamWriter.Close();

                HttpWebResponse response = (HttpWebResponse)wbRequest.GetResponse();

                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                JObject jo = (JObject)JsonConvert.DeserializeObject(retString);

                if (jo["errno"].ToString().Contains('0') && (jo["error"].ToString().Contains("succ")))
                {
                    JToken js = (JToken)jo["data"];
                    uuid = js["cmd_uuid"].ToString();
                    
                }
                this.Dispatcher.Invoke(d, jo.ToString());
                jo = null;
                uri = null;
                headers = null;
                if (uuid!="")
                {
                    DateTime dt = DateTime.Now;
                    TimeSpan span = DateTime.Now - dt;
                    while (span.Minutes < 2)
                    {
                        span = DateTime.Now - dt;

                        uri = new Uri(HostName + "/" + uuid);
                        wbRequest = (HttpWebRequest)WebRequest.Create(uri);
                        headers = wbRequest.Headers;
                        headers.Add(APIKEY);
                        wbRequest.Headers = headers;
                        wbRequest.ContentType = "application/json";
                        wbRequest.AllowAutoRedirect = false;
                        wbRequest.Timeout = 5000;
                        wbRequest.Method = "GET";
                        response = (HttpWebResponse)wbRequest.GetResponse();

                        myResponseStream = response.GetResponseStream();
                        myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                        retString = myStreamReader.ReadToEnd();
                        myStreamReader.Close();
                        myResponseStream.Close();
                        jo = (JObject)JsonConvert.DeserializeObject(retString); 
                        if (jo["errno"].ToString().Contains('0') && (jo["error"].ToString().Contains("succ")))
                        {
                            JToken js = (JToken)jo["data"];
                            String status = js["status"].ToString();
                            if (status=="4")
                            {
                                this.Dispatcher.Invoke(d, jo.ToString());
                                uri = new Uri(HostName + "/" + uuid + "/resp");
                                wbRequest = (HttpWebRequest)WebRequest.Create(uri);
                                headers = wbRequest.Headers;
                                headers.Add(APIKEY);
                                wbRequest.Headers = headers;
                                wbRequest.ContentType = "application/json";
                                wbRequest.AllowAutoRedirect = false;
                                wbRequest.Timeout = 5000;
                                wbRequest.Method = "GET";
                                response = (HttpWebResponse)wbRequest.GetResponse();

                                myResponseStream = response.GetResponseStream();
                                myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                                retString = myStreamReader.ReadToEnd();
                                myStreamReader.Close();
                                myResponseStream.Close();
                                jo = (JObject)JsonConvert.DeserializeObject(retString);
                                this.Dispatcher.Invoke(d, jo.ToString());
                                if (req_thread != null)
                                    req_thread.Abort();
                            }
                            
                        }
                        Thread.Sleep(1000);
                    }
                    
                }
            }
            catch
            {

            }
            finally
            {
                if(req_thread!=null)
                req_thread.Abort();
            }
        }

        private void UpdateText(String text)
        {
            Text_Receive.Text += text;
            Text_Receive.Text +="\r\n"+ DateTime.Now.ToString()+"\r\n";
        }

        void Frm_ChangeTextEvent(string Para_Name, string Para_Vaule)
        {
            bool reapet = false;
            for (int i = 0; i < req_para.Rows.Count; i++)
            {
                 if(req_para.Rows[i]["参数"].ToString()== Para_Name)
                {
                    reapet = true;
                }
            }
            if(reapet)
            {
                MessageBox.Show("禁止添加相同类型参数名，请删除原参数重新添加！");
            }
            else
            {
                DataRow drt = req_para.NewRow();
                drt["参数"] = Para_Name;
                drt["参数值"] = Para_Vaule;
                req_para.Rows.Add(drt);
                Dg_Req_Para.ItemsSource = req_para.DefaultView;
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfa.AppSettings.Settings[Para_Name].Value = Para_Vaule;
                cfa.Save();
            }
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (req_thread != null)
                req_thread.Abort();
        }

        private void Dg_Req_Para_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
            string newValue = (e.EditingElement as TextBox).Text;
            if(LastVaule!=newValue)
            {

            }
        }

        private void Btn_Update_Click(object sender, RoutedEventArgs e)
        {
           
            if(Update_thread==null|| Update_thread.IsAlive==false)
            {

                Text_Receive.Text = String.Empty;
                Update_thread = new Thread(UpdateHttp);
                Update_thread.Start();
            }
            
           
        }

        private void UpdateHttp()
        {
            String Device_id = "11393745";
            Uri UpdateUri = new Uri("http://api.heclouds.com/devices/" + Device_id + "/datapoints?datastream_id=REPORT_STATE");
            MyDelegate d = new MyDelegate(UpdateText);
            




            HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(UpdateUri);
            WebHeaderCollection headers = wbRequest.Headers;
            headers.Add("api-key:DLy5z6Bks=RibaGWFMJC7IDhC0A=");
            wbRequest.Headers = headers;
            wbRequest.ContentType = "application/json";
            wbRequest.AllowAutoRedirect = false;
            wbRequest.Timeout = 5000;
            wbRequest.Method = "GET";

            try
            {



                HttpWebResponse response = (HttpWebResponse)wbRequest.GetResponse();

                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                JObject jo = (JObject)JsonConvert.DeserializeObject(retString);
                if (jo["errno"].ToString().Contains('0') && (jo["error"].ToString().Contains("succ")))
                {
                    JToken js = (JToken)jo["data"]["datastreams"][0]["datapoints"][0]["value"];
                    // JToken jv = (JToken)js["datapoints"];
                    String tmp_str = jo["data"]["datastreams"][0]["datapoints"][0]["at"].ToString()+"\r\n";
                    tmp_str += js.ToString();
                   
                    this.Dispatcher.Invoke(d, tmp_str);
                }

                //this.Dispatcher.Invoke(d, jo.ToString());



            }
            catch
            {
                this.Dispatcher.Invoke(d, "请求超时");
            }
            Update_thread.Abort();
        }

        private void Dg_Req_Para_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
           // LastVaule = (e.EditingEventArgs as TextBox).Text;
        }
    }
}
