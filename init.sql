-- Create Configs table if it doesn't exist
CREATE TABLE IF NOT EXISTS "Configs" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Type" VARCHAR(50) NOT NULL,
    "Value" TEXT NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "ApplicationName" VARCHAR(255) NOT NULL
);

-- Initialize default configuration data
INSERT INTO "Configs" ("Id", "Name", "Type", "Value", "IsActive", "ApplicationName")
VALUES
    (1, 'SiteName', 'string', 'soty.io', TRUE, 'SERVICE-A'),
    (2, 'IsBasketEnabled', 'bool', '1', TRUE, 'SERVICE-B'),
    (3, 'MaxItemCount', 'int', '50', FALSE, 'SERVICE-A')
ON CONFLICT ("Id") DO NOTHING;
