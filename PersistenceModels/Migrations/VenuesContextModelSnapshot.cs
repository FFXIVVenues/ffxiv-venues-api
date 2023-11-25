﻿// <auto-generated />
using System;
using System.Collections.Generic;
using FFXIVVenues.Api.PersistenceModels.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FFXIVVenues.Api.PersistenceModels.Migrations
{
    [DbContext(typeof(FFXIVVenuesDbContext))]
    partial class VenuesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Venues")
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.VenueView", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("At")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("VenueId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("VenueId");

                    b.ToTable("VenueViews", "VenueMetrics");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Location", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("Apartment")
                        .HasColumnType("integer");

                    b.Property<string>("DataCenter")
                        .HasColumnType("text");

                    b.Property<string>("District")
                        .HasColumnType("text");

                    b.Property<string>("Override")
                        .HasColumnType("text");

                    b.Property<int>("Plot")
                        .HasColumnType("integer");

                    b.Property<int>("Room")
                        .HasColumnType("integer");

                    b.Property<bool>("Subdivision")
                        .HasColumnType("boolean");

                    b.Property<int>("Ward")
                        .HasColumnType("integer");

                    b.Property<string>("World")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Locations", "Venues");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Notice", b =>
                {
                    b.Property<string>("VenueId")
                        .HasColumnType("text");

                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("End")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Start")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("VenueId", "Id");

                    b.ToTable("Notices", "Venues");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Schedule", b =>
                {
                    b.Property<string>("VenueId")
                        .HasColumnType("text");

                    b.Property<int>("Day")
                        .HasColumnType("integer");

                    b.Property<int>("StartHour")
                        .HasColumnType("integer");

                    b.Property<int>("StartMinute")
                        .HasColumnType("integer");

                    b.Property<int?>("EndHour")
                        .HasColumnType("integer");

                    b.Property<int?>("EndMinute")
                        .HasColumnType("integer");

                    b.Property<string>("LocationId")
                        .HasColumnType("text");

                    b.Property<string>("TimeZone")
                        .HasColumnType("text");

                    b.HasKey("VenueId", "Day", "StartHour", "StartMinute");

                    b.HasIndex("LocationId");

                    b.ToTable("Schedules", "Venues");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.ScheduleOverride", b =>
                {
                    b.Property<string>("VenueId")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Start")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("End")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Open")
                        .HasColumnType("boolean");

                    b.HasKey("VenueId", "Start");

                    b.ToTable("ScheduleOverrides", "Venues");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Venue", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Added")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Approved")
                        .HasColumnType("boolean");

                    b.Property<string>("Banner")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("Deleted")
                        .HasColumnType("timestamp with time zone");

                    b.Property<List<string>>("Description")
                        .HasColumnType("text[]");

                    b.Property<string>("Discord")
                        .HasColumnType("text");

                    b.Property<bool>("Hiring")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LocationId")
                        .HasColumnType("text");

                    b.Property<List<string>>("Managers")
                        .HasColumnType("text[]");

                    b.Property<string>("MareCode")
                        .HasColumnType("text");

                    b.Property<string>("MarePassword")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("ScopeKey")
                        .HasColumnType("text");

                    b.Property<bool>("Sfw")
                        .HasColumnType("boolean");

                    b.Property<List<string>>("Tags")
                        .HasColumnType("text[]");

                    b.Property<string>("Website")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Venues", "Venues");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.VenueView", b =>
                {
                    b.HasOne("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Venue", "Venue")
                        .WithMany()
                        .HasForeignKey("VenueId");

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Notice", b =>
                {
                    b.HasOne("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Venue", "Venue")
                        .WithMany("Notices")
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Schedule", b =>
                {
                    b.HasOne("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.HasOne("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Venue", "Venue")
                        .WithMany("Schedule")
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.ScheduleOverride", b =>
                {
                    b.HasOne("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Venue", "Venue")
                        .WithMany("ScheduleOverrides")
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Venue", b =>
                {
                    b.HasOne("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Location", "Location")
                        .WithMany("Venues")
                        .HasForeignKey("LocationId");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Location", b =>
                {
                    b.Navigation("Venues");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Venue", b =>
                {
                    b.Navigation("Notices");

                    b.Navigation("Schedule");

                    b.Navigation("ScheduleOverrides");
                });
#pragma warning restore 612, 618
        }
    }
}
