using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AzureMachineLearningClient
{
    public class AzureMLClient
    {
        public AzureMLClient()
        {
            var score = new
            {
                Inputs = new Dictionary<string, DataTable>()
                    {
                        {
                        "Input1", // The input name
                        new DataTable()
                        {
                            ColumnsNames=new string[] {null}, // Your Columns name eg. ColumnsNames=new string[] {"Age", "education", "sex"},
                            Values= new string[,] { {null, null }} // The value eg. Values= new string[,] { {"0","28", "0","value", "0","1" }} 
                        }
                    },
                    },
                GlobalParameters = new Dictionary<string, string>() { }
            };
            invokeRequestResponseService(score).Wait();
        }
        public class DataTable
        {
            public string[] ColumnsNames { get; set; }
            public string[,] Values { get; set; }
        }

        static async Task invokeRequestResponseService(object score)
        {
            using (var client = new HttpClient())
            {
               
                const string apiKey = "Your key";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("your scheme name", apiKey);
                client.BaseAddress = new Uri("YOUR URI");
                HttpResponseMessage resp = await client.PostAsJsonAsync("", score);
                if (resp.IsSuccessStatusCode)
                {
                    string result = await resp.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine("Error {0}", resp.StatusCode);
                }

            }
        }
    }
}
