﻿// <auto-generated />
using System;
using Forum.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Forum.API.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20190319110027_updateDB4.0")]
    partial class updateDB40
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Forum.API.Models.DiscussionParticipants", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DiscussionId");

                    b.HasKey("Id");

                    b.HasIndex("DiscussionId");

                    b.ToTable("DiscussionsParticipants");
                });

            modelBuilder.Entity("Forum.API.Models.DiscussionResponses", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CreatedById");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("DiscussionId");

                    b.Property<string>("Response");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("DiscussionId");

                    b.ToTable("DiscussionResponses");
                });

            modelBuilder.Entity("Forum.API.Models.Discussions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int?>("DocumentId");

                    b.Property<DateTime?>("EndDate");

                    b.Property<bool>("IsClosed");

                    b.Property<string>("Subject");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.HasIndex("UserId");

                    b.ToTable("Discussions");
                });

            modelBuilder.Entity("Forum.API.Models.Document", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.ToTable("Document");
                });

            modelBuilder.Entity("Forum.API.Models.OrganizationType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DiscussionParticipantsId");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("DiscussionParticipantsId");

                    b.ToTable("OrganizationType");
                });

            modelBuilder.Entity("Forum.API.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("UserId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Forum.API.Models.DiscussionParticipants", b =>
                {
                    b.HasOne("Forum.API.Models.Discussions", "Discussion")
                        .WithMany()
                        .HasForeignKey("DiscussionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Forum.API.Models.DiscussionResponses", b =>
                {
                    b.HasOne("Forum.API.Models.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Forum.API.Models.Discussions", "Discussion")
                        .WithMany("DiscussionResponses")
                        .HasForeignKey("DiscussionId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Forum.API.Models.Discussions", b =>
                {
                    b.HasOne("Forum.API.Models.Document", "Document")
                        .WithMany()
                        .HasForeignKey("DocumentId");

                    b.HasOne("Forum.API.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Forum.API.Models.OrganizationType", b =>
                {
                    b.HasOne("Forum.API.Models.DiscussionParticipants", "DiscussionParticipants")
                        .WithMany("OrganizationType")
                        .HasForeignKey("DiscussionParticipantsId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
