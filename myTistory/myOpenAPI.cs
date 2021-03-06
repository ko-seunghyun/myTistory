﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.IO;
using System.Web;

namespace myTistory
{
    public class myOpenAPI
    {
        public const string AuthURL = "https://www.tistory.com/oauth/authorize?";
        public const string BlogInfoURL = "https://www.tistory.com/apis/blog/info?access_token=";
        public const string RedirectURL = "http://fallingstar.tistory.com/";
        public const string WriteURL = "https://www.tistory.com/apis/post/write";
        public const string ReadURL = "https://www.tistory.com/apis/post/read?";
        public const string AttachURL = "https://www.tistory.com/apis/post/attach";

        public const string DELIM_ACC_TOK = "#access_token";
        public const string DELIM_STAT = "&state=";
        public string ACCESS_TOKEN = "";

        public Dictionary<string, string> imgDic = new Dictionary<string, string>();

        public myOpenAPI() { }

        /// <summary>
        /// 토큰을 받아오기 위해 인증URL을 만듬.
        /// </summary>
        /// <returns>인증 URL</returns>
        public string getAuthURL()
        {
            StringBuilder dataParams = new StringBuilder();
            dataParams.Append("client_id=74e30b40c4ffe56b9e6b1b016575c2bd&");
            dataParams.Append("redirect_uri=" + RedirectURL + "& ");
            dataParams.Append("response_type=token");

            return AuthURL + dataParams;
        }

        /// <summary>
        /// 토큰 정보를 가져온다.
        /// </summary>
        /// <param name="url">파싱할 url</param>
        public void getAccessToken(string url)
        {

            int idx = url.IndexOf("#access_token");

            string temp = url.Substring(idx + DELIM_ACC_TOK.Length + 1);

            ACCESS_TOKEN = temp.Substring(0, temp.Length - DELIM_STAT.Length);
        }

        /// <summary>
        /// 블로그 정보를 가져온다.
        /// </summary>
        public string [] getBlogInfo()
        {
            ArrayList list = new ArrayList();

            StringBuilder dataParams = new StringBuilder();
            dataParams.Append(ACCESS_TOKEN);

            //블로그 정보 받기.
            XmlDocument xml = httpResponse(BlogInfoURL, dataParams); // XmlDocument 생성
            //xml.Save(@"C:\테스트.xml");

            XmlNodeList xnList = xml.GetElementsByTagName("blog"); //접근할 노드
            //XmlNodeList xnList = xml.SelectNodes("/tistory/item"); //접근할 노드

            foreach (XmlNode xn in xnList)
            {
                string blogURL = xn["url"].InnerText; //블로그주소
                string blogName = xn["name"].InnerText; //블로그이름

                //블로그 이름 추가
                list.Add(blogName);
            }

            if (list.Count == 0)
                return null;
            else
                return (string[])list.ToArray(typeof(string));
        }

        /// <summary>
        /// 글쓰기 api
        /// </summary>
        /// <param name="blogName">블로그 이름</param>
        /// <param name="title">게시글 제목</param>
        /// <param name="contents">게시글 내용</param>
        public void writePost(string blogName, string title, string contents)
        {
            //이미지를 포함한 content는 replacer로 교체
            foreach (KeyValuePair<string, string> pair in imgDic)
            {
                contents = contents.Replace(pair.Key, pair.Value);
            }

            StringBuilder dataParams = new StringBuilder();
            dataParams.Append("access_token="+ ACCESS_TOKEN);
            dataParams.Append("&blogName=" + blogName);
            dataParams.Append("&title="+title);
            dataParams.Append("&content="+ contents);

            //&nbsp는 원래 탭인데, 원노트에서는 줄바꿈으로 쓰이고 있음.
            //이 태그가 들어가면 글 잘림 현상 나타남.
            dataParams = dataParams.Replace("&nbsp;", "<br>");

            //글쓰기 응답 받음.
            XmlDocument xml = httpResponseByPost(WriteURL, dataParams);

            XmlNodeList xnList = xml.GetElementsByTagName("tistory"); //접근할 노드

            foreach (XmlNode xn in xnList)
            {
                string status = xn["status"].InnerText; //글쓰기 성공여부, 성공 200
                string postid = xn["postId"].InnerText; //글번호
                string fullUrl = xn["url"].InnerText; //글 주소
            }

        }


