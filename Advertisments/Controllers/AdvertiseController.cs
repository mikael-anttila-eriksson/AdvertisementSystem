using Advertisments.ModelMethods;
using Advertisments.Models;
using Advertisments.ViewModels;
using Advertisments.Enums;
using Advertisments.Other;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FrankFurter;

namespace Advertisments.Controllers;

public class AdvertiseController : Controller
{
    public IActionResult NewsPaperAnnouncementPage()
    {        
        return View();
    }
    [HttpPost]
    public IActionResult NewsPaperAnnouncementPage(bool isCompany, string subNumber)
    {
        if(isCompany)
        {
            
            return RedirectToAction("CompanyInfo");
        }
        else // is Subscriber
        {
            // GET Subscriber
            Subscriber subscriber = null;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = Connection.MyAddress();
                Task<HttpResponseMessage> responseTask = client.GetAsync("api/subscriber/" + subNumber.ToString());
                responseTask.Wait();

                HttpResponseMessage result = responseTask.Result;
                string info = result.Content.ReadAsStringAsync().Result;
                if (result.IsSuccessStatusCode)
                {
                    Task<Subscriber> readTask = result.Content.ReadAsAsync<Subscriber>();
                    readTask.Wait();
                    subscriber = readTask.Result;

                    // Save to next page
                    string jsString = JsonConvert.SerializeObject(subscriber);
                    HttpContext.Session.SetString("currSubscriber", jsString);
                }
                else
                {
                    // Subscriber not Found!
                    ViewBag.error = "Subscriber not found, check ID is correct";
                    return View();
                }
            }
            return RedirectToAction("SubscriberDetails", "Subscriber");
        }
    }
    public IActionResult SubscriberInfo()
    {
        // Get Current Subscriber
        string jsString = HttpContext.Session.GetString("currSubscriber");
        Subscriber subscriber = JsonConvert.DeserializeObject<Subscriber>(jsString);
        if(subscriber == null)
        {
            // Subscriber Not Found!
            TempData["error"] = "Subscriber ID not found";
            return RedirectToAction("NewsPaperAnnouncementPage");
        }
        // Found
        ViewBag.info = "Prepate Post";
        return View(subscriber);
    }
    [HttpPost]
    public IActionResult SubscriberInfo(Subscriber subscriber)
    {
        ViewBag.info = "Received POST";
        return View();
    }
    //---------------------------------------------------------------
    #region CRUD - Company/Advertiser
    
    public IActionResult CompanyInfo()
    {
        AdvertiserViewModel vmAdvertiser = new();            
        return View(vmAdvertiser);
    }
    [HttpPost]
    public IActionResult CompanyInfo(AdvertiserViewModel newCompany, bool useBilling, string address, int postNr, string city)
    {
        int rowsAffected = 0;
        newCompany.DeliveryAddress.Type = AddressType.Delivery;
        if(useBilling)
        {
            newCompany.BillingAddress = new AdvertiserAdress();
            newCompany.BillingAddress.Type = AddressType.Billing;
            newCompany.BillingAddress.Address = address;
            newCompany.BillingAddress.PostNumber = postNr;
            newCompany.BillingAddress.City = city;
        }
        
        
        rowsAffected = AdvertiserMethods.RegisterCompany(newCompany, useBilling, out string errorMsg);
        
        if((rowsAffected == 3 && useBilling) || (rowsAffected == 2 && !useBilling))
        {
            // Save Company for CreateAd-Action - All in one line              
            //HttpContext.Session.SetString("newCompanyId",
            //    JsonConvert.SerializeObject(
            //        AdvertiserMethods.GetAdvertisers(out errorMsg).LastOrDefault().Id
            //        )
            //    );
            // Save Company for CreateAd-Action - Separeted lines
            int id = AdvertiserMethods.GetLastCreatedCompany(out errorMsg);
            string jsString = JsonConvert.SerializeObject(id);
            HttpContext.Session.SetString("newCompanyId", jsString);

            TempData["info"] = "Company created";
            return RedirectToAction("CreateAd", new {isCompany = true});
        }            
        else
        {
            //ViewBag.error = "A mock error";
            ViewBag.error = errorMsg;
            return View(newCompany);
        }
    }
    public IActionResult ShowCompanies()    // Started working on - But is not necessary
    {
        IEnumerable<Advertiser> companyList;
        companyList = AdvertiserMethods.GetAdvertisers(out string errorMsg);
        IEnumerable<AdvertiserViewModel> vmList;
        List<AdvertiserViewModel> list = new();
        AdvertiserViewModel vm = null;
        foreach(Advertiser company in companyList)
        {
            vm = AdvertiserMethods.MapModelToVm(company);
            //vm.BillingAddress = new();
            list.Add(vm);
        }
        if (errorMsg == "")
        {
            vmList = list;
            return View(vmList);
        }
        else
        {
            ViewBag.error = errorMsg;
            return View(new AdvertiserViewModel());
        }
    }
    #endregion CRUD - Company/Advertiser
    //---------------------------------------------------------------
    #region CRUD - Ads
    public IActionResult CreateAd(bool isCompany)
    {
        Advertiser company = new();
        Ads ad = new();
        if(isCompany)
        {
            // Get current Company
            string jsString = HttpContext.Session.GetString("newCompanyId");
            int id = JsonConvert.DeserializeObject<int>(jsString);
            company = AdvertiserMethods.GetAdvertiserById(id, out string errorMsg); // hade id+1 innan --> Error. Nu id bara.
            // Ad found?
            if (company == null)
            {
                // No
                TempData["error"] = errorMsg;
                return RedirectToAction("NewsPaperAnnouncementPage");
            }
            // Found
            // Set Ad
            ad = InitializeAd(isCompany, company.Id);
        }
        else // Is Subscriber
        {
            
            // Get Current Subscriber
            string jsString = HttpContext.Session.GetString("currSubscriber");
            Subscriber subscriber = JsonConvert.DeserializeObject<Subscriber>(jsString);
            if (subscriber == null)
            {
                // Subscriber Not Found!
                TempData["error"] = "Subscriber not found";
                return RedirectToAction("NewsPaperAnnouncementPage");
            }
            // Found
            // Set Ad
            ad = InitializeAd(isCompany, subscriber.SubscriberId);            
        }

        // Yes
        return View(ad);
    }
    [HttpPost]
    public IActionResult CreateAd(Ads ad)
    {
        int rowsAffected = 0;
        rowsAffected = AdsMethods.InsertAd(ad, out string errorMsg);
        if (rowsAffected == 1)
        {
            TempData["info"] = "Ad created";
            return RedirectToAction("ShowAds");
        }
        else
        {
            //ViewBag.error = "A mock error";
            ViewBag.error = errorMsg;
            return View(ad);
        }
    }
    [HttpGet]
    public IActionResult EditAd(int id)
    {
        Ads ad = new();
        ad = AdsMethods.GetAdById(id, out string errorMsg);
        ViewBag.error = errorMsg;
        return View(ad);
    }
    [HttpPost]
    public IActionResult EditAd(Ads ad)
    {
        int rowsAffected = 0;
        rowsAffected = AdsMethods.UpdateAd(ad, out string errorMsg);
        if (rowsAffected == 1)
        {
            TempData["info"] = "Ad updated";
            return RedirectToAction("ShowAds");
        }
        else
        {
            //ViewBag.error = "A mock error";
            ViewBag.error = errorMsg;
            return View(ad);
        }
    }
    [HttpGet]
    public IActionResult DeleteAd(int id)
    {
        if (AdsMethods.DeleteAd(id, out string errorMsg) == 1)
        {
            TempData["info"] = "Ad deleted";
            return RedirectToAction("ShowAds");
        }
        else
        {
            //ViewBag.error = "A mock error";
            TempData["error"] = errorMsg;
            return RedirectToAction("ShowAds");
        }
    }

    public IActionResult ShowAds()
    {        
        IEnumerable<Ads> ads;
        ads = GetAllAds(out string errorMsg);
        if(errorMsg == "")
        {
            return View(ads);
        }
        else
        {
            ViewBag.error = errorMsg;
            return View(new Ads()) ;
        }       
    }
    public IEnumerable<Ads> GetAllAds(out string errorMsg)
    {
        IEnumerable<Ads> ads;
        ads = AdsMethods.GetAds(out errorMsg);
        return ads;
        
    }
    [HttpPost]
    public IActionResult GetExRate(string currency) //need to ad exRate to Ads -> AdsViewModel
    {
        // ALT 1: Way to use - Get info in a simple way
        using(Client client = new Client()) 
        {
            var unitTask = client.GetCurrenciesAsync();
            unitTask.Wait();
            var unit = unitTask.Result;
            ViewBag.unit = unit[currency];        
        }
        var taskRate =  Client.GetLatestRateAsync("SEK", currency); // Can´t do (SEK, SEK) :P
        taskRate.Wait();
        decimal? rate = taskRate.Result;
        ViewBag.rate = rate;

        //Get all ads again
        IEnumerable<Ads> ads = GetAllAds(out string errorMsg);
        if(errorMsg != "") ViewBag.error = errorMsg;
        return View("ShowAds", ads);                
    }
    private async Task GetRatesAsync()
    {
        // ALT 1: Way to use - Get info in a simple way
        
        var rateAsync = await Client.GetLatestRateAsync("SEK", "USD");
        using (var clientCurrency = new Client())
        {
            
            Dictionary<string, string> currencies = await clientCurrency.GetCurrenciesAsync();
            var usd = currencies["USD"];
            var euro = currencies["EUR"];

        }
        // ALT 2: Also possible but not needed tu use ...
        using (HttpClient client = new())
        {
            //client.BaseAddress = Connection.MyAddress();
            Task<HttpResponseMessage> getTast = client.GetAsync("https://api.frankfurter.app/latest");
            getTast.Wait();
            HttpResponseMessage result = getTast.Result;
            var resd = result.Content.ReadAsStringAsync();
            // ... then extract info in some way
        }
    }
    #endregion CRUD - Ads
    //---------------------------------------------------------------
    
    //---------------------------------------------------------------
    private Ads InitializeAd(bool isCompany, int id)
    {
        Ads ad = new();
        if(isCompany)
        {
            ad.SubscriberId = null;
            ad.AdvertiserId = id;
            ad.PriceAd = 40;
        }
        else // is Subscriber
        {
            ad.SubscriberId = id;
            ad.AdvertiserId = null;
            ad.PriceAd = 0;
        }
        return ad;
    }
}
