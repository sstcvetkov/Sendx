CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "Messages" (
    "Id" uuid NOT NULL,
    "Content" text NULL,
    "ClientIp" text NULL,
    "Created" timestamp without time zone NOT NULL,
    CONSTRAINT "PK_Messages" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200602064110_Initial', '3.1.4');
