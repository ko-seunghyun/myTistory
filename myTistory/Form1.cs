using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.OneNote;
using System.Xml.Linq;
using System.Net;
using System.IO;

namespace myTistory
{
    public partial class Form1 : Form
    {
        private string srcPath = "";
        private string dstPath = "";

        private string AuthURL = "https://www.tistory.com/oauth/authorize";
        private string BlogInfoURL = "https://www.tistory.com/apis/blog/info?access_token=";

        private string DELIM_ACC_TOK = "#access_token";
        private string DELIM_STAT = "&state=";
        private string ACCESS_TOKEN = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "원노트 파일 (*.one)|*.one";//추후지원 word
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                srcPath = txb_path.Text = ofd.FileName;
                dstPath = srcPath.Replace(srcPath.Substring(srcPath.LastIndexOf(".")), ".mht");
            }
        }

        public void oneToMht()
        {
            Microsoft.Office.Interop.OneNote.Application onenoteApp = new Microsoft.Office.Interop.OneNote.Application();

            string sectionId;
            onenoteApp.OpenHierarchy(srcPath, null, out sectionId);

            try
            {
                onenoteApp.Publish(sectionId, dstPath, Microsoft.Office.Interop.OneNote.PublishFormat.pfMHTML, "");
            }
            catch
            {
                return;
            }
        }

        private void btn_auth_Click(object sender, EventArgs e)
        {
            StringBuilder dataParams = new StringBuilder();
            dataParams.Append("client_id=74e30b40c4ffe56b9e6b1b016575c2bd&");
            dataParams.Append("redirect_uri=http://fallingstar.tistory.com/&");
            dataParams.Append("response_type=token");
            //dataParams.Append("response_type=code");

            
            axWebBrowser1.Navigate(AuthURL + "?" + dataParams);


            /* POST */
            // HttpWebRequest 객체 생성, 설정
            /*HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUri);
            request.Method = "POST";    // 기본값 "GET"
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteDataParams.Length;
            
             
            // 요청 String -> 요청 Byte 변환
            byte[] byteDataParams = UTF8Encoding.UTF8.GetBytes(dataParams.ToString());

            // 요청 Byte -> 요청 Stream 변환
            Stream stDataParams = request.GetRequestStream();
            stDataParams.Write(byteDataParams, 0, byteDataParams.Length);
            stDataParams.Close();
             */

            /* GET */
            // GET 방식은 Uri 뒤에 보낼 데이터를 입력하시면 됩니다.
            /*HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AuthURL + "?" + dataParams);
            request.Method = "GET";


            // 요청, 응답 받기
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // 응답 Stream 읽기
                Stream stReadData = response.GetResponseStream();
                StreamReader srReadData = new StreamReader(stReadData, Encoding.Default);

                // 응답 Stream -> 응답 String 변환
                string strResult = srReadData.ReadToEnd();

                Console.WriteLine(strResult);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                string t = ex.StackTrace;
            }*/


        }

        private void axWebBrowser1_DocumentComplete(object sender, AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEvent e)
        {
            string url;

            url = e.uRL.ToString();

            int idx = url.IndexOf("#access_token");

            //엑세스 토큰 받아옴.
            if (idx > 0)
            {
                string temp = url.Substring(idx + DELIM_ACC_TOK.Length + 1);

                ACCESS_TOKEN = temp.Substring(0, temp.Length - DELIM_STAT.Length);

                StringBuilder dataParams = new StringBuilder();
                dataParams.Append(ACCESS_TOKEN);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BlogInfoURL + dataParams);
                request.Method = "GET";


                // 요청, 응답 받기
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    // 응답 Stream 읽기
                    Stream stReadData = response.GetResponseStream();
                    StreamReader srReadData = new StreamReader(stReadData, Encoding.UTF8);

                    // 응답 Stream -> 응답 String 변환
                    string strResult = srReadData.ReadToEnd();

                    Console.WriteLine(strResult);
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    string t = ex.StackTrace;
                }
            }
        }
    }
}
