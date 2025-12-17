CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Users" (
    "UserID" text NOT NULL,
    "Firstname" text NOT NULL,
    "Lastname" text NOT NULL,
    "Email" text NOT NULL,
    "Hash" text NOT NULL,
    "Role" text NOT NULL,
    "Firstlogin" boolean NOT NULL,
    "IsActive" boolean NOT NULL,
    "SubscriptionExpiresAt" timestamp with time zone,
    "Balance" numeric NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("UserID")
);

CREATE TABLE "WinningBoards" (
    "WinningBoardID" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_WinningBoards" PRIMARY KEY ("WinningBoardID")
);

CREATE TABLE "Boards" (
    "BoardID" text NOT NULL,
    "BoardSize" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    "IsRepeating" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UserID" text NOT NULL,
    CONSTRAINT "PK_Boards" PRIMARY KEY ("BoardID"),
    CONSTRAINT "FK_Boards_Users_UserID" FOREIGN KEY ("UserID") REFERENCES "Users" ("UserID") ON DELETE CASCADE
);

CREATE TABLE "Transactions" (
    "TransactionID" text NOT NULL,
    "TransactionString" text NOT NULL,
    "TransactionDate" timestamp with time zone NOT NULL,
    "Amount" numeric NOT NULL,
    "UserID" text NOT NULL,
    CONSTRAINT "PK_Transactions" PRIMARY KEY ("TransactionID"),
    CONSTRAINT "FK_Transactions_Users_UserID" FOREIGN KEY ("UserID") REFERENCES "Users" ("UserID") ON DELETE CASCADE
);

CREATE TABLE "WinningNumbers" (
    "WinningNumberID" text NOT NULL,
    "WinningBoardID" text NOT NULL,
    "Number" integer NOT NULL,
    CONSTRAINT "PK_WinningNumbers" PRIMARY KEY ("WinningNumberID"),
    CONSTRAINT "FK_WinningNumbers_WinningBoards_WinningBoardID" FOREIGN KEY ("WinningBoardID") REFERENCES "WinningBoards" ("WinningBoardID") ON DELETE CASCADE
);

CREATE TABLE "BoardNumber" (
    "BoardNumberID" text NOT NULL,
    "BoardID" text NOT NULL,
    "WinningBoardID" text NOT NULL,
    "Number" integer NOT NULL,
    CONSTRAINT "PK_BoardNumber" PRIMARY KEY ("BoardNumberID"),
    CONSTRAINT "FK_BoardNumber_Boards_BoardID" FOREIGN KEY ("BoardID") REFERENCES "Boards" ("BoardID") ON DELETE CASCADE
);

CREATE INDEX "IX_BoardNumber_BoardID" ON "BoardNumber" ("BoardID");

CREATE INDEX "IX_Boards_UserID" ON "Boards" ("UserID");

CREATE INDEX "IX_Transactions_UserID" ON "Transactions" ("UserID");

CREATE INDEX "IX_WinningNumbers_WinningBoardID" ON "WinningNumbers" ("WinningBoardID");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251208175222_AddedSubscriptionExpiresAtToUsers', '9.0.11');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251211090623_Baseline', '9.0.11');

ALTER TABLE "Transactions" ADD "Pending" boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251211090910_AddedPendingToTransactions', '9.0.11');

COMMIT;

