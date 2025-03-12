import { PrismaClient } from "@prisma/client";

class WolfdenService {
  private prisma: PrismaClient;

  constructor() {
    this.prisma = new PrismaClient();
  }
  async getAllWolfdens() {
    try {
      const wolfdens = await this.prisma.wolfden.findMany({
        select: {
            id: true,
            queryName: true,
            createdAt: true,
            resultCount: true,
            results: true
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