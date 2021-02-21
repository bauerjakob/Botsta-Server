﻿// <auto-generated />
using System;
using Botsta.DataStorage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Botsta.DataStorage.Migrations
{
    [DbContext(typeof(PsqlContext))]
    [Migration("20210220174300_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Botsta.DataStorage.Models.Bot", b =>
                {
                    b.Property<Guid>("BotId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("BotName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ChatroomId")
                        .HasColumnType("uuid");

                    b.Property<string>("HashedApiKey")
                        .HasColumnType("text");

                    b.Property<string>("WebhookUrl")
                        .HasColumnType("text");

                    b.HasKey("BotId");

                    b.HasIndex("ChatroomId");

                    b.ToTable("Bots");
                });

            modelBuilder.Entity("Botsta.DataStorage.Models.Chatroom", b =>
                {
                    b.Property<Guid>("ChatroomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("ChatroomId");

                    b.ToTable("Chatroom");
                });

            modelBuilder.Entity("Botsta.DataStorage.Models.Message", b =>
                {
                    b.Property<Guid>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ChatroomId")
                        .HasColumnType("uuid");

                    b.Property<string>("MessageJson")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MyProperty")
                        .HasColumnType("text");

                    b.HasKey("MessageId");

                    b.HasIndex("ChatroomId");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("Botsta.DataStorage.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ChatroomId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Registerd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.HasIndex("ChatroomId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Botsta.DataStorage.Models.Bot", b =>
                {
                    b.HasOne("Botsta.DataStorage.Models.Chatroom", null)
                        .WithMany("Bots")
                        .HasForeignKey("ChatroomId");
                });

            modelBuilder.Entity("Botsta.DataStorage.Models.Message", b =>
                {
                    b.HasOne("Botsta.DataStorage.Models.Chatroom", null)
                        .WithMany("Messages")
                        .HasForeignKey("ChatroomId");
                });

            modelBuilder.Entity("Botsta.DataStorage.Models.User", b =>
                {
                    b.HasOne("Botsta.DataStorage.Models.Chatroom", null)
                        .WithMany("Users")
                        .HasForeignKey("ChatroomId");
                });

            modelBuilder.Entity("Botsta.DataStorage.Models.Chatroom", b =>
                {
                    b.Navigation("Bots");

                    b.Navigation("Messages");

                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
