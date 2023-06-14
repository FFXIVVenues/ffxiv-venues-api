using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Internal;
using FFXIVVenues.Api.PersistenceModels.Entities.Venues;

namespace FFXIVVenues.Api.PersistenceModels.Mapping;

public class InPlaceListMemberResolver<S, D> : IMemberValueResolver<VenueModels.Venue, Venue, List<S>, List<D>>
{
    private MemberExpression[] _members;

    public List<D> Resolve(VenueModels.Venue source, Venue destination, List<S> sourceMember, List<D> destMember, ResolutionContext context)
    {
        if (destMember == null)
            return context.Mapper.Map<List<D>>(sourceMember);

        var newList = (from s in sourceMember 
            // todo: 
            //  s and d are two different types but we're applying the same Member object against them. 
            //  So it may not work. We may have to map s to d first, or do some kind of s:d member dictionary. 
            //  Also need to figure out how these members get in in the first place, since we can't call this class. 
            //  May a [MapKey] attribute, or [MapKey(1)] and the numbers are matched up.
            let match = destMember.FirstOrDefault(d => 
                _members.All(m => m.Member.GetMemberValue(s) == m.Member.GetMemberValue(d))) 
            select match != null ?
                context.Mapper.Map(s, match) : context.Mapper.Map<D>(s)).ToList();

        destMember.Clear();
        destMember.AddRange(newList);
        return destMember;
    }

    public InPlaceListMemberResolver<S, D> MapByMember(params MemberExpression[] members)
    {
        this._members = members;
        return this;
    }
    
}