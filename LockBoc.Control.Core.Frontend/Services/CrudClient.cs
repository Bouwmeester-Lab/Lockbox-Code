using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Frontend.Contracts;
using LockBoxControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LockBoxControl.Core.Frontend.Services
{
    public class CrudClient<TEntity, TId> : ICrudClient<TEntity, TId>
        where TEntity : IEntity<TId>
        where TId : IEquatable<TId>
    {
        protected virtual string Path
        {
            get
            {
                if (ControllerName.EndsWith("s"))
                {
                    return $"/api/{ControllerName}";
                }
                else if (ControllerName.EndsWith("y"))
                {
                    return $"/api/{ControllerName.TrimEnd('y')}ies";
                }
                else
                {
                    return $"/api/{ControllerName}s";
                }
            }
        }
        protected virtual string ControllerName =>typeof(TEntity).Name;

        private readonly HttpClient httpClient;
        public CrudClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.DeleteAsync($"{Path}/{id}", cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<TEntity>?> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(Path, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<TEntity>>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync($"{Path}/{id}", cancellationToken: cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TEntity>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync($"{Path}/count", cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return int.Parse(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
        }

        public async Task<PagedResult<TEntity>?> GetPageAsync(int pageNumber, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync($"{Path}/page?pageNumber={pageNumber}&pageSize={pageSize}", cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PagedResult<TEntity>>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<TEntity?> SaveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostAsJsonAsync(Path, entity, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TEntity>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PutAsJsonAsync($"{Path}/{entity.Id}", entity, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}
