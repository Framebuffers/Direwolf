import { Prisma, PrismaClient } from "@prisma/client";

class WolfdenService {
  private prisma: PrismaClient;

  constructor() {
    this.prisma = new PrismaClient();
  }

  async function main()
  {
    
  }

  async getAllWolfdens() {
    try {
      const wolfdens = await this.prisma.wolfden.findMany({
        select: {
          id: true,
          createdAt: true,
          timeTaken: true,
          sequenceNumber: true,
          resultCount: true,
          howlerName: true,
          testName: true,
          fileName: true,
          fileVersion: true,
          fileOrigin: true,
          wasCompleted: true,
          data: true
        },
      });
      return wolfdens;
    } catch (error) {
      console.error("Error fetching Wolfdens:", error);
      throw error;
    }
  }

  async createWolfden(data: any) {
    try {
      const wolfden = await this.prisma.wolfden.create({
        data,
      });
      return wolfden;
    } catch (error) {
      console.error("Error creating Wolfden:", error);
      throw error;
    }
  }
}

export const wolfdenService = new WolfdenService();