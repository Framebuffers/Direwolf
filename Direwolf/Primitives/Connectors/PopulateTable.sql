CREATE TABLE wolfpack (
                            id SERIAL PRIMARY KEY,
                            documentName VARCHAR(255) NOT NULL,
                            fileOrigin VARCHAR(255) NOT NULL,
                            documentVersion TEXT NOT NULL,
                            wasCompleted BOOLEAN NOT NULL DEFAULT false,
                            timeTaken DOUBLE PRECISION NOT NULL DEFAULT 0,
                            createdAt TIMESTAMP(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            guid TEXT NOT NULL,
                            resultCount INTEGER NOT NULL DEFAULT 0,
                            testName VARCHAR(255) NOT NULL,
                            results JSON,
                            testId INTEGER NOT NULL,

                            CONSTRAINT "Wolfpack_pkey" PRIMARY KEY ("id")
);

create table wolfpack (
    id serial primary key,
    wolfpack_guid varchar(36) not null unique,
    
)

create table revit_performance (
  id serial primary key,
  episode_id 
);

-- CREATE TABLE revit_document (
--     "id" SERIAL NOT NULL,
--     "documentUniqueId" 
-- ) 