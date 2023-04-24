using LockboxControl.Core.Contracts;
using LockboxControl.Core.Models;
using LockBoxControl.Api.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LockBoxControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusesController : CrudBaseController<ArduinoStatus, Guid, DateTime>
    {
        public StatusesController(IGenericQueryableRepositoryService<ArduinoStatus, Guid> repositoryService, ILogger<CrudBaseController<ArduinoStatus, Guid, DateTime>> logger) : base(repositoryService, logger)
        {
        }

        protected override Expression<Func<ArduinoStatus, DateTime>> OrderExpression
        {
            get
            {
                return value => value.StatusDateTime;
            }
        }
    }
}
