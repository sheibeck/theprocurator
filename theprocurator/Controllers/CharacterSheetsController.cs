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
using static theprocurator.Helpers.AjaxHelper;
using NotyNotification.Extension;
using theprocurator.Helpers;
using System.Drawing;
using System.IO;
using HiQPdf;
using System.Threading.Tasks;

namespace theprocurator.Controllers
{
    [Authorize]
    public class CharacterSheetsController : Controller
    {
        private TheProcuratorDbContext db = new TheProcuratorDbContext();

        // GET: CharacterSheets
        [AllowAnonymous]
        public ActionResult Index()
        {
            string currentUserId = IdentityExtensions.GetUserId(this.User.Identity);

            var characterSheet = db.CharacterSheet
                                    .Include(c => c.Characters)
                                    .Include(c => c.User)
                                    .OrderBy(c => c.CharacterSheetName)
                                    .Where(c => c.UserId == currentUserId);
            return View(characterSheet.ToList());
        }

        [AllowAnonymous]
        public ActionResult Search(string searchtext)
        {
            var characterSheet = db.CharacterSheet
                                    .Include(c => c.Characters)
                                    .Include(c => c.User)
                                    .OrderBy(c => c.CharacterSheetName)
                                    .Where(c => c.Published == true);

            if (!string.IsNullOrEmpty(searchtext))
            {
                characterSheet = characterSheet.Where(c => c.CharacterSheetName.Contains(searchtext)
                                                || c.CharacterSheetTheme.Contains(searchtext)
                                                || c.User.UserName.Contains(searchtext));
            }

            ViewBag.SearchText = searchtext;

            return View(characterSheet.ToList());
        }

        // GET: CharacterSheets/Create
        [Authorize]
        public ActionResult Create()
        {
            var characterSheet = new CharacterSheet();
            // revalidate after fetching the logged in user
            characterSheet.UserId = IdentityExtensions.GetUserId(User.Identity);
                        
            return View(characterSheet);
        }

        // POST: Character/Copy
        [Authorize]
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
            characterSheet.ParentId = id;
            characterSheet.CharacterSheetId = Guid.NewGuid();
            characterSheet.CharacterSheetName = characterSheet.CharacterSheetName + " copy";
            characterSheet.CharacterSheetUrl = characterSheet.CharacterSheetUrl + "-copy";
            characterSheet.Published = false;
            characterSheet.UpdatedOn = DateTime.Now;
            db.CharacterSheet.Add(characterSheet);

            SaveDBChanges(characterSheet.CharacterSheetId, "false");

            return RedirectToAction("Edit", new { id = characterSheet.CharacterSheetId })
                    .WithNotification("Character sheet was added to your collection.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success);
        }

        // POST: CharacterSheets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAjax]
        [ValidateJSONAntiForgeryHeader]
        public ActionResult Edit(CharacterSheet characterSheet, string MinorVersion)
        {
            if (ModelState.IsValid && CheckSecurity(characterSheet.UserId))
            {
                characterSheet.UpdatedOn = DateTime.Now;
                db.Entry(characterSheet).State = EntityState.Modified;
                SaveDBChanges(characterSheet.CharacterSheetId, MinorVersion);

                return Json(Helpers.AjaxHelper.Notify("Character sheet saved.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success), JsonRequestBehavior.AllowGet);
            }

            return Json(Helpers.AjaxHelper.Notify("Error saving character sheet.", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.error, true), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the database changes.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        private async Task<int> SaveDBChanges(Guid id, string MinorVersion)
        {
            // if this is a major version change (i.e. they click the save button)
            // then make a new screenshot
            if (MinorVersion.ToLower() == "false")
            {
                await Task.Run(() => this.ToThumbnail(Request, id.ToString()));                
            }
            return db.SaveChanges();
        }

        [AllowAnonymous]
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
        [Authorize]
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
        [Authorize]
        [HttpPost]
        [ValidateAjax]
        [ValidateJSONAntiForgeryHeader]
        public ActionResult Create(CharacterSheet characterSheet)
        {
            if (ModelState.IsValid && CheckSecurity(characterSheet.UserId))
            {
                characterSheet.UpdatedOn = DateTime.Now;
                db.Entry(characterSheet).State = EntityState.Added;
                SaveDBChanges(characterSheet.CharacterSheetId, "false");

                return Json(Helpers.AjaxHelper.Notify("Character sheet created.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success, false, Url.Action("Edit", "CharacterSheets", new { id = characterSheet.CharacterSheetId })), JsonRequestBehavior.AllowGet);
            }            
            return Json(Helpers.AjaxHelper.Notify("Error creating character sheet.", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.error, true), JsonRequestBehavior.AllowGet);
        }

        // GET: CharacterSheets/Delete/5
        [Authorize]
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
            return View(characterSheet).WithNotification("Deleting this sheet will also delete any characters created with it. This action cannot be undone!", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.warning, 5000);
        }

        // POST: CharacterSheets/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            CharacterSheet characterSheet = db.CharacterSheet.Find(id);
            if (CheckSecurity(characterSheet.UserId))
            {
                IEnumerable<Character> chars = db.Character.Where(c => c.CharacterSheetId == id);
                db.Character.RemoveRange(chars);
                db.CharacterSheet.Remove(characterSheet);
                db.SaveChanges();

                var thumbDir = Server.MapPath("~/Content/CharacterSheet/Thumbnails/");
                var path = Path.Combine(thumbDir, id.ToString());

                FileInfo fi = new FileInfo(path);

                if (fi.Exists)
                {
                    fi.Delete();
                }

                return RedirectToAction("Index").WithNotification("Character sheet deleted.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success);
            }
            return RedirectToAction("Index").WithNotification("Error deleting character sheet.", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.error);
        }

        private bool CheckSecurity(string userId)
        {
            if (IdentityExtensions.GetUserId(User.Identity) != userId)
                return false;
            else
                return true;
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
