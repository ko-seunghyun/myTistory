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

        private string BlogName = "";
        private bool isOpenFile = false;

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
            API.writePost(cb_blog.Text, contents);
        }
    }
}
