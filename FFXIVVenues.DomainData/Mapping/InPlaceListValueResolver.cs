using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using FFXIVVenues.DomainData.Entities.Venues;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FFXIVVenues.DomainData.Mapping;

public class InPlaceListValueResolver
{
    protected record TypeMap(Type SourceType, Type DestinationType);
    protected record PropMap(PropertyInfo SourceProp, PropertyInfo DestinationProp);
    protected static Dictionary<TypeMap, List<PropMap>> PropsMap = new();
    
    public static void Map<S, D>(Expression<Func<S, object>> srcMember, Expression<Func<S, object>> dstMember)
    {
        var typeMap = new TypeMap(typeof(S), typeof(D));
        var propMap = new PropMap(srcMember.GetPropertyAccess(), dstMember.GetPropertyAccess());
        PropsMap.TryAdd(typeMap, new List<PropMap>());
        PropsMap[typeMap].Add(propMap);
    }
}

public class InPlaceListValueResolver<S, D> : InPlaceListValueResolver, IMemberValueResolver<VenueModels.Venue, Venue, List<S>, List<D>>
{

    public List<D> Resolve(VenueModels.Venue source, Venue destination, List<S> sourceList, List<D> destinationList, ResolutionContext context)
    {
        if (destinationList == null)
            return context.Mapper.Map<List<D>>(sourceList);

        var typeMap = new TypeMap(typeof(S), typeof(D));
        if (!PropsMap.TryGetValue(typeMap, out var propMaps))
            return context.Mapper.Map<List<D>>(sourceList);

        var newList = (from s in sourceList 
            let match = destinationList.FirstOrDefault(d => 
                propMaps.All(map => map.SourceProp.GetValue(s) == map.DestinationProp.GetValue(d))) 
            select match != null ?
                context.Mapper.Map(s, match) : context.Mapper.Map<D>(s)).ToList();

        destinationList.Clear();
        destinationList.AddRange(newList);
        return destinationList;
    }

}