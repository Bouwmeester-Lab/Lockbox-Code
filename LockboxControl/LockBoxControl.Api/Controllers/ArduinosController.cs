using LockBoxControl.Core.Contracts;
using LockBoxControl.Api.Controllers.Base;
using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LockBoxControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArduinosController : CrudBaseController<Arduino>
    {
        public ArduinosController(IQueryableRepositoryService<Arduino> repositoryService, ILogger<CrudBaseController<Arduino>> logger) : base(repositoryService, logger)
        {
        }
    }
}
