using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using theprocurator.Data;
using theprocurator.Data.Model;
using Microsoft.AspNet.Identity;

namespace theprocurator.Controllers
{
    public class CharacterSheetsController : Controller
    {
        private TheProcuratorDbContext db = new TheProcuratorDbContext();

        // GET: CharacterSheets
        public ActionResult Index()
        {
            var characterSheet = db.CharacterSheet;
            return View(characterSheet.ToList());
        }

        // GET: CharacterSheets/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CharacterSheet characterSheet = db.CharacterSheet.Find(id);
            if (characterSheet == null)
            {
                return HttpNotFound();
            }
            return View(characterSheet);
        }

        // GET: CharacterSheets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CharacterSheets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CharacterSheetId,CharacterSheetName,CharacterSheetUrl,CharacterSheetForm")] CharacterSheet characterSheet)
        {
            if (ModelState.IsValid)
            {
                characterSheet.UserId = IdentityExtensions.GetUserId(User.Identity);
                characterSheet.CharacterSheetId = Guid.NewGuid();
                db.CharacterSheet.Add(characterSheet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(characterSheet);
        }

        // GET: CharacterSheets/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CharacterSheet characterSheet = db.CharacterSheet.Find(id);
            if (characterSheet == null)
            {
                return HttpNotFound();
            }
            
            return View(characterSheet);
        }

        // POST: CharacterSheets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CharacterSheetId,CharacterSheetName,CharacterSheetUrl,CharacterSheetForm")] CharacterSheet characterSheet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(characterSheet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(characterSheet);
        }

        // GET: CharacterSheets/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CharacterSheet characterSheet = db.CharacterSheet.Find(id);
            if (characterSheet == null)
            {
                return HttpNotFound();
            }
            return View(characterSheet);
        }

        // POST: CharacterSheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            CharacterSheet characterSheet = db.CharacterSheet.Find(id);
            db.CharacterSheet.Remove(characterSheet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