        /// <summary>
        /// 파일 업로드 부분.
        /// </summary>
        /// <param name="blogName">블로그 이름</param>
        /// <param name="data">키/값 페어 사전</param>
        public void uploadFile(string blogName, Dictionary<string, string> data)
        {
            //여기에서 pair key = C:\img.jpg value = <IMG src="..." width =""...>
            foreach (KeyValuePair<string, string> pair in data)
            {
                
                //img file to byte array
                FileStream fs = new FileStream(pair.Key, FileMode.Open, FileAccess.Read);
                byte[] imgData = new byte[fs.Length];
                fs.Read(imgData, 0, imgData.Length);
                fs.Close();

                // Generate post objects
                Dictionary<string, object> postParameters = new Dictionary<string, object>();
                postParameters.Add("access_token", ACCESS_TOKEN);
                postParameters.Add("blogName", blogName);
                //postParameters.Add("uploadedfile", pair.Key);

                postParameters.Add("uploadedfile", new FormUpload.FileParameter(imgData, pair.Key, "image/jpeg"));

                // Create request and receive response
                string userAgent = "Someone";

                // Process response
                XmlDocument document = new XmlDocument();
                try
                {
                    HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(AttachURL, userAgent, postParameters);

                    // 응답 Stream 읽기
                    Stream stReadData = webResponse.GetResponseStream();
                    StreamReader srReadData = new StreamReader(stReadData, Encoding.UTF8);

                    // 응답 Stream -> 응답 String 변환
                    //strResult = srReadData.ReadToEnd();
                    document.Load(srReadData);

                }
                catch (Exception ex)
                {
                    document = null;
                }

                XmlNodeList xnList = document.GetElementsByTagName("tistory"); //접근할 노드

                string replacer = "";
                foreach (XmlNode xn in xnList)
                {
                    replacer = xn["replacer"].InnerText;
                }

                imgDic.Add(pair.Value, replacer);

            }

        }

        public void readPost(string blogName, string postId)
        {
            
            StringBuilder dataParams = new StringBuilder();
            dataParams.Append("access_token=" + ACCESS_TOKEN);
            dataParams.Append("&blogName=" + blogName);
            dataParams.Append("&postId=" + 27);

            //블로그 정보 받기.
            XmlDocument xml = httpResponse(ReadURL, dataParams); // XmlDocument 생성
            //xml.Save(@"C:\테스트.xml");

            XmlNodeList xnList = xml.GetElementsByTagName("tistory"); //접근할 노드
            //XmlNodeList xnList = xml.SelectNodes("/tistory/item"); //접근할 노드

            /*foreach (XmlNode xn in xnList)
            {
                string blogURL = xn["url"].InnerText; //블로그주소
                string blogName = xn["name"].InnerText; //블로그이름

                //블로그 이름 추가
                list.Add(blogName);
            }*/
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


        private XmlDocument httpResponseByPost(string url, StringBuilder param, byte[] fileData = null)
        {
            XmlDocument document = new XmlDocument();

            // 요청 String -> 요청 Byte 변환
            byte[] byteDataParams = UTF8Encoding.UTF8.GetBytes(param.ToString());
            
            if(fileData != null)
            {
                byte[] temp = new byte[byteDataParams.Length + fileData.Length];
                Array.Copy(byteDataParams, temp, byteDataParams.Length);
                Array.Copy(fileData, 0, temp, byteDataParams.Length, fileData.Length);
                byteDataParams = temp;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";    // 기본값 "GET"
            request.KeepAlive = true;
            if (fileData != null)
                request.ContentType = "multipart/form-data; boundary=" + "---------------------------" + DateTime.Now.Ticks.ToString("x"); 
            else
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";// "text/html; charset=UTF-8"; 
            request.ContentLength = byteDataParams.Length;

            // 요청 Byte -> 요청 Stream 변환
            Stream stDataParams = request.GetRequestStream();
            stDataParams.Write(byteDataParams, 0, byteDataParams.Length);
            stDataParams.Close();

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

    }
}
