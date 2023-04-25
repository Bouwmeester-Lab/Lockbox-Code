using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockBoxControl.Storage.Extensions
{
    public static class DiscriminatorExtensions
    {
        /// <summary>
        /// Allows you to get only one type of objects ignoring the hierarchy by filtering on the Discriminator column. Make sure to be using TPH pattern in EF Core. This will not work if there's no "Discriminator" column.
        /// </summary>
        /// <typeparam name="TIn">The in type</typeparam>
        /// <typeparam name="TOut">The type to cast the answer to. This is the value used in the discriminator.</typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static IQueryable<TOut> OfOnlyType<TIn, TOut>(this IQueryable<TIn> queryable)
        {
            return queryable.Where(x => EF.Property<string>(x, "Discriminator") == typeof(TOut).Name).Cast<TOut>();
        }

        public static IQueryable<T> OfOnlyType<T>(this IQueryable<T> queryable)
        {
            return queryable.OfOnlyType<T, T>();
        }
        /// <summary>
        /// Applies a query filter based on the discriminator column in the TPH (table per hierarchy) pattern in EF Core. The aim is to select only the items that are that class only. And not all the derived classes as well.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityTypeBuilder"></param>
        /// <returns></returns>
        public static EntityTypeBuilder<T> ApplyOnlyTypeQueryFilter<T>(this EntityTypeBuilder<T> entityTypeBuilder)
            where T : class
        {
            return entityTypeBuilder.HasQueryFilter(x => EF.Property<string>(x, "Discriminator") == typeof(T).Name);
        }
    }
}
