using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Data.Entities;

[PrimaryKey("EventRef", "UserRef")]
[Table("EventUser")]
public partial class EventUser
{
    [Key]
    public int EventRef { get; set; }

    [Key]
    public int UserRef { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Creation { get; set; }

    [ForeignKey("EventRef")]
    [InverseProperty("EventUsers")]
    public virtual Event EventRefNavigation { get; set; } = null!;

    [ForeignKey("UserRef")]
    [InverseProperty("EventUsers")]
    public virtual User UserRefNavigation { get; set; } = null!;
}
