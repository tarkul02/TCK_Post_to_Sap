using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;

namespace SAP_Batch_GR_TR.Class
{
    class LineNotify
    {

        internal void FNLineNotify(string ValidateMessage)
        {
            LineNotifyClass(ValidateMessage);
        }
        public async void LineNotifyClass(string ValidateMessage)
        {
            string accessToken = "Hgzg7Idj9s0c7L3Rlloh5lr2wFtuJZYzTJxBo2aFQL4"; // Replace with your Line Notify access token
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string message = ValidateMessage;
                    //string message = "Error_test";
                    string url = $"https://notify-api.line.me/api/notify";

                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var content = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("message", message),
                    });

                    var response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Line Notify message sent successfully!");
                        //Application.Exit();
                    }
                    else
                    {
                        Console.WriteLine("Failed to send Line Notify message. Status code: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

        }

    }
}