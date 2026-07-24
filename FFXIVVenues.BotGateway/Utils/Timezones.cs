using System.Collections.ObjectModel;
using TimeZoneMap = (string TimeZoneKey, string TimeZoneLabel, string TimeZoneDescription);

namespace FFXIVVenues.BotGateway.Utils;

public static class TimeZones
{
    // ReSharper disable once CollectionNeverUpdated.Global
    public static readonly ReadOnlyCollection<TimeZoneMap> SupportedTimeZones = new ([
        ( "America/New_York",    "Eastern Standard Time (EST)",   "UTC-5 Standard, UTC-4 Summer" ),
        ( "America/Chicago",     "Central Standard Time (CST)",   "UTC-6 Standard, UTC-5 Summer" ),
        ( "America/Denver",      "Mountain Standard Time (MST)",  "UTC-7 Standard, UTC-6 Summer" ),
        ( "America/Los_Angeles", "Pacific Standard Time (PST)",   "UTC-8 Standard, UTC-7 Summer" ),
        ( "America/Halifax",     "Atlantic Standard Time (AST)",  "UTC-4 Standard, UTC-3 Summer" ),
        ( "UTC",                 "Server Time (UTC)",             "UTC+0" ),
        ( "Europe/London",       "Greenwich Mean Time (GMT)",     "UTC+0 Standard, UTC+1 Summer" ),
        ( "Europe/Budapest",     "Central European Time (CET)",   "UTC+1 Standard, UTC+2 Summer" ),
        ( "Europe/Chisinau",     "Eastern European Time (EET)",   "UTC+2 Standard, UTC+3 Summer" ),
        ( "Asia/Tokyo",          "Japan Standard Time (JST)",     "UTC+9" ),
        ( "Asia/Hong_Kong",      "Hong Kong Time (HKT)",          "UTC+8" ),
        ( "Asia/Singapore",      "Singapore Time (SGT)",          "UTC+8" ),
        ( "Asia/Manila",         "Philippine Time (PHT)",         "UTC+8" ),
        ( "Asia/Bangkok",        "Indochina Time (ICT)",          "UTC+7" ),
        ( "Australia/Perth",     "Australian Western Time (AWST)","UTC+8" ),
        ( "Australia/Adelaide",  "Australian Central Time (ACST)","UTC+9:30 Standard, UTC+10:30 Summer" ),
        ( "Australia/Sydney",    "Australian Eastern Time (AEST)","UTC+10 Standard, UTC+11 Summer" )
    ]);
}