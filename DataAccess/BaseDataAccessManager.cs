namespace RussellScreener.DataAccess {
    public abstract class BaseDataAccessManager {

        #region Methods

        /// <summary>
        /// Return the name of the cache to be used for this manager.
        /// </summary>
        /// <returns>Name of the cache to use for this manager</returns>
        public abstract string GetCacheFileName();

        #endregion Methods
    }
}