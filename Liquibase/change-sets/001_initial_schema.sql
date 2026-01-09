--liquibase formatted sql

--changeset dkash:001_initial_schema

CREATE TABLE IF NOT EXISTS "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Username" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(255) NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "Role" VARCHAR(50) NOT NULL DEFAULT 'User',
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "UQ_Users_Username" UNIQUE ("Username"),
    CONSTRAINT "UQ_Users_Email" UNIQUE ("Email")
);

CREATE TABLE IF NOT EXISTS "Products" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(200) NOT NULL,
    "Description" VARCHAR(1000),
    "Price" DECIMAL(18,2) NOT NULL,
    "Subject" VARCHAR(100) NOT NULL,
    "Difficulty" VARCHAR(50) NOT NULL DEFAULT 'Medium',
    "EstimatedHours" INTEGER NOT NULL,
    "IsAvailable" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP
);

CREATE TABLE IF NOT EXISTS "Orders" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Pending',
    "TotalAmount" DECIMAL(18,2) NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "CompletedAt" TIMESTAMP,
    CONSTRAINT "FK_Orders_Users" FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "OrderProducts" (
    "Id" SERIAL PRIMARY KEY,
    "OrderId" INTEGER NOT NULL,
    "ProductId" INTEGER NOT NULL,
    "PriceAtOrder" DECIMAL(18,2) NOT NULL,
    "Quantity" INTEGER NOT NULL DEFAULT 1,
    CONSTRAINT "FK_OrderProducts_Orders" FOREIGN KEY ("OrderId") REFERENCES "Orders"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_OrderProducts_Products" FOREIGN KEY ("ProductId") REFERENCES "Products"("Id") ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS "ApiKeys" (
    "Id" SERIAL PRIMARY KEY,
    "Key" VARCHAR(100) NOT NULL UNIQUE,
    "Description" VARCHAR(200),
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ExpiresAt" TIMESTAMP NOT NULL
);

CREATE INDEX IF NOT EXISTS "IX_Products_Subject" ON "Products"("Subject");
CREATE INDEX IF NOT EXISTS "IX_Products_IsAvailable" ON "Products"("IsAvailable");
CREATE INDEX IF NOT EXISTS "IX_Orders_UserId" ON "Orders"("UserId");
CREATE INDEX IF NOT EXISTS "IX_Orders_Status" ON "Orders"("Status");
CREATE INDEX IF NOT EXISTS "IX_ApiKeys_Key" ON "ApiKeys"("Key");

--rollback DROP TABLE IF EXISTS "OrderProducts";
--rollback DROP TABLE IF EXISTS "Orders";
--rollback DROP TABLE IF EXISTS "Products";
--rollback DROP TABLE IF EXISTS "ApiKeys";
--rollback DROP TABLE IF EXISTS "Users";

