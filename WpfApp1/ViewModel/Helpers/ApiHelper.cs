using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModel.Helpers
{
    internal class ApiHelper
    {
        public static string Put(string Url, string json, int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage message = client.PutAsync(Url+"/"+id, content).Result;
                return message.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string Get(string Url)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage message = client.GetAsync(Url).Result;
                return message.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
