﻿using System;
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
using static theprocurator.Helpers.AjaxHelper;
using NotyNotification.Extension;
using theprocurator.Helpers;
using System.Drawing;
using System.IO;
using HiQPdf;

namespace theprocurator.Controllers
{
    public class CharacterSheetsController : Controller
    {
        private TheProcuratorDbContext db = new TheProcuratorDbContext();

        // GET: CharacterSheets
        public ActionResult Index()
        {
            string currentUserId = IdentityExtensions.GetUserId(this.User.Identity);

            var characterSheet = db.CharacterSheet
                                    .Include(c => c.Characters)
                                    .Include(c => c.User)
                                    .Where(c => c.UserId == currentUserId);
            return View(characterSheet.ToList());
        }

        // GET: CharacterSheets/Create
        public ActionResult Create()
        {
            var characterSheet = new CharacterSheet();
            // revalidate after fetching the logged in user
            characterSheet.UserId = IdentityExtensions.GetUserId(User.Identity);
                        
            return View(characterSheet);
        }

        // POST: Character/Copy       
        public ActionResult Copy(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CharacterSheet characterSheet = db.CharacterSheet.AsNoTracking().FirstOrDefault(cs => cs.CharacterSheetId == id);
            //db.CharacterSheet.Find(id);

            if (characterSheet == null)
            {
                return HttpNotFound();
            }

            // copy this sheet into the persons list of sheets
            db.Entry(characterSheet).State = EntityState.Detached;
            characterSheet.UserId = IdentityExtensions.GetUserId(User.Identity);
            characterSheet.CharacterSheetId = Guid.NewGuid();
            characterSheet.CharacterSheetName = characterSheet.CharacterSheetName + " copy";
            characterSheet.CharacterSheetUrl = characterSheet.CharacterSheetUrl + "-copy";
            characterSheet.UpdatedOn = DateTime.Now;
            db.CharacterSheet.Add(characterSheet);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = characterSheet.CharacterSheetId })
                    .WithNotification("Character sheet was added to your collection.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success);
        }

        // POST: CharacterSheets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAjax]
        public ActionResult Edit(CharacterSheet characterSheet)
        {            
            if (ModelState.IsValid)
            {
                characterSheet.UpdatedOn = DateTime.Now;
                db.Entry(characterSheet).State = EntityState.Modified;
                SaveDBChanges(characterSheet.CharacterSheetId);

                return Json(Helpers.AjaxHelper.Notify("Character sheet saved.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success), JsonRequestBehavior.AllowGet);
            }

            return Json(Helpers.AjaxHelper.Notify("Error saving character sheet.", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.error, true), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the database changes.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        private int SaveDBChanges(Guid id)
        {            
            this.ToThumbnail(id.ToString());
            return db.SaveChanges();
        }

        public ActionResult Print(Guid id)
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
        [ValidateAjax]
        public ActionResult Create(CharacterSheet characterSheet)
        {
            if (ModelState.IsValid)
            {
                characterSheet.UpdatedOn = DateTime.Now;
                db.Entry(characterSheet).State = EntityState.Added;
                SaveDBChanges(characterSheet.CharacterSheetId);

                return Json(Helpers.AjaxHelper.Notify("Character sheet created.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success, false, Url.Action("Edit", "CharacterSheets", new { id = characterSheet.CharacterSheetId })), JsonRequestBehavior.AllowGet);
            }            
            return Json(Helpers.AjaxHelper.Notify("Error creating character sheet.", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.error, true), JsonRequestBehavior.AllowGet);
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
            return View(characterSheet).WithNotification("Deleting this character sheet cannot be undone!", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.warning);
        }

        // POST: CharacterSheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            CharacterSheet characterSheet = db.CharacterSheet.Find(id);
            db.CharacterSheet.Remove(characterSheet);
            db.SaveChanges();
            
            return RedirectToAction("Index").WithNotification("Character sheet deleted.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success);
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
