﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace theprocurator.Data.Model
{
    public class Character
    {
        public Character()
        {

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
        public Guid CharacerSheetId { get; set; }
        public CharacterSheet CharacterSheet { get; set; }

        // User        
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
