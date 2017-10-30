using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace E_Internship_Journal.Data.Migrations
{
    public partial class a : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyAddress = table.Column<string>(type: "VARCHAR(MAX)", nullable: false),
                    CompanyName = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_CompanyId", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseCode = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    CourseName = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_CourseId", x => x.CourseId);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationPins",
                columns: table => new
                {
                    RegistrationPinId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pin = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_RegistrationPinId", x => x.RegistrationPinId);
                    table.ForeignKey(
                        name: "FK_RegistrationPins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    ProjectName = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    SupervisorId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_ProjectId", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_Projects_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Projects_AspNetUsers_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    BatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BatchName = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_BatchId", x => x.BatchId);
                    table.ForeignKey(
                        name: "FK_Batches_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "competencies",
                columns: table => new
                {
                    CompetencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(MAX)", nullable: false),
                    TitleDescriotion = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_CompetencyId", x => x.CompetencyId);
                    table.ForeignKey(
                        name: "FK_competencies_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBatches",
                columns: table => new
                {
                    UserBatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_UserBatchId", x => x.UserBatchId);
                    table.ForeignKey(
                        name: "FK_UserBatches_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBatches_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Internship_Records",
                columns: table => new
                {
                    InternshipRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(type: "VARCHAR(MAX)", nullable: false),
                    CompanyCheck1a = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCheck1b = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCheck2a = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCheck2b = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCheck2c = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCheck2d = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCheck2e = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCheck2f = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCheck3a = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCheck3b = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCheck3c = table.Column<bool>(type: "bit", nullable: false),
                    FeedbackEnjoy = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    FeedbackExperiences = table.Column<bool>(type: "bit", nullable: false),
                    FeedbackImproved = table.Column<bool>(type: "bit", nullable: false),
                    VARCHAR100 = table.Column<string>(name: "VARCHAR(100)", type: "bit", nullable: true),
                    FeedbackRecommend = table.Column<bool>(type: "bit", nullable: false),
                    FeedbackUseful = table.Column<bool>(type: "bit", nullable: false),
                    LiaisonOfficerId = table.Column<string>(nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    PresentationGrading = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ReflectionGrading = table.Column<int>(type: "int", nullable: false),
                    UserBatchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_InternshipRecordId", x => x.InternshipRecordId);
                    table.ForeignKey(
                        name: "FK_Internship_Records_AspNetUsers_LiaisonOfficerId",
                        column: x => x.LiaisonOfficerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Internship_Records_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Internship_Records_UserBatches_UserBatchId",
                        column: x => x.UserBatchId,
                        principalTable: "UserBatches",
                        principalColumn: "UserBatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Month_Records",
                columns: table => new
                {
                    MonthId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Approved = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CommunicationGrading = table.Column<int>(type: "int", nullable: true),
                    IndependenceGrading = table.Column<int>(type: "int", nullable: true),
                    InternshipRecordId = table.Column<int>(type: "int", nullable: false),
                    MentorSessionDateTimeEnd = table.Column<DateTime>(nullable: true),
                    MentorSessionDateTimeStart = table.Column<DateTime>(nullable: true),
                    MentorSessionReflection = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    OverallFeedback = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    OverallGrading = table.Column<int>(type: "int", nullable: true),
                    PerformanceGrading = table.Column<int>(type: "int", nullable: true),
                    SoftSkillsCompetency = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    TechnicalCompetency = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    TechnicalGrading = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_MonthId", x => x.MonthId);
                    table.ForeignKey(
                        name: "FK_Month_Records_Internship_Records_InternshipRecordId",
                        column: x => x.InternshipRecordId,
                        principalTable: "Internship_Records",
                        principalColumn: "InternshipRecordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TouchPoint_Record",
                columns: table => new
                {
                    TouchPointId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comments = table.Column<string>(type: "VARCHAR(MAX)", nullable: false),
                    InternshipRecordId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_TouchPointId", x => x.TouchPointId);
                    table.ForeignKey(
                        name: "FK_TouchPoint_Record_Internship_Records_InternshipRecordId",
                        column: x => x.InternshipRecordId,
                        principalTable: "Internship_Records",
                        principalColumn: "InternshipRecordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Competency_Checkeds",
                columns: table => new
                {
                    MonthRecordId = table.Column<int>(type: "int", nullable: false),
                    CompetencyId = table.Column<int>(type: "int", nullable: false),
                    CompentencyCheckedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_MonthCompetency", x => new { x.MonthRecordId, x.CompetencyId });
                    table.UniqueConstraint("PrimaryKey_CompentencyCheckedId", x => x.CompentencyCheckedId);
                    table.ForeignKey(
                        name: "FK_Competency_Checkeds_competencies_CompetencyId",
                        column: x => x.CompetencyId,
                        principalTable: "competencies",
                        principalColumn: "CompetencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Competency_Checkeds_Month_Records_MonthRecordId",
                        column: x => x.MonthRecordId,
                        principalTable: "Month_Records",
                        principalColumn: "MonthId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Day_Records",
                columns: table => new
                {
                    DayId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ArrivalTime = table.Column<DateTime>(nullable: false),
                    CompanyOff = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    Date = table.Column<DateTime>(nullable: false),
                    DepartureTime = table.Column<DateTime>(nullable: false),
                    MC = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    MonthRecordId = table.Column<int>(type: "int", nullable: false),
                    WeekNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_DayId", x => x.DayId);
                    table.ForeignKey(
                        name: "FK_Day_Records_Month_Records_MonthRecordId",
                        column: x => x.MonthRecordId,
                        principalTable: "Month_Records",
                        principalColumn: "MonthId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    TaskRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DayRecordId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(MAX)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_TaskRecordId", x => x.TaskRecordId);
                    table.ForeignKey(
                        name: "FK_Tasks_Day_Records_DayRecordId",
                        column: x => x.DayRecordId,
                        principalTable: "Day_Records",
                        principalColumn: "DayId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CourseId",
                table: "AspNetUsers",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "Batch_BatchName_UniqueConstraint",
                table: "Batches",
                column: "BatchName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Batches_CourseId",
                table: "Batches",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_competencies_CourseId",
                table: "competencies",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Competency_Checkeds_CompetencyId",
                table: "Competency_Checkeds",
                column: "CompetencyId");

            migrationBuilder.CreateIndex(
                name: "Course_CourseCode_UniqueConstraint",
                table: "Courses",
                column: "CourseCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Day_Records_MonthRecordId",
                table: "Day_Records",
                column: "MonthRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Internship_Records_LiaisonOfficerId",
                table: "Internship_Records",
                column: "LiaisonOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_Internship_Records_ProjectId",
                table: "Internship_Records",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Internship_Records_UserBatchId",
                table: "Internship_Records",
                column: "UserBatchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Month_Records_InternshipRecordId",
                table: "Month_Records",
                column: "InternshipRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CompanyID",
                table: "Projects",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_SupervisorId",
                table: "Projects",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationPins_UserId",
                table: "RegistrationPins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DayRecordId",
                table: "Tasks",
                column: "DayRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_TouchPoint_Record_InternshipRecordId",
                table: "TouchPoint_Record",
                column: "InternshipRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBatches_BatchId",
                table: "UserBatches",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBatches_UserId",
                table: "UserBatches",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Courses_CourseId",
                table: "AspNetUsers",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Courses_CourseId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Competency_Checkeds");

            migrationBuilder.DropTable(
                name: "RegistrationPins");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "TouchPoint_Record");

            migrationBuilder.DropTable(
                name: "competencies");

            migrationBuilder.DropTable(
                name: "Day_Records");

            migrationBuilder.DropTable(
                name: "Month_Records");

            migrationBuilder.DropTable(
                name: "Internship_Records");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "UserBatches");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CourseId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }
    }
}
