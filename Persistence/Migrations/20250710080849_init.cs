using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgeChangeType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title_Value = table.Column<string>(type: "text", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeChangeType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Doctor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Fullname_Value = table.Column<string>(type: "text", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalProcedureRecordType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title_Value = table.Column<string>(type: "text", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalProcedureRecordType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HealthCondType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title_Value = table.Column<string>(type: "text", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthCondType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Fullname_Value = table.Column<string>(type: "text", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcedureType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StandartDuration = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Title_Value = table.Column<string>(type: "text", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReservedDate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservedDate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkinCareType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title_Value = table.Column<string>(type: "text", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkinCareType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkinFeatureType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title_Value = table.Column<string>(type: "text", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkinFeatureType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email_Value = table.Column<string>(type: "text", nullable: false),
                    PasswordHashed_Value = table.Column<string>(type: "text", nullable: false),
                    Username_Value = table.Column<string>(type: "text", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatientCard",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Age = table.Column<byte>(type: "smallint", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Adress_Value = table.Column<string>(type: "text", nullable: false),
                    Complaints_Value = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber_Value = table.Column<string>(type: "text", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientCard_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgeChange",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgeChange_AgeChangeType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "AgeChangeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgeChange_PatientCard_PatientCardId",
                        column: x => x.PatientCardId,
                        principalTable: "PatientCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalProcedureRecord",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: true),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalProcedureRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalProcedureRecord_ExternalProcedureRecordType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ExternalProcedureRecordType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExternalProcedureRecord_PatientCard_PatientCardId",
                        column: x => x.PatientCardId,
                        principalTable: "PatientCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthCond",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthCond", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthCond_HealthCondType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "HealthCondType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HealthCond_PatientCard_PatientCardId",
                        column: x => x.PatientCardId,
                        principalTable: "PatientCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientSpecifics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sleep = table.Column<string>(type: "text", nullable: false),
                    Diet = table.Column<string>(type: "text", nullable: false),
                    Sport = table.Column<string>(type: "text", nullable: false),
                    WorkEnviroment = table.Column<string>(type: "text", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientSpecifics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientSpecifics_PatientCard_PatientCardId",
                        column: x => x.PatientCardId,
                        principalTable: "PatientCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Procedure",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsComplete = table.Column<bool>(type: "boolean", nullable: false),
                    IsPostponded = table.Column<bool>(type: "boolean", nullable: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Procedure_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Procedure_PatientCard_PatientCardId",
                        column: x => x.PatientCardId,
                        principalTable: "PatientCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Procedure_ProcedureType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ProcedureType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkinCare",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkinCare", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkinCare_PatientCard_PatientCardId",
                        column: x => x.PatientCardId,
                        principalTable: "PatientCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkinCare_SkinCareType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SkinCareType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkinFeature",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkinFeature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkinFeature_PatientCard_PatientCardId",
                        column: x => x.PatientCardId,
                        principalTable: "PatientCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkinFeature_SkinFeatureType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SkinFeatureType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcedureId = table.Column<Guid>(type: "uuid", nullable: false),
                    SendingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsSent = table.Column<bool>(type: "boolean", nullable: false),
                    Message_Value = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber_Value = table.Column<string>(type: "text", nullable: false),
                    IsSoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_Procedure_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "Procedure",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgeChange_PatientCardId",
                table: "AgeChange",
                column: "PatientCardId");

            migrationBuilder.CreateIndex(
                name: "IX_AgeChange_TypeId",
                table: "AgeChange",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalProcedureRecord_PatientCardId",
                table: "ExternalProcedureRecord",
                column: "PatientCardId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalProcedureRecord_TypeId",
                table: "ExternalProcedureRecord",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthCond_PatientCardId",
                table: "HealthCond",
                column: "PatientCardId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthCond_TypeId",
                table: "HealthCond",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ProcedureId",
                table: "Notification",
                column: "ProcedureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientCard_PatientId",
                table: "PatientCard",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientSpecifics_PatientCardId",
                table: "PatientSpecifics",
                column: "PatientCardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_DoctorId",
                table: "Procedure",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_PatientCardId",
                table: "Procedure",
                column: "PatientCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_TypeId",
                table: "Procedure",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SkinCare_PatientCardId",
                table: "SkinCare",
                column: "PatientCardId");

            migrationBuilder.CreateIndex(
                name: "IX_SkinCare_TypeId",
                table: "SkinCare",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SkinFeature_PatientCardId",
                table: "SkinFeature",
                column: "PatientCardId");

            migrationBuilder.CreateIndex(
                name: "IX_SkinFeature_TypeId",
                table: "SkinFeature",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgeChange");

            migrationBuilder.DropTable(
                name: "ExternalProcedureRecord");

            migrationBuilder.DropTable(
                name: "HealthCond");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PatientSpecifics");

            migrationBuilder.DropTable(
                name: "ReservedDate");

            migrationBuilder.DropTable(
                name: "SkinCare");

            migrationBuilder.DropTable(
                name: "SkinFeature");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "AgeChangeType");

            migrationBuilder.DropTable(
                name: "ExternalProcedureRecordType");

            migrationBuilder.DropTable(
                name: "HealthCondType");

            migrationBuilder.DropTable(
                name: "Procedure");

            migrationBuilder.DropTable(
                name: "SkinCareType");

            migrationBuilder.DropTable(
                name: "SkinFeatureType");

            migrationBuilder.DropTable(
                name: "Doctor");

            migrationBuilder.DropTable(
                name: "PatientCard");

            migrationBuilder.DropTable(
                name: "ProcedureType");

            migrationBuilder.DropTable(
                name: "Patient");
        }
    }
}
