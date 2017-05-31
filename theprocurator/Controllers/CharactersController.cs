using Microsoft.AspNet.Identity;
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
using static theprocurator.Helpers.AjaxHelpers;

namespace theprocurator.Controllers
{
    public class CharactersController : Controller
    {
        private TheProcuratorDbContext db = new TheProcuratorDbContext();

        // GET: Characters
        public ActionResult Index()
        {
            var character = db.Character.Include(c => c.User);
            return View(character.ToList());
        }

        // GET: Characters/Details/5
        public ActionResult View(Guid? id)
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
            return View(character);
        }

        // GET: Characters/Create
        public ActionResult Create()
        {
            var character = new Character();
            // revalidate after fetching the logged in user
            character.UserId = IdentityExtensions.GetUserId(User.Identity);

            // TODO: allow user to pick sheet
            character.CharacterSheetId = db.CharacterSheet.FirstOrDefault().CharacterSheetId;
            character.CharacterSheet = db.CharacterSheet.FirstOrDefault();
            return View(character);
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
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", character.UserId);
            return View(character);
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
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", character.UserId);
            return View(character);
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
            return View(character);
        }

        // POST: Characters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Character character = db.Character.Find(id);
            db.Character.Remove(character);
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
