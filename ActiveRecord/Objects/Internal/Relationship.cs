using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord.Interfaces;

namespace ActiveRecord.Objects.Internal
{
    public abstract class Relationship<T> : IRelationship where T : class, IActiveRecord, new()
    {
        public enum LoadStatuses
        {
            NotLoaded,
            Loaded
        }

        public LoadMethods LoadMethod = LoadMethods.Active;

        private SaveMethods _saveMethod = SaveMethods.Save;
        public SaveMethods SaveMethod
        {
            get { return _saveMethod; }
            set { _saveMethod = value; }
        }
        
        protected LoadStatuses _loadStatus = LoadStatuses.NotLoaded;
        protected IActiveRecord _parentObject;

        protected List<T> ObjectList = new List<T>();
        
        public void InitializeParent(IActiveRecord parentObject)
        {
            _parentObject = parentObject;
        }

        public void Load()
        {
            if (LoadMethod == LoadMethods.Active)
            {
                Retrieve();
            }
        }

        public void Unload()
        {
            this._loadStatus = LoadStatuses.NotLoaded;

            ObjectList.Clear();
            ObjectList = new List<T>();
        }

        protected void EnsureLoaded()
        {
            if (this._loadStatus == LoadStatuses.NotLoaded && _parentObject != null)
            {
                Retrieve();
            }
        }
        
        public abstract void Save();
        public abstract void Delete();
        
        protected abstract void Retrieve();
        
           
    }
}
