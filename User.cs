using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Data.Entities;

public partial class User
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    [InverseProperty("UserRefNavigation")]
    public virtual ICollection<EventUser> EventUsers { get; set; } = new List<EventUser>();
}
