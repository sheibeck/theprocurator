using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace theprocurator.Data.Model
{
    public class CharacterSheetHistory : IHistoryRepository
    {
        public CharacterSheetHistory()
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
    }
}
