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
    public class RequestsController : CrudBaseController<Request, Guid, DateTime>
    {
        public RequestsController(IGenericQueryableRepositoryService<Request, Guid> repositoryService, ILogger<CrudBaseController<Request, Guid, DateTime>> logger) : base(repositoryService, logger)
        {
        }

        protected override Expression<Func<Request, DateTime>> OrderExpression
        {
            get
            {
                return value => value.RequestDateTime;
            }
        }
    }
}
