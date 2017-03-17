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

        private myOpenAPI API = null;

        private bool isOpenFile = false;
        private string title = "";

        private Dictionary<string, string> imgDic = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();

            API = new myOpenAPI();
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
                //타이틀(제목) 확인을 위한 xml 생성.
                string xml;
                XmlDocument document = new XmlDocument();
                
                onenoteApp.GetHierarchy(sectionId, HierarchyScope.hsPages, out xml);

                document.LoadXml(xml);
               
                XmlNodeList xnList = document.GetElementsByTagName("one:Page"); //접근할 노드

                string pageId = "";
                foreach (XmlNode xn in xnList)
                {
                    title = xn.Attributes["name"].Value; // get page title
                    pageId = xn.Attributes["ID"].Value; //get page id
                }

                document.RemoveAll();
                
                onenoteApp.GetPageContent(pageId, out xml, PageInfo.piAll);
                document.LoadXml(xml);

                /*xnList = document.GetElementsByTagName("one:OEChildren/one:T"); //접근할 노드

                contents = xnList[0].InnerText;

                foreach (XmlNode xn in xnList)
                {
                    title = xn.Attributes["name"].Value; // get page title
                    pageId = xn.Attributes["ID"].Value; //get page id

                }*/

                //document.Save(@"c:\1111.xml");

                onenoteApp.Publish(sectionId, dstPath, Microsoft.Office.Interop.OneNote.PublishFormat.pfMHTML, "");
            }
            catch
            {
                return;
            }
        }

        private void btn_auth_Click(object sender, EventArgs e)
        {
            axWebBrowser1.Navigate(API.getAuthURL());
        }

        private void axWebBrowser1_DocumentComplete(object sender, AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEvent e)
        {
            string url;

            url = e.uRL.ToString();

            //엑세스 토큰 받아옴. 처음 로그인 시 사용.
            if (url.Contains(myOpenAPI.DELIM_ACC_TOK))
            {
                API.getAccessToken(url);

                loadBlogNames();
            }
            else if (isOpenFile)
            {
                
                IHTMLDocument2 doc = (IHTMLDocument2)axWebBrowser1.Document;
                IHTMLElementCollection elAll = doc.all;
                
                IHTMLControlRange imgRange = (IHTMLControlRange)((HTMLBody)doc.body).createControlRange();

                foreach (IHTMLImgElement img in doc.images)
                {
                    imgRange.add((IHTMLControlElement)img);

                    //string src = img.src;

                    imgRange.execCommand("Copy", false, null);

                    using (Bitmap bmp = (Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap))
                    {
                        string path = @"C:\temp\" + img.nameProp;

                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);

                        try
                        {
                            bmp.Save(path);
                        }
                        catch(Exception err)
                        {
                            err.ToString();
                        }
                        


                        //imgDic.Add(path,"");
                    }
                }

                foreach (IHTMLElement elem in elAll)
                {
                    //img 태그만 얻어온다.
                    if (elem.tagName == "IMG")
                    {
                        string outerHtml = elem.outerHTML;
                        int startIndex = outerHtml.IndexOf("\"") + 1;
                        int lastIndex = outerHtml.LastIndexOf("\"") - 1;
                        int length = lastIndex - startIndex + 1;
                        string src = outerHtml.Substring(startIndex, length);
                        string key = @"C:\temp\" + src.Split('/')[1];
                        imgDic[key] = outerHtml;
                    }
                }

                contents = doc.body.parentElement.outerHTML;
                //Console.WriteLine(contents);

                API.uploadFile(cb_blog.Text, imgDic);

                isOpenFile = false;
            }
        }

        private void loadBlogNames()
        {
            string [] names = API.getBlogInfo();
            
            foreach(string name in names)
                cb_blog.Items.Add(name);

            cb_blog.SelectedIndex = 0;
        }

        /// <summary>
        /// mht 파일에서 컨텐츠를 만들어 낸다.
        /// </summary>
        private void makeContents(string mhtFile)
        {
            axWebBrowser1.Navigate(mhtFile);
        }

        private void makContetns(string sectionId, string pageId)
        {

        }

        private void parseImage(string mhtFile)
        {

        }

        /// <summary>
        /// 처음 글쓰기로 업로드 하면.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_upload_Click(object sender, EventArgs e)
        {
            API.writePost(cb_blog.Text, title, contents);
        }

        //Mid
        /// <summary>
        /// 문자열 원본의 지정한 위치에서 부터 추출할 갯수 만큼 문자열을 가져옵니다.
        /// </summary>
        /// <param name="sString">문자열 원본</param>
        /// <param name="nStart">추출을 시작할 위치</param>
        /// <param name="nLength">추출할 갯수</param>
        /// <returns>추출된 문자열</returns>
        public string Mid(string sString, int nStart, int nLength)
        {
            string sReturn;

            //VB에서 문자열의 시작은 0이 아니므로 같은 처리를 하려면 
            //스타트 위치를 인덱스로 바꿔야 하므로 -1을 하여
            //1부터 시작하면 0부터 시작하도록 변경하여 준다.
            --nStart;

            //시작위치가 데이터의 범위를 안넘겼는지?
            if (nStart <= sString.Length)
            {
                //안넘겼다.

                //필요한 부분이 데이터를 넘겼는지?
                if ((nStart + nLength) <= sString.Length)
                {
                    //안넘겼다.
                    sReturn = sString.Substring(nStart, nLength);
                }
                else
                {
                    //넘겼다.

                    //데이터 끝까지 출력
                    sReturn = sString.Substring(nStart);
                }

            }
            else
            {
                //넘겼다.

                //그렇다는 것은 데이터가 없음을 의미한다.
                sReturn = string.Empty;
            }

            return sReturn;
        }

        //Left
        /// <summary>
        /// 문자열 원본에서 왼쪽에서 부터 추출한 갯수만큼 문자열을 가져옵니다.
        /// </summary>
        /// <param name="sString">문자열 원본</param>
        /// <param name="nLength">추출할 갯수</param>
        /// <returns>추출된 문자열</returns>
        public string Left(string sString, int nLength)
        {
            string sReturn;

            //추출할 갯수가 문자열 길이보다 긴지?
            if (nLength > sString.Length)
            {
                //길다!

                //길다면 원본의 길이만큼 리턴해 준다.
                nLength = sString.Length;
            }

            //문자열 추출
            sReturn = sString.Substring(0, nLength);

            return sReturn;
        }

        //Right
        /// <summary>
        /// 문자열 원본에서 오른쪽에서 부터 추출한 갯수만큼 문자열을 가져옵니다.
        /// </summary>
        /// <param name="sString">문자열 원본</param>
        /// <param name="nLength">추출할 갯수</param>
        /// <returns>추출된 문자열</returns>
        public string Right(string sString, int nLength)
        {
            string sReturn;

            //추출할 갯수가 문자열 길이보다 긴지?
            if (nLength > sString.Length)
            {
                //길다!

                //길다면 원본의 길이만큼 리턴해 준다.
                nLength = sString.Length;
            }

            //문자열 추출
            sReturn = sString.Substring(sString.Length - nLength, nLength);

            return sReturn;
        }
    }
}
