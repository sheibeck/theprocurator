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

namespace theprocurator.Controllers
{
    public class CharacterListsController : Controller
    {
        private TheProcuratorDbContext db = new TheProcuratorDbContext();

        // GET: CharacterLists
        public ActionResult Index()
        {         
            string currentUserId = IdentityExtensions.GetUserId(this.User.Identity);
            var list = db.CharacterLists
                                .Include(c => c.Characters)
                                .Include(c => c.User)
                              .Where(c => c.UserId == currentUserId);
            return View(list.ToList());
        }

        // GET: CharacterLists/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CharacterList characterList = db.CharacterLists.Find(id);
            if (characterList == null)
            {
                return HttpNotFound();
            }
            return View(characterList);
        }

        // GET: CharacterLists/Create
        public ActionResult Create()
        {
            CharacterList characterList = new CharacterList();
            characterList.CharacterListId = Guid.NewGuid();
            characterList.UpdatedOn = DateTime.Now;
            characterList.UserId = IdentityExtensions.GetUserId(this.User.Identity);

            GetViewData(null);

            return View(characterList);
        }

        // POST: CharacterLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind()] CharacterList characterList, string[] selectedCharacters)
        {           
            if (ModelState.IsValid)
            {
                db.Entry(characterList).State = EntityState.Added;
                characterList.UpdatedOn = DateTime.Now;
           
                // add selected characters to this list               
                selectedCharacters.ToList().ForEach(t =>                   
                   characterList.Characters.Add(db.Character.Find(Guid.Parse(t)))
                );
                
                // save the list
                db.CharacterLists.Add(characterList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            GetViewData(characterList.CharacterListId);

            return View(characterList);
        }

        // GET: CharacterLists/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CharacterList characterList = db.CharacterLists.Find(id);                                            
            if (characterList == null)
            {
                return HttpNotFound();
            }

            GetViewData(id);

            return View(characterList);
        }     

        // POST: CharacterLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind()] CharacterList characterList, string[] selectedCharacters)
        {
            if (ModelState.IsValid)
            {
                db.Entry(characterList).State = EntityState.Modified;
                characterList.UpdatedOn = DateTime.Now;

                // load character data for editing
                var item = db.Entry<CharacterList>(characterList);
                item.Collection(i => i.Characters).Load();
                
                // add selected characters to this list
                characterList.Characters.Clear();
                selectedCharacters.ToList().ForEach(t =>
                   characterList.Characters.Add(db.Character.Find(Guid.Parse(t)))
                );

                // save changes
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            GetViewData(characterList.CharacterListId);

            return View(characterList);
        }

        // GET: CharacterLists/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CharacterList characterList = db.CharacterLists.Find(id);
            if (characterList == null)
            {
                return HttpNotFound();
            }
            return View(characterList);
        }

        // POST: CharacterLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            CharacterList characterList = db.CharacterLists.Find(id);
            db.CharacterLists.Remove(characterList);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region helpers
        /// <summary>
        /// Gets the view data.
        /// </summary>
        /// <param name="id">The identifier.</param>
        private void GetViewData(Guid? id)
        {
            string userId = IdentityExtensions.GetUserId(this.User.Identity);
            var characters = db.Character.Include("CharacterSheet").Where(c => c.UserId == userId);

            if (id != null)
            {
                CharacterList characterList = db.CharacterLists
                                                .Where(c => c.CharacterListId == id)
                                                .Include("Characters")
                                                .FirstOrDefault();

                ViewBag.CharacterList = new MultiSelectList(characters, "id", "description", characterList.Characters.Select(c => c.CharacterId));
            }
            else
            {
                ViewBag.CharacterList = new MultiSelectList(characters, "id", "description");
            }

        }
        # endregion helpers

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
