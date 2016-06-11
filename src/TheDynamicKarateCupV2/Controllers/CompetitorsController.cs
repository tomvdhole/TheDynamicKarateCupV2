using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using TheDynamicKarateCupV2.Models;
using TheDynamicKarateCupV2.Services;
using System.Collections.Generic;
using TheDynamicKarateCupV2.ViewModels.Competitors;

namespace TheDynamicKarateCupV2.Controllers
{
    public class CompetitorsController : Controller
    {
        private ApplicationDbContext _context;

        public CompetitorsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Competitors
        public IActionResult Index(int clubID)
        {
            SecurityServices secServices = new SecurityServices(_context);
            bool isValid = secServices.IsClubIDValidToClubNumber(clubID, User.Identity.Name);

            if (isValid == true)
            {
                CompetitorServices services = new CompetitorServices(_context);
                CompetitorsClubIDViewModel competitorsClubIDViewModel = new CompetitorsClubIDViewModel();
                competitorsClubIDViewModel.Competitors = services.GetSubscribedCompetitors(clubID);
                competitorsClubIDViewModel.ClubID = clubID;
                return View(competitorsClubIDViewModel);
            }
            else
            {
                return RedirectToAction("YouCanOnlyLookUpYourOwnData", "Verify");
            }
        }

        // GET: Competitors/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Competitor competitor = _context.Competitor.Single(m => m.CompetitorID == id);
            if (competitor == null)
            {
                return HttpNotFound();
            }

            return View(competitor);
        }

        // GET: Competitors/Create
        public IActionResult Create(int clubID)
        {
            SecurityServices secServices = new SecurityServices(_context);
            bool isValid = secServices.IsClubIDValidToClubNumber(clubID, User.Identity.Name);

            if (isValid == true)
            {
                CompetitorsViewModel competitorsVM = new CompetitorsViewModel();
                competitorsVM.Competitor = new Competitor();
                competitorsVM.Competitor.ClubID = clubID;
                return View(competitorsVM);
            }
            else
            {
                return RedirectToAction("YouCanOnlyLookUpYourOwnData", "Verify");
            }
        }

        // POST: Competitors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CompetitorsViewModel competitorsVM)
        {
            if (ModelState.IsValid)
            {
                SecurityServices secServices = new SecurityServices(_context);
                bool isValid = secServices.IsClubIDValidToClubNumber(competitorsVM.Competitor.ClubID, User.Identity.Name);

                if (isValid == true)
                {
                    CompetitorServices competitorServices = new CompetitorServices(_context);
                    competitorServices.SaveCompetitor(competitorsVM.Competitor);
                    CategoryServices categoryServices = new CategoryServices(_context);
                    categoryServices.DefineCategories(competitorsVM.Competitor);
                    return RedirectToAction("Index", competitorsVM.Competitor.ClubID);
                }
                else
                {
                    return RedirectToAction("YouCanOnlyLookUpYourOwnData", "Verify");
                }    
            }
            return View(competitorsVM);
        }

        // GET: Competitors/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            CompetitorServices services = new CompetitorServices(_context);
            Competitor competitor = services.GetCompetitor((int) id);
            if (competitor == null)
            {
                return HttpNotFound();
            }
            //ViewData["ClubID"] = new SelectList(_context.Club, "ClubID", "Club", competitor.ClubID);
            return View(competitor);
        }

        // POST: Competitors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Competitor competitor)
        {
            if (ModelState.IsValid)
            {
                _context.Update(competitor);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewData["ClubID"] = new SelectList(_context.Club, "ClubID", "Club", competitor.ClubID);
            return View(competitor);
        }

        // GET: Competitors/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            CompetitorServices competitorServices = new CompetitorServices(_context);
            Competitor competitor = competitorServices.GetCompetitor((int) id);
            if (competitor == null)
            {
                return HttpNotFound();
            }

            return View(competitor);
        }

        // POST: Competitors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            CompetitorServices competitorServices = new CompetitorServices(_context);
            Competitor competitor = competitorServices.GetCompetitor(id);
            competitorServices.DeleteCompetitor(competitor);

            return RedirectToAction("Index");
        }
    }
}

// ViewData["ClubID"] = new SelectList(_context.Club, "ClubID", "Club", competitorsVM.Competitor.ClubID);
