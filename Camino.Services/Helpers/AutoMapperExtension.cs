using AutoMapper;

namespace Camino.Services.Helpers
{
    public static class AutoMapperExtension
    {
        public static IMapper Mapper { get; set; }
        #region Utilities

        /// <summary>
        /// Execute a mapping from the source object to a new destination object. The source type is inferred from the source object
        /// </summary>
        /// <typeparam name="TDestination">Destination object type</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <returns>Mapped destination object</returns>
        public static TDestination Map<TDestination>(this object source)
        {
            //use AutoMapper for mapping objects
            return Mapper.Map<TDestination>(source);
        }

        /// <summary>
        /// Execute a mapping from the source object to the existing destination object
        /// </summary>
        /// <typeparam name="TSource">Source object type</typeparam>
        /// <typeparam name="TDestination">Destination object type</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <param name="destination">Destination object to map into</param>
        /// <returns>Mapped destination object, same instance as the passed destination object</returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            //use AutoMapper for mapping objects
            return Mapper.Map(source, destination);
        }

        #endregion
    }
}
