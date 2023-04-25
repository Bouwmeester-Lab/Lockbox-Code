using LockBoxControl.Core.Contracts;
using LockBoxControl.Api.Contracts;
using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LockBoxControl.Api.Controllers.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class CrudBaseController<TEntity> : ControllerBase, ICrudController<TEntity> where TEntity : class, IEntity
    {
        protected IQueryableRepositoryService<TEntity> _repositoryService;
        private readonly ILogger<CrudBaseController<TEntity>> _logger;

        public CrudBaseController(IQueryableRepositoryService<TEntity> repositoryService, ILogger<CrudBaseController<TEntity>> logger)
        {
            _repositoryService = repositoryService;
            _logger = logger;

        }

        //[RequiredScope(ApiScopes.Read)]
        [HttpGet("count")]
        public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await _repositoryService.QueryAll().CountAsync(cancellationToken).ConfigureAwait(false);
        }

        // GET: api/<ValuesController>
        /// <summary>
        /// Gets all the items of this kind.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[RequiredScope(ApiScopes.Read)]
        public virtual Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _repositoryService.GetAllAsync(cancellationToken);
        }
        /// <summary>
        /// Gets a page from the database
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //[RequiredScope(ApiScopes.Read)]
        [HttpGet("page")]
        public async Task<PagedResult<TEntity>> GetPageAsync([FromQuery][Required] int pageNumber, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if(pageNumber < 0 || pageSize < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(pageNumber)} and {nameof(pageSize)} must be greater than 0.");
            }
            var results = await _repositoryService.GetPage(pageNumber, pageSize).ToListAsync(cancellationToken);
            var count = await _repositoryService.QueryAll().CountAsync(cancellationToken);

            return new PagedResult<TEntity>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Results = results,
                TotalMatches = count
            };
        }
        /// <summary>
        /// Get an item with its id.
        /// </summary>
        /// <param name="id">long describing the id of the item.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        //[RequiredScope(ApiScopes.Read)]
        public virtual Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return _repositoryService.GetAsync(id, cancellationToken);
        }
        /// <summary>
        /// Save an item.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// 
        //[Authorize(Policy = PredifinedRoles.AdminsPolicyName)]
        [HttpPost]
        //[RequiredScope(ApiScopes.Write)]
        public virtual async Task<ActionResult<TEntity>> SaveAsync([FromBody] TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                await _repositoryService.CreateAsync(entity).ConfigureAwait(false);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                //log
                _logger.LogError(ex, "");
                if (ex.InnerException != null)
                    return BadRequest(ex.InnerException.Message);
                return BadRequest(ex.Message);
            }

        }
        /// <summary>
        /// Deletes the item with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //[Authorize(Policy = PredifinedRoles.AdminsPolicyName)]
        [HttpDelete("{id}")]
        //[RequiredScope(ApiScopes.Delete)]
        public virtual async Task<IActionResult> DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _repositoryService.DeleteAsync(id).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return BadRequest(ex);
            }
        }
        // PUT api/<ValuesController>/5
        /// <summary>
        /// Updates an item with a given id with the given json.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //[Authorize(Policy = PredifinedRoles.AdminsPolicyName)]
        [HttpPut("{id}")]
        //[RequiredScope(ApiScopes.Update)]
        public virtual async Task<IActionResult> UpdateAsync(long id, [FromBody] TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                return BadRequest(new ArgumentNullException(nameof(entity)));
            if (id == 0)
                return BadRequest(new ArgumentException("id cannot be 0"));
            if (entity.Id != id)
                return BadRequest(new ArgumentException("id and entitie's id must be the same."));
            await _repositoryService.UpdateAsync(entity).ConfigureAwait(false);
            return Ok();
        }
    }
}
