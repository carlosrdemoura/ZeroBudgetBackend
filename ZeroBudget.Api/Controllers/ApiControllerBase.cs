using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ZeroBudget.Api.Controllers;

[ApiController]
[Authorize]
public abstract class ApiControllerBase : ControllerBase;
