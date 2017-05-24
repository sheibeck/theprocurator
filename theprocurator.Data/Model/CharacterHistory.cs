using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace theprocurator.Data.Model
{
    public class CharacterHistory : IHistoryRepository
    {
        public CharacterHistory()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HistoryId { get; set; }

        [Required]
        public string Version { get; set; }

        [Required]
        public string Date { get; set; }

        [Required]
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
                
    }
}
