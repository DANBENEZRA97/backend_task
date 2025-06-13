using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Data.Entities;

public partial class Event
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    public int MaxRegistrations { get; set; }

    [StringLength(50)]
    public string Location { get; set; } = null!;

    [InverseProperty("EventRefNavigation")]
    [JsonIgnore]
    public virtual ICollection<EventUser> EventUsers { get; set; } = new List<EventUser>();
}
