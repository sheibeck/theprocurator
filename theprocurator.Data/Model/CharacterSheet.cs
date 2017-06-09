using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace theprocurator.Data.Model
{
    public class CharacterSheet
    {
        public CharacterSheet()
        {

        }

        [Key]
        public Guid CharacterSheetId { get; set; }
        
        [Required]
        [StringLength(250)]
        [Display(Name = "Name")]
        public string CharacterSheetName { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Url")]
        public string CharacterSheetUrl { get; set; }

        [Required]        
        [Display(Name = "Form")]
        public string CharacterSheetForm { get; set; }
        
        [Display(Name = "Theme")]
        public string CharacterSheetTheme { get; set; }


        public Guid ParentId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool Published { get; set; }

        // Foreign Keys      
        [Required] 
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public ICollection<Character> Characters { get; set; }
    }
}
