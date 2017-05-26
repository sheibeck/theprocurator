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
using static theprocurator.Helpers.AjaxHelpers;

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

            // GET: CharacterSheets/Create
        public ActionResult Create()
        {
            var characterSheet = new CharacterSheet();
            return View(characterSheet);
        }

        // POST: CharacterSheets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [ValidateJSONAntiForgeryHeader]
        public ActionResult Edit([Bind(Include = "CharacterSheetId,CharacterSheetName,CharacterSheetUrl,CharacterSheetForm,UserId")] CharacterSheet characterSheet)
        {            
            if (ModelState.IsValid)
            {
                db.Entry(characterSheet).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, redirect = false, responseText = "Character sheet saved." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, redirect = false, responseText = "Error saving character sheet." }, JsonRequestBehavior.AllowGet);
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
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CharacterSheetId,CharacterSheetName,CharacterSheetUrl,CharacterSheetForm")] CharacterSheet characterSheet)
        {
            // revalidate after fetching the logged in user
            characterSheet.UserId = IdentityExtensions.GetUserId(User.Identity);
            ModelState.Clear();
            TryValidateModel(characterSheet);

            if (ModelState.IsValid)
            {
                db.Entry(characterSheet).State = EntityState.Added;
                db.SaveChanges();                
                return Json(new { success = true, redirect = true, responseText = Url.Action("Edit", new { id = characterSheet.CharacterSheetId }) }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, redirect = false, responseText = "Error saving character sheet." }, JsonRequestBehavior.AllowGet);
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
