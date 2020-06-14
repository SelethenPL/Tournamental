using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tournamental.Models
{
    [MetadataType(typeof(TournamentMetadata))]
    public partial class Tournament
    {
        public SelectList DisciplineList = new SelectList(
            new List<SelectListItem>
            {
                new SelectListItem { Text="", Value="", Selected=true},
                new SelectListItem { Text="Football", Value="football"},
                new SelectListItem { Text="Basketball", Value="basketball"},
                new SelectListItem { Text="Triathlon", Value="triathlon"},
                new SelectListItem { Text="Swimming", Value="swimming"},
                new SelectListItem { Text="Archery", Value="archery"}
            });
    }

    public class TournamentMetadata
    {
        [Display(Name = "Tournament Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Tournament name can't be empty.")]
        public string Name { get; set; }

        [Display(Name = "Discipline")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Discipline can't be empty")]
        public string Discipline { get; set; }

        [Display(Name = "Organizer ID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to be logged in, to add new")]
        public int Organizer { get; set; }

        [Display(Name = "Start Time")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Start time can't be left empty")]
        [DataType(DataType.Date)]
        public DateTime Time { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Max number of participants")]
        [Range(0, int.MaxValue, ErrorMessage = "Number can't be negative")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Number of participants required")]
        public int MaxParticipant { get; set; }
        
        [Display(Name = "Application deadline")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Deadline can't be empty")]
        [DataType(DataType.Date)]
        public DateTime ApplicationDeadline { get; set; }

        [Display(Name = "Link to sponsor logo")]
        public string SponsorLogo { get; set; }

        [Display(Name = "Number of ranked players")]
        [Range(0, int.MaxValue, ErrorMessage = "Number can't be negative")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Number of ranked players can't be empty")]
        public int RankedPlayers { get; set; }

    }
}