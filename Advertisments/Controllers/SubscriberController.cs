using Advertisments.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Unicode;
using Advertisments.Other;
using Newtonsoft.Json;

namespace Advertisments.Controllers
{
    public class SubscriberController : Controller
    {
        public IActionResult ShowSubscribers()
        {
            IEnumerable<Subscriber> subscribersList;
            using (var client = new HttpClient())
            {
                client.BaseAddress = Connection.MyAddress();
                // GET
                var responseTask = client.GetAsync("api/Subscriber"); // Send GET to API -->
                responseTask.Wait();
                
                var result = responseTask.Result;   // Get back Response from API <--
                if(result.IsSuccessStatusCode)
                {
                    // Use ; Microsoft.Aspnet.WebApi.Client -nuget
                    var readTask = result.Content.ReadAsAsync<List<Subscriber>>();
                    readTask.Wait();
                    subscribersList = readTask.Result;
                }
                else // web api sent error responseTask
                {                    
                    subscribersList = Enumerable.Empty<Subscriber>();
                    ModelState.AddModelError(string.Empty, "Server error!!");
                }
            }
            return View(subscribersList);
        }
        [HttpGet]
        public IActionResult CreateSubscriber()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateSubscriber(Subscriber subscriber)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Connection.MyAddress();
                // POST
                var postTask = client.PostAsJsonAsync<Subscriber>("api/Subscriber", subscriber);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["info"] = "Subscriber created :D";
                    return RedirectToAction("ShowSubscribers");
                }
            }
            ModelState.AddModelError(string.Empty, "Server error :P");  // Visas som Error Summary!!! :D
            ViewBag.error = ModelState.ErrorCount;                      // # errors 
            return View(subscriber);
        }
        [HttpGet]
        public ActionResult EditSubscriber(int id)
        {
            Subscriber subscriber = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = Connection.MyAddress();
                // GET
                Task<HttpResponseMessage> responseTask = client.GetAsync("api/Subscriber/" + id.ToString());
                responseTask.Wait();

                HttpResponseMessage result = responseTask.Result;
                
                if (result.IsSuccessStatusCode)
                {
                    Task<Subscriber> readTask = result.Content.ReadAsAsync<Subscriber>();
                    readTask.Wait();
                    subscriber = readTask.Result;
                }
            }
            return View(subscriber);
        }
        [HttpPost]
        public ActionResult EditSubscriber(Subscriber subscriber)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = Connection.MyAddress();
                // PUT
                Task<HttpResponseMessage> putTask = client.PutAsJsonAsync<Subscriber>("api/subscriber", subscriber);
                putTask.Wait();

                HttpResponseMessage result = putTask.Result;
                if(result.IsSuccessStatusCode)
                {
                    TempData["info"] = $"Subscriber: {subscriber.SubscriberId} updated :D";
                    return RedirectToAction("SubscriberDetails");
                }
                
            }
            ViewBag.error = "Server error?! While updating...";
            ModelState.AddModelError(string.Empty, "något"); // Jag ville ha ModelState error, den e cool XD :D
            return View(subscriber);
        }
        [HttpGet]
        public ActionResult DeleteSubscriber(int id)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = Connection.MyAddress();
                // DELETE
                Task<HttpResponseMessage> deleteTask = client.DeleteAsync("api/subscriber/" + id.ToString());
                deleteTask.Wait();

                HttpResponseMessage result = deleteTask.Result;
                if(result.IsSuccessStatusCode)
                {
                    TempData["info"] = $"Subscriber ID: {id} was deleted";
                    return RedirectToAction("ShowSubscribers");
                }
            }
            TempData["error"] = "Error, server? Subscriber could not be deleted!";
            return RedirectToAction("ShowSubscribers");
        }
        [HttpGet]
        public IActionResult SubscriberDetails()
        {
            // Get Current Subscriber
            string jsString = HttpContext.Session.GetString("currSubscriber");
            Subscriber subscriber = JsonConvert.DeserializeObject<Subscriber>(jsString);
            if (subscriber == null)
            {
                // Subscriber Not Found!
                TempData["error"] = "Subscriber ID not found";
                return RedirectToAction("NewsPaperAnnouncementPage", "Advertise");
            }
            // Found
            ViewBag.info = $"Welcome {subscriber.FirstName} {subscriber.LastName}\nID: {subscriber.SubscriberId}";
            return View(subscriber);
        }       
        //---------------------------------------------------------------

    }
}
