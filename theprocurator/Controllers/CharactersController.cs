using Microsoft.AspNet.Identity;
using NotyNotification.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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

        [Authorize]
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

        [AllowAnonymous]
        public ActionResult Search(string searchtext)
        {
            var characters = db.Character
                                    .Include(c => c.CharacterSheet)
                                    .Include(c => c.User)
                                    .OrderBy(c => c.CharacterName)
                                    .Where(c => c.Published == true);                                    

            if (!string.IsNullOrEmpty(searchtext))
            {
                characters = characters.Where(c => c.CharacterSheet.CharacterSheetName.Contains(searchtext)
                                                || c.CharacterSheet.CharacterSheetTheme.Contains(searchtext)
                                                || c.User.UserName.Contains(searchtext)
                                                || c.CharacterName.Contains(searchtext));
            }

            ViewBag.SearchText = searchtext;

            return View(characters.ToList());
        }


        [Authorize]
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
            character.ParentId = id;
            character.CharacterId = Guid.NewGuid();
            character.CharacterName = character.CharacterName + " copy";
            character.CharacterUrl = character.CharacterUrl.Replace(" ", "-") + "-copy";
            character.Published = false;            
            character.UpdatedOn = DateTime.Now;
            db.Character.Add(character);
            db.SaveChanges();

            // copy the character asset folder
            CopyAssetDirectory(character.CharacterId, character.ParentId);            

            return RedirectToAction("Edit", new { id = character.CharacterId })
                    .WithNotification("Character was added to your collection.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success);
        }

        private void CopyAssetDirectory(Guid characterId, Guid parentId)
        {
            var SourcePath = Server.MapPath(String.Format("~/Content/Characters/{0}", parentId));
            var DestinationPath = Server.MapPath(String.Format("~/Content/Characters/{0}", characterId));

            DirectoryCopy(SourcePath, DestinationPath, true);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        [AllowAnonymous]
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

        [Authorize]
        // GET: Characters/Create
        public ActionResult Create(Guid id)
        {
            if (id == Guid.Empty)
            {
                return RedirectToAction("Index", "CharacterSheets")
                    .WithNotification("Choose a sheet from your collection and press the <a href='' class='btn btn-success btn-xs'><span class='glyphicon glyphicon-user'></span> Create Character</a> button.",
                        NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.information, 3000, true, false);
            }

            var character = new Character();
            
            // grab the character sheet we want to make this character from
            character.UserId = IdentityExtensions.GetUserId(User.Identity);
            character.CharacterSheet = db.CharacterSheet.Where(c => c.CharacterSheetId == id)
                                            .FirstOrDefault();
            character.CharacterId = Guid.NewGuid();

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
        [Authorize]
        [HttpPost]
        [ValidateAjax]
        [ValidateJSONAntiForgeryHeader]
        public ActionResult Create(Character character)
        {           
            if (ModelState.IsValid && CheckSecurity(character.UserId))
            {

                // create an asset directory for the character
                var charDir = Server.MapPath(string.Format("~/Content/Characters/{0}", character.CharacterId));
                if (!Directory.Exists(charDir))
                {
                    Directory.CreateDirectory(charDir);
                }


                character.UpdatedOn = DateTime.Now;
                db.Entry(character).State = EntityState.Added;
                db.SaveChanges();

                return Json(Helpers.AjaxHelper.Notify("Character created.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success, false, Url.Action("Edit", "Characters", new { id = character.CharacterId})), JsonRequestBehavior.AllowGet);
            }

            return Json(Helpers.AjaxHelper.Notify("Error creating character.", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.error, true), JsonRequestBehavior.AllowGet);            
        }

        // GET: Characters/Edit/5
        [Authorize]
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


        /// <summary>
        /// Files the upload.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAjax]
        public ActionResult FileUpload(string id)
        {
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk                        
                        var imageDir = Server.MapPath(String.Format("~/Content/Characters/{0}", id));

                        if (!Directory.Exists(imageDir))
                        {
                            Directory.CreateDirectory(imageDir);
                        }

                        var path = Path.Combine(imageDir, fileContent.FileName);
                        
                        using (FileStream fs = new FileStream(path, FileMode.Create))
                        {
                            stream.CopyTo(fs);                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            return Json("File uploaded successfully");
        }

        [Authorize]
        [HttpPost]
        [ValidateAjax]
        public ActionResult FileDelete(string fileName, string id)
        {
            try
            {                
                var imageDir = Server.MapPath(string.Format("~/Content/Characters/{0}", id));
                var path = Path.Combine(imageDir, fileName);

                FileInfo fi = new FileInfo(path);

                if (fi.Exists)
                {
                    fi.Delete();
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Delete failed");
            }

            return Json("File deleted successfully");
        }

        // POST: Characters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAjax]
        [ValidateJSONAntiForgeryHeader]
        public ActionResult Edit(Character character)
        {
            if (ModelState.IsValid && CheckSecurity(character.UserId))
            {
                character.UpdatedOn = DateTime.Now;
                db.Entry(character).State = EntityState.Modified;
                db.SaveChanges();
                return Json(Helpers.AjaxHelper.Notify("Character saved.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success), JsonRequestBehavior.AllowGet);
            }
            return Json(Helpers.AjaxHelper.Notify("Error saving character.", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.error, true), JsonRequestBehavior.AllowGet);
        }

        // GET: Characters/Delete/5
        [Authorize]
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
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Character character = db.Character.Find(id);
            if (CheckSecurity(character.UserId))
            {
                db.Character.Remove(character);
                db.SaveChanges();

                // cleanup the asset folder
                var charDir = Server.MapPath(string.Format("~/Content/Characters/{0}", id));
                if (Directory.Exists(charDir))
                {
                    Directory.Delete(charDir);
                }

                return RedirectToAction("Index").WithNotification("Character deleted.", NotyNotification.Model.Position.topRight, NotyNotification.Model.AlertType.success);
            }
            return RedirectToAction("Index").WithNotification("Error deleting character.", NotyNotification.Model.Position.center, NotyNotification.Model.AlertType.error);
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
