using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.CustomAttributes
{
    public abstract class CustomAttribute<T> where T : IConvertible
    {
        private string key;
        private T value;
        private string updatedByUser;
        private DateTime updatedDate = DateTime.Now;

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        public T Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public string UpdatedByUser
        {
            get { return updatedByUser; }
            set { updatedByUser = value; }
        }

        public DateTime UpdatedDate
        {
            get { return updatedDate; }
            set { updatedDate = value; }
        }

        public void SetAttributeValue(T value, string updatedByUser)
        {
            this.value = value;
            this.updatedByUser = updatedByUser;
            this.updatedDate = DateTime.Now;
        }

        public T GetAttributeValue(string key)
        {
            return value;
        }
    }
}
