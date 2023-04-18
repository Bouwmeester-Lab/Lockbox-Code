using LockboxControl.Core.Contracts;
using LockboxControl.Core.Models;
using LockBoxControl.Api.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LockBoxControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : CrudBaseController<Command>
    {
        public CommandsController(IQueryableRepositoryService<Command> repositoryService, ILogger<CrudBaseController<Command>> logger) : base(repositoryService, logger)
        {
        }
    }
}
