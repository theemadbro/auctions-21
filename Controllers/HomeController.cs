using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using beltfix.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace beltfix.Controllers
{
    public static class SessionExtensions
    {
        // We can call ".SetObjectAsJson" just like our other session set methods, by passing a key and a value
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            // This helper function simply serializes theobject to JSON and stores it as a string in session
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        
        // generic type T is a stand-in indicating that we need to specify the type on retrieval
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            // Upon retrieval the object is deserialized based on the type we specified
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
    
    public class HomeController : Controller
    {
        private beltfixContext _context;
        public HomeController(beltfixContext context)
        {
            _context = context;
        }

///////////////////////////////////////////////////////////////////////////////////
// 
        public IActionResult CheckFinishedAuctions()
        {
            List<Auctions> allAuctions = _context.auctions.Include(a => a.seller).Include(a => a.bidders).ThenInclude(b => b.bidder).ToList();
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            foreach(var auct in allAuctions)
            {
                var today = DateTime.Now;
                TimeSpan remaining = auct.endDate.Subtract(today);
                if (remaining.TotalSeconds <= 0)
                {
                    if (auct.bidders.Count == 0)
                    {
                        _context.auctions.Remove(auct);
                        _context.SaveChanges();
                    }
                    else
                    {
                        Bidders highestbidder = _context.bidders.Where(b => b.auctionid == auct.id).Include(b => b.bidder).OrderBy(b => b.bidamount).First();
                        auct.seller.wallet += auct.bid;
                        if (ret[0].id == auct.seller.id)
                        {
                            ret[0].wallet += auct.bid;
                            HttpContext.Session.SetObjectAsJson("curr", ret);
                        }

                        highestbidder.bidder.wallet -= auct.bid;
                        if (ret[0].id == highestbidder.id)
                        {
                            ret[0].wallet -= auct.bid;
                            HttpContext.Session.SetObjectAsJson("curr", ret);
                        }

                        foreach(var bids in auct.bidders)
                        {
                            _context.bidders.Remove(bids);
                        }
                        _context.auctions.Remove(auct);
                        _context.SaveChanges();
                    }
                }
            }
            return RedirectToAction("");
        }
///////////////////////////////////////////////////////////////////////////////////

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            if(ret == null)
            {
                CurrentUser newcurr = new CurrentUser();
                newcurr.id = 0;
                ViewBag.CurrentUser = newcurr;
                List<object> temp = new List<object>();
                temp.Add(newcurr);
                HttpContext.Session.SetObjectAsJson("curr", temp);
                return View();
            }
            else if (ret.Count == 0)
            {
                ViewBag.CurrentUser = ret[0];
                return View();
            }
            else
            {
                ViewBag.CurrentUser = ret[0];
                return View();
            }
        }

        

        [HttpGet]
        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            CheckFinishedAuctions();
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            if (ret == null || ret[0].id == 0)
            {
                return RedirectToAction("");
            }
            else
            {
                CurrentUser test = ret[0];
                Users update = _context.users.SingleOrDefault(x => x.id == ret[0].id);
                test.wallet = update.wallet;
                ViewBag.CurrentUser = ret[0];
                List<Auctions> allauctions = _context.auctions.Include(a => a.seller).OrderBy(a => a.endDate).ToList();
                ViewBag.auctionlist = allauctions;
                return View("Dashboard");
            }
        }
        
        [HttpGet]
        [Route("new")]
        public IActionResult New()
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
           if (ret == null || ret[0].id == 0)
            {
                return RedirectToAction("");
            }
            else
            {
                ViewBag.CurrentUser = ret[0];
                return View("CreateAuction");
            }
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create(Auctions inp)
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            if (ret == null || ret[0].id == 0)
            {
                return RedirectToAction("");
            }
            else
            {
                ViewBag.CurrentUser = ret[0];
                if(ModelState.IsValid)
                {
                    inp.sellerid = ret[0].id;
                    _context.Add(inp);
                    _context.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    return View("CreateAuction");
                }
            }
        }

        [HttpPost]
        [Route("delete/{auctid}")]
        public IActionResult Delete(int auctid)
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            if (ret == null || ret[0].id == 0)
            {
                return RedirectToAction("");
            }
            else
            {
                Auctions todelete = _context.auctions.Include(w => w.bidders).SingleOrDefault(w => w.id == auctid);
                if(ret[0].id != todelete.sellerid || todelete == null)
                {
                    return RedirectToAction("Dashboard");
                }
                foreach(var x in todelete.bidders)
                {
                    _context.bidders.Remove(x);
                }
                _context.auctions.Remove(todelete);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        [Route("auction")]
        public IActionResult Auction()
        {
            return RedirectToAction("Dashboard");
        }
        [HttpGet]
        [Route("auction/{auctid}")]
        public IActionResult ViewAuction(int auctid)
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            if (ret == null || ret[0].id == 0)
            {
                return RedirectToAction("");
            }
            else
            {
                string LocalVariable = HttpContext.Session.GetString("biderror");
                if(LocalVariable != null)
                {
                    ViewBag.error = LocalVariable;
                }
                ViewBag.CurrentUser = ret[0];
                Auctions getauct = _context.auctions.Include(a => a.seller).Include(a => a.bidders).SingleOrDefault(w => w.id == auctid);
                List<Bidders> bidlist = _context.bidders.Where(b => b.auctionid == auctid).Include(b => b.bidder).OrderByDescending(b => b.bidamount).ToList();
                ViewBag.auct = getauct;
                ViewBag.bidlist = bidlist;
                return View();
            }
        }

        [HttpPost]
        [Route("auction/{auctid}/placebid")]
        public IActionResult PlaceBid(int auctid, int bidamt)
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            if (ret == null || ret[0].id == 0)
            {
                return RedirectToAction("");
            }
            else
            {
                Auctions getauct = _context.auctions.Include(a => a.seller).Include(a => a.bidders).SingleOrDefault(a => a.id == auctid);
                List<Bidders> bidlist = _context.bidders.Where(b => b.auctionid == auctid).Include(b => b.bidder).OrderBy(b => b.bidamount).ToList();
                if(getauct.bid >= bidamt)
                {
                    ViewBag.CurrentUser = ret[0];
                    ViewBag.auct = getauct;
                    ViewBag.bidlist = bidlist;
                    ViewBag.error = "Your Bid must be higher than the current highest!";
                    return View("ViewAuction");
                }
                if(bidamt > ret[0].wallet)
                {
                    ViewBag.CurrentUser = ret[0];
                    ViewBag.auct = getauct;
                    ViewBag.bidlist = bidlist;
                    ViewBag.error = "Your can't Bid more than what you have!";
                    return View("ViewAuction");
                }
                Bidders newbid = new Bidders();
                newbid.bidderid = ret[0].id;
                newbid.auctionid = auctid;
                newbid.bidamount = bidamt;
                getauct.bid = bidamt;
                _context.Add(newbid);
                _context.SaveChanges();
                return RedirectToAction("ViewAuction");
            }
        }

        

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
