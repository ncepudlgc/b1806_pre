using Microsoft.AspNetCore.Mvc;

namespace Bravia.Api.Controllers;

public abstract class BraviaController : ControllerBase
{
    public abstract string ControllerName { get; }
}