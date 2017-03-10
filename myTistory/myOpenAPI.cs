using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.IO;

namespace myTistory
{
    public class myOpenAPI
    {
        public const string AuthURL = "https://www.tistory.com/oauth/authorize";
        public const string BlogInfoURL = "https://www.tistory.com/apis/blog/info?access_token=";
        public const string RedirectURL = "http://fallingstar.tistory.com/";
        public const string WriteURL = "https://www.tistory.com/apis/post/write";
        public const string DELIM_ACC_TOK = "#access_token";
        public const string DELIM_STAT = "&state=";
        public string ACCESS_TOKEN = "";

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

            return AuthURL + "?" + dataParams;
        }

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
                string blogURL = xn["url"].InnerText; //블로그이름
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

    }
}
