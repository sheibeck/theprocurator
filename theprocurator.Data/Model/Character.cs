using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace theprocurator.Data.Model
{
    public class Character
    {
        public Character()
        {
            this.CharacterLists = new HashSet<CharacterList>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CharacterId { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Name")]
        public string CharacterName { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Url")]
        public string CharacterUrl { get; set; }

        [Required]        
        [Display(Name = "Data")]
        public string CharacterData { get; set; }

        // Character Sheet
        [Required]
        public Guid CharacterSheetId { get; set; }
        public CharacterSheet CharacterSheet { get; set; }

        public Guid ParentId { get; set; }
        public DateTime UpdatedOn { get; set; }        
        public bool Published { get; set; }

        // Foreign Keys
        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<CharacterList> CharacterLists { get; set; }
    }
}
