using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.Common.Domain
{
    /// <summary>
    /// This class provides a consistent implementation of Equals and GetHashCode.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    public abstract class EqualityAndHashCodeProvider<TId> where TId : struct 
    {
        private int? oldHashCode;
        protected TId id;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public virtual TId Id 
        {
            get { return id; }
            set { id = value; } 
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
	        if (ReferenceEquals(obj, null))
	        {
				return false;
	        }

	        if (!GetType().IsInstanceOfType(obj)
				&& !obj.GetType().IsInstanceOfType(this))
	        {
				return false;
	        }

	        var other = (EqualityAndHashCodeProvider<TId>) obj;

	        if (other.IsTransient() && this.IsTransient())
	        {
				return ReferenceEquals(other, this);
	        }
                
            return other.Id.Equals(Id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (oldHashCode.HasValue)
			{ return oldHashCode.Value;}

            if (this.IsTransient())
            {
                oldHashCode = base.GetHashCode();
                return oldHashCode.Value;
            }

            return Id.GetHashCode();
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="x">The x.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(EqualityAndHashCodeProvider<TId> x, EqualityAndHashCodeProvider<TId> y)
        {
            return Equals(x, y);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="x">The x.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(EqualityAndHashCodeProvider<TId> x, EqualityAndHashCodeProvider<TId> y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Determines whether this instance is transient (The Id still has the default value for the type of the Id)
        /// </summary>
        /// <returns></returns>
        public virtual bool IsTransient()
        {
            return Equals(Id, default(TId));
        }
    }
}
