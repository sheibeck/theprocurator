using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace theprocurator.Data.Model
{
    public class CharacterList
    {
        public CharacterList()
        {
            this.Characters = new HashSet<Character>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CharacterListId { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Name")]
        public string CharacterListName { get; set; }
       
        public DateTime UpdatedOn { get; set; }        
        public bool Published { get; set; }

        // Foreign Keys      
        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Character> Characters { get; set; }
    }
}
