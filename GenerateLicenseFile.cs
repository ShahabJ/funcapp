using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace funcApp
{
    public static class GenerateLicenseFile
    {
        [FunctionName("GenerateLicenseFile")]
        public static async Task Run([QueueTrigger("orders", Connection = "AzureWebJobsStorage")]Order order,
            IBinder binder,
            ILogger log)
        {
            var outputBlob = await binder.BindAsync<TextWriter>(
                new BlobAttribute($"licenses/{order.OrderId}.lic") 
                { 
                    Connection = "AzureWebJobsStorage"
                });
            outputBlob.WriteLine($"Order Id : {order.OrderId}");
            outputBlob.WriteLine($"Email: {order.Email}");
            outputBlob.WriteLine($"Product Id : {order.ProductId}");
            outputBlob.WriteLine($"Purchase Date : {DateTime.UtcNow}");

            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(order.Email + "secret"));
            outputBlob.WriteLine($"secret code : {BitConverter.ToString(hash).Replace("-", "")}");
        }
    }
}
