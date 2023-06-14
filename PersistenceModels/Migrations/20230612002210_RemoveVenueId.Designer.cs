﻿// <auto-generated />
using System;
using System.Collections.Generic;
using FFXIVVenues.Api.PersistenceModels.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FFXIVVenues.Api.PersistenceModels.Migrations
{
    [DbContext(typeof(VenuesContext))]
    [Migration("20230612002210_RemoveVenueId")]
    partial class RemoveVenueId
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<DateTime>("At")
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
                        .ValueGeneratedOnAdd()
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
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<DateTime>("End")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<DateTime>("Start")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("VenueId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("VenueId");

                    b.ToTable("Notices", "Venues");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.OpenOverride", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<DateTime>("End")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Open")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("Start")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("VenueId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("VenueId");

                    b.ToTable("OpenOverrides", "Venues");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Opening", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<int>("Day")
                        .HasColumnType("integer");

                    b.Property<string>("LocationId")
                        .HasColumnType("text");

                    b.Property<string>("VenueId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.HasIndex("VenueId");

                    b.ToTable("Openings", "Venues");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Venue", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("Added")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Approved")
                        .HasColumnType("boolean");

                    b.Property<string>("Banner")
                        .HasColumnType("text");

                    b.Property<List<string>>("Description")
                        .HasColumnType("text[]");

                    b.Property<string>("Discord")
                        .HasColumnType("text");

                    b.Property<DateTime>("HiddenUntil")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Hiring")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastModified")
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
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.OpenOverride", b =>
                {
                    b.HasOne("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Venue", "Venue")
                        .WithMany("OpenOverrides")
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Opening", b =>
                {
                    b.HasOne("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.HasOne("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Venue", "Venue")
                        .WithMany("Openings")
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Time", "End", b1 =>
                        {
                            b1.Property<string>("OpeningId")
                                .HasColumnType("text");

                            b1.Property<int>("Hour")
                                .HasColumnType("integer");

                            b1.Property<int>("Minute")
                                .HasColumnType("integer");

                            b1.Property<bool>("NextDay")
                                .HasColumnType("boolean");

                            b1.Property<string>("TimeZone")
                                .HasColumnType("text");

                            b1.HasKey("OpeningId");

                            b1.ToTable("Openings", "Venues");

                            b1.WithOwner()
                                .HasForeignKey("OpeningId");
                        });

                    b.OwnsOne("FFXIVVenues.Api.PersistenceModels.Entities.Venues.Time", "Start", b1 =>
                        {
                            b1.Property<string>("OpeningId")
                                .HasColumnType("text");

                            b1.Property<int>("Hour")
                                .HasColumnType("integer");

                            b1.Property<int>("Minute")
                                .HasColumnType("integer");

                            b1.Property<bool>("NextDay")
                                .HasColumnType("boolean");

                            b1.Property<string>("TimeZone")
                                .HasColumnType("text");

                            b1.HasKey("OpeningId");

                            b1.ToTable("Openings", "Venues");

                            b1.WithOwner()
                                .HasForeignKey("OpeningId");
                        });

                    b.Navigation("End");

                    b.Navigation("Location");

                    b.Navigation("Start");

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

                    b.Navigation("OpenOverrides");

                    b.Navigation("Openings");
                });
#pragma warning restore 612, 618
        }
    }
}
