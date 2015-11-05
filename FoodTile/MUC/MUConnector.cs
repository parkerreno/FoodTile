using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace MUC
{
    public class MUConnector : IDisposable
    {
        private string user;
        private string password;

        private CookieContainer cookieJar;
        private HttpClient client; // need to use same client to keep the cookies (for now)
        
        public MUConnector(PasswordCredential p)
        {
            user = p.UserName;
            p.RetrievePassword();
            password = p.Password;
            InitClient();
        }

        /// <summary>
        /// Creates the HttpClient with handler
        /// </summary>
        private void InitClient()
        {
            cookieJar = new CookieContainer();
            var handler = new HttpClientHandler()
            {
                CookieContainer = cookieJar,
                UseCookies = true,
            };

            client = new HttpClient(handler);
        }

        /// <summary>
        /// Login to MyUW
        /// </summary>
        public async void Login()
        {
            const string LOGIN_URL = "https://weblogin.washington.edu/";
            const string GET_COOKIES_URL = "https://my.uw.edu/PubCookie.reply";
            
            var postData = new Dictionary<string, string>();
            postData.Add("user", user); // Not all of these may be necessary, but this config is tested and works
            postData.Add("pass", password);
            postData.Add("relay_url", GET_COOKIES_URL);
            postData.Add("create_ts", UnixTimeNow().ToString());
            postData.Add("submit", "Log in");
            postData.Add("one", "my.uw.edu");
            postData.Add("two", "MyUW");
            postData.Add("three", "1");
            postData.Add("four", "a5");
            postData.Add("five", "GET");
            postData.Add("six", "my.uw.edu");
            postData.Add("seven", "Lw==");
            postData.Add("reply", "1");
            postData.Add("first_kiss", UnixTimeNow() + "-052697");
            postData.Add("creds_from_greq", "1");

            var postContent = new FormUrlEncodedContent(postData);
            var response = await client.PostAsync(LOGIN_URL, postContent);

            string pubCookie = GetValue("pubcookie_g", await response.Content.ReadAsStringAsync());

            postData.Clear(); // Prepare for next post
            postData.Add("pubcookie_g", pubCookie);
            postData.Add("redirect_url", "https://my.uw.edu/Lw==");
            postData.Add("post_stuff", "");
            postData.Add("get_args", "");

            postContent = new FormUrlEncodedContent(postData);
            await client.PostAsync(GET_COOKIES_URL, postContent); // after this, we are logged in.
        }

        public async Task<UserHfsData> GetHfsData()
        {
            const string HFS_DATA_URL = "https://my.uw.edu/api/v1/hfs/";

            var resp = await client.GetAsync(HFS_DATA_URL);
            var raw = await resp.Content.ReadAsStreamAsync();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(UserHfsData));
            return (UserHfsData)ser.ReadObject(raw);
        }

        /// <summary>
        /// Some hacked together method to get a form value from HTML
        /// </summary>
        /// <param name="name">Name of form element</param>
        /// <param name="content">HTML to search</param>
        /// <returns>Value from specified name</returns>
        public static string GetValue(string name, string content)
        {
            if (!name.Contains(name))
            {
                return null;
            }
            int start = content.IndexOf($"name=\"{name}\"");
            string toReturn = content.Substring(start);
            toReturn = toReturn.Replace($"name=\"{name}\" value=\"", "");
            int end = toReturn.IndexOf("\"");
            return toReturn.Substring(0, end);
        }

        /// <summary>
        /// Gets the current unix time
        /// </summary>
        /// <returns>64bit UNIX Time (seconds from 1Jan1970 UTC)</returns>
        public static long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        public void Dispose()
        {
            ((IDisposable)client).Dispose();
        }
    }
}
