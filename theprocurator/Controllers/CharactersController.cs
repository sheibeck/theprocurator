using Microsoft.AspNet.Identity;
using NotyNotification.Extension;
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
using theprocurator.Helpers;
using static theprocurator.Helpers.AjaxHelper;

namespace theprocurator.Controllers
{
    public class CharactersController : Controller
    {
        private TheProcuratorDbContext db = new TheProcuratorDbContext();        

        // GET: Characters
        public ActionResult Index()
        {
            string currentUserId = IdentityExtensions.GetUserId(this.User.Identity);
            var character = db.Character
                                .Include(c => c.CharacterSheet)
                                .Include(c => c.User)
                              .Where( c => c.UserId == currentUserId);
            return View(character.ToList());
        }

        // POST: CharacterSheets/Copy       
        public ActionResult Copy(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Character character = db.Character.AsNoTracking().FirstOrDefault(cs => cs.CharacterId == id);
            //db.CharacterSheet.Find(id);

            if (character == null)
            {
                return HttpNotFound();
            }

            // copy this sheet into the persons list of sheets
            db.Entry(character).State = EntityState.Detached;
            character.UserId = IdentityExtensions.GetUserId(User.Identity);
            character.CharacterId = Guid.NewGuid();
            character.CharacterName = character.CharacterName + " copy";
            character.CharacterUrl = character.CharacterUrl + "-copy";
            db.Character.Add(character);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = character.CharacterId })
                    .WithNotification("Character was added to your collection.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success);
        }

        public FileStreamResult Pdf(Guid id)
        {
            Character character = db.Character.Find(id);            
            return this.PrintToPdf(id.ToString(), character.CharacterName);
        }

        public ActionResult Print(Guid id)
        {     
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Character character = db.Character.Include(c => c.CharacterSheet)
                                    .Where(c => c.CharacterId == id).FirstOrDefault();
            if (character == null)
            {
                return HttpNotFound();
            }

            return View(character);
        }
   
        // GET: Characters/Create
        public ActionResult Create(Guid id)
        {
            var character = new Character();
            
            // grab the character sheet we want to make this character from
            character.UserId = IdentityExtensions.GetUserId(User.Identity);
            character.CharacterSheet = db.CharacterSheet.Where(c => c.CharacterSheetId == id)
                                            .FirstOrDefault();

            if (character.CharacterSheet == null)
            {
                return Json(Helpers.AjaxHelper.Notify("Could not find the selected character sheet.", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.error, true), JsonRequestBehavior.AllowGet);
            }

            else
            {
                character.CharacterSheetId = character.CharacterSheet.CharacterSheetId;
                return View(character).WithNotification(string.Format("Creating a new character with sheet: {0}", character.CharacterSheet.CharacterSheetName), NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.information);
            }
        }

        // POST: Characters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAjax]
        public ActionResult Create(Character character)
        {           
            if (ModelState.IsValid)
            {                
                db.Entry(character).State = EntityState.Added;             
                db.SaveChanges();

                return Json(Helpers.AjaxHelper.Notify("Character created.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success, false, Url.Action("Edit", "Characters", new { id = character.CharacterId})), JsonRequestBehavior.AllowGet);
            }

            return Json(Helpers.AjaxHelper.Notify("Error creating character.", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.error, true), JsonRequestBehavior.AllowGet);            
        }

        // GET: Characters/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Character character = db.Character.Include( c => c.CharacterSheet)
                                    .Where( c => c.CharacterId == id).FirstOrDefault();
            if (character == null)
            {
                return HttpNotFound();
            }
            
            return View(character);
        }

        // POST: Characters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAjax]
        public ActionResult Edit(Character character)
        {
            if (ModelState.IsValid)
            {
                db.Entry(character).State = EntityState.Modified;
                db.SaveChanges();
                return Json(Helpers.AjaxHelper.Notify("Character saved.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success), JsonRequestBehavior.AllowGet);
            }
            return Json(Helpers.AjaxHelper.Notify("Error saving character.", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.error, true), JsonRequestBehavior.AllowGet);
        }

        // GET: Characters/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Character character = db.Character.Find(id);
            if (character == null)
            {
                return HttpNotFound();
            }
            return View(character).WithNotification("Deleting this character cannot be undone!", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.warning); ;
        }

        // POST: Characters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Character character = db.Character.Find(id);
            db.Character.Remove(character);
            db.SaveChanges();

            return RedirectToAction("Index").WithNotification("Character deleted.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success);
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
