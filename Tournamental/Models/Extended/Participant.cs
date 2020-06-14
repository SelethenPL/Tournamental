using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tournamental.Models
{
    [MetadataType(typeof(ParticipantMetadata))]
    public partial class Participant
    {

    }

    public class ParticipantMetadata
    {
        [Display(Name = "User's Licence ID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Licence ID cannot be null")]
        public string LicenceNumber { get; set; }

        [Display(Name = "User's current rank")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Rank cannot be null")]
        [Range(0, int.MaxValue, ErrorMessage = "Rank cannot be below zero.")]
        public int CurrentRanking { get; set; }
    }
}