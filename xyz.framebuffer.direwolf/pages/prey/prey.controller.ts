import { Request, Response } from "express";
import { preyService } from "./prey.service";

class PreyController {
  constructor() {}

  async getAllPrey(req: Request, res: Response) {
    try {
      const result = await preyService.getAllPrey();
      res.status(200).send(result);
    } catch (error) {
      console.error("Error fetching users:", error);
      res.status(500).send("Internal Server Error");
    }
  }

  async createPrey(req: Request, res: Response) {
    try {
      const result = await preyService.createPrey(req.body);
      res.status(200).send(result);
    } catch (error) {
      console.error("Error fetching users:", error);
      res.status(500).send("Internal Server Error");
    }
  }
}

export const preyController = new PreyController();