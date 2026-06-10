using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Database.Abstractions
{
    public interface IEntity<TKey>
    {
        /// <summary>
        /// Gets or sets the primary key of the entity.
        /// </summary>
        TKey Id { get; set; }
    }
}
