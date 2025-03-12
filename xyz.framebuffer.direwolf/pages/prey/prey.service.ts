import { PrismaClient } from "@prisma/client";

class PreyService {
  private prisma: PrismaClient;

  constructor() {
    this.prisma = new PrismaClient();
  }
  async getAllPrey() {
    try {
      const preys = await this.prisma.prey.findMany({
        select: {
            id: true,
            key: true,
            value: true,
        },
      });
      return preys;
    } catch (error) {
      console.error("Error fetching Preys:", error);
      throw error;
    }
  }

  async createPrey(data: any) {
    try {
      const prey = await this.prisma.prey.create({
        data,
      });
      return prey;
    } catch (error) {
      console.error("Error creating Prey:", error);
      throw error;
    }
  }
}

export const preyService = new PreyService();