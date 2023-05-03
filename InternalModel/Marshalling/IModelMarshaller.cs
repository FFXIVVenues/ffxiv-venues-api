namespace FFXIVVenues.Api.InternalModel.Marshalling;

public interface IModelMarshaller
{ 
    VenueModels.Venue MarshalAsPublicModel(Venue venue);
    Venue MarshalAsInternalModel(in Venue existingVenue, VenueModels.Venue venue);
    Venue MarshalAsInternalModel(VenueModels.Venue venue, string owningKey);
}