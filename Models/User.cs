﻿using System;
using System.Collections.Generic;

namespace HomyWayAPI.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int Gid { get; set; }

    public virtual Group GidNavigation { get; set; } = null!;
}
