﻿using System.Collections.Generic;
using FFXIVVenues.Api.PersistenceModels.Entities.Venues;
using FFXIVVenues.VenueModels.Observability;

namespace FFXIVVenues.Api.Observability;

public record VenueObservation(ObservableOperation Operation, 
        string SubjectId,
        string SubjectName, 
        bool Approved,
        string DataCenter, 
        string World, 
        List<string> Managers)
    : Observation(Operation, SubjectId, SubjectName)
{
    public static VenueObservation FromVenue(ObservableOperation op, Venue venue) =>
        new(op, venue.Id, venue.Name, venue.Approved, venue.Location.DataCenter, venue.Location.World, venue.Managers);
}