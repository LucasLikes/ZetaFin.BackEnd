using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZetaFin.Application.DTOs;

public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
