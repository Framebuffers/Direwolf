-- CreateTable
CREATE TABLE "wolfpack" (
    "id" TEXT NOT NULL,
    "createdAt" TIMESTAMP(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "timeTaken" DOUBLE PRECISION NOT NULL DEFAULT 0.0,
    "sequenceNumber" SERIAL NOT NULL,
    "resultCount" INTEGER NOT NULL,
    "howlerName" TEXT NOT NULL,
    "testName" TEXT NOT NULL,
    "fileName" TEXT NOT NULL,
    "fileVersion" TEXT NOT NULL,
    "fileOrigin" TEXT NOT NULL,
    "wasCompleted" BOOLEAN NOT NULL DEFAULT false,
    "data" TEXT[],

    CONSTRAINT "wolfpack_pkey" PRIMARY KEY ("id")
);
