using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockBoxControl.Core.Contracts;

/// <summary>
/// An entity with an ID of type <typeparamref name="T"/>
/// </summary>
public interface IEntity<T>
{
    /// <summary>
    /// Entity's ID.
    /// </summary>
    T Id { get; set; }
}
/// <summary>
/// Default IEntity with type <see cref="long"/>
/// </summary>
public interface IEntity : IEntity<long> { }
