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
using mshtml;
using System.Xml;

namespace myTistory
{
    public partial class Form1 : Form
    {
        private string srcPath = "";
        private string dstPath = "";
        private string contents = "";

        private string AuthURL = "https://www.tistory.com/oauth/authorize";
        private string BlogInfoURL = "https://www.tistory.com/apis/blog/info?access_token=";
        private string RedirectURL = "http://fallingstar.tistory.com/";
        private string WriteURL = "https://www.tistory.com/apis/post/write";

        private string DELIM_ACC_TOK = "#access_token";
        private string DELIM_STAT = "&state=";
        private string ACCESS_TOKEN = "";

        private string BlogName = "";
        private bool isOpenFile = false;

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

            //oneNote to mht 파일
            oneToMht();

            isOpenFile = true;

            //mht에서 contents 만들기.
            makeContents(dstPath);

        }

        private void oneToMht()
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
            dataParams.Append("redirect_uri="+ RedirectURL +"& ");
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

            //엑세스 토큰 받아옴. 처음 로그인 시 사용.
            if (idx > 0)
            {
                string temp = url.Substring(idx + DELIM_ACC_TOK.Length + 1);

                ACCESS_TOKEN = temp.Substring(0, temp.Length - DELIM_STAT.Length);

                getBlogInfo();
            }
            else if (isOpenFile)
            {
                
                IHTMLDocument2 doc = (IHTMLDocument2)axWebBrowser1.Document;

                contents = doc.body.parentElement.outerHTML;

                IHTMLControlRange imgRange = (IHTMLControlRange)((HTMLBody)doc.body).createControlRange();

                foreach (IHTMLImgElement img in doc.images)
                {
                    imgRange.add((IHTMLControlElement)img);

                    string src = img.src;

                    imgRange.execCommand("Copy", false, null);

                    using (Bitmap bmp = (Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap))
                    {
                        bmp.Save(@"C:\" + img.nameProp);
                    }
                }

                //Console.WriteLine(contents);

                isOpenFile = false;
            }
        }

        /// <summary>
        /// mht 파일에서 컨텐츠를 만들어 낸다.
        /// </summary>
        private void makeContents(string mhtFile)
        {
            axWebBrowser1.Navigate(mhtFile);
        }

        /// <summary>
        /// 블로그 정보를 가져온다.
        /// </summary>
        private void getBlogInfo()
        {
            StringBuilder dataParams = new StringBuilder();
            dataParams.Append(ACCESS_TOKEN);

            //블로그 정보 받기.
            XmlDocument xml = httpResponse(BlogInfoURL, dataParams); // XmlDocument 생성
            xml.Save(@"C:\테스트.xml");

            XmlNodeList xnList = xml.GetElementsByTagName("blog"); //접근할 노드
            //XmlNodeList xnList = xml.SelectNodes("/tistory/item"); //접근할 노드

            foreach (XmlNode xn in xnList)
            {
                string blogURL = xn["url"].InnerText; //블로그이름
                string blogName = xn["name"].InnerText; //블로그이름

                //블로그 이름 추가
                cb_blog.Items.Add(blogName);
                cb_blog.SelectedIndex = 0;
                //string lng = xn["point"]["y"].InnerText;
            }
        }

        /// <summary>
        /// http 객체를 통해 응답을 받아온다. 응답은 xml 형식이다.
        /// </summary>
        /// <param name="url">api 주소</param>
        /// <param name="param">파라미터 값</param>
        /// <returns>xml 응답</returns>
        private XmlDocument httpResponse(string url, StringBuilder param)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + param);
            request.Method = "GET";

            XmlDocument document = new XmlDocument();


            // 요청, 응답 받기
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // 응답 Stream 읽기
                Stream stReadData = response.GetResponseStream();
                StreamReader srReadData = new StreamReader(stReadData, Encoding.UTF8);

                // 응답 Stream -> 응답 String 변환
                //strResult = srReadData.ReadToEnd();
                document.Load(srReadData);

            }
            catch (Exception ex)
            {
                document = null;
            }

            return document;
        }

        private void parseImage(string mhtFile)
        {

        }

        private void btn_upload_Click(object sender, EventArgs e)
        {

        }
    }
}
