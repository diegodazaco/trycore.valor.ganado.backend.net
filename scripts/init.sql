CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260520160554_InitialCreate') THEN
    CREATE TABLE projects (
        id uuid NOT NULL,
        name character varying(200) NOT NULL,
        description character varying(1000),
        cutoff_date timestamp without time zone NOT NULL,
        created_at timestamp without time zone NOT NULL,
        updated_at timestamp without time zone NOT NULL,
        CONSTRAINT "PK_projects" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260520160554_InitialCreate') THEN
    CREATE TABLE activities (
        id uuid NOT NULL,
        project_id uuid NOT NULL,
        name character varying(300) NOT NULL,
        budget_at_completion numeric(18,4) NOT NULL,
        planned_progress_percentage numeric(5,2) NOT NULL,
        actual_progress_percentage numeric(5,2) NOT NULL,
        actual_cost numeric(18,4) NOT NULL,
        created_at timestamp without time zone NOT NULL,
        updated_at timestamp without time zone NOT NULL,
        CONSTRAINT "PK_activities" PRIMARY KEY (id),
        CONSTRAINT "FK_activities_projects_project_id" FOREIGN KEY (project_id) REFERENCES projects (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260520160554_InitialCreate') THEN
    CREATE INDEX "IX_activities_project_id" ON activities (project_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260520160554_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260520160554_InitialCreate', '8.0.11');
    END IF;
END $EF$;
COMMIT;

